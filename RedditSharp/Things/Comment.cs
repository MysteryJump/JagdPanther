﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedditSharp.Things
{
    public class Comment : VotableThing
    {
        private const string CommentUrl = "/api/comment";
        private const string DistinguishUrl = "/api/distinguish";
        private const string EditUserTextUrl = "/api/editusertext";
        private const string RemoveUrl = "/api/remove";
        private const string SetAsReadUrl = "/api/read_message";

        [JsonIgnore]
        private Reddit Reddit { get; set; }
        [JsonIgnore]
        private IWebAgent WebAgent { get; set; }

        public Comment Init(Reddit reddit, JToken json, IWebAgent webAgent, Thing sender)
        {
            var data = CommonInit(reddit, json, webAgent, sender);
            ParseComments(reddit, json, webAgent, sender);
            JsonConvert.PopulateObject(data.ToString(), this, reddit.JsonSerializerSettings);
            return this;
        }
        public async Task<Comment> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent, Thing sender)
        {
            var data = CommonInit(reddit, json, webAgent, sender);
            await ParseCommentsAsync(reddit, json, webAgent, sender);
            await Task.Factory.StartNew(() => JsonConvert.PopulateObject(data.ToString(), this, reddit.JsonSerializerSettings));
            return this;
        }

        private JToken CommonInit(Reddit reddit, JToken json, IWebAgent webAgent, Thing sender)
        {
            base.Init(reddit, webAgent, json);
            var data = json["data"];
            Reddit = reddit;
            WebAgent = webAgent;
            this.Parent = sender;

            // Handle Reddit's API being horrible
            if (data["context"] != null)
            {
                var context = data["context"].Value<string>();
                LinkId = context.Split('/')[4];
            }
         
            return data;
        }

        private void ParseComments(Reddit reddit, JToken data, IWebAgent webAgent, Thing sender)
        {
            // Parse sub comments
            var replies = data["data"]["replies"];
            var subComments = new List<Comment>();
            if (replies != null && replies.Count() > 0)
            {
                foreach (var comment in replies["data"]["children"])
                    subComments.Add(new Comment().Init(reddit, comment, webAgent, sender));
            }
            Comments = subComments.ToArray();
        }

        private async Task ParseCommentsAsync(Reddit reddit, JToken data, IWebAgent webAgent, Thing sender)
        {
            // Parse sub comments
            var replies = data["data"]["replies"];
            var subComments = new List<Comment>();
            if (replies != null && replies.Count() > 0)
            {
                foreach (var comment in replies["data"]["children"])
                    subComments.Add(await new Comment().InitAsync(reddit, comment, webAgent, sender));
            }
            Comments = subComments.ToArray();            
        }

        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("banned_by")]
        public string BannedBy { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("body_html")]
        public string BodyHtml { get; set; }
        [JsonProperty("parent_id")]
        public string ParentId { get; set; }
        [JsonProperty("subreddit")]
        public string Subreddit { get; set; }
        [JsonProperty("approved_by")]
        public string ApprovedBy { get; set; }
        [JsonProperty("author_flair_css_class")]
        public string AuthorFlairCssClass { get; set; }
        [JsonProperty("author_flair_text")]
        public string AuthorFlairText { get; set; }
        [JsonProperty("gilded")]
        public int Gilded { get; set; }
        [JsonProperty("link_id")]
        public string LinkId { get; set; }
        [JsonProperty("link_title")]
        public string LinkTitle { get; set; }
        [JsonProperty("num_reports")]
        public int? NumReports { get; set; }
        [JsonProperty("distinguished")]
        [JsonConverter(typeof(DistinguishConverter))]
        public DistinguishType Distinguished { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonIgnore]
        public IList<Comment> Comments { get; private set; }

        [JsonIgnore]
        public Thing Parent { get; internal set; }

        public override string Shortlink
        {
            get
            {
                // Not really a "short" link, but you can't actually use short links for comments
                return String.Format("{0}://{1}/r/{2}/comments/{3}/_/{4}",
                                     RedditSharp.WebAgent.Protocol, RedditSharp.WebAgent.RootDomain,
                                     this.Subreddit, this.Parent.Id, this.Id);
            }
        }

        public Comment Reply(string message)
        {
            if (Reddit.User == null)
                throw new AuthenticationException("No user logged in.");
            var request = WebAgent.CreatePost(CommentUrl);
            var stream = request.GetRequestStream();
            WebAgent.WritePostBody(stream, new
            {
                text = message,
                thing_id = FullName,
                uh = Reddit.User.Modhash,
                api_type = "json"
                //r = Subreddit
            });
            stream.Close();
            try
            {
                var response = request.GetResponse();
                var data = WebAgent.GetResponseString(response.GetResponseStream());
                var json = JObject.Parse(data);
                if (json["json"]["ratelimit"] != null)
                    throw new RateLimitException(TimeSpan.FromSeconds(json["json"]["ratelimit"].ValueOrDefault<double>()));
                return new Comment().Init(Reddit, json["json"]["data"]["things"][0], WebAgent, this);
            }
            catch (WebException ex)
            {
                var error = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                return null;
            }
        }

        public void Distinguish(DistinguishType distinguishType)
        {
            if (Reddit.User == null)
                throw new AuthenticationException("No user logged in.");
            var request = WebAgent.CreatePost(DistinguishUrl);
            var stream = request.GetRequestStream();
            string how;
            switch (distinguishType)
            {
                case DistinguishType.Admin:
                    how = "admin";
                    break;
                case DistinguishType.Moderator:
                    how = "yes";
                    break;
                case DistinguishType.None:
                    how = "no";
                    break;
                default:
                    how = "special";
                    break;
            }
            WebAgent.WritePostBody(stream, new
            {
                how,
                id = Id,
                uh = Reddit.User.Modhash
            });
            stream.Close();
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JObject.Parse(data);
            if (json["jquery"].Count(i => i[0].Value<int>() == 11 && i[1].Value<int>() == 12) == 0)
                throw new AuthenticationException("You are not permitted to distinguish this comment.");
        }

        /// <summary>
        /// Replaces the text in this comment with the input text.
        /// </summary>
        /// <param name="newText">The text to replace the comment's contents</param>        
        public void EditText(string newText)
        {
            if (Reddit.User == null)
                throw new Exception("No user logged in.");

            var request = WebAgent.CreatePost(EditUserTextUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
            {
                api_type = "json",
                text = newText,
                thing_id = FullName,
                uh = Reddit.User.Modhash
            });
            var response = request.GetResponse();
            var result = WebAgent.GetResponseString(response.GetResponseStream());
            JToken json = JToken.Parse(result);
            if (json["json"].ToString().Contains("\"errors\": []"))
                Body = newText;
            else
                throw new Exception("Error editing text.");
        }

        public void Remove()
        {
            RemoveImpl(false);
        }

        public void RemoveSpam()
        {
            RemoveImpl(true);
        }

        private void RemoveImpl(bool spam)
        {
            var request = WebAgent.CreatePost(RemoveUrl);
            var stream = request.GetRequestStream();
            WebAgent.WritePostBody(stream, new
            {
                id = FullName,
                spam = spam,
                uh = Reddit.User.Modhash
            });
            stream.Close();
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
        }

        public void SetAsRead()
        {
            var request = WebAgent.CreatePost(SetAsReadUrl);
            WebAgent.WritePostBody(request.GetRequestStream(), new
                                 {
                                     id = FullName,
                                     uh = Reddit.User.Modhash,
                                     api_type = "json"
                                 });
            var response = request.GetResponse();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
        }
    }

    public enum DistinguishType
    {
        Moderator,
        Admin,
        Special,
        None
    }

    internal class DistinguishConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DistinguishType) || objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var value = token.Value<string>();
            if (value == null)
                return DistinguishType.None;
            switch (value)
            {
                case "moderator": return DistinguishType.Moderator;
                case "admin": return DistinguishType.Admin;
                case "special": return DistinguishType.Special;
                default: return DistinguishType.None;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var d = (DistinguishType)value;
            if (d == DistinguishType.None)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(d.ToString().ToLower());
        }
    }

}
