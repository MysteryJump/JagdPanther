using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JagdPanther.View
{
	public static class ViewportHelper
	{
		// see also https://social.msdn.microsoft.com/Forums/vstudio/en-US/e6ccfec3-3dc0-4702-9d0d-1cfa55ecfc90/itemscontrol-get-visible-items?forum=wpf
		public static bool IsInViewport(Control item)
		{
			if (item == null) return false;
			ItemsControl itemsControl = null;

			itemsControl = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
		

			ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ItemsControl>(itemsControl);
			ScrollContentPresenter scrollContentPresenter = (ScrollContentPresenter)scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer);
			MethodInfo isInViewportMethod = scrollViewer.GetType().GetMethod("IsInViewport", BindingFlags.NonPublic | BindingFlags.Instance);

			return (bool)isInViewportMethod.Invoke(scrollViewer, new object[] { scrollContentPresenter, item });
		}
	}

	public static class VisualTreeHelper
	{
		private static void GetVisualChildren<T>(DependencyObject current, Collection<T> children) where T : DependencyObject
		{
			if (current != null)
			{
				if (current.GetType() == typeof(T))
				{
					children.Add((T)current);
				}

				for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(current); i++)
				{
					GetVisualChildren<T>(System.Windows.Media.VisualTreeHelper.GetChild(current, i), children);
				}
			}
		}

		public static Collection<T> GetVisualChildren<T>(DependencyObject current) where T : DependencyObject
		{
			if (current == null)
			{
				return null;
			}

			Collection<T> children = new Collection<T>();

			GetVisualChildren<T>(current, children);

			return children;
		}

		public static T GetVisualChild<T, P>(P templatedParent)
			where T : FrameworkElement
			where P : FrameworkElement
		{
			Collection<T> children = VisualTreeHelper.GetVisualChildren<T>(templatedParent);

			foreach (T child in children)
			{
				if (child.TemplatedParent == templatedParent)
				{
					return child;
				}
			}

			return null;
		}
	}
}

