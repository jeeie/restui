using Photonware.RestUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photonware.RestUI.SL8.Execution
{
    public class ExecutionManager
    {
        private static ExecutionManager instance = new ExecutionManager();
        public static ExecutionManager Instance { get { return instance; } }
        private ExecutionManager()
        {
            
        }

        private readonly List<UserCaseExecution> usercases = new List<UserCaseExecution>();

        public object SyncRoot { get { return instance; } }

        public IEnumerable<UserCaseExecution> UserCases
        {
            get { return usercases.AsEnumerable<UserCaseExecution>(); }
        }

        public void Add(UserCaseExecution usercase)
        {
            lock (this.SyncRoot)
            {
                usercases.Add(usercase);
            }
        }

        public void Remove(UserCaseExecution usercase)
        {
            lock (this.SyncRoot)
            {
                if (usercases.Contains(usercase))
                {
                    usercases.Remove(usercase);
                }
            }
        }
        public void Clear()
        {
            lock (this.SyncRoot)
            {
                usercases.Clear();
            }
        }
        
    }
}
