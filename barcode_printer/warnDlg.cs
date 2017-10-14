using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace barcode_printer
{
    public partial class warnDlg : Form
    {
        public warnDlg()
        {
            InitializeComponent();
        }

        private void warnDlg_Load(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            dataConfig.diaglog_warn_flag = false;
            this.Close();
        }
    }
}