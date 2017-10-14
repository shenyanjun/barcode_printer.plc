using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
namespace MakeRegisterInfo
{
    public partial class MainForm : Form
    {

       
        public MainForm()
        {
            InitializeComponent();
            
        }

        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            string cpu_info = "";
            cpu_info = Common.ComputerInfo.GetComputerInfo();
            EncryptionHelper help = new EncryptionHelper(EncryptionKeyEnum.KeyB);
            string md5String = help.GetMD5String(cpu_info);
            string registInfo = help.EncryptString(md5String);
            RegistFileHelper.WriteRegistFile(registInfo);
            MessageBox.Show("在程序运行的目录下面生成注册文件成功！");
        }
    }
}