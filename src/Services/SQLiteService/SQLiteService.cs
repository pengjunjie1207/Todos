using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SQLiteService
{
    class SQLiteService
    {
        public static SQLiteConnection conn;

        public static void LoadDatabase()
        {
            // Get a reference to the SQLite database
            conn = new SQLiteConnection("SQLiteTODO.db");

            string sql = @"CREATE TABLE IF NOT EXISTS
                                TodoItem (id         VARCHAR( 36 ) PRIMARY KEY NOT NULL,
                                         title       VARCHAR( 140 ),
                                         detail      VARCHAR( 140 ),
                                         date        DATETIME
                            )";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }
        }
    }
}
