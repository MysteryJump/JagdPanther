using JagdPanther.Model;
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
    /// Interaction logic for LoginAndGenerateNewUserWindow.xaml
    /// </summary>
    public partial class LoginAndGenerateNewUserWindow : Window
    {
        public LoginAndGenerateNewUserWindow()
        {
            InitializeComponent();
        }

        private void textBlock5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.reddit.com/help/useragreement");
        }

        private void textBlock6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.reddit.com/help/privacypolicy");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var r = new RedditSharp.Reddit();
            if (this.create_password == this.password_validation)
            {
                if (this.mail.Text == String.Empty)
                    r.RegisterAccount(this.create_user.Text, this.create_password.Text);
                else
                {
                    r.RegisterAccount(this.create_user.Text, this.create_password.Text, this.mail.Text);
                }

				var re = new Account();
				re.UserName = this.create_user.Text;
                re.Password = this.create_password.Text;
				Data = re;
            }
            this.Close();

        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            var r = new Account();
			r.UserName = this.log_user.Text;
            r.Password = this.log_pass.Text;
			Data = r;
            this.Close();
        }

		public Account Data { get; set; }

	}
}
