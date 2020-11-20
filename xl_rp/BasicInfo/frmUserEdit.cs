using System;
using System.Windows.Forms;
using xl_rp.Entity;

namespace xl_rp.BasicInfo
{
    public partial class frmUserEdit : Form
    {
        private User _user;
        private Sqlite _lit;
        public frmUserEdit(int id=0)
        {
            InitializeComponent();
            _lit = new xl_rp.Sqlite();
            
            if (id == 0)
            {
                _user = new User();
                this.Text = "新增";
            }
            else
            {
                _user = User.Load(_lit,id);

                txtNumber.Text = _user.number;
                txtName.Text = _user.name;
                txtPsw.Text = _user.password;
                this.Text = _user.name;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _user.number = txtNumber.Text;
                _user.name = txtName.Text;
                _user.password = txtPsw.Text;
                _user.Save(_lit);
                
                this.Hide();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
