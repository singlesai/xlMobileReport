using System;
using System.Collections.Generic;
using System.Data;

namespace xl_rp.Entity
{
    class Db
    {
        public int id { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string strConn { get; set; }

        public static Db Load(Sqlite lit,int id)
        {
            Db rst = new Entity.Db();
            DataTable dt = lit.GetDataTable(string.Format("select fid,fnumber,fname,ftype,fstrconn from t_db where fid={0}", id));
            if (dt.Rows.Count > 0)
            {
                rst.id = (int)dt.Rows[0]["fid"];
                rst.number = dt.Rows[0]["fnumber"].ToString();
                rst.name = dt.Rows[0]["fname"].ToString();
                rst.type = dt.Rows[0]["ftype"].ToString();
                rst.strConn = dt.Rows[0]["fstrconn"].ToString();
            }
            return rst;
        }

        public static List<Db> Load(Sqlite lit)
        {
            List<Db> rst = new List<Db>();
            DataTable dt = lit.GetDataTable(string.Format("select fid,fnumber,fname,ftype,fstrconn from t_db"));
            foreach(DataRow dr in dt.Rows)
            {
                rst.Add(new Entity.Db()
                {
                    id = (int)dr["fid"],
                    number = dr["fnumber"].ToString(),
                    name = dr["fname"].ToString(),
                    type = dr["ftype"].ToString(),
                    strConn = dr["fstrconn"].ToString()
                });
            }
            return rst;
        }

        public int Save(Sqlite lit)
        {
            number = number.Trim();
            name = name.Trim();
            if (string.IsNullOrEmpty(number)) throw new Exception("编号不能为空");
            if (string.IsNullOrEmpty(name)) throw new Exception("名称不能为空");
            if (string.IsNullOrEmpty(type)) throw new Exception("数据库类型不能为空");
            if (string.IsNullOrEmpty(strConn)) throw new Exception("链接字符串不能为空");
            DataTable dt = lit.GetDataTable(string.Format("select * from t_db where fid<>{0} and fnumber='{1}'", id, number));
            if (dt.Rows.Count > 0) throw new Exception(string.Format("已存在编号为'{0}'的记录", number));
            int recCnt = 0;
            if (id == 0)
            {
                dt = lit.GetDataTable("select max(fid) id from t_db");
                if (dt.Rows.Count <= 0)
                {
                    id = 1;
                }
                else
                {
                    id = (dt.Rows[0]["id"].ToString() == "" ? 0 : int.Parse(dt.Rows[0]["id"].ToString())) + 1;
                }
                recCnt = lit.ExcSql(string.Format("insert into t_db(fid,fnumber,fname,ftype,fstrconn) values({0},'{1}','{2}','{3}','{4}')", id, number, name, type, strConn));
            }
            else
            {
                recCnt = lit.ExcSql(string.Format("update t_db set fnumber='{1}',fname='{2}',ftype='{3}',fstrconn='{4}' where fid={0}", id, number, name, type, strConn));
            }
            return recCnt;
        }

        public static int Delete(Sqlite lit,int id)
        {
            return lit.ExcSql(string.Format("delete from t_db where fid={0}", id));
        }
    }
}
