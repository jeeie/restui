using Photonware.RestUI.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace RestUI.Content
{
    /// <summary>
    /// ExecutionStepList.xaml 的交互逻辑
    /// </summary>
    public partial class ExecutionStepList : UserControl
    {
        public event EventHandler ActionSelected;


        private UserCaseExecution userCase = null;

        public ExecutionStepList()
        {
            InitializeComponent();
            this.Loaded += ExecutionStepList_Loaded;
            this.Unloaded += ExecutionStepList_Unloaded;
            
        }

        void ExecutionStepList_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.userCase != null)
            {
                this.userCase.StepExecuted -= userCase_StepExecuted;
                this.userCase.StepStatusChanged -= userCase_StepStatusChanged;
            }
            this.actionListView.SelectionChanged -= actionListView_SelectionChanged;
        }

        void ExecutionStepList_Loaded(object sender, RoutedEventArgs e)
        {
            this.actionListView.SelectionChanged += actionListView_SelectionChanged;
        }

        void actionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ActionSelected != null)
            {
                this.ActionSelected(this.actionListView.SelectedItem, EventArgs.Empty);
            }
        }

        public void SetUserCaseExecution(UserCaseExecution userCase)
        {
            this.userCase = userCase;
            if (this.userCase != null)
            {
                this.actionListView.ItemsSource = new ObservableCollection<ExecutionAction>(userCase.Context.ExecutionActions);
                this.userCase.StepExecuted += userCase_StepExecuted;
                this.userCase.StepStatusChanged += userCase_StepStatusChanged;
            }

        }

        void userCase_StepStatusChanged(object sender, StepStatusChangedEventArgs e)
        {
            this.UpdateContentAsync();
        }

        void userCase_StepExecuted(object sender, UserCaseExecutionEventArgs e)
        {
            this.UpdateContentAsync();
        }

        private void UpdateContentAsync()
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new System.Action(UpdateContent));
        }

        private void UpdateContent()
        {
            this.actionListView.ItemsSource = new ObservableCollection<ExecutionAction>(userCase.Context.ExecutionActions);
            
        }
    }
}
