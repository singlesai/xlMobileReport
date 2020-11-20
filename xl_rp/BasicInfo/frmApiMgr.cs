using System.Data;
using System.Windows.Forms;

namespace xl_rp.BasicInfo
{
    public partial class frmApiMgr : Form
    {
        private Sqlite lit;
        public frmApiMgr()
        {
            InitializeComponent();

            lit = new xl_rp.Sqlite();

            dgData.ColumnCount = 4;
            dgData.ColumnHeadersVisible = true;
            dgData.Columns[0].Name = "id";
            dgData.Columns[0].DataPropertyName = "fid";
            dgData.Columns[0].HeaderText = "id";
            dgData.Columns[0].Visible = false;
            dgData.Columns[1].DataPropertyName = "fnumber";
            dgData.Columns[1].HeaderText = "编号";
            dgData.Columns[2].DataPropertyName = "fname";
            dgData.Columns[2].HeaderText = "名称";
            dgData.Columns[3].DataPropertyName = "fnote";
            dgData.Columns[3].HeaderText = "备注";

            DataTable dt = lit.GetDataTable("select * from sqlite_master where name='t_Api'");
            if (dt.Rows.Count <= 0)
            {
                lit.ExcSql("create table t_Api(fid int,fnumber varchar(255),fname varchar(255),fnote varchar(8000))");
            }
            dt = lit.GetDataTable("select * from sqlite_master where name='t_ApiDb'");
            if (dt.Rows.Count <= 0)
            {
                lit.ExcSql("create table t_ApiDb(fapiid int,fdbid int,fstrsql varchar(8000))");
            }
            dt = lit.GetDataTable("select * from sqlite_master where name='t_ApiParam'");
            if (dt.Rows.Count <= 0)
            {
                lit.ExcSql("create table t_ApiParam(fapiid int,fname varchar(255),fkey varchar(255),fdefval varchar(255),fnote varchar(8000))");
            }
            dt = lit.GetDataTable("select * from sqlite_master where name='t_ApiRight'");
            if (dt.Rows.Count <= 0)
            {
                lit.ExcSql("create table t_ApiRight(fuserid int,fapiid int)");
            }

            refreshData();
        }

        private void refreshData()
        {
            dgData.ReadOnly = true;
            DataTable dt = lit.GetDataTable("select fid,fnumber,fname,fnote from t_Api");
            dgData.DataSource = dt;
        }

        private void btnNew_Click(object sender, System.EventArgs e)
        {
            frmApiEdit edit = new frmApiEdit();
            edit.ShowDialog();
            refreshData();
            edit.Dispose();
        }

        private void btnEdit_Click(object sender, System.EventArgs e)
        {
            int id = (int)dgData.Rows[dgData.SelectedCells[0].RowIndex].Cells[0].Value;
            frmApiEdit edit = new BasicInfo.frmApiEdit(id);
            edit.ShowDialog();
            refreshData();
            edit.Dispose();
        }

        private void btnDel_Click(object sender, System.EventArgs e)
        {
            int id = (int)dgData.Rows[dgData.SelectedCells[0].RowIndex].Cells[0].Value;
            string number = dgData.Rows[dgData.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
            DialogResult dr = MessageBox.Show(string.Format("确认删除'{0}'?", number), "提示", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                lit.ExcSql(string.Format("delete from t_Api where fid={0}", id));
                lit.ExcSql(string.Format("delete from t_ApiDb where fapiid={0}", id));
                lit.ExcSql(string.Format("delete from t_ApiParam where fapiid={0}", id));
                lit.ExcSql(string.Format("delete from t_ApiRight where fapiid={0}", id));
                refreshData();
            }
        }
    }
}
