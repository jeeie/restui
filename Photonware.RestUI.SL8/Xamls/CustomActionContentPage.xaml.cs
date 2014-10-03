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
    public partial class CustomActionContentPage : PhoneApplicationPage
    {
        public CustomActionContentPage()
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

            if (App.SelectedCustomActionViewModel != null)
            {
                this.DataContext = App.SelectedCustomActionViewModel;
            }
        }

        private void wrapButton_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var button = sender;
                if (this.textBox.TextWrapping == TextWrapping.Wrap)
                {
                    this.textBox.TextWrapping = TextWrapping.NoWrap;
                    (button as ApplicationBarIconButton).Text = "wrap";
                }
                else
                {
                    (button as ApplicationBarIconButton).Text = "unwrap";
                    this.textBox.TextWrapping = TextWrapping.Wrap;
                }
            });

        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            this.textBox.Text = App.SelectedCustomActionViewModel.Content;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            App.SelectedCustomActionViewModel.CustomAction.Content = this.textBox.Text;
            App.SelectedCustomActionViewModel.Content = this.textBox.Text;
            UserCaseManager.Instance.save();
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}