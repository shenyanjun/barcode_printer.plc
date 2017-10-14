using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace barcode_printer {
    public partial class frmPrintMinMax : Form {
        public frmPrintMinMax() {
            InitializeComponent();
            //dataConfig.loadConfig();
            tbMin.Text = dataConfig.print_min_val.ToString();
            tbMax.Text = dataConfig.print_max_val.ToString();
        }
        private void btnOK_Click(object sender, EventArgs e) {
            int max = 0;
            if (tbMax.Text != "") {
                max = int.Parse(tbMax.Text.Trim());
            }
            int min = 0;
            if (tbMin.Text != "") {
                min = int.Parse(tbMin.Text.Trim());
            }
            if (max <= min) {
                if (dataConfig.lang == "zh")
                    MessageBox.Show("上限必须大于下限值!");
                else
                    MessageBox.Show("max value must large than min value!");
                return;
            } else if (max < 0 || min < 0) {
                if (dataConfig.lang == "zh")
                    MessageBox.Show("上下限值必须大于0!");
                else
                    MessageBox.Show("max and min value must large than 0!");
                return;
            } else if (min <= 0) {
                if (dataConfig.lang == "zh")
                    MessageBox.Show("下限必须大于等于1!");
                else
                    MessageBox.Show("min value must large than 0");
                return;
            }
            dataConfig.print_max_val = max;
            dataConfig.print_min_val = min;
            dataConfig.saveParamMinMax();
            this.Close();
        }

        private void frmPrintMinMax_Load(object sender, EventArgs e)
        {

        }
    }
}