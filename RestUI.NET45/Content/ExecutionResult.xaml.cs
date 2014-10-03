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
using System.Windows.Threading;

namespace RestUI.Content
{
    /// <summary>
    /// ExecutionResult.xaml 的交互逻辑
    /// </summary>
    public partial class ExecutionResult : UserControl
    {

        private ExecutionAction action = null;

        public ExecutionResult()
        {
            InitializeComponent();
        }

        public void SetExecutionAction(ExecutionAction action)
        {
            if (this.action != null)
            {
                this.action.ExecutionStatusChanged -= action_ExecutionStatusChanged;
            }
            this.action = action;

            if (this.action != null)
            {
                this.action.ExecutionStatusChanged += action_ExecutionStatusChanged;
            }

            this.UpdateContentAsync();
        }

        void action_ExecutionStatusChanged(object sender, ExecutionStatusChangedEventArgs e)
        {
            UpdateContentAsync();
        }

        private void UpdateContentAsync()
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new System.Action(UpdateContent));
        }

        private void UpdateContent()
        {
            contentBox.Text = action == null || action.Content == null ? string.Empty : action.Content;
            resultBox.Text = action == null || action.Result == null ? string.Empty : action.Result.ToString();
            
        }
    }
}
