using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RedditSharp;

namespace JagdPanther.Dialogs
{
    /// <summary>
    /// Interaction logic for SubmitWindow.xaml
    /// </summary>
    public partial class SubmitWindow : Window
    {
        public SubmitWindow()
        {
            InitializeComponent();
			this.Loaded += Window_Loaded;    
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//flairs.Items.Add(new UserFlairTemplate() { Text = "<なし>", CssClass = null});
			//Flairs.ForEach(x =>
			//{
			//	flairs.Items.Add(x);
			//});
		}

		private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(text.Text) || !string.IsNullOrWhiteSpace(title.Text))
                this.Close();
            IsLinkPost = c1.IsChecked == true;
            PostString = text.Text;
            IsOk = true;
            Title = title.Text;
			SelectedFlair = flairs.SelectedItem as UserFlairTemplate;
			IsNsfw = nsfw.IsChecked == true;
            this.Close();
        }

        private void c1_Checked(object sender, RoutedEventArgs e)
        {
            if (c1.IsChecked == true)
            {
                text.AcceptsReturn = false;
            }
            else
            {
                text.AcceptsReturn = true;
            }
        }

        public bool IsLinkPost { get; set; }
        public string PostString { get; set; }
        public bool IsOk { get; set; }
        public string Title { get; set; }
		public UserFlairTemplate SelectedFlair { get; set; }
		public bool IsNsfw { get; set; }
		public List<UserFlairTemplate> Flairs { get; internal set; }
	}
}
