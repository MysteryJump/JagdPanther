using JagdPanther.Model;
using JagdPanther.ViewModel;
using ReactiveUI;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Def = JagdPanther.Properties.Settings;
using System.ComponentModel;

namespace JagdPanther
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		public bool isdes;

        public MainWindow()
		{
			InitializeComponent();
			two.Oriented = Orientation.Horizontal;
			two.Oriented = Orientation.Vertical;
			two.Oriented = Orientation.Horizontal;

			MessageBus.Current.Listen<Orientation>("ChangeViewState").Subscribe(x =>
			{
				two.Oriented = x;
			});
			MessageBus.Current.Listen<string>("ErrorMessage").Subscribe(x =>
				{
					ssss.Content = x;
				});
			MessageBus.Current.Listen<string>("BoardTreeWidth")
				.Subscribe(_ => boardTree.Width = boardTree.Width.Value == 0 ? new GridLength(Def.Default.BoardTreeWidth) : new GridLength(0));
			MessageBus.Current.Listen<IThreadListViewer>("ThreadListSelectTab")
				.Subscribe(x => threadListTab.SelectedItem = x);
			MessageBus.Current.Listen<Thread>("ThreadSelectTab")
				.Subscribe(x =>
				{
					threadTab.SelectedItem = x;
				});
			MessageBus.Current.Listen<Account>("ChangeLoggedUser")
				.Subscribe(x => logged.Header = x.UserName);

			LoadWindowState();
		}

		private void LoadWindowState()
		{
			Height = Def.Default.WindowHeight;
			Width = Def.Default.WindowWidth;
			WindowState = Def.Default.WindowState;
			boardTree.Width = new GridLength(Def.Default.BoardTreeWidth);
			two.Oriented = Def.Default.IsThreeColumn ? Orientation.Vertical : Orientation.Horizontal;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			Def.Default.WindowHeight = (int)Height;
			Def.Default.WindowWidth = (int)Width;
			Def.Default.WindowState = 
				WindowState == WindowState.Minimized ? WindowState.Normal : WindowState;
			Def.Default.IsThreeColumn = two.Oriented == Orientation.Horizontal ? false : true;
			Def.Default.BoardTreeWidth = (int)boardTree.Width.Value;
			Def.Default.Save();

			base.OnClosing(e);
		}

		// これはVMに置くべき
		// Issueに加えようか
		// リファクタリングはあとからでいいだろ、ゴミ
		public void ThreadListColumnClicked(object sender, RoutedEventArgs e)
		{
			var y = (sender as GridViewColumnHeader).Tag.ToString();

			var x = ((MainViewModel)(DataContext)).ThreadListTabs.SelectedTab;
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
