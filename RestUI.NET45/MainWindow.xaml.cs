using FirstFloor.ModernUI.Windows.Controls;
using Photonware.RestUI.Core;
using System;
using System.Net;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace RestUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {

        private Photonware.RestUI.CommonApi.IFileStore fileUtils = new RestUI.Utils.FileUtils();
 
        public MainWindow()
        {

            //Photonware.RestUI.HttpServer.HttpServer httpServer = new Photonware.RestUI.HttpServer.HttpServer();
            //httpServer.Start();

            InitializeComponent();
            this.Closed += MainWindow_Closed;
            this.Closing += MainWindow_Closing;
            RestUI.Utils.SshTunnelManager.Instance.SetParameters(
                Properties.Settings.Default.SshHost,
                Properties.Settings.Default.SshPort,
                Properties.Settings.Default.SshUsername,
                Properties.Settings.Default.SshPassword
                );
            RestUI.Utils.SshTunnelManager.Instance.IsEnabled = Properties.Settings.Default.EnableSshTunnel;

            // Enable Tls12, due to the following code is unavaible in .net4.0: "System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;"
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );

            Color color = new Color();
            color.A = RestUI.Properties.Settings.Default.ThemeColor.A;
            color.R = RestUI.Properties.Settings.Default.ThemeColor.R;
            color.G = RestUI.Properties.Settings.Default.ThemeColor.G;
            color.B = RestUI.Properties.Settings.Default.ThemeColor.B;
            FirstFloor.ModernUI.Presentation.AppearanceManager.Current.AccentColor = color;

            if (RestUI.Properties.Settings.Default.Theme == "light")
            {
                FirstFloor.ModernUI.Presentation.AppearanceManager.Current.ThemeSource = FirstFloor.ModernUI.Presentation.AppearanceManager.LightThemeSource;
            }
            else if (RestUI.Properties.Settings.Default.Theme == "hello kitty")
            {
                FirstFloor.ModernUI.Presentation.AppearanceManager.Current.ThemeSource = new Uri("/RestUI;component/Assets/HelloKitty.xaml", UriKind.Relative);
            }
            else
            {
                FirstFloor.ModernUI.Presentation.AppearanceManager.Current.ThemeSource = FirstFloor.ModernUI.Presentation.AppearanceManager.DarkThemeSource;
            }


            UserCaseManager.Instance.FileUtils = fileUtils;
            UserCaseManager.Instance.init();

            // start to init actions
            lock (ActionManager.Instance.SyncRoot)
            {
                Photonware.RestUI.Actions.NET4.RestAction restAction = new Photonware.RestUI.Actions.NET4.RestAction();
                ActionManager.Instance.Actions.Add(restAction.Key, restAction);
                restAction.SshTunnelManager = RestUI.Utils.SshTunnelManager.Instance;

                Photonware.RestUI.Actions.NET4.PyScriptAction pyAction = new Photonware.RestUI.Actions.NET4.PyScriptAction();
                ActionManager.Instance.Actions.Add(pyAction.Key, pyAction);
                pyAction.SshTunnelManager = RestUI.Utils.SshTunnelManager.Instance;
                try
                {
                    string[] lines = fileUtils.ReadFromFile("Xshd" + fileUtils.PathSeparater + "Python.xshd");
                    StringBuilder sb = new StringBuilder();
                    foreach (string line in lines)
                    {
                        sb.AppendLine(line);
                    }
                    pyAction.Xshd = sb.ToString();
                }
                catch { }

                Photonware.RestUI.Actions.JavaScriptAction jsAction = new Photonware.RestUI.Actions.JavaScriptAction();
                ActionManager.Instance.Actions.Add(jsAction.Key, jsAction);
                jsAction.SshTunnelManager = RestUI.Utils.SshTunnelManager.Instance;
                try
                {
                    string[]  lines = fileUtils.ReadFromFile("Xshd" + fileUtils.PathSeparater + "Javascript.xshd");
                    StringBuilder sb = new StringBuilder();
                    foreach (string line in lines)
                    {
                        sb.AppendLine(line);
                    }
                    jsAction.Xshd = sb.ToString();
                }
                catch { }
#if MYSQL
                Photonware.RestUI.Actions.NET4.MySqlAction mysqlAction = new Photonware.RestUI.Actions.NET4.MySqlAction();
                ActionManager.Instance.Actions.Add(mysqlAction.Key, mysqlAction);
                mysqlAction.SshTunnelManager = RestUI.Utils.SshTunnelManager.Instance;
#endif
                Photonware.RestUI.Actions.NET4.SshAction sshAction = new Photonware.RestUI.Actions.NET4.SshAction();
                ActionManager.Instance.Actions.Add(sshAction.Key, sshAction);
                sshAction.SshTunnelManager = RestUI.Utils.SshTunnelManager.Instance;

                Photonware.RestUI.Actions.SleepAction sleepAction = new Photonware.RestUI.Actions.SleepAction();
                ActionManager.Instance.Actions.Add(sleepAction.Key, sleepAction);
                sleepAction.SshTunnelManager = RestUI.Utils.SshTunnelManager.Instance;

                Photonware.RestUI.Actions.TextAction textAction = new Photonware.RestUI.Actions.TextAction();
                ActionManager.Instance.Actions.Add(textAction.Key, textAction);
                textAction.SshTunnelManager = RestUI.Utils.SshTunnelManager.Instance;
                
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (UserCaseManager.Instance.Modified)
            {
                var v = new FirstFloor.ModernUI.Windows.Controls.ModernDialog
                {
                    Title = "Save",
                    Content = "User case modified, save it?",
                    MinWidth = 400
                };

                var yesCommand = new FirstFloor.ModernUI.Presentation.RelayCommand(o =>
                {
                    v.DialogResult = true;
                    v.Close();
                });

                var cancelCommand = new FirstFloor.ModernUI.Presentation.RelayCommand(o =>
                {
                    v.DialogResult = false;
                    e.Cancel = true;
                });

                v.YesButton.Command = yesCommand;
                v.CancelButton.Command = cancelCommand;

                v.Buttons = new Button[] { v.YesButton, v.NoButton, v.CancelButton };

                v.ShowDialog();
                if ((bool)v.DialogResult)
                {
                    UserCaseManager.Instance.save();
                }
            }

        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            RestUI.Utils.SshTunnelManager.Instance.CloseSshClient();
        }

    }
}
