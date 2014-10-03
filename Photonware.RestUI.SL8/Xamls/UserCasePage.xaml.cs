using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Photonware.RestUI.SL8.ViewModels;
using Photonware.RestUI.Core;
using Photonware.RestUI.SL8.Execution;

namespace Photonware.RestUI.SL8.Xamls
{
    public partial class UserCasePage : PhoneApplicationPage
    {
        public UserCasePage()
        {
            InitializeComponent();
            this.DataContext = App.SelectedUserCaseViewModel;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (App.SelectedUserCaseViewModel != null)
            {
                if (!App.SelectedUserCaseViewModel.IsActionLoaded)
                {
                    App.SelectedUserCaseViewModel.LoadAction();
                }
            }
            
        }

        private void actionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        private void runButton_Click_1(object sender, EventArgs e)
        {
            var uc = App.SelectedUserCaseViewModel.UserCase.Clone();
            UserCaseExecution exec = new UserCaseExecution(uc);
            ExecutionManager.Instance.Add(exec);
            App.SelectedUserCaseExecution = exec;
            App.SelectedUserCase = uc;
            NavigationService.Navigate(new Uri("/Xamls/UserCaseExecutionPage.xaml?operation=run", UriKind.Relative));
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("delete the Usercase?", "Delete", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                UserCaseManager.Instance.RemoveUserCase(App.SelectedUserCaseViewModel.UserCase);
                UserCaseManager.Instance.save();
                App.SelectedUserCaseViewModel = null;
                NavigationService.GoBack();
            }
        }

        private void actionList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector list = sender as LongListSelector;
            if (list.SelectedItem != null)
            {
                App.SelectedCustomActionViewModel = list.SelectedItem as CustomActionViewModel;
                NavigationService.Navigate(new Uri("/Xamls/CustomActionContentPage.xaml", UriKind.Relative));
            }
        }
    }
}