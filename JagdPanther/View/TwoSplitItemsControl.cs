using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JagdPanther.View
{
	public class TwoSplitItemsControl : Grid
	{
		private Orientation oriented;
		private GridSplitter splitter;

		public Orientation Oriented
		{
			get { return (Orientation)GetValue(OrientedProperty); }
			set { SetValue(OrientedProperty, value); }
		}

		private GridSplitter GenerateGridSplitter(Orientation orient)
		{
			var gs = new GridSplitter();
			switch (orient)
			{
				case Orientation.Horizontal:
					gs.Height = 2;
					gs.HorizontalAlignment = HorizontalAlignment.Stretch;
					gs.VerticalAlignment = VerticalAlignment.Center;
					break;
				case Orientation.Vertical:
					gs.HorizontalAlignment = HorizontalAlignment.Center;
					gs.VerticalAlignment = VerticalAlignment.Stretch;
					gs.Width = 2;
					break;
			}
			return gs;
		}

		public TwoSplitItemsControl() : base()
		{

		}

		public UIElement FirstItem
		{
			get { return firstItem; }
			set
			{
				if (firstItem != null)
					Children.Remove(firstItem);
				firstItem = value;
				Children.Add(value);
				if (Oriented == Orientation.Vertical)
					value.SetValue(ColumnProperty, 0);
				else
					value.SetValue(RowProperty, 0);

			}
		}
		private UIElement firstItem;

		public UIElement SecondItem
		{
			get { return secondItem; }
			set
			{
				if (secondItem != null)
					Children.Remove(secondItem);
				secondItem = value;
				Children.Add(value);
				if (Oriented == Orientation.Vertical)
					value.SetValue(ColumnProperty, 2);
				else
					value.SetValue(RowProperty, 2);
			}
		}
		private UIElement secondItem;

		public static readonly DependencyProperty OrientedProperty =
		DependencyProperty.Register("Oriented",
									typeof(Orientation),
									typeof(TwoSplitItemsControl),
									new FrameworkPropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnOrientedChanged)));

		private static void OnOrientedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TwoSplitItemsControl t = d as TwoSplitItemsControl;
			if (t != null)
			{
				switch (t.Oriented)
				{
					case Orientation.Horizontal:
						{
							if (t.splitter != null)
								t.Children.Remove(t.splitter);
							t.ColumnDefinitions.Clear();
							t.RowDefinitions.Add(new RowDefinition());
							t.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2) });
							t.RowDefinitions.Add(new RowDefinition());

							var gs = t.splitter = t.GenerateGridSplitter(Orientation.Vertical);
							t.Children.Add(gs);
							gs.SetValue(RowProperty, 1);
						}
						break;
					case Orientation.Vertical:
						{
							if (t.splitter != null)
								t.Children.Remove(t.splitter);
							t.RowDefinitions.Clear();
							t.ColumnDefinitions.Add(new ColumnDefinition());
							t.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2) });
							t.ColumnDefinitions.Add(new ColumnDefinition());

							var gs = t.splitter = t.GenerateGridSplitter(Orientation.Vertical);
							t.Children.Add(gs);
							gs.SetValue(ColumnProperty, 1);
						}
						break;
				}
				if (t.firstItem != null && t.secondItem != null)
				{
					switch (t.Oriented)
					{
						case Orientation.Horizontal:
							{
								t.firstItem.SetValue(RowProperty, 0);
								t.secondItem.SetValue(RowProperty, 2);
							}
							break;
						case Orientation.Vertical:
							{
								t.firstItem.SetValue(ColumnProperty, 0);
								t.secondItem.SetValue(ColumnProperty, 2);
							}
							break;
					}
				}
			}
		}
	}
}
