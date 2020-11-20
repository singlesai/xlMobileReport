using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using xl_rp.BasicInfo;
using xl_rp.Entity;

namespace xl_rp
{
    public partial class frmMain : Form
    {
        private static HttpListener httpPostRequest = new HttpListener();
        private HttpServer srv = new HttpServer();
        public frmMain()
        {
            InitializeComponent();
            txtStaticPath.Text = System.Environment.CurrentDirectory+"\\client";
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            srv.requestHandler += Srv_requestHandler;
        }

        private void Srv_requestHandler(HttpListenerContext ctx)
        {
            try
            {
                List<string> sl = new List<string>();
                if (txtInfo.Text != "")
                {
                    sl = new List<string>(txtInfo.Lines);
                }
                string info = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + ctx.Request.Url.OriginalString;
                if (sl.Count >= 1000)
                {
                    sl.RemoveAt(sl.Count - 1);
                }
                sl.Insert(0, info);
                txtInfo.Lines = sl.ToArray();

                string route = ctx.Request.RawUrl.Split('?')[0];
                ApiObj ao = ApiObj.Load(new xl_rp.Sqlite(), route);
                if (ao == null)
                {
                    ctx.Response.StatusCode = 404;
                    ctx.Response.StatusDescription = "Url Not Found";
                    ctx.Response.Close();
                    return;
                }

                ctx.Response.StatusCode = 200;
                //接收Get参数
                string type = ctx.Request.QueryString["type"];
                string userId = ctx.Request.QueryString["userId"];
                string password = ctx.Request.QueryString["password"];
                string filename = Path.GetFileName(ctx.Request.RawUrl);
                string userName = "test";//HttpUtility.ParseQueryString(filename).Get("userName");//避免中文乱码
                Dictionary<string, string> param = new Dictionary<string, string>();
                foreach (string k in ctx.Request.QueryString.AllKeys)
                {
                    param.Add(k, ctx.Request.QueryString[k]);
                }
                //进行处理
                //Console.WriteLine("收到数据:" + userName);

                //接收POST参数
                Stream stream = ctx.Request.InputStream;
                System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
                String body = reader.ReadToEnd();
                //Console.WriteLine("收到POST数据:" + HttpUtility.UrlDecode(body));
                //Console.WriteLine("解析:" + HttpUtility.ParseQueryString(body).Get("userName"));

                //使用Writer输出http响应代码,UTF8格式
                using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
                {
                    string rst = Newtonsoft.Json.JsonConvert.SerializeObject(ao.Exec(param));
                    writer.Write(rst);
                    writer.Close();
                    ctx.Response.Close();
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.btnStart.Text == "启动")
                {
                    srv.domain = txtDomain.Text;
                    srv.port = (int)txtPort.Value;
                    srv.staticPath = txtStaticPath.Text;
                    srv.start();
                    this.btnStart.Text = "停止";
                }
                else
                {
                    srv.stop();
                    this.btnStart.Text = "启动";
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStaticPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (!string.IsNullOrEmpty(fbd.SelectedPath))
            {
                txtStaticPath.Text = fbd.SelectedPath;
            }
        }

        private void mnuUserMgr_Click(object sender, EventArgs e)
        {
            frmUserMgr frm = new frmUserMgr();
            frm.ShowDialog();
        }

        private void mnuDBMgr_Click(object sender, EventArgs e)
        {
            frmDbMgr frm = new frmDbMgr();
            frm.ShowDialog();
        }

        private void mnuApiMgr_Click(object sender, EventArgs e)
        {
            frmApiMgr frm = new BasicInfo.frmApiMgr();
            frm.ShowDialog();
        }
    }
}
