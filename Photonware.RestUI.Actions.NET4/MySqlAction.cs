#if MYSQL
using MySql.Data.MySqlClient;
using Photonware.RestUI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Actions.NET4
{
    public class MySqlAction:Photonware.RestUI.Core.Action
    {
        public MySqlAction() : base()
        {
            this.Text = "MySQL";
            this.Key = "mysql_action";
            
        }
        public override bool Execute(ExecutionAction action, UserCaseExecutionContext context, out object result)
        {
            try
            {
                string connectionString = string.Empty;
                string commandText = string.Empty;

                using (StringReader reader = new StringReader(action.Content))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.Trim().StartsWith("#"))
                        {
                            continue;
                        }

                        if (line.StartsWith("Server"))
                        {
                            connectionString = line;
                        }
                        else
                        {
                            commandText = line;
                        }

                    }
                }
                //string connectionString = "Server=192.168.101.106;Database=sysbench;Uid=sysbench;Pwd=sysbench;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = commandText;

                        using (MySqlDataReader sqlReader = cmd.ExecuteReader())
                        {
                            List<object[]> list = new List<object[]>();


                            //result = string.Format("{0} rows found\r\n", );
                            result = null;
                            while (sqlReader.Read())
                            {
                                object[] objects = new object[sqlReader.FieldCount];
                                int num = sqlReader.GetValues(objects);
                                list.Add(objects);
                            }

                            result = new MySqlResult(list);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                result = e.Message;
                return false;
            }
        }
        
    }

    public class MySqlResult
    {
        public System.Collections.ObjectModel.ReadOnlyCollection<object[]> Data { get; private set; }

        public MySqlResult(List<object[]> result)
        {
            this.Data = result.AsReadOnly();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (object[] objects in Data)
            {
                string r = string.Empty;
                for (int i = 0; i < objects.Length; i++)
                {
                    sb.Append(objects[i].ToString() + "\t|");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
#endif