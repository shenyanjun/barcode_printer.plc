using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace barcode_printer {
    public partial class frmLogin : Form {

        public frmLogin() {
            InitializeComponent();
            DialogResult = DialogResult.No;
            textBoxUserName.Text = _user_info.user_name;
        }
        private string getMd5(string info) {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string md5_bytes = BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(info))).Replace("-","");
            return md5_bytes;
        }
        private user_info _user_info = new user_info();

        private void buttonOK_Click(object sender, EventArgs e) {
            string pass = getMd5(textBoxPassWord.Text.Trim());
            string pass_hass = _user_info.pass_word.ToUpper();
            if (pass_hass == pass && textBoxUserName.Text.Trim().ToLower() == "admin") {
                DialogResult = DialogResult.OK;
            } else {
                if (dataConfig.lang == "zh")
                    MessageBox.Show("√‹¬Î¥ÌŒÛ!");
                else
                    MessageBox.Show("error password!");
                DialogResult = DialogResult.No;
            }
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}