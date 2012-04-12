using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace Procon.DB
{
    public interface IDatabaseProvider
    {
        DatabaseProvider ExecuteNonQuery(string SqlQuery);

        DatabaseProvider ExecuteNonQuery(MySqlCommand MySqlCommand);

        DataTable FillDataTable(string SelectQuery);

        DataTable FillDataTable(MySqlCommand selectCommand);
    }
}
