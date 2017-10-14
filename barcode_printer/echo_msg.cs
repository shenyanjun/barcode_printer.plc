using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace barcode_printer {
    public partial class echo_msg : UserControl {
        public echo_msg() {
            InitializeComponent();
        }
        private string _msg = "";
        public string msg {
            set {
                timer_cleanup.Interval = 3000;
                lbMsg.Text = value;
                _msg = value;
            }
        }
        /// <summary>
        /// ¶¨Ê±Ë¢ÐÂ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_cleanup_Tick(object sender, EventArgs e) {
            lbMsg.Text = "";
        }
    }
}
