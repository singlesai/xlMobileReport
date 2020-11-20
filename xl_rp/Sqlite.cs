using System.IO;
using System.Data.SQLite;
using System.Data;

namespace xl_rp
{
    class Sqlite
    {
        private string _fileName;
        private string _strConn;
        public Sqlite(string fileName="")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                _fileName = System.Environment.CurrentDirectory + "/Data.db";
            }
            else
            {
                _fileName = fileName;
            }
            if (!File.Exists(_fileName))
                SQLiteConnection.CreateFile(_fileName);

            SQLiteConnectionStringBuilder strConn = new SQLiteConnectionStringBuilder();
            strConn.DataSource = "/Data.db";
            _strConn = strConn.ToString();
        }

        public int ExcSql(string strSql, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_strConn))
            {
                conn.Open();
                using(SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = strSql;
                    foreach(SQLiteParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string sqlString, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_strConn))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;
                    foreach (SQLiteParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        public DataTable GetDataTable(string sqlString, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_strConn))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;
                    foreach (SQLiteParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    DataSet ds = new DataSet();
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(ds);
                    return ds.Tables[0];
                }
            }
        }
    }
}
