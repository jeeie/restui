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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestUI.Pages
{
    /// <summary>
    /// Interaction logic for SshTunnelSettings.xaml
    /// </summary>
    public partial class SshTunnelSettings : UserControl
    {
        public SshTunnelSettings()
        {
            InitializeComponent();
            this.Loaded += Home_Loaded;
            
        }

        void Home_Loaded(object sender, RoutedEventArgs e)
        {
            this.resetButton_Click(null, null);
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.EnableSshTunnel = this.enableSshTunnel.IsChecked.Value;
            Properties.Settings.Default.SshUsername = this.sshUsername.Text.Trim();
            Properties.Settings.Default.SshPassword = this.sshPassword.Password.Trim();
            Properties.Settings.Default.SshHost = this.sshHost.Text.Trim();
            Properties.Settings.Default.SshPort = Convert.ToInt32(this.sshPort.Text);
            Properties.Settings.Default.Save();

            RestUI.Utils.SshTunnelManager.Instance.SetParameters(
                Properties.Settings.Default.SshHost,
                Properties.Settings.Default.SshPort,
                Properties.Settings.Default.SshUsername,
                Properties.Settings.Default.SshPassword
                );
            RestUI.Utils.SshTunnelManager.Instance.IsEnabled = Properties.Settings.Default.EnableSshTunnel;

        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            this.enableSshTunnel.IsChecked = Properties.Settings.Default.EnableSshTunnel;
            this.sshUsername.Text = Properties.Settings.Default.SshUsername;
            this.sshPassword.Password = Properties.Settings.Default.SshPassword;
            this.sshHost.Text = Properties.Settings.Default.SshHost;
            this.sshPort.Text = Convert.ToString(Properties.Settings.Default.SshPort);
        }
    }
}
