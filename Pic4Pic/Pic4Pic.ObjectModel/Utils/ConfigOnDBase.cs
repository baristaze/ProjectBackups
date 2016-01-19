using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Pic4Pic.ObjectModel
{
    public class ConfigItem : IDBEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            this.Name = reader.GetString(index++);

            if (!reader.IsDBNull(index))
            {
                this.Value = reader.GetString(index);
            }
            index++;
        }
    }

    public class ConfigOnDBase : ConfigBase
    {
        public TaskConfigMeta TaskConfigMeta { get; set; }
        protected Dictionary<string, string> configItems = new Dictionary<string, string>();

        public override bool Init()
        {
            if (this.TaskConfigMeta == null)
            {
                return false;
            }

            using (SqlConnection conn = new SqlConnection(this.TaskConfigMeta.ConfigDBaseConnStr)) 
            {
                conn.Open();

                List<ConfigItem> items = Database.ExecSProc<ConfigItem>(
                    conn, 
                    null, 
                    "[dbo].[GetSetup]",
                    new SqlParameter("@ProjectId", this.TaskConfigMeta.ProjectId),
                    new SqlParameter("@EnvironmentId", this.TaskConfigMeta.EnvironmentId));

                this.configItems.Clear();
                foreach (ConfigItem item in items)
                {
                    this.configItems.Add(item.Name, item.Value);
                }

                return true;
            }
        }

        protected override string GetFromStore(string key)
        {
            if (this.configItems.ContainsKey(key))
            {
                return this.configItems[key];
            }

            return null;
        }
    }
}
