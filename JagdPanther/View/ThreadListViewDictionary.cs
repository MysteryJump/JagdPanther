using JagdPanther.Model;
using JagdPanther.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JagdPanther.View
{
	public partial class ThreadListViewDictionary
	{
		private bool isdes;

		public MainViewModel DataContext { get; private set; }

		public void ThreadListColumnClicked(object sender, RoutedEventArgs e)
		{
			var y = (sender as GridViewColumnHeader).Tag.ToString();

			var x = ((MainViewModel)(Application.Current.MainWindow.DataContext)).ThreadListTabs.SelectedTab;
			List<Thread> data;
			if (!isdes)
			{
				switch (y)
				{
					case "Title":
						data = new List<Thread>(x.ThreadList).OrderBy(z => z.Title).ToList();
						break;
					case "CreatedTime":
						data = new List<Thread>(x.ThreadList).OrderBy(z => z.CreatedTime).ToList();
						break;
					case "VoteCount":
						data = new List<Thread>(x.ThreadList).OrderBy(z => z.VoteCount).ToList();
						break;
					case "CommentCount":
						data = new List<Thread>(x.ThreadList).OrderBy(z => z.CommentCount).ToList();
						break;
					case "Speed":
						data = new List<Thread>(x.ThreadList).OrderBy(z => z.Speed).ToList();
						break;
					case "Flair":
						data = new List<Thread>(x.ThreadList).OrderBy(z => z.Flair).ToList();
						break;
					default:
						return;
				}
			}
			else
			{
				switch (y)
				{
					case "Title":
						data = new List<Thread>(x.ThreadList).OrderByDescending(z => z.Title).ToList();
						break;
					case "CreatedTime":
						data = new List<Thread>(x.ThreadList).OrderByDescending(z => z.CreatedTime).ToList();
						break;
					case "VoteCount":
						data = new List<Thread>(x.ThreadList).OrderByDescending(z => z.VoteCount).ToList();
						break;
					case "CommentCount":
						data = new List<Thread>(x.ThreadList).OrderByDescending(z => z.CommentCount).ToList();
						break;
					case "Speed":
						data = new List<Thread>(x.ThreadList).OrderByDescending(z => z.Speed).ToList();
						break;
					case "Flair":
						data = new List<Thread>(x.ThreadList).OrderByDescending(z => z.Flair).ToList();
						break;
					default:
						return;
				}
			}
			x.ThreadList.Clear();
			isdes = !isdes;
			data.ForEach(p => x.ThreadList.Add(p));
		}
	}
}
