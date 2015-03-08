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
    /// Interaction logic for NewBoardWindow.xaml
    /// </summary>
    public partial class NewBoardWindow : Window
    {
        public NewBoardWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
			IsOk = true;
            this.Close();
        }

        public string BoardNameText { get { return this.textBox.Text; } }

        public string UrlText { get { return this.textBox_Copy.Text; } }

        public Model.BoardLocate BoardType
        {
            get
            {
                return (Model.BoardLocate)Enum.Parse(typeof(Model.BoardLocate),((this.comboBox.SelectedItem as ComboBoxItem).Name));
            }
        }

		public bool IsOk { get; set; }

	}
}
