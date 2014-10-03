using Photonware.RestUI.Core;
using Photonware.RestUI.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Photonware.RestUI.WP.Actions
{
    public class SshAction : Photonware.RestUI.Core.Action
    {
        private readonly Regex commandRegex = new Regex(@"^\s*?Command=(.*?)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private readonly Regex serverRegex = new Regex(@"^\s*?Server=(.*?)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private readonly Regex portRegex = new Regex(@"^?\s*?Port=(.*?)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private readonly Regex usernameRegex = new Regex(@"^\s*?Username=(.*?)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private readonly Regex passwordRegex = new Regex(@"^\s*?Password=(.*?)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private readonly Regex shellRegex = new Regex(@"^\s*?Shell=(.*?)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private Renci.SshNet.SshClient sshClient = null;
        private string username = string.Empty;
        private string password = string.Empty;
        private string host = string.Empty;
        private int port = 22;
        private string command = string.Empty;
        private string shell = "bash";

        public SshAction()
        {
            this.Text = "Ssh";
            this.Key = "ssh_action";
            
            AddExample(new ActionExample()
            {
                Text = "ls -l",
                Content = "Server=10.175.183.181\r\nPort=22\r\nUsername=root\r\nPassword=root\r\nCommand=ls -l\r\n"
            });
        }

        public override bool Execute(ExecutionAction action, UserCaseExecutionContext context, out object result)
        {
            result = string.Empty;
            bool success = true;

            string commandText = string.Empty;

            try
            {

                string sshServer = ContextHelper.ExecuteInnerScript(ContextHelper.ReplaceVariaibles(serverRegex.Match(action.Content).Groups[1].Value, context), context);
                string sshPort = ContextHelper.ExecuteInnerScript(ContextHelper.ReplaceVariaibles(portRegex.Match(action.Content).Groups[1].Value, context), context);
                string username = ContextHelper.ExecuteInnerScript(ContextHelper.ReplaceVariaibles(usernameRegex.Match(action.Content).Groups[1].Value, context), context);
                string password = ContextHelper.ExecuteInnerScript(ContextHelper.ReplaceVariaibles(passwordRegex.Match(action.Content).Groups[1].Value, context), context);
                MatchCollection matches = commandRegex.Matches(action.Content);
                List<string> commands = new List<string>();
                foreach (Match m in matches)
                {
                    commands.Add(m.Groups[1].Value);
                }

                if (string.IsNullOrEmpty(sshServer))
                {
                    throw new Exception("no ssh server configured");
                }

                if (string.IsNullOrEmpty(sshPort))
                {
                    throw new Exception("no ssh server port configured");
                }

                string _host = sshServer;
                int _port = Convert.ToInt32(sshPort);

                if (this.SshTunnelManager.IsEnabled)
                {
                    this.SshTunnelManager.GetProxyAddress(sshServer, _port, out _host, out _port);
                }

                using (sshClient = new Renci.SshNet.SshClient(_host, _port, username, password))
                {
                    sshClient.Connect();
                    //Renci.SshNet.SshCommand sshCommand = sshClient.RunCommand(shell);

                    foreach (string command in commands)
                    {
                        string _cmd = ContextHelper.ReplaceVariaibles(command, context);
                        _cmd = ContextHelper.ExecuteInnerScript(_cmd, context);
                        //matches = new Regex("<<([^<>]*?)>>").Matches(_cmd);
                        //foreach (Match m in matches)
                        //{
                        //    if (context.Variables.Exists(m.Groups[1].Value.Trim()))
                        //    {
                        //        _cmd = _cmd.Replace("<<" + m.Groups[1].Value + ">>", context.Variables.Get(m.Groups[1].Value.Trim()).ToString());

                        //    }
                        //    else if (GlobalVariables.Exists(m.Groups[1].Value.Trim()))
                        //    {
                        //        _cmd = _cmd.Replace("<<" + m.Groups[1].Value + ">>", GlobalVariables.Get(m.Groups[1].Value.Trim()).ToString());
                        //    }
                        //}

                        Renci.SshNet.SshCommand sshCommand = sshClient.RunCommand(_cmd);

                        //sshCommand.Execute();
                        if (sshCommand.ExitStatus == 0)
                        {

                        }
                        if (!string.IsNullOrEmpty(sshCommand.Result))
                        {
                            result += sshCommand.Result + "\r\n";
                        }
                    }
                    //if (sshCommand.ExitStatus != 0) success = false;

                }

                
            }
            catch (Exception e)
            {
                result += e.Message + "\r\n";
                success = false;
            }

            return success;
        }
    }
}
