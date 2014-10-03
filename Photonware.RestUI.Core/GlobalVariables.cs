using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core
{
    public class GlobalVariables
    {
        private static readonly GlobalVariables instance = new GlobalVariables();

        public static GlobalVariables Instance { get { return instance; } }

        private GlobalVariables() { }

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

        public static void Set(string name, object value)
        {
            Instance.set(name, value);
        }

        public static object Get(string name)
        {
            return Instance.get(name);
        }

        public static bool Exists(string name)
        {
            return Instance.exists(name);
        }

        public static void Remove(string name)
        {

        }
    }
}
