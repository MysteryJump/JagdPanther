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
using System.Windows.Shapes;

namespace JagdPanther.Dialogs
{
    /// <summary>
    /// Interaction logic for Version.xaml
    /// </summary>
    public partial class VersionWindow : Window
    {
        public VersionWindow()
        {
            InitializeComponent();
            this.textBlock.Text = $"Now version is {Model.ReadonlyVars.ProgramVer}";

        }
    }
}
