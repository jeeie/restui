using IronPython.Runtime.Types;
using Photonware.RestUI.Core;
using Photonware.RestUI.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Actions.NET4
{
    public class PyScriptAction : Photonware.RestUI.Core.Action
    {


        public PyScriptAction()
            : base()
        {
            Key = "python_script_action";
            Text = "Python Script";
            //this.Xshd = Encoding.UTF8.GetString(Photonware.RestUI.Actions.NET4.Properties.Resources.Python);
            Examples.Add(new ActionExample()
            {
                Text="Example",
                Content=
@"
i=1

Context.Variables.Set(""i"", i)

def increase(n):
  return n+1

Output.AppendLine(str(increase(i)));

"

            });
        }

        public override bool Execute(ExecutionAction action, UserCaseExecutionContext context, out object result)
        {
            

            try
            {
                context.ScriptManager.SetVariable("py", "GlobalVariables", DynamicHelpers.GetPythonTypeFromType(typeof(GlobalVariables)));
                context.ScriptManager.SetVariable("py", "success", true);
                context.ScriptManager.SetVariable("py", "Output", new StringBuilder());
                context.ScriptManager.SetVariable("py", "Context", context);
                context.ScriptManager.SetVariable("py", "Json", Photonware.RestUI.Utils.Json.Instance);
                context.ScriptManager.SetVariable("py", "StringUtils", new StringUtils());

                if (action.LoopContext != null)
                {
                    context.ScriptManager.SetVariable("py", "LoopContext", action.LoopContext);
                }

                context.ScriptManager.Run("py", action.Content);

                StringBuilder sb = context.ScriptManager.GetVariable<StringBuilder>("py", "Output");
                result = sb;
                 
                bool ret = context.ScriptManager.GetVariable<bool>("py", "success");
                return ret;

            }
            catch (Exception e)
            {
                result = e.Message;
                return false;
            }


        }


    }
}
