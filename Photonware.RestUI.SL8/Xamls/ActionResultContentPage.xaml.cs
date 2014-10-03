using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Photonware.RestUI.Core;

namespace Photonware.RestUI.SL8.Xamls
{
    public partial class ActionResultContentPage : PhoneApplicationPage
    {
        public ActionResultContentPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    titleBox.Text = App.SelectedExecutionActon.Text;
                    statusBox.Text = App.SelectedExecutionActon.StatusText;
                    
                    //this.textBox.Text = App.SelectedExecutionActon.Result.ToString();
                    this.textBox.NavigateToString(EscapeHtml(App.SelectedExecutionActon.Result.ToString()));
                    //this.textBlockContent.Text = App.SelectedExecutionActon.Result.ToString();

                }
                catch { }
            });
        }

        private string EscapeHtml(string html)
        {
            string str = html.Replace("<", "&lt;").Replace(">", "&gt;");
            str = System.Text.RegularExpressions.Regex.Replace(str, "(?<=[\r]*)\n", "<br/>");
            return string.Format("<html><body>{0}</body></html>", str);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    System.Windows.Clipboard.SetText(App.SelectedExecutionActon.Result.ToString());
                }
                catch { }
            });
        }

    }
}