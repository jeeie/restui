using Photonware.RestUI.Core.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Scripts
{
    public class PythonExecutor : IScriptExecutor
    {
        private Microsoft.Scripting.Hosting.ScriptEngine engine = null;
        private Microsoft.Scripting.Hosting.ScriptScope scope = null;

        public PythonExecutor()
        {
            engine = IronPython.Hosting.Python.CreateEngine();
            //scope = engine.CreateScope();
            scope = engine.Runtime.Globals;
        }

        public string Name { get { return "py"; } }

        public void Run(string script)
        {
            engine.Execute(script, scope);
        }

        public T Evaluate<T>(string script)
        {
            return engine.Execute<T>(script, scope);
        }

        public void SetVariable(string name, object value)
        {
            scope.SetVariable(name, value);
        }


        public T GetVariable<T>(string name)
        {
            return scope.GetVariable<T>(name);
        }
    }
}
