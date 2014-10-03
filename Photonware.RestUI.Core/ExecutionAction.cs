using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core
{
    public class ExecutionAction : CustomAction
    {

        public event EventHandler<ExecutionStatusChangedEventArgs> ExecutionStatusChanged;

        private ExecutionStatus status = ExecutionStatus.Ready;

        //public string Text { get; set; }
        //public string Key { get; set; }
        //public string Content { get; set; }
        public object Result { get; set; }
        public ExecutionStatus Status
        {
            get { return status; }
            set
            {
                if (value != status)
                {
                    status = value;
                    if (ExecutionStatusChanged != null)
                    {
                        ExecutionStatusChanged(this, new ExecutionStatusChangedEventArgs(status));
                    }
                    if (this.UserCaseExecution != null)
                    {
                        this.UserCaseExecution.OnStepStatusChanged(this, status);
                    }
                }
            }
        }

        public string StatusText { get { return Convert.ToString(this.Status); } }
        public UserCaseExecution UserCaseExecution { get; set; }

        public LoopExecutionContext LoopContext
        {
            get;
            set;
        }

        public ExecutionAction()
        {
            this.Text = string.Empty;
            this.Key = string.Empty;
            this.Content = string.Empty;
            this.Result = string.Empty;
            this.Status = ExecutionStatus.Ready;
            this.LoopContext = null;
        }

        public override string ToString()
        {
            return this.Text;
        }
    }


}
