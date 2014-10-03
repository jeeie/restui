using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using System.IO;

namespace Photonware.RestUI.Core
{
    public abstract class Action
    {
        public string Key { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string Xshd { get; set; }
        public Photonware.RestUI.Utils.DefaultSshTunnelManager SshTunnelManager { get; set; }

        public List<ActionExample> Examples { get; protected set; }

        public Action()
        {
            this.Examples = new List<ActionExample>();
            this.SshTunnelManager = null;
        }

        public virtual void AddExample(ActionExample example)
        {
            Examples.Add(example);
        }

        public abstract bool Execute(ExecutionAction action, UserCaseExecutionContext context, out object result);

        public override string ToString()
        {
            return Text;
        }

    }
}
