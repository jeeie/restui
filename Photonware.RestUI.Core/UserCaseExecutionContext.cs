using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core
{
    public class UserCaseExecutionContext
    {

        private object scriptManagerlocker = new object();
        private Photonware.RestUI.Core.Scripts.ScriptManager scriptManager = null;
        private List<UserCaseExecutionContext> contexts = new List<UserCaseExecutionContext>();
        private UserCaseExecutionContext parent = null;

        public UserCaseExecutionContext ParentContext
        {
            get { return parent; }
        }
        public IEnumerable<UserCaseExecutionContext> SubContexts
        {
            get { return contexts.AsEnumerable<UserCaseExecutionContext>(); }
        }
        public List<ExecutionAction> ExecutionActions { get; private set; }
        public ExecutionAction LastExecutedAction { get; set; }
        public int CurrentIndex { get; set; }
        public int LoopIndex { get; set; }
        public ExecutionAction CurrentAction { get; set; }
        public ExecutionAction NextAction
        {
            get
            {
                return this.GetActionByIndex(CurrentIndex + 1);
            }
        }
        public ContextVariables Variables { get; private set; }

        public Photonware.RestUI.Core.Scripts.ScriptManager ScriptManager
        {
            get
            {
                if (scriptManager != null)
                {
                    return scriptManager;
                }
                else
                {
                    lock (scriptManagerlocker)
                    {
                        if (scriptManager == null)
                        {
                            scriptManager = Photonware.RestUI.Core.Scripts.ScriptManager.Create();
                        }
                        return scriptManager;
                    }
                }
            }
            private set
            {
                lock (scriptManagerlocker)
                {
                    this.scriptManager = value;
                }
            }
        }

        public UserCaseExecutionContext(List<ExecutionAction> actions)
        {
            this.ExecutionActions = actions;
            //this.ScriptManager = Scripts.ScriptManager.Create();

            this.LastExecutedAction = null;
            this.CurrentIndex = -1;
            this.LoopIndex = 0;
            this.CurrentAction = null;
            this.Variables = new ContextVariables();
        }

        public UserCaseExecutionContext(List<ExecutionAction> actions, UserCaseExecutionContext parent)
        {
            this.ExecutionActions = actions;
            this.scriptManager = parent.scriptManager;

            this.LastExecutedAction = null;
            this.CurrentIndex = -1;
            this.LoopIndex = 0;
            this.CurrentAction = null;
            this.Variables = new ContextVariables();

        }

        public ExecutionAction GetFirstAction()
        {
            if (this.ExecutionActions == null || this.ExecutionActions.Count == 0)
            {
                return null;
            }

            return this.ExecutionActions[0];
        }

        public ExecutionAction GetLastAction(int index)
        {
            if (this.ExecutionActions == null || this.ExecutionActions.Count == 0)
            {
                return null;
            }
            
            if (this.ExecutionActions.Count - 1 + index < 0) return null;

            if (index >= 0)
            {
                return this.ExecutionActions[this.ExecutionActions.Count - 1];
            }
            else
            {
                return this.ExecutionActions[this.ExecutionActions.Count - 1 + index];
            }

        }

        public ExecutionAction GetLastAction()
        {
            return GetLastAction(0);
        }

        public ExecutionAction GetActionByIndex(int index)
        {
            if (this.ExecutionActions == null || this.ExecutionActions.Count == 0)
            {
                return null;
            }

            if (index < 0)
            {
                return this.ExecutionActions[0];
            }
            else if (index < this.ExecutionActions.Count)
            {
                return this.ExecutionActions[index];
            }
            else
            {
                return null;
            }
        }

        public ExecutionAction GetActionFromCurrentPosition(int index)
        {
            if (this.ExecutionActions == null || this.ExecutionActions.Count == 0)
            {
                return null;
            }

            if (index + CurrentIndex < 0)
            {
                return this.ExecutionActions[0];
            }
            else if (index + CurrentIndex < this.ExecutionActions.Count)
            {
                return this.ExecutionActions[index + CurrentIndex];
            }
            else
            {
                return null;
            }
        }

        public void AddSubContext(UserCaseExecutionContext context)
        {
            this.contexts.Add(context);
        }

        public void RemoveSubContext(UserCaseExecutionContext context)
        {
            if (this.contexts.Contains(context))
            {
                this.contexts.Remove(context);
            }
        }

    }
}
