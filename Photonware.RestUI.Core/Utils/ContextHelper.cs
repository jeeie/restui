using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Photonware.RestUI.Core.Utils
{
    public class ContextHelper
    {
        public static string ReplaceVariaibles(string input, UserCaseExecutionContext context)
        {
            string content = input;
            MatchCollection matches = new Regex("<<([^<>]*?)>>").Matches(content);
            foreach (Match m in matches)
            {
                UserCaseExecutionContext _context = context;
                bool found = false;
                while (_context != null)
                {
                    if (_context.Variables.Exists(m.Groups[1].Value.Trim()))
                    {
                        content = content.Replace("<<" + m.Groups[1].Value + ">>", _context.Variables.Get(m.Groups[1].Value.Trim()).ToString());
                        found = true;
                        break;
                    }
                    _context = _context.ParentContext;
                }

                if (!found && GlobalVariables.Exists(m.Groups[1].Value.Trim()))
                {
                    content = content.Replace("<<" + m.Groups[1].Value + ">>", GlobalVariables.Get(m.Groups[1].Value.Trim()).ToString());
                }
            }
            return content;
        }

        public static string ExecuteInnerScript(string input, UserCaseExecutionContext context)
        {
            string content = input;
            MatchCollection matches = new Regex("{{(.*?)}}").Matches(content);
            foreach (Match m in matches)
            {
                var match = Regex.Match(m.Groups[1].Value, "(.*?)||(.*)", RegexOptions.Multiline);
                //string[] token = m.Groups[1].Value.Split(new string[] { "||" }, 2, StringSplitOptions.None);
                if (match.Groups.Count == 2)
                {
                    string replacement = context.ScriptManager.Evaluate<string>(match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim()) as string;
                    content = content.Replace("{{" + m.Groups[1].Value + "}}", replacement);
                }
            }
            return content;
        }
    }
}
