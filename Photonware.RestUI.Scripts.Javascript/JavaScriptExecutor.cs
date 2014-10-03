using Photonware.RestUI.Core.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Scripts
{
    public class JavaScriptExecutor : IScriptExecutor
    {

        private Jint.Engine engine = null;

        public JavaScriptExecutor()
        {
            engine = new Jint.Engine(cfg => cfg.AllowClr());
        }

        public string Name
        {
            get { return "js"; }
        }

        public void Run(string script)
        {
            engine.Execute(script);
        }

        public T Evaluate<T>(string script)
        {
            try
            {
                var result = (T)engine.Execute(script).GetCompletionValue().ToObject();
                return result;
            }
            catch
            {
                return default(T);
            }
        }

        public void SetVariable(string name, object value)
        {
            engine.SetValue(name, value);
        }

        public T GetVariable<T>(string name)
        {
            try
            {
                var result = (T)engine.GetValue(name).ToObject();
                return result;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
