using System;
using System.Windows.Forms;
using xl_rp.Entity;

namespace xl_rp.BasicInfo
{
    public partial class frmDbEdit : Form
    {
        private Db _db;
        private Sqlite _lit;
        public frmDbEdit(int id=0)
        {
            InitializeComponent();
            _lit = new xl_rp.Sqlite();
            
            if (id == 0)
            {
                _db = new Entity.Db();
                this.Text = "新增";
            }
            else
            {
                _db = Db.Load(_lit,id);
                txtNumber.Text = _db.number;
                txtName.Text = _db.name;
                txtType.Text = _db.type;
                txtStrConn.Text = _db.strConn;
                this.Text = _db.name;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _db.number = txtNumber.Text;
                _db.name = txtName.Text;
                _db.type = txtType.Text;
                _db.strConn = txtStrConn.Text;
                _db.Save(_lit);
                
                this.Hide();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
