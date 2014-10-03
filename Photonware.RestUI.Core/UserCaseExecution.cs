using Photonware.RestUI.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core
{
    public class UserCaseExecution
    {
        private UserCase userCase = null;
        private ExecutionStatus status = ExecutionStatus.Ready;
        private bool cancelled = false;
        private object runLocker = new object();

        public UserCaseExecution(UserCase userCase)
        {

            this.userCase = userCase;
            this.Status = ExecutionStatus.Ready;
            List<ExecutionAction> list  = new List<ExecutionAction>();

            foreach (CustomAction ca in userCase.Actions)
            {
                if (ca is CustomLoopAction)
                {
                    LoopExecutionAction lea = new LoopExecutionAction()
                    {
                        Text = ca.Text,
                        Key = ca.Key,
                        Content = ca.Content,
                        Result = string.Empty,
                        IsDisabled = ca.IsDisabled,
                        IsTentative = ca.IsTentative
                    };
                    list.Add(lea);

                    foreach (CustomAction ca1 in ((CustomLoopAction)ca).SubActions)
                    {
                        lea.AddSubAction(new ExecutionAction()
                        {
                            Text = ca1.Text,
                            Key = ca1.Key,
                            Content = ca1.Content,
                            Result = string.Empty,
                            IsDisabled = ca1.IsDisabled,
                            IsTentative = ca1.IsTentative
                        });
                    }
                }
                else if (ca is CustomAction)
                {
                    list.Add(new ExecutionAction()
                    {
                        Text = ca.Text,
                        Key = ca.Key,
                        Content = ca.Content,
                        Result = string.Empty,
                        IsDisabled = ca.IsDisabled,
                        IsTentative = ca.IsTentative
                    });
                }
            }
            //this.ExecutionActions = list.AsReadOnly();

            this.Context = new UserCaseExecutionContext(list.ToList<ExecutionAction>());
            
        }

        public event EventHandler<UserCaseExecutionEventArgs> StepExecuted;
        public event EventHandler<UserCaseExecutionEventArgs> Executed;
        public event EventHandler<ExecutionStatusChangedEventArgs> ExecutionStatusChanged;
        public event EventHandler<StepStatusChangedEventArgs> StepStatusChanged;

        public string Text
        {
            get { return this.userCase.Text; }
        }
        public ExecutionStatus Status
        {
            get { return status; }
            private set
            {
                if (value != status)
                {
                    status = value;
                    if (ExecutionStatusChanged != null)
                    {
                        ExecutionStatusChanged(this, new ExecutionStatusChangedEventArgs(status));
                    }
                }
            }
        }

        public UserCaseExecutionContext Context { get; private set; }

        public object SyncRoot { get { return this; } }

        public void run()
        {
            lock (runLocker)
            {
                if (this.cancelled)
                {
                    this.Status = ExecutionStatus.Cancelled;
                    return;
                }

                this.Status = ExecutionStatus.Ready;

                if (userCase == null)
                {
                    this.Status = ExecutionStatus.Fail;
                    return;
                }
            }

            this.Status = ExecutionStatus.Running;

            foreach (ExecutionAction ea in this.Context.ExecutionActions) {
                this.Context.CurrentIndex ++;
                this.Context.CurrentAction = ea;

                bool success = false;
                if (!ea.IsDisabled)
                {

                    RestUI.Core.Action action = ActionManager.Instance.GetAction(ea.Key);

                    if (action == null)
                    {
                        ea.Status = ExecutionStatus.Skipped;
                        continue;
                    }

                    object result = string.Empty;
                    ea.UserCaseExecution = this;
                    ea.Status = ExecutionStatus.Running;


                    if (this.cancelled)
                    {
                        this.Status = ExecutionStatus.Cancelled;
                        return;
                    }

                    success = action.Execute(ea, this.Context, out result);
                    ea.Result = result;

                    if (success)
                    {
                        ea.Status = ExecutionStatus.Success;
                    }
                    else
                    {
                        ea.Status = ExecutionStatus.Fail;
                    }

                    if (StepExecuted != null) StepExecuted(this, new UserCaseExecutionEventArgs(this, ea));

                    if (!ea.IsTentative)
                    {
                        if (!success)
                        {
                            this.Status = ExecutionStatus.Fail;
                            if (Executed != null) Executed(this, new UserCaseExecutionEventArgs(this, null));
                            return;
                        }
                    }

                    this.Context.LastExecutedAction = ea;

                }
                else
                {
                    ea.Status = ExecutionStatus.Disabled;
                }
                
            }

            if (Executed != null) Executed(this, new UserCaseExecutionEventArgs(this, null));

            this.Status = ExecutionStatus.Done;

        }

        public void stop()
        {
            lock (runLocker)
            {
                this.cancelled = true;
                this.Status = ExecutionStatus.Canceling;
            }
        }

        public bool IsRunning()
        {
            return this.Status == ExecutionStatus.Running && (!this.cancelled);
        }

        public bool IsCancelled()
        {
            return this.Status == ExecutionStatus.Canceling || this.Status == ExecutionStatus.Cancelled;
        }

        public void OnStepStatusChanged(ExecutionAction action, ExecutionStatus status)
        {
            if (StepStatusChanged != null)
            {
                this.StepStatusChanged(this, new StepStatusChangedEventArgs(action, status));
            }
        }

    }

    public enum ExecutionStatus
    {
        Ready = 0,
        Running = 1,
        Success = 2,
        Fail = 3,
        Canceling = 4,
        Cancelled = 5,
        Disabled = 6,
        Tentative = 7,
        Skipped = 8,
        Done = 9
    }

    public class UserCaseExecutionEventArgs : EventArgs
    {
        public UserCaseExecution UserCaseExecution { get; private set; }
        public ExecutionAction ExecutionAction { get; private set; }

        public UserCaseExecutionEventArgs(UserCaseExecution userCase, ExecutionAction action)
        {
            this.UserCaseExecution = userCase;
            this.ExecutionAction = action;
        }
    }

    public class ExecutionStatusChangedEventArgs : EventArgs
    {

        public ExecutionStatus Status { get; private set; }

        public ExecutionStatusChangedEventArgs(ExecutionStatus status)
        {

            this.Status = status;
        }
    }

    public class StepStatusChangedEventArgs : EventArgs
    {

        public ExecutionStatus Status { get; private set; }
        public ExecutionAction Action { get; private set; }

        public StepStatusChangedEventArgs(ExecutionAction action, ExecutionStatus status)
        {
            this.Action = action;
            this.Status = status;
        }
    }
}
