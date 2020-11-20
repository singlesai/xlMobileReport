using System.Data;
using System.Windows.Forms;
using xl_rp.Entity;

namespace xl_rp.BasicInfo
{
    public partial class frmUserMgr : Form
    {
        private Sqlite lit;
        public frmUserMgr()
        {
            InitializeComponent();
            
            lit = new xl_rp.Sqlite();

            dgData.ColumnCount = 3;
            dgData.ColumnHeadersVisible = true;
            dgData.Columns[0].Name = "id";
            dgData.Columns[0].DataPropertyName = "fid";
            dgData.Columns[0].HeaderText = "id";
            dgData.Columns[0].Visible = false;
            dgData.Columns[1].DataPropertyName = "fnumber";
            dgData.Columns[1].HeaderText = "编号";
            dgData.Columns[2].DataPropertyName = "fname";
            dgData.Columns[2].HeaderText = "名称";

            DataTable dt = lit.GetDataTable("select * from sqlite_master where name='t_user'");
            if (dt.Rows.Count <= 0)
            {
                lit.ExcSql("create table t_user(fid int,fnumber varchar(255),fname varchar(255),fpsw varchar(255))");
            }

            refreshData();
        }

        private void refreshData()
        {
            dgData.ReadOnly = true;
            DataTable dt = lit.GetDataTable("select fid,fnumber,fname from t_user");
            dgData.DataSource = dt;
        }

        private void btnNew_Click(object sender, System.EventArgs e)
        {
            frmUserEdit edit = new frmUserEdit();
            edit.ShowDialog();
            refreshData();
            edit.Dispose();
        }

        private void btnEdit_Click(object sender, System.EventArgs e)
        {
            int id = (int)dgData.Rows[dgData.SelectedCells[0].RowIndex].Cells[0].Value;
            frmUserEdit edit = new BasicInfo.frmUserEdit(id);
            edit.ShowDialog();
            refreshData();
            edit.Dispose();
        }

        private void btnDel_Click(object sender, System.EventArgs e)
        {
            int id = (int)dgData.Rows[dgData.SelectedCells[0].RowIndex].Cells[0].Value;
            string number = dgData.Rows[dgData.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
            DialogResult dr = MessageBox.Show(string.Format("确认删除'{0}'?",number), "提示", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                User.Delete(lit,id);
                refreshData();
            }
        }
    }
}
