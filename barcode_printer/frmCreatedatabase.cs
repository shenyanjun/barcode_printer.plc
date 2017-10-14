using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using System.Data.Sql;
using System.Text.RegularExpressions;
namespace barcode_printer
{
    public partial class frmCreatedatabase : Form
    {
        private SQLiteConnection conn = new SQLiteConnection();
        private bool _isSet = false;
        public frmCreatedatabase(string caption, bool set)
        {
            InitializeComponent();
            if (String.IsNullOrEmpty(caption)) {
                if (dataConfig.lang == "zh")
                    caption = "������ӡ����";
                else
                    caption = "create print data";
            }
            this.Text = caption;
            this.DialogResult = DialogResult.No;
            _isSet = set;
        }
        //update version sn_max, sn_max sn_print_magic_code sn_check_repeat
        private const string create_database =    "CREATE TABLE IF NOT EXISTS [sn] ("
                                                  + "[id] INTEGER PRIMARY KEY AUTOINCREMENT, "
                                                  + " [sn] TEXT,"
                                                  + " [availed_sn] TEXT, "
                                                  + " [print_time] TIMESTAMP,"
                                                  + " [success] TEXT);"
                                                  + " CREATE TABLE IF NOT EXISTS [sn_property] ([start_sn] CHAR(10), "
                                                  + " [sn_max] CHAR(10) DEFAULT('9999'), " 
                                                  + " [sn_min] CHAR(10) DEFAULT('0'), " 
                                                  + " [sn_print_magic_code] TEXT DEFAULT(''), "
                                                  + " [sn_check_repeat] BOOL DEFAULT(1), [scan_check] BOOL DEFAULT(1));"
                                                  + " CREATE TABLE IF NOT EXISTS [log] ( "
                                                  + " [log_time] TIMESTAMP, "
                                                  + " [log_msg] TEXT); ";
                                                 
        private void frmCreatedatabase_Load(object sender, EventArgs e)
        {
            //dataConfig.loadConfig();
            string path = dataConfig.code_printer_template_path;
            string tmp_name = "";
            if (path != "")
            {
                tmp_name = Path.GetFileNameWithoutExtension(path);
                path = Path.GetDirectoryName(path);
            }
            
            string db_file_name = path + "\\" + tmp_name + ".db3";
            dataConfig.to_print_data_path = db_file_name;
            dataConfig.savePrintDataPath();
            conn.ConnectionString = "Data Source = " + db_file_name;
            textBoxDatabase.Text = tmp_name + ".db3";
            if (File.Exists(db_file_name) && _isSet) {
                sn_property sn = new sn_property(db_file_name);
                textBoxStartSN.Text = sn.sn_start;
                textBox1.Text = sn.sn_min;
                textBox2.Text = sn.sn_max;
                checkBox2.Checked = sn.scan_check;
                checkBox1.Checked = sn.sn_check_repeat;
                richTextBox1.Text = sn.sn_print_magic_code;
            }
            if (!_isSet || !File.Exists(db_file_name)) {
                //�����Զ������ӡ�ű�����
                string prn_file = dataConfig.code_printer_template_path;
                FileStream fs = new FileStream(prn_file, FileMode.Open);
                StreamReader rd = new StreamReader(fs);
                string content = rd.ReadToEnd();
                richTextBox1.Text = content;
                fs.Close();
            }
        }
        //
        /// <summary>
        /// �������ݿ��ļ�,������Ҫ��һ���䶯����Ҫ�ÿͻ�ʵʱ�������������ݿ��Ƿ�ɹ�(������������ʾ����)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //dataConfig.loadConfig();
            frmLogin login = new frmLogin();
            if (login.ShowDialog() == DialogResult.OK) {

                string path = dataConfig.code_printer_template_path;
                string tmp_name = "";
                if (path != "") {
                    tmp_name = Path.GetFileNameWithoutExtension(path);
                    path = Path.GetDirectoryName(path);
                }
                string db_file_name = path + "\\" + tmp_name + ".db3";
                //!4.5
                /*
                if (File.Exists(db_file_name))
                    return;
                 * */
                string txtStartSN = textBoxStartSN.Text.Trim();
                string txtMin = textBox1.Text.Trim();
                string txtMax = textBox2.Text.Trim();
                if (txtStartSN == "" || txtMin == "" || txtMax == "") {
                    if (dataConfig.lang == "zh")
                        MessageBox.Show("������һ����Ч������!");
                    else
                        MessageBox.Show("please input validate number!");
                    return;
                }
                Regex r = new Regex(@"^[0-9]*$");
                bool m0 = r.IsMatch(txtStartSN);
                bool m1 = r.IsMatch(txtMin);
                bool m2 = r.IsMatch(txtMax);
                if (!m0 || !m1 || !m2) {
                    MessageBox.Show("������1λ��������!");
                    return;
                }
                int start = Convert.ToInt32(txtStartSN);
                int min = Convert.ToInt32(txtMin);
                int max = Convert.ToInt32(txtMax);
                if (start < min) {
                    if (dataConfig.lang == "zh")
                        MessageBox.Show("��ʼֵ������ڵ�����Сֵ!");
                    else
                        MessageBox.Show("start value must large min value!");
                    return;
                }
                if (min > max) {
                    if (dataConfig.lang == "zh")
                        MessageBox.Show("��Сֵ����С�ڵ������ֵ!");
                    else
                        MessageBox.Show("min value must small than max value!");
                    return;
                }
                if (start > max) {
                    if (dataConfig.lang == "zh")
                        MessageBox.Show("��ʼֵ����С�ڵ������ֵ!");
                    else
                        MessageBox.Show("start value must less than max value!");
                    return;
                }
                conn.Open();
                //�������ݿ�
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = create_database;
                cmd.ExecuteNonQuery();
                //MessageBox.Show("�����ɹ�!");
                cmd.CommandText = "SELECT count(*) as n FROM [sn_property]";
                SQLiteDataReader rd = cmd.ExecuteReader();
                if (rd.Read()) {
                    int n = Convert.ToInt32(rd["n"]);
                    rd.Close();
                    if (n <= 0) {
                        string insert = "INSERT INTO [sn_property] ([start_sn], [sn_max],[sn_min], [sn_print_magic_code], [sn_check_repeat], [scan_check]) values (\'1\', \'9999\', \'0\', \'\', \'1\',\'1\');";
                        cmd.CommandText = insert;
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.Close();
                this.DialogResult = DialogResult.OK;
                /*
                string sql = "insert into [sn_property] (start_sn) values (\'" + txtStartSN + "\'" + ");";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                * */
                sn_property sn_pro = new sn_property(db_file_name);
                sn_pro.sn_print_magic_code = richTextBox1.Text;
                sn_pro.sn_check_repeat = checkBox1.Checked;
                sn_pro.sn_max = txtMax;
                sn_pro.sn_min = txtMin;
                sn_pro.sn_start = txtStartSN;
                sn_pro.scan_check = checkBox2.Checked;
                sn_pro.update();
                this.Close();
                if (dataConfig.lang == "zh")
                    MessageBox.Show("����ɹ�!");
                else
                    MessageBox.Show("save successful!");
            }
        }
        //����
        private void button2_Click(object sender, EventArgs e) {
            if (dataConfig.lang == "zh")
            {
                openFileDialog1.Title = "��ѡ���ӡģ���ļ�";
                openFileDialog1.Filter = "��ӡģ���ļ�|*.prn|�����ļ�|*.*";
            }
            else
            {
                openFileDialog1.Title = "please select print template";
                openFileDialog1.Filter = "template file|*.prn|all type|*.*";
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                //�����Զ������ӡ�ű�����
                string prn_file = openFileDialog1.FileName;
                FileStream fs = new FileStream(prn_file, FileMode.Open);
                StreamReader rd = new StreamReader(fs);
                string content = rd.ReadToEnd();
                richTextBox1.Text = content;
                fs.Close();
            }
        }
        //����
        private void button3_Click(object sender, EventArgs e) {
            if (dataConfig.lang == "zh")
            {
                saveFileDialog1.Title = "���ݴ�ӡģ���ļ�";
                saveFileDialog1.Filter = "��ӡģ���ļ�|*.prn|�����ļ�|*.*";
            }
            else
            {
                saveFileDialog1.Title = "backup print template file";
                saveFileDialog1.Filter = "template file|*.prn|all type|*.*";
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                string prn_content = richTextBox1.Text;
                string file_name = saveFileDialog1.FileName;
                FileStream file_handle = new FileStream(file_name, FileMode.OpenOrCreate);
                StreamWriter wr = new StreamWriter(file_handle);
                wr.Write(prn_content);
                wr.Flush();
                file_handle.Close();
                if (dataConfig.lang == "zh")
                    MessageBox.Show("�����ɹ�!");
                else
                    MessageBox.Show("export successful!");
            }
        }
    }
}