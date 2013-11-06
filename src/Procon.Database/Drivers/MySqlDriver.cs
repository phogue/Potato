using System;
using MySql.Data.MySqlClient;

namespace Procon.Database.Drivers {
    public class MySqlDriver : SqlDriver {

        public override String Name {
            get { return "MySQL"; }
        }

        public override bool OpenConnection() {
            bool opened = true;

            if (this.Connection != null) {
                this.Connection.Close();
                this.Connection = null;
            }

            MySqlConnectionStringBuilder mscsb = new MySqlConnectionStringBuilder {
                Server = this.Hostname,
                Port = this.Port,
                Database = this.Database,
                UserID = this.Uid,
                Password = this.Password,
                UseCompression = true
            };

            try {
                this.Connection = new MySqlConnection(mscsb.ToString());
                this.Connection.Open();
            }
            catch {
                opened = false;
            }

            return opened;
        }
    }
}
