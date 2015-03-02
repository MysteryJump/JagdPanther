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
    /// Interaction logic for OpenWriteWindow.xaml
    /// </summary>
    public partial class OpenWriteWindow : Window
    {
        public OpenWriteWindow()
        {
            InitializeComponent();
        }

        private void write_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.IsClickWriteButton = true;
        }
        public string WriteCommentData
        {
            get { return this.data.Text; }
        }

        public bool IsClickWriteButton
        {
            get;set;
        }
    }
}
