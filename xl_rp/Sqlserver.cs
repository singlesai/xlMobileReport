using System.Data.SqlClient;
using System.Data;

namespace xl_rp
{
    class Sqlserver
    {
        private string _strConn;
        public Sqlserver(string strConn)
        {
            _strConn = strConn;
        }

        public int ExcSql(string strSql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_strConn))
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = strSql;
                    foreach(SqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string sqlString, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_strConn))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;
                    foreach (SqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        public DataTable GetDataTable(string sqlString, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(_strConn))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlString;
                    foreach (SqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    DataSet ds = new DataSet();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    return ds.Tables[0];
                }
            }
        }
    }
}
