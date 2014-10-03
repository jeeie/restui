using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core
{
    public class LoopExecutionAction : ExecutionAction
    {
        private object runningLocker = new object();

        private List<ExecutionAction> actions = new List<ExecutionAction>();

        public IEnumerable<ExecutionAction> SubActions
        {
            get
            {
                lock (runningLocker)
                {
                    return actions.AsEnumerable<ExecutionAction>();
                }
            }
        }

        public void AddSubAction(ExecutionAction action)
        {
            lock (this.runningLocker)
            {
                this.actions.Add(action);
            }
        }

    }
}
