﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JagdPanther.View
{
	public interface ISettingControl
	{
		void Save();
		void Load();
	}
}
