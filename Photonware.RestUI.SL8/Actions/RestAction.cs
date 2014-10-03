﻿using Photonware.RestUI.Actions;
using Photonware.RestUI.Core;
using Photonware.RestUI.WP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Photonware.RestUI.WP.Actions
{
    public partial class RestAction : Photonware.RestUI.Actions.RestAction
    {

        public RestAction()
            : base()
        {
            this._addExamples();
        }

        private void _addExamples()
        {
            this.AddExample(new ActionExample()
            {
                Text = "Google.com",
                Content = "GET http://www.google.com HTTP/1.1"
            });

        }

        public override bool Execute(ExecutionAction action, UserCaseExecutionContext context, out object result)
        {
            
            try
            {
                string request = action.Content;
                MatchCollection matches = new Regex("<<([^<>]*?)>>").Matches(request);
                foreach (Match m in matches)
                {
                    if (context.Variables.Exists(m.Groups[1].Value.Trim()))
                    {
                        request = request.Replace("<<" + m.Groups[1].Value + ">>", context.Variables.Get(m.Groups[1].Value.Trim()).ToString());

                    }
                    else if (GlobalVariables.Exists(m.Groups[1].Value.Trim()))
                    {
                        request = request.Replace("<<" + m.Groups[1].Value + ">>", GlobalVariables.Get(m.Groups[1].Value.Trim()).ToString());
                    }
                }

                matches = new Regex("{{(.*?)}}").Matches(request);
                foreach (Match m in matches)
                {
                    string[] token = m.Groups[1].Value.Split(new string[] { "||" }, 2, StringSplitOptions.None);
                    if (token.Length == 2)
                    {
                        string replacement = context.ScriptManager.Evaluate<string>(token[0].Trim(), token[1].Trim());
                        request = request.Replace("{{" + m.Groups[1].Value + "}}", replacement);
                    }
                }

                HttpClient client = new HttpClient();
                client.SshTunnelManager = this.SshTunnelManager;

                bool ret = client.SendRequest(request);
                result = new RestActionResult(client.StatusCode, client.RequestHeader, client.RequestContent, client.ResponseHeader, client.ResponseContent, client.ResponseData);
                //result = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", client.RequestHeader, client.RequestContent, client.ResponseHeader, client.ResponseContent);
                return ret;
            }
            catch (Exception e)
            {
                result = new RestActionResult(0, "", "", "", e.Message, null);
                return false;
            }
        }
    }
}
