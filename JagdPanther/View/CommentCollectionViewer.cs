using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JagdPanther.View
{
	public class CommentCollectionViewer : ItemsControl
	{

		public object[] ShowedItems
		{
			get
			{
				var list = new List<object>();
				Enumerable.Range(0, ItemsSource.OfType<object>().Count())
					.ToList()
					.ForEach(x =>
					{
						var item = ItemContainerGenerator.ContainerFromIndex(x);
						if (ViewportHelper.IsInViewport(item as Control))
						{
							list.Add(item);
						}
					});
				return list.ToArray();
			}
		}

		public void BringItemIntoView(object item)
		{
			ItemsSource.OfType<object>()
				.ToList()
				.ForEach(x =>
				{
					if (item == x)
					{
						this.ScrollIntoView(item);
					}
				});
		}
	}
}
