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
using System.Collections.ObjectModel;

namespace Photonware.RestUI.SL8.Xamls
{
    public partial class UserCaseExecutionPage : PhoneApplicationPage
    {

        private UserCase userCase = null;
        private UserCaseExecutionViewModel viewModel = null;
        private UserCaseExecution userCaseExecution = null;
             
        public UserCaseExecutionPage()
        {
            InitializeComponent();
            this.userCase = App.SelectedUserCase;
            //userCaseExecution = App.UserCaseExecution;

        }

        ~UserCaseExecutionPage()
        {
            this.userCaseExecution.StepExecuted -= userCaseExecution_StepExecuted;
            this.userCaseExecution.StepStatusChanged -= userCaseExecution_StepStatusChanged;
        }

        void userCaseExecution_StepStatusChanged(object sender, StepStatusChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("step status changed " + e.Action.Text + " " + e.Action.Status);
            this.Dispatcher.BeginInvoke(() =>
            {
                lock (viewModel)
                {
                    viewModel.LoadData();
                }

            });
            
        }

        void userCaseExecution_StepExecuted(object sender, UserCaseExecutionEventArgs e)
        {
           /* System.Diagnostics.Debug.WriteLine("step executed " + e.ExecutionAction.Text);

            Dispatcher.BeginInvoke(() =>
            {
                lock (viewModel)
                {
                    //viewModel.LoadData();
                }
                this.actionList.ItemsSource = new ObservableCollection<ExecutionAction>(this.userCaseExecution.Context.ExecutionActions);
            });
            */
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string operation = string.Empty;
            try
            {
                operation = NavigationContext.QueryString["operation"];
            }
            catch { }

            if (operation.Trim().ToLower() == "show")
            {
                this.userCaseExecution = App.SelectedUserCaseExecution;
                viewModel = new UserCaseExecutionViewModel() { UserCaseExecution = App.SelectedUserCaseExecution };
                this.userCaseExecution.StepExecuted += userCaseExecution_StepExecuted;
                this.userCaseExecution.StepStatusChanged += userCaseExecution_StepStatusChanged;
                viewModel.LoadData();
                this.DataContext = viewModel;
            }
            else
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    //this.userCaseExecution = new UserCaseExecution(this.userCase);
                    this.userCaseExecution = App.SelectedUserCaseExecution;
                    viewModel = new UserCaseExecutionViewModel() { UserCaseExecution = App.SelectedUserCaseExecution };
                    this.userCaseExecution.StepExecuted += userCaseExecution_StepExecuted;
                    this.userCaseExecution.StepStatusChanged += userCaseExecution_StepStatusChanged;

                    System.Threading.Tasks.Task.Run(() =>
                    {
                        if (!viewModel.IsDataLoaded) viewModel.LoadData();

                        Dispatcher.BeginInvoke(() =>
                        {
                            this.DataContext = viewModel;
                        });

                        this.userCaseExecution.Context.ScriptManager.AddScriptExecutor(App.JavaScriptExecutor);

                        this.userCaseExecution.run();
                    });
                }
            }
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            /*
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (this.userCaseExecution != null)
                {
                    this.userCaseExecution.stop();
                }
            }*/
        }

        private void actionList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                App.SelectedExecutionActon = listBox.SelectedItem as ExecutionAction;
                NavigationService.Navigate(new Uri("/Xamls/ActionResultContentPage.xaml", UriKind.Relative));

            }
        }
    }
}