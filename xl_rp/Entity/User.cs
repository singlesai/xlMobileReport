using System;
using System.Collections.Generic;
using System.Data;

namespace xl_rp.Entity
{
    class User
    {
        public int id { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public static User Load(Sqlite lit,int id)
        {
            User rst = new Entity.User();
            DataTable dt = lit.GetDataTable(string.Format("select fid,fnumber,fname,fpsw from t_user where fid={0}", id));
            if (dt.Rows.Count > 0)
            {
                rst.id = (int)dt.Rows[0]["fid"];
                rst.number = dt.Rows[0]["fnumber"].ToString();
                rst.name = dt.Rows[0]["fname"].ToString();
                rst.password = dt.Rows[0]["fpsw"].ToString();
            }
            return rst;
        }

        public static List<User> Load(Sqlite lit)
        {
            List<User> rst = new List<Entity.User>();
            DataTable dt = lit.GetDataTable(string.Format("select fid,fnumber,fname,fpsw from t_user"));
            foreach(DataRow dr in dt.Rows)
            {
                rst.Add(new Entity.User()
                {
                    id = (int)dr["fid"],
                    number =dr["fnumber"].ToString(),
                    name = dr["fname"].ToString(),
                    password = dr["fpsw"].ToString()
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
            DataTable dt = lit.GetDataTable(string.Format("select * from t_user where fid<>{0} and fnumber='{1}'", id, number));
            if (dt.Rows.Count > 0) throw new Exception(string.Format("已存在编号为'{0}'的记录", number));
            int recCnt = 0;
            if (id == 0)
            {
                dt = lit.GetDataTable("select max(fid) id from t_user");
                if (dt.Rows.Count <= 0)
                {
                    id = 1;
                }
                else
                {
                    id = (dt.Rows[0]["id"].ToString() == "" ? 0 : int.Parse(dt.Rows[0]["id"].ToString())) + 1;
                }
                recCnt = lit.ExcSql(string.Format("insert into t_user(fid,fnumber,fname,fpsw) values({0},'{1}','{2}','{3}')", id, number, name, password));
            }
            else
            {
                recCnt = lit.ExcSql(string.Format("update t_user set fnumber='{1}',fname='{2}',fpsw='{3}' where fid={0}", id, number, name, password));
            }
            return recCnt;
        }

        public static int Delete(Sqlite lit,int id)
        {
            return lit.ExcSql(string.Format("delete from t_user where fid={0}", id));
        }
    }
}
