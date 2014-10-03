using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core
{
    public class LoopExecutionContext
    {

        private object scriptManagerlocker = new object();
        private RestUI.Core.Scripts.ScriptManager scriptManager = null;

        public ReadOnlyCollection<ExecutionAction> ExecutionActions { get; private set; }
        public ExecutionAction LastExecutedAction { get; set; }
        public int CurrentIndex { get; set; }
        public ExecutionAction CurrentAction { get; set; }
        public ExecutionAction NextAction
        {
            get
            {
                return this.GetActionByIndex(CurrentIndex + 1);
            }
        }
        public ContextVariables Variables { get; private set; }

        public RestUI.Core.Scripts.ScriptManager ScriptManager
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
                            scriptManager = RestUI.Core.Scripts.ScriptManager.Create();
                        }
                        return scriptManager;
                    }
                }
            }
        }

        public LoopExecutionContext(ReadOnlyCollection<ExecutionAction> actions)
        {
            this.ExecutionActions = actions;
            //this.ScriptManager = Scripts.ScriptManager.Create();

            this.LastExecutedAction = null;
            this.CurrentIndex = -1;
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
        

    }
}
