using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Photonware.RestUI.Core
{
    public class UserCase
    {

        public string Text { get; set; }
        public string Description { get; set; }
        public List<CustomAction> Actions { get; protected set; }

        public UserCase()
        {
            this.Text = string.Empty;
            this.Description = string.Empty;
            this.Actions = new List<CustomAction>();
        }

        public UserCase(string text) : this()
        {
            this.Text = text;
        }

        public void AddAction(CustomAction action)
        {
            this.Actions.Add(action);
            UserCaseManager.Instance.Modified = true;
        }

        public void RemoveAction(CustomAction action)
        {
            if (Actions.Contains(action))
            {
                this.Actions.Remove(action);
                UserCaseManager.Instance.Modified = true;
            }
        }

        public void ClearAction(CustomAction action)
        {
            this.Actions.Clear();
            UserCaseManager.Instance.Modified = true;
        }

        public void InsertActionAfter(CustomAction target, CustomAction action)
        {
            if (this.Actions.Contains(target))
            {
                int idx = this.Actions.IndexOf(target);
                this.Actions.Insert(idx + 1, action);
                UserCaseManager.Instance.Modified = true;
            }
            else
            {
                this.AddAction(action);
            }
        }
        public void InsertActionBefore(CustomAction target, CustomAction action)
        {
            if (this.Actions.Contains(target))
            {
                int idx = this.Actions.IndexOf(target);
                this.Actions.Insert(idx, action);
                UserCaseManager.Instance.Modified = true;
            }
            else
            {
                this.AddAction(action);
            }
        }

        public void MoveUp(CustomAction action)
        {
            int index = -1;
            for (int i = 0; i < this.Actions.Count; i++)
            {
                if (this.Actions[i] == action)
                {
                    index = i;
                }
            }
            if ((index == -1) || (index == 0)) return;
            this.Actions.RemoveAt(index);
            this.Actions.Insert(index - 1, action);
            UserCaseManager.Instance.Modified = true;
        }

        public void MoveDown(CustomAction action)
        {
            int index = -1;
            for (int i = 0; i < this.Actions.Count; i++)
            {
                if (this.Actions[i] == action)
                {
                    index = i;
                }
            }
            if ((index == this.Actions.Count - 1)) return;
            this.Actions.Insert(index + 2, action); 
            this.Actions.RemoveAt(index);
            UserCaseManager.Instance.Modified = true;
        }

        public static UserCase ParseJsonString(string json)
        {
            try
            {
                UserCase uc = Newtonsoft.Json.JsonConvert.DeserializeObject<UserCase>(json);

                return uc;
            }
            catch { }

            return null;
        }

        public override string ToString()
        {
            return Text;
        }

        public string ToJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public UserCase Clone()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<UserCase>(this.ToJsonString());
        }

    }
}
