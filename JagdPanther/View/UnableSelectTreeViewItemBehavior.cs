using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace JagdPanther.View
{
	public class UnableSelectTreeViewItemBehavior : Behavior<TreeView>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.SelectedItemChanged += AssociatedObject_SelectedItemChanged;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.SelectedItemChanged -= AssociatedObject_SelectedItemChanged;
		}

		private void AssociatedObject_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
		{
			var treeItem = sender as TreeView;
            if (e.NewValue == null)
				return;
			var item = (treeItem.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItem);
			if (item == null)
			{

				var i = treeItem.ItemsSource.OfType<TreeViewComment>()
					.ToList();

				var q = new Queue<Tuple<TreeViewComment,TreeViewItem>>();
				i.ForEach(x =>
				{
					var t2 = treeItem.ItemContainerGenerator.ContainerFromItem(x) as TreeViewItem;
					q.Enqueue(new Tuple<TreeViewComment, TreeViewItem>(x,t2));
				});
				while (q.Count != 0)
				{
					var de = q.Dequeue();
					if (de.Item2 == null)
						continue;
					if (de.Item1.Children.Count > 0)
					{
						de.Item1.Children.ForEach(x =>
						{
							q.Enqueue(new Tuple<TreeViewComment, TreeViewItem>(x, de.Item2.ItemContainerGenerator.ContainerFromItem(x) as TreeViewItem));
						});
					}
					de.Item2.IsSelected = false;
				}
				return;
			}
			item.IsSelected = false;
			
		}
	}
}
