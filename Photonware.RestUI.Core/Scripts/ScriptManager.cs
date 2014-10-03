using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core.Scripts
{
    public class ScriptManager
    {

        private readonly List<IScriptExecutor> executors = new List<IScriptExecutor>();

        public ScriptManager()
        {
            //executors.Add(new PythonExecutor());
            //executors.Add(new CSScriptExecutor());
        }

        public static ScriptManager Create()
        {
            return new ScriptManager();
        }

        public void AddScriptExecutor(IScriptExecutor scriptExecutor)
        {
            executors.Add(scriptExecutor);
        }

        public void Run(string scriptName, string script)
        {
            IScriptExecutor executor = null;
            foreach (IScriptExecutor exe in executors)
            {
                if (exe.Name.Equals(scriptName, StringComparison.CurrentCultureIgnoreCase))
                {
                    executor = exe;
                    break;
                }
            }

            if (executor != null)
            {
                executor.Run(script);
            }
        }

        public T Evaluate<T>(string scriptName, string script)
        {
            IScriptExecutor executor = null;
            foreach (IScriptExecutor exe in executors)
            {
                if (exe.Name.Equals(scriptName, StringComparison.CurrentCultureIgnoreCase))
                {
                    executor = exe;
                    break;
                }
            }

            if (executor != null)
            {
                return executor.Evaluate<T>(script);
            }
            return default(T);
        }

        public void SetVariable(string scriptName, string varName, object varValue)
        {
            IScriptExecutor executor = null;
            foreach (IScriptExecutor exe in executors)
            {
                if (exe.Name.Equals(scriptName, StringComparison.CurrentCultureIgnoreCase))
                {
                    executor = exe;
                    break;
                }
            }

            if (executor != null)
            {
                executor.SetVariable(varName, varValue);
            }
        }

        public T GetVariable<T>(string scriptName, string varName)
        {
            IScriptExecutor executor = null;
            foreach (IScriptExecutor exe in executors)
            {
                if (exe.Name.Equals(scriptName, StringComparison.CurrentCultureIgnoreCase))
                {
                    executor = exe;
                    break;
                }
            }

            if (executor != null)
            {
                return executor.GetVariable<T>(varName);
            }

            return default(T);
        }
    }
}
