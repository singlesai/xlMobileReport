using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using xl_rp.Entity;

namespace xl_rp.BasicInfo
{
    public partial class frmApiEdit : Form
    {
        private ApiObj _apiObj;
        private Sqlite _lit;
        public frmApiEdit(int id=0)
        {
            InitializeComponent();
            _lit = new xl_rp.Sqlite();
            listDb.DisplayMember = "name";
            listUser.DisplayMember = "name";
            listRight.DisplayMember = "name";

            dgParam.ColumnCount = 4;
            dgParam.ColumnHeadersVisible = true;
            dgParam.Columns[0].DataPropertyName = "name";
            dgParam.Columns[0].HeaderText = "名称";
            dgParam.Columns[1].DataPropertyName = "key";
            dgParam.Columns[1].HeaderText = "sql标记";
            dgParam.Columns[2].DataPropertyName = "defValue";
            dgParam.Columns[2].HeaderText = "默认值";
            dgParam.Columns[3].DataPropertyName = "note";
            dgParam.Columns[3].HeaderText = "备注";

            if (id == 0)
            {
                _apiObj = new Entity.ApiObj();
                this.Text = "新增";
            }
            else
            {
                _apiObj = ApiObj.Load(_lit, id);
                this.Text = _apiObj.name;
            }
            txtName.DataBindings.Add("Text", _apiObj, "name");
            txtNumber.DataBindings.Add("Text", _apiObj, "number");
            txtNote.DataBindings.Add("Text", _apiObj, "note");

            dgParam.DataSource = _apiObj.param;
            listRight.DataSource = _apiObj.right;
            listDb.DataSource = Db.Load(_lit);
            List<User> users = User.Load(_lit);
            for(int i = users.Count - 1; i >= 0; --i)
            {
                for(int j = 0; j < _apiObj.right.Count; j++)
                {
                    if (_apiObj.right[j].id == users[i].id)
                    {
                        users.RemoveAt(i);
                    }
                }
            }
            listUser.DataSource = users;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _apiObj.Save(_lit);
                
                this.Hide();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPropAdd_Click(object sender, EventArgs e)
        {
            frmApiParamEdit frm = new BasicInfo.frmApiParamEdit();
            frm.ShowDialog();
            if (frm.isRet)
            {
                _apiObj.param.Add(frm.data);
                dgParam.DataSource = null;
                dgParam.DataSource = _apiObj.param;
            }
        }

        private void btnRightAdd_Click(object sender, EventArgs e)
        {
            User user = (User)listUser.SelectedItem;
            if (user != null)
            {
                List<User> us = (List<User>)listUser.DataSource;
                us.Remove(user);
                _apiObj.right.Add(user);
                listRight.DataSource = null;
                listRight.DataSource = _apiObj.right;
                listUser.DataSource = null;
                listUser.DataSource = us;
                listUser.DisplayMember = "name";
                listRight.DisplayMember = "name";
            }
        }

        private void btnRightDel_Click(object sender, EventArgs e)
        {
            User user = (User)listRight.SelectedItem;
            if(user!=null)
            {
                List<User> us = (List<User>)listUser.DataSource;
                us.Add(user);
                listUser.DataSource = us;
                _apiObj.right.Remove(user);
                listRight.DataSource = null;
                listRight.DataSource = _apiObj.right;
                listUser.DataSource = null;
                listUser.DataSource = us;
                listUser.DisplayMember = "name";
                listRight.DisplayMember = "name";
            }
        }

        private void btnPropDel_Click(object sender, EventArgs e)
        {
            _apiObj.param.Remove((ApiParam)dgParam.SelectedRows[0].DataBoundItem);
            dgParam.DataSource = null;
            dgParam.DataSource = _apiObj.param;
        }
        

        private void txtSql_TextChanged(object sender, EventArgs e)
        {
            Db db = (Db)listDb.SelectedItem;
            if (db != null)
            {
                bool exist = false;
                foreach(ApiDb ad in _apiObj.db)
                {
                    if (ad.db.id == db.id)
                    {
                        ad.strSql = txtSql.Text;
                        exist = true;
                        break;
                    }
                }
                if(!exist)
                {
                    _apiObj.db.Add(new Entity.ApiDb()
                    {
                        db = db,
                        strSql = txtSql.Text
                    });
                }
            }
        }

        private void listDb_SelectedValueChanged(object sender, EventArgs e)
        {
            Db db = (Db)listDb.SelectedItem;
            if (db != null)
            {
                bool exist = false;
                foreach(ApiDb ad in _apiObj.db)
                {
                    if (ad.db.id == db.id)
                    {
                        txtSql.Text = ad.strSql;
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    txtSql.Text = "";
                }
            }
        }

        private void txtSql_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    ApiDb cad = null;
                    Db db = (Db)listDb.SelectedItem;
                    if (db != null)
                    {
                        foreach (ApiDb ad in _apiObj.db)
                        {
                            if (ad.db.id == db.id)
                            {
                                cad = ad;
                                break;
                            }
                        }
                        if (cad == null)
                        {
                            cad = new Entity.ApiDb()
                            {
                                db = db,
                                strSql = txtSql.Text
                            };
                            _apiObj.db.Add(cad);
                        }
                    }
                    Dictionary<string, string> apd = new Dictionary<string, string>();
                    foreach(ApiParam ap in _apiObj.param)
                    {
                        apd.Add(ap.key, ap.defValue);
                    }
                    dgData.DataSource = cad.Exec(apd);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
