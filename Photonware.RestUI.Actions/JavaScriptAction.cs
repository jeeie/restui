using Photonware.RestUI.Core;
using Photonware.RestUI.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Actions
{
    public class JavaScriptAction : Photonware.RestUI.Core.Action
    {


        public JavaScriptAction()
            : base()
        {
            Key = "javascript_action";
            Text = "Java Script";
            //this.Xshd = Encoding.UTF8.GetString(Photonware.RestUI.Actions.NET4.Properties.Resources.Python);
        }

        public override bool Execute(ExecutionAction action, UserCaseExecutionContext context, out object result)
        {
            

            try
            {
                context.ScriptManager.SetVariable("js", "GlobalVariables", GlobalVariables.Instance);
                context.ScriptManager.SetVariable("js", "success", true);
                context.ScriptManager.SetVariable("js", "Output", new StringBuilder());
                context.ScriptManager.SetVariable("js", "Context", context);
                context.ScriptManager.SetVariable("js", "Json", Photonware.RestUI.Utils.Json.Instance);
                context.ScriptManager.SetVariable("js", "StringUtils", new StringUtils());

                if (action.LoopContext != null)
                {
                    context.ScriptManager.SetVariable("js", "LoopContext", action.LoopContext);
                }

                context.ScriptManager.Run("js", action.Content);

                StringBuilder sb = context.ScriptManager.GetVariable<StringBuilder>("js", "Output");
                result = sb;
                 
                bool ret = context.ScriptManager.GetVariable<bool>("js", "success");
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
