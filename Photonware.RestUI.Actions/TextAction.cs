using Photonware.RestUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Photonware.RestUI.Actions
{
    public class TextAction : RestUI.Core.Action
    {

        public TextAction()
            : base()
        {

            Key = "text_action";
            Text = "Text";
            Description = "";
        }

        public override bool Execute(ExecutionAction action, UserCaseExecutionContext context, out object result)
        {
            try
            {
                string content = action.Content;
                content = RestUI.Core.Utils.ContextHelper.ReplaceVariaibles(content, context);
                result = RestUI.Core.Utils.ContextHelper.ExecuteInnerScript(content, context);
                return true;
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            
            return false;
        }

    }
}
