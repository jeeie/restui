using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Photonware.RestUI.WP.Utils;
using System.Windows.Input;

namespace Photonware.RestUI.SL8.Xamls
{
    public partial class ActionListPage : PhoneApplicationPage
    {
        public ActionListPage()
        {
            InitializeComponent();
            this.portBox.InputScope = new System.Windows.Input.InputScope()
            {
                Names = { new InputScopeName() { NameValue = InputScopeNameValue.Digits } }
            };

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (IsolatedStorageSettings.ApplicationSettings.Contains("useSshTunnel")) {
                bool check = false;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<bool>("useSshTunnel", out check))
                {
                    enableSshCheckBox.IsChecked = check;
                }
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("sshHost"))
            {
                string host = string.Empty;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("sshHost", out host))
                {
                    serverHostBox.Text = host;
                }
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("sshPort"))
            {
                string port = string.Empty;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("sshPort", out port))
                {
                    portBox.Text = port;
                }
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("sshUsername"))
            {
                string username = string.Empty;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("sshUsername", out username))
                {
                    userBox.Text = username;
                }
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("sshPassword"))
            {
                string password = string.Empty;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("sshPassword", out password))
                {
                    passwordBox.Password = password;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("useSshTunnel"))
            {
                IsolatedStorageSettings.ApplicationSettings["useSshTunnel"] = enableSshCheckBox.IsChecked;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("useSshTunnel", enableSshCheckBox.IsChecked);
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("sshHost"))
            {
                IsolatedStorageSettings.ApplicationSettings["sshHost"] = serverHostBox.Text;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("sshHost", serverHostBox.Text);
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("sshPort"))
            {
                IsolatedStorageSettings.ApplicationSettings["sshPort"] = portBox.Text;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("sshPort", portBox.Text);
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("sshUsername"))
            {
                IsolatedStorageSettings.ApplicationSettings["sshUsername"] = userBox.Text;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("sshUsername", userBox.Text);

            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("sshPassword"))
            {
                IsolatedStorageSettings.ApplicationSettings["sshPassword"] = passwordBox.Password;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("sshPassword", passwordBox.Password);
            }
            
            IsolatedStorageSettings.ApplicationSettings.Save();
            decimal port = 22;
            try
            {
                if (!string.IsNullOrEmpty(portBox.Text))
                {
                    port = Convert.ToDecimal(portBox.Text);
                }
            }
            catch { }
            SshTunnelManager.Instance.SetParameters(serverHostBox.Text, (int)port, userBox.Text, passwordBox.Password);
            SshTunnelManager.Instance.IsEnabled = (bool)enableSshCheckBox.IsChecked;
            NavigationService.GoBack();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}