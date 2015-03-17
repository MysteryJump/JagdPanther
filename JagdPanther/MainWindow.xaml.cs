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
			MessageBus.Current.Listen<Orientation>("ChangeViewState").Subscribe(x => {
				two.Oriented = x;
			});
        }

		public void ThreadListColumnClicked(object sender, RoutedEventArgs e)
		{
			var y = (sender as GridViewColumnHeader).Tag.ToString();

			var x = ((MainViewModel)(this.DataContext)).ThreadListTabs.SelectedTab;
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
