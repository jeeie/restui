using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestUI.Utils
{
    public class Json
    {
        public static JObject ParseJsonString(string jsonString)
        {
            dynamic o = JObject.Parse(jsonString);
            return o;
        }

        public static string ToString(dynamic o)
        {
            if (o == null) return "null";
            return o.ToString();
        }

        public static void AddJToken(JObject o, string key, string value)
        {
            o.Add(key, JToken.Parse(value));
        }

        public static void AddJProperty(JObject o, string key, object value)
        {
            o.Add(new JProperty(key, value));
        }

        public static dynamic AddJObject(JArray o, string value)
        {
            var obj = JToken.Parse(value);
            o.Add(obj);
            return obj;
        }

        public static dynamic Get(JObject o, string key)
        {
            var obj = o[key];
            return obj; ;
        }

        public static dynamic GetByIndex(dynamic o, int index) {
            var obj = o[index];
            return obj;
        }

        public static bool Compare(JObject o1, JObject o2)
        {
            var ret= JToken.DeepEquals(o1, o2);
            return ret;
        }

        public static JArray CreateJArray()
        {
            return new JArray();
        }

        public static JObject CreateJObject()
        {
            return new JObject();
        }
    }
}
