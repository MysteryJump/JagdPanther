﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JagdPanther.View
{
	public class ThreadViewDataTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			switch (Properties.Settings.Default.IsWebView)
			{
				case true:
					return ((FrameworkElement)container).FindResource("threadWebView") as DataTemplate;
				case false:
					return ((FrameworkElement)container).FindResource("threadView") as DataTemplate;
			} 
			return base.SelectTemplate(item, container);
		}
	}
}
