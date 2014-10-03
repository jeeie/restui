using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Photonware.RestUI.Core
{
    public class CustomAction
    {
        public string Text { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsTentative { get; set; }

        public CustomAction()
        {
            this.Text = string.Empty;
            this.Key = string.Empty;
            this.Content = string.Empty;
            this.IsDisabled = false;
            this.IsTentative = false;
        }

        public override string ToString()
        {
            return this.Text;
        }

        public string ToJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public CustomAction Clone()
        {
            return ParseJsonString(this.ToJsonString());
        }


        public static CustomAction ParseJsonString(string json)
        {
            try
            {
                CustomAction ca = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomAction>(json);
                if (string.IsNullOrEmpty(ca.Key)) return null;
                return ca;
            }
            catch { }

            return null;
        }

    }
}
