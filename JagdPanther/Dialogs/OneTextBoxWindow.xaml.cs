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
    /// Interaction logic for OneTextBoxWindow.xaml
    /// </summary>
    public partial class OneTextBoxWindow : Window
    {
        public OneTextBoxWindow(string label,string buttonText)
        {
            InitializeComponent();
            this.LabelText = this.Title = label;
            this.ButtonText = buttonText;
        }

        public OneTextBoxWindow(string label, string buttonText,string title)
        {
            InitializeComponent();
            this.LabelText = label;
            this.Title = title;
            this.ButtonText = buttonText;
        }

        public string LabelText
        {
            set
            {
                this.textBlock.Text = value;
            }
        }
        public string TextBoxContent { get { return this.textBox.Text; } }
        public string WindowTitle { set { this.Title = value; } }
        public string ButtonText { set { this.button.Content = value; } }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
