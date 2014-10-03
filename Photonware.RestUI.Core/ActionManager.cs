using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photonware.RestUI.Core
{
    public class ActionManager
    {
        private static readonly ActionManager manager = new ActionManager();

        public static ActionManager Instance { get { return manager; } }

        public Dictionary<string,Action> Actions { get; private set; }

        public object SyncRoot { get { return this; } }

        private ActionManager()
        {
            this.Actions = new Dictionary<string, Action>();

            init();
        }

        private void init()
        {
            
        }

        public Action GetAction(string key)
        {
            lock (this.SyncRoot)
            {
                if (this.Actions.ContainsKey(key))
                {
                    return this.Actions[key];
                }
                return null;
            }
        }
    }
}
