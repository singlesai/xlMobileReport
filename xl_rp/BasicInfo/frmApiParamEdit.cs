using System;
using System.Windows.Forms;
using xl_rp.Entity;

namespace xl_rp.BasicInfo
{
    public partial class frmApiParamEdit : Form
    {
        public ApiParam data;
        private bool _isRet;

        public bool isRet
        {
            get { return _isRet; }
        }
        public frmApiParamEdit(int id=0)
        {
            InitializeComponent();

            if (id == 0)
            {
                data = new Entity.ApiParam();
                this.Text = "新增";
            }
            else
            {
                this.Text = data.name;
            }
            txtKey.DataBindings.Add("Text", data, "key");
            txtName.DataBindings.Add("Text", data, "name");
            txtDefValue.DataBindings.Add("Text", data, "defValue");
            txtNote.DataBindings.Add("Text", data, "note");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(data.name)) throw new Exception("编号不能为空");
                if (string.IsNullOrEmpty(data.key)) throw new Exception("Sql标记不能为空");
                _isRet = true;
                this.Hide();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
