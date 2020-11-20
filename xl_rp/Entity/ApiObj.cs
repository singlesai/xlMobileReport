using System;
using System.Collections.Generic;
using System.Data;

namespace xl_rp.Entity
{
    class ApiObj
    {
        public ApiObj()
        {
            param = new List<Entity.ApiParam>();
            right = new List<Entity.User>();
            db = new List<Entity.ApiDb>();
        }

        public int id { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string note { get; set; }

        public List<ApiParam> param { get; set; }
        public List<User> right { get; set; }
        public List<ApiDb> db { get; set; }

        public static ApiObj Load(Sqlite lit,int id)
        {
            ApiObj rst = new Entity.ApiObj();
            DataTable dt = lit.GetDataTable(string.Format("select fid,fnumber,fname,fnote from t_Api where fid={0}",id));
            if (dt.Rows.Count > 0)
            {
                rst.id = (int)dt.Rows[0]["fid"];
                rst.number = dt.Rows[0]["fnumber"].ToString();
                rst.name = dt.Rows[0]["fname"].ToString();
                rst.note = dt.Rows[0]["fnote"].ToString();
                rst.param = new List<Entity.ApiParam>();
                rst.right = new List<Entity.User>();
                rst.db = new List<Entity.ApiDb>();
            }
            dt = lit.GetDataTable(string.Format("select fname,fkey,fdefval,fnote from t_ApiParam where fapiid={0}", id));
            foreach(DataRow dr in dt.Rows)
            {
                rst.param.Add(new Entity.ApiParam()
                {
                    name = dr["fname"].ToString(),
                    key = dr["fkey"].ToString(),
                    note = dr["fnote"].ToString(),
                    defValue = dr["fdefval"].ToString()
                });
            }
            dt = lit.GetDataTable(string.Format("select i.fid,i.fnumber,i.fname,i.fpsw from t_user i join t_ApiRight j on j.fuserid=i.fid where j.fapiid={0}",id));
            foreach(DataRow dr in dt.Rows)
            {
                rst.right.Add(new Entity.User()
                {
                    id = (int)dr["fid"],
                    number = dr["fnumber"].ToString(),
                    name = dr["fname"].ToString(),
                    password = dr["fpsw"].ToString()
                });
            }
            dt = lit.GetDataTable(string.Format("select i.fid,i.fnumber,i.fname,i.fstrconn,i.ftype,j.fstrsql from t_db i join t_ApiDb j on j.fdbid=i.fid where j.fapiid={0}",id));
            foreach(DataRow dr in dt.Rows)
            {
                rst.db.Add(new Entity.ApiDb()
                {
                    db = new Entity.Db()
                    {
                        id=(int)dr["fid"],
                        number=dr["fnumber"].ToString(),
                        name=dr["fname"].ToString(),
                        type=dr["ftype"].ToString(),
                        strConn=dr["fstrconn"].ToString()
                    },
                    strSql = dr["fstrsql"].ToString()
                });
            }
            return rst;
        }

        public static ApiObj Load(Sqlite lit,string number)
        {

            ApiObj rst = new Entity.ApiObj();
            DataTable dt = lit.GetDataTable(string.Format("select fid,fnumber,fname,fnote from t_Api where fnumber='{0}'", number));
            if (dt.Rows.Count <= 0) return null;

            rst.id = (int)dt.Rows[0]["fid"];
            rst.number = dt.Rows[0]["fnumber"].ToString();
            rst.name = dt.Rows[0]["fname"].ToString();
            rst.note = dt.Rows[0]["fnote"].ToString();
            rst.param = new List<Entity.ApiParam>();
            rst.right = new List<Entity.User>();
            rst.db = new List<Entity.ApiDb>();

            dt = lit.GetDataTable(string.Format("select fname,fkey,fdefval,fnote from t_ApiParam where fapiid={0}", rst.id));
            foreach (DataRow dr in dt.Rows)
            {
                rst.param.Add(new Entity.ApiParam()
                {
                    name = dr["fname"].ToString(),
                    key = dr["fkey"].ToString(),
                    note = dr["fnote"].ToString(),
                    defValue = dr["fdefval"].ToString()
                });
            }
            dt = lit.GetDataTable(string.Format("select i.fid,i.fnumber,i.fname,i.fpsw from t_user i join t_ApiRight j on j.fuserid=i.fid where j.fapiid={0}", rst.id));
            foreach (DataRow dr in dt.Rows)
            {
                rst.right.Add(new Entity.User()
                {
                    id = (int)dr["fid"],
                    number = dr["fnumber"].ToString(),
                    name = dr["fname"].ToString(),
                    password = dr["fpsw"].ToString()
                });
            }
            dt = lit.GetDataTable(string.Format("select i.fid,i.fnumber,i.fname,i.fstrconn,i.ftype,j.fstrsql from t_db i join t_ApiDb j on j.fdbid=i.fid where j.fapiid={0}", rst.id));
            foreach (DataRow dr in dt.Rows)
            {
                rst.db.Add(new Entity.ApiDb()
                {
                    db = new Entity.Db()
                    {
                        id = (int)dr["fid"],
                        number = dr["fnumber"].ToString(),
                        name = dr["fname"].ToString(),
                        type = dr["ftype"].ToString(),
                        strConn = dr["fstrconn"].ToString()
                    },
                    strSql = dr["fstrsql"].ToString()
                });
            }
            return rst;
        }
        public int Save(Sqlite lit)
        {
            if (string.IsNullOrEmpty(number)) throw new System.Exception("编号不能为空");
            if (string.IsNullOrEmpty(name)) throw new Exception("名称不能为空");
            number = number.Trim();
            name = name.Trim();
            DataTable dt = lit.GetDataTable(string.Format("select * from t_Api where fid<>{0} and fnumber='{1}'", id, number));
            if (dt.Rows.Count > 0) throw new Exception(string.Format("已存在编号为'{0}'的记录", number));
            int recCnt = 0;
            if (id == 0)
            {
                dt = lit.GetDataTable("select max(fid) id from t_Api");
                if (dt.Rows.Count <= 0)
                {
                    id = 1; 
                }
                else
                {
                    id = (dt.Rows[0]["id"].ToString() == "" ? 0 : int.Parse(dt.Rows[0]["id"].ToString())) + 1;
                }
                recCnt = lit.ExcSql(string.Format("insert into t_Api(fid,fnumber,fname,fnote) values({0},'{1}','{2}','{3}')", id, number, name, note));
            }
            else
            {
                recCnt = lit.ExcSql(string.Format("update t_Api set fnumber='{1}',fname='{2}',fnote='{3}' where fid={0}", id, number, name, note));
            }
            lit.ExcSql(string.Format("delete from t_ApiParam where fapiid={0}", id));
            foreach(ApiParam ap in param)
            {
                lit.ExcSql(string.Format("insert into t_ApiParam(fapiid,fname,fkey,fdefval,fnote) values({0},'{1}','{2}','{3}','{4}')",id,ap.name,ap.key,ap.defValue,ap.note));
            }
            lit.ExcSql(string.Format("delete from t_ApiRight where fapiid={0}", id));
            foreach (User u in right)
            {
                lit.ExcSql(string.Format("insert into t_ApiRight(fapiid,fuserid) values({0},'{1}')", id, u.id));
            }
            lit.ExcSql(string.Format("delete from t_ApiDb where fapiid={0}", id));
            foreach (ApiDb ad in db)
            {
                lit.ExcSql(string.Format("insert into t_ApiDb(fapiid,fdbid,fstrsql) values({0},'{1}','{2}')", id, ad.db.id, ad.strSql.Replace("'","''")));
            }
            return recCnt;
        }

        public static int Delete(Sqlite lit,int id)
        {
            lit.ExcSql(string.Format("delete from t_ApiDb where fapiid={0}", id));
            lit.ExcSql(string.Format("delete from t_ApiParam where fapiid={0}", id));
            lit.ExcSql(string.Format("delete from t_ApiRight where fapiid={0}", id));
            return lit.ExcSql(string.Format("delete from t_Api where fid={0}", id));
        }

        public Dictionary<string,DataTable> Exec(Dictionary<string,string> p=null)
        {
            if (p == null) p = new Dictionary<string, string>();
            Dictionary<string, string> apd = new Dictionary<string, string>(); 
            foreach(ApiParam ap in param)
            {
                if (p.ContainsKey(ap.name))
                {
                    apd.Add(ap.key, p[ap.name]);
                }else
                {
                    apd.Add(ap.key, ap.defValue);
                }
            }
            Dictionary<string, DataTable> rst = new Dictionary<string, DataTable>();
            foreach(ApiDb _db in db)
            {
                DataTable dt = _db.Exec(apd);
                if (dt == null) continue;
                rst.Add(_db.db.name, dt);
            }
            return rst;
        }
    }

    public class ApiParam
    {
        public string name { get; set; }
        public string key { get; set; }
        public string defValue { get; set; }
        public string note { get; set; }
    }

    class ApiDb
    {
        public Db db { get; set; } 
        public string strSql { get; set; }

        public DataTable Exec(Dictionary<string,string> param=null)
        {
            if (string.IsNullOrEmpty(strSql)) return null;
            foreach(KeyValuePair<string,string> kvp in param)
            {
                strSql = strSql.Replace(kvp.Key, kvp.Value);
            }
            DataTable dt = null;
            switch (db.type)
            {
                case "sqlserver":
                    Sqlserver mssql = new xl_rp.Sqlserver(db.strConn);
                    dt = mssql.GetDataTable(strSql);
                    break;
                default:
                    break;
            }
            return dt;
        }
    }
}
