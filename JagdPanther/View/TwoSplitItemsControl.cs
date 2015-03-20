using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JagdPanther.View
{
	// See also https://gist.github.com/MysteryJump/fae95f07652b99459fa4
	public class TwoSplitItemsControl : Grid
	{
		private Orientation oriented;
		private GridSplitter splitter;
		public Orientation Oriented
		{
			get { return oriented; }
			set
			{
				if (value == oriented)
					return;
				switch (value)
				{
					case Orientation.Horizontal:
						{
							if (splitter != null)
								Children.Remove(splitter);
							ColumnDefinitions.Clear();
							RowDefinitions.Add(new RowDefinition());
							RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2) });
							RowDefinitions.Add(new RowDefinition());

							var gs = splitter = GenerateGridSplitter(Orientation.Horizontal);
							Children.Add(gs);
							gs.SetValue(RowProperty, 1);
						}
						break;
					case Orientation.Vertical:
						{
							if (splitter != null)
								Children.Remove(splitter);
							RowDefinitions.Clear();
							ColumnDefinitions.Add(new ColumnDefinition());
							ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2) });
							ColumnDefinitions.Add(new ColumnDefinition());

							var gs = splitter = GenerateGridSplitter(Orientation.Vertical);
							Children.Add(gs);
							gs.SetValue(ColumnProperty, 1);
						}
						break;
				}
				if (firstItem != null && secondItem != null)
				{
					switch (value)
					{
						case Orientation.Horizontal:
							{
								firstItem.SetValue(RowProperty, 0);
								secondItem.SetValue(RowProperty, 2);
							}
							break;
						case Orientation.Vertical:
							{
								firstItem.SetValue(ColumnProperty, 0);
								secondItem.SetValue(ColumnProperty, 2);
							}
							break;
					}
				}
				oriented = value;
			}
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


	}
}
