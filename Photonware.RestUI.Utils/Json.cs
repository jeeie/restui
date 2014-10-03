using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Utils
{
    public class Json
    {

        private static Json instance = new Json();

        public static Json Instance { get { return instance; } }

        private Json()
        {

        }

        public JObject ParseJsonString(string jsonString)
        {
            JObject o = JObject.Parse(jsonString);
            return o;
        }

        public string ToString(object o)
        {
            if (o == null) return "null";
            return o.ToString();
        }

        public void AddJToken(JObject o, string key, string value)
        {
            o.Add(key, JToken.Parse(value));
        }

        public void AddJProperty(JObject o, string key, object value)
        {
            o.Add(new JProperty(key, value));
        }

        public dynamic AddJObject(JArray o, string value)
        {
            var obj = JToken.Parse(value);
            o.Add(obj);
            return obj;
        }

        public dynamic Get(JObject o, string key)
        {
            var obj = o[key];
            return obj; ;
        }

        public dynamic GetByIndex(JArray o, int index) {
            var obj = o[index];
            return obj;
        }

        public bool Compare(JObject o1, JObject o2)
        {
            var ret= JToken.DeepEquals(o1, o2);
            return ret;
        }

        public JArray CreateJArray()
        {
            return new JArray();
        }

        public JObject CreateJObject()
        {
            return new JObject();
        }
    }
}
