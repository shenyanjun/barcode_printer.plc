using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace barcode_printer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (System.Diagnostics.Process.GetProcessesByName(
             System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1) {
                MessageBox.Show("应用程序已经启动过了。");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainFrm());
            //Application.Run(new frmLogin());
        }
    }
}