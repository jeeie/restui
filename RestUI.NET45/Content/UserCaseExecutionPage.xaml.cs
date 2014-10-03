using Photonware.RestUI.Core;
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

namespace RestUI.Content
{
    /// <summary>
    /// UserCaseExecutionPage.xaml 的交互逻辑
    /// </summary>
    public partial class UserCaseExecutionPage : UserControl
    {

        private UserCase userCase = null;
        private UserCaseExecution userCaseExecution = null;
        private Task task = null;

        public UserCaseExecutionPage()
        {
            InitializeComponent();
            this.Loaded += UserCaseExecutionPage_Loaded;
            this.Unloaded += UserCaseExecutionPage_Unloaded;
        }

        void UserCaseExecutionPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.userCaseExecution != null)
            {
                this.userCaseExecution.stop();
                this.userCaseExecution.StepExecuted -= userCaseExecution_StepExecuted;
                this.userCaseExecution.Executed -= userCaseExecution_Executed;
                this.userCaseExecution.ExecutionStatusChanged -= userCaseExecution_ExecutionStatusChanged;

            }
            if (task != null)
            {
                
            }
        }

        void UserCaseExecutionPage_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Loaded");
            
            
            task = new Task(() =>
            {
                this.userCaseExecution.Context.ScriptManager.AddScriptExecutor(new Photonware.RestUI.Scripts.PythonExecutor());
                this.userCaseExecution.Context.ScriptManager.AddScriptExecutor(new Photonware.RestUI.Scripts.JavaScriptExecutor());

                this.userCaseExecution.run();
            });
            task.Start();
        }

        public UserCase UserCase
        {
            get { return userCase; }
            set
            {
                if (this.userCase != value)
                {
                    this.userCase = value;
                    this.titleBlock.Text = this.userCase.Text;

                    if (this.userCaseExecution != null)
                    {
                        this.userCaseExecution.StepExecuted -= userCaseExecution_StepExecuted;
                        this.userCaseExecution.Executed -= userCaseExecution_Executed;
                        this.userCaseExecution.ExecutionStatusChanged -= userCaseExecution_ExecutionStatusChanged;
                        
                    }

                    this.userCaseExecution = new UserCaseExecution(this.userCase);
                    this.userCaseExecution.StepExecuted += userCaseExecution_StepExecuted;
                    this.userCaseExecution.Executed += userCaseExecution_Executed;
                    this.userCaseExecution.ExecutionStatusChanged += userCaseExecution_ExecutionStatusChanged;

                    this.executionStepList.SetUserCaseExecution(this.userCaseExecution);
                }
            }
        }

        void userCaseExecution_ExecutionStatusChanged(object sender, ExecutionStatusChangedEventArgs e)
        {
            
        }

        void userCaseExecution_Executed(object sender, UserCaseExecutionEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("UserCase Finished");
        }

        void userCaseExecution_StepExecuted(object sender, UserCaseExecutionEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("one step executed");
        }

        private void executionStepList_ActionSelected(object sender, EventArgs e)
        {
            executionResult.SetExecutionAction(sender as ExecutionAction);
        }
    }
}
