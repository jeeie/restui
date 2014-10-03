using Photonware.RestUI.CommonApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photonware.RestUI.Core
{
    public class UserCaseManager
    {
        private static readonly UserCaseManager manager = new UserCaseManager();

        public static UserCaseManager Instance { get { return manager; } }

        public List<UserCase> UserCases { get; private set; }

        public object SyncRoot { get { return this; } }

        public bool Modified { get; set; }

        public IFileStore FileUtils { get; set; }

        private UserCaseManager()
        {
            this.UserCases = new List<UserCase>();
            this.Modified = false;
        }

        public void init()
        {
            lock (this.SyncRoot)
            {
                UserCases.Clear();
                try
                {
                    string[] lines = FileUtils.ReadFromFile("usercases.json");
                    if (lines != null)
                    {
                        foreach (string line in lines)
                        {
                            if (string.IsNullOrEmpty(line)) continue;
                            UserCase uc = Newtonsoft.Json.JsonConvert.DeserializeObject<UserCase>(line);
                            this.UserCases.Add(uc);
                        }
                    }
                }
                catch { }
            }
        }

        public void save()
        {
            lock (this.SyncRoot)
            {
                StringBuilder sb = new StringBuilder();
                foreach (UserCase uc in this.UserCases)
                {
                    sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(uc));
                }
                FileUtils.WriteToFile("usercases.json", sb.ToString(), true);
                this.Modified = false;
            }
        }


        public void AddUserCase(UserCase userCase)
        {
            lock (this.SyncRoot)
            {
                if (!this.UserCases.Contains(userCase))
                {
                    this.UserCases.Add(userCase);
                    this.Modified = true;

                }
            }
        }

        public void RemoveUserCase(UserCase userCase)
        {
            lock (this.SyncRoot)
            {
                if (this.UserCases.Contains(userCase))
                {
                    this.UserCases.Remove(userCase);
                    this.Modified = true;

                }
            }
        }

        public void InsertAfter(UserCase current, UserCase userCase)
        {
            lock (this.SyncRoot)
            {
                if (this.UserCases.Contains(current))
                {
                    int idx = this.UserCases.IndexOf(current);
                    this.UserCases.Insert(idx + 1, userCase);
                    this.Modified = true;
                }
                else
                {
                    this.AddUserCase(userCase);
                }
            }
        }

        public void InsertBefore(UserCase current, UserCase userCase)
        {
            lock (this.SyncRoot)
            {
                if (this.UserCases.Contains(current))
                {
                    int idx = this.UserCases.IndexOf(current);
                    this.UserCases.Insert(idx, userCase);
                    this.Modified = true;
                }
                else
                {
                    this.AddUserCase(userCase);
                }
            }
        }

    }
}
