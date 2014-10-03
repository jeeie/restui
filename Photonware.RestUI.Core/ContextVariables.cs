using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core
{
    public class ContextVariables
    {
        public ContextVariables() { }

        private readonly Dictionary<string, object> vars = new Dictionary<string, object>();

        public void set(string name, object value)
        {
            if (vars.ContainsKey(name))
            {
                vars[name] = value;
            }
            else
            {
                vars.Add(name, value);
            }
        }

        public object get(string name)
        {
            if (vars.ContainsKey(name))
            {
                return vars[name];
            }
            return "";
        }

        public bool exists(string name)
        {
            return (vars.ContainsKey(name));
        }

        public void Set(string name, object value)
        {
            this.set(name, value);
        }

        public object Get(string name)
        {
            return this.get(name);
        }

        public bool Exists(string name)
        {
            return this.exists(name);
        }

        public static void Remove(string name)
        {

        }
    }
}
