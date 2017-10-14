using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Text.RegularExpressions;
namespace barcode_printer {
    public partial class frmSelectPrintSN : Form {
        public frmSelectPrintSN() {
            InitializeComponent();
            txtPrintSnNum.Focus();
        }
        private int _to_print_sn_num = 0;
        public int to_print_sn_num {
            get {
                return _to_print_sn_num;
            }
        }
        private void btnOK_Click(object sender, EventArgs e) {
            string sn_num = txtPrintSnNum.Text ;
            if (sn_num != "")
            {
                Regex r = new Regex(@"^[0-9]*$");
                bool m0 = r.IsMatch(sn_num);
                if (!m0)
                {
                    if (dataConfig.lang == "zh")
                        MessageBox.Show("请输入1位以上数字!");
                    else
                        MessageBox.Show("please input large than 9 number!");
                    return;
                }
                _to_print_sn_num = int.Parse(sn_num);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void frmSelectPrintSN_Load(object sender, EventArgs e)
        {
            txtPrintSnNum.Focus();
        }
    }
}