using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.Sql;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.IO.Ports;
using proto_parase;
using proto;
using DBUtility.SQLite;
using System.Globalization;
using Common;


namespace barcode_printer
{
    public partial class mainFrm : Form
    {
        //加入打印日志分析功能
        /*
        private FileStream _log_file = null;
        private StreamWriter _log_write = null;
        private void initLog() {
            _log_file = new FileStream(Application.StartupPath + @"\print.log", FileMode.Append);
            _log_write = new StreamWriter(_log_file);
            log("\r\n");
        }
        private void closeLog() {
            _log_file.Close();
        }
        private void log(string msg) {
            _log_write.Write(msg);
            _log_write.Flush();
        }
         * */
        //扫描仪是否打开
        private bool bScanerOpen = true;
        //private ReadBarCode barcode = null;
        //scanner _scanner = null;
        //打印机是否打开
        private bool bIOOpen = false;
        //选择模版并打开数据库
        private bool bDatabaseOpen = false;
        //开始sn
        private int start_sn = 0;
        //串口接收协议
        private proto_parase.proto_parase _proto_parase = new proto_parase.proto_parase();
        //串口处理协议
        private proto.proto _proto = new proto.proto();
        //主函数
        private SQLiteConnection sqlite_conn = new SQLiteConnection();
        //待打印列表
        private DataTable product_list_table = new DataTable("product_list");
        private SQLiteCommandBuilder cmdbuilder_product = null;
        private SQLiteDataAdapter adp_product = null;
        //日志数据库
        private DataTable log_table = new DataTable("log_list");
        private SQLiteDataAdapter adp_log = null;
        private SQLiteCommandBuilder cmdbuilder_log = null;
        //每个打印完后，等待扫描的超时时间
        //private int scan_timeout = 0;
        //检测到的打印结果
        private bool bIsScannerOK = true;
        //数据库当前的页码
        private int page_offset = 0;
        //所有记录条目个数
        private int print_all_counts = 0;
        //所有页码
        private int log_all_page_count = 0;
        //打印个数
        private int to_print_sn_num = 0;
        //前面打印的sn和id
        private int pre_print_sn = 0, pre_print_id = 0;
        //------------------------------------------------------------------------------------------------------------
        private DateTime _now = DateTime.Now;//当前日期
        //private bool _datatime_chaneged = false;
        //修复一个问题 2016.4.24,修改配方的最大最小值是要跟着起作用

        //添加的新的问题修复
        private sn_property _sn_property = null;
        public mainFrm() {
            InitializeComponent();
        }
        //取消扫描
        private void btnCancelScan_Click(object sender, EventArgs e) {
            DialogResult r = MessageBox.Show("是否取消扫描校验?", "警告", MessageBoxButtons.YesNo);
            //继续打印
            if (r == DialogResult.Yes) {
                if (added_row != null) {
                    string sql = "update sn set  success = \'{0}\' where sn = \'{1}\';";
                    added_row["success"] = "NS";
                    sql = string.Format(sql, "NS", Convert.ToString(added_row["sn"]));
                    added_row = null;//取消打印
                    SQLiteHelper.ExecuteNonQuery(sqlite_conn, sql, null);
                }
                //使得打印按钮有效，继续打印
                btnCancelScan.Visible = false;
                bIsScannerOK = true;//假设已经扫描ok
                set_opt_btn(true);
                lbWarningMsg.Text = "";
            }
        }
        /// <summary>
        /// 设置打印按钮的状态
        /// </summary>
        /// <param name="opt"></param>
        private void set_opt_btn(bool opt) {
            this.Invoke((EventHandler)(delegate{
            manual_print.Enabled = opt;
            }));
        }

        private void assert_print_menu()
        {
            bool isOK = bIOOpen && bIsScannerOK;
            if (isOK)
            {
                manual_print.Enabled = true;
            }
        }

        //
        //配置条码打印机
        //
        private void toolstrip_codeprint_Click(object sender, EventArgs e){
            //先关闭io串口
            if (serialport_iotrans.IsOpen)
                serialport_iotrans.Close(); 
            frmPrinterset printer = new frmPrinterset();
            DialogResult r =  printer.ShowDialog();
            if (r == DialogResult.Yes) {
                //fix bug
                //io板子
                if (dataConfig.io_dev != "" && dataConfig.io_dev_speed != 0) {
                    serialport_iotrans.BaudRate = dataConfig.io_dev_speed;
                    serialport_iotrans.PortName = dataConfig.io_dev;
                    try {
                        serialport_iotrans.Open();
                        MessageBox.Show("打开PLC通讯板的串口成功!");
                        bIOOpen = true;
                        //assert_print_menu();
                    } catch (Exception ex) {
                        MessageBox.Show("打开PLC通讯板的串口失败:" + ex.Message);
                        bIOOpen = false;
                    }
                }
                /*
                //扫描
                if (dataConfig.scan_serial_name != "")
                {
                    _scanner = new scanner(dataConfig.scan_serial_name);
                    _scanner.BarCodeEvent = new scanner.BarCodeDelegate(barcode_event);
                    if (_scanner.openScanner())
                    {
                        toolstrip_codecheck.Text = "关闭标签扫描仪";
                        bScanerOpen = true;
                        manual_print.Enabled = true;//test
                    }
                    else
                    {
                        string path = dataConfig.code_printer_template_path;
                        string tmp_name = "";
                        if (path != "") {
                            tmp_name = Path.GetFileNameWithoutExtension(path);
                            path = Path.GetDirectoryName(path);
                        }
                        string db_file_name = path + "\\" + tmp_name + ".db3";
                        sn_property sn_pro = new sn_property(db_file_name);
                        if (sn_pro.scan_check) {
                            manual_print.Enabled = false;//test
                            bScanerOpen = false;
                        }
                    }
                }
                 * */
            }
        }

        //检查数据库释放需要修改表名称
        private void check_and_fix_db(string db_name) {
            SQLiteConnection con = new SQLiteConnection();
            con.ConnectionString = "Data Source = " + db_name;
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM sqlite_master where tbl_name = " + "\'sn_property\'", con);
            SQLiteDataReader rd = cmd.ExecuteReader();
            string sql= "";
            if (rd.Read()) {
                sql = Convert.ToString(rd["sql"]);
            }
            rd.Close();
            string exec = "";
            if (!string.IsNullOrEmpty(sql)) {
                if (!sql.Contains("[sn_max]")) {
                    exec = "ALTER TABLE sn_property ADD COLUMN [sn_max] CHAR(10) DEFAULT('9999')";
                    cmd.CommandText = exec;
                    cmd.ExecuteNonQuery();
                }
                if (!sql.Contains("[sn_min]")) {
                    exec = "ALTER TABLE sn_property ADD COLUMN [sn_min] CHAR(10) DEFAULT('0')";
                    cmd.CommandText = exec;
                    cmd.ExecuteNonQuery();
                }
                if (!sql.Contains("[sn_print_magic_code]")) {
                    exec = "ALTER TABLE sn_property ADD COLUMN [sn_print_magic_code] TEXT DEFAULT('')";
                    cmd.CommandText = exec;
                    cmd.ExecuteNonQuery();
                }
                if (!sql.Contains("[sn_check_repeat]")) {
                    exec = "ALTER TABLE sn_property ADD COLUMN [sn_check_repeat] BOOL DEFAULT(1)";
                    cmd.CommandText = exec;
                    cmd.ExecuteNonQuery();
                }
                if (!sql.Contains("[scan_check]")) {
                    exec = "ALTER TABLE sn_property ADD COLUMN [scan_check] BOOL DEFAULT(1)";
                    cmd.CommandText = exec;
                    cmd.ExecuteNonQuery();
                }
            //没有表，需要创建
            } else {
                string create_table = "CREATE TABLE IF NOT EXISTS [sn_property] ([start_sn] CHAR(10), "
                                      + " [sn_max] CHAR(10) DEFAULT('9999'), "
                                      + " [sn_min] CHAR(10) DEFAULT('0'), "
                                      + " [sn_print_magic_code] TEXT DEFAULT(''), "
                                      + " [sn_check_repeat] BOOL DEFAULT(1), [scan_check] BOOL DEFAULT(1));"
                                      +"  INSERT INTO [sn_property] ([start_sn], [sn_max],[sn_min], [sn_print_magic_code], [sn_check_repeat], [scan_check]) values (\'1\', \'9999\', \'0\', \'\', \'1\',\'1\');";
                cmd.CommandText = create_table;
                cmd.ExecuteNonQuery();                     
            }
            con.Close();
        }
        //改变标签文件
        private void chanegPrintTemp(string lableName, bool isTruePath, string label_path)
        {
            string data_file_name = "";
            string tmp_file_name = "";
            if (isTruePath){
                data_file_name = Path.GetDirectoryName(lableName) + "\\" + Path.GetFileNameWithoutExtension(lableName) + ".db3";
                tmp_file_name = lableName;
            }
            else{
                data_file_name = label_path + "\\" + lableName + ".db3";
                tmp_file_name = label_path + "\\" + lableName + ".prn";
            }
            if (!File.Exists(tmp_file_name))
            {
                MessageBox.Show("不存在打印模板文件!请添加后再试!");
                return;
            }
            if (!File.Exists(data_file_name))
            {
                if (MessageBox.Show("发现没有连接的打印数据库文件,是否创建?", "帮助", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    dataConfig.code_printer_template_path = tmp_file_name;//现在不用不到这个了
                    dataConfig.savePrintTemplatePath();
                    dataConfig.to_print_data_path = data_file_name;
                    dataConfig.savePrintDataPath();
                    Text = "标签打印软件  " + Properties.Resources.version + "  当前标签:" + Path.GetFileNameWithoutExtension(dataConfig.code_printer_template_path);
                    frmCreatedatabase createdb = new frmCreatedatabase("创建打印数据", false);
                    if (createdb.ShowDialog() == DialogResult.OK)
                    {
                        //判断是是否需添加表的结构
                        check_and_fix_db(data_file_name);
                        Text = "标签打印软件  " + Properties.Resources.version;
                        load_data();
                        send_plc_chg_lab_result(Path.GetFileNameWithoutExtension(tmp_file_name));
                        return;
                    }
                }
            }
            //这里有问题
            dataConfig.code_printer_template_path = tmp_file_name;
            dataConfig.savePrintTemplatePath();
            Text = "标签打印软件  " + Properties.Resources.version + "  当前标签:" + Path.GetFileNameWithoutExtension(dataConfig.code_printer_template_path);
            dataConfig.to_print_data_path = data_file_name;
            product_list_table.Clear();
            //判断是是否需添加表的结构
            check_and_fix_db(data_file_name);
            sn_property property = new sn_property(dataConfig.to_print_data_path);
            //---------------------------------------------------------------------
            //复初始值
            /*if (prev_min_val == 0) */
            {
                prev_min_val = int.Parse(property.sn_min);
            }
            /*if (prev_max_val == 0) */
            {
                prev_max_val = int.Parse(property.sn_max);
            }
            //---------------------------------------------------------------------
            if (property.sn_print_magic_code == "")
            {
                //加入自动载入打印脚本数据
                string prn_file = dataConfig.code_printer_template_path;
                FileStream fs = new FileStream(prn_file, FileMode.Open);
                StreamReader rd = new StreamReader(fs);
                string content = rd.ReadToEnd();
                property.sn_print_magic_code = content;
                fs.Close();
                property.update();
            }
            if (dataConfig.to_print_data_path != "")
            {
                load_data();
                send_plc_chg_lab_result(Path.GetFileNameWithoutExtension(tmp_file_name));
            }
            dataConfig.savePrintDataPath();
        }
        private void create_labels_dir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        private string copy_label_to_dir(string path, string file)
        {
            string file_without_path = Path.GetFileName(file);
            if (path == string.Empty)
            {
                path = "c:\\labels";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.Copy(file, path+"\\"+file_without_path, true);
            return path + "\\" + file_without_path;
        }
        //
        //选择打印模版
        //
        private void toolstrp_printtemple_Click(object sender, EventArgs e)
        {
            if (timer_cheksn.Enabled)
                return;
            lbWarningMsg.Text = "";//2016.4.25
            openFileDialog1.Title = "请选择打印模版文件";
            openFileDialog1.Filter = "打印模版文件|*.prn|所有文件|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK){
                string tmpName = openFileDialog1.FileName;
                string path_name = "c:\\labels";
                tmpName = copy_label_to_dir(path_name, tmpName);
                chanegPrintTemp(tmpName, true, path_name);
            }
        }
        //
        //关闭程序
        //
        private void mainFrm_FormClosing(object sender, FormClosingEventArgs e){
            string msg = "";
            string msg_caption = "";
            msg = "are ready to leave application?";
            msg_caption = "warning";
            /*
            if (dataConfig.lang == "en")
            {
                msg = "你是否要离开打印程序?";
                msg_caption = "警告";
            }
            else
            {
                msg = "are ready to leave application?";
                msg_caption = "warning";
            }
             * */
            if (MessageBox.Show(msg, msg_caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //closeLog();
                //关闭检查日期
                stop_date_change();
                //关闭串口打印业务
                stop_print_content();
                if (!bIsScannerOK) {
                    if (added_row != null) {
                        string sql = "update sn set  success = \'{0}\' where sn= \'{1}\';";
                        added_row["success"] = "NS";
                        sql = string.Format(sql, "NS", Convert.ToString(added_row["sn"]));
                        added_row = null;//取消打印
                        SQLiteHelper.ExecuteNonQuery(sqlite_conn, sql, null);
                    }
                    //使得打印按钮有效，继续打印
                    btnCancelScan.Visible = false;
                    bIsScannerOK = true;//假设已经扫描ok
                    set_opt_btn(true);
                    lbWarningMsg.Text = "";
                    //barcode.Stop();
                }
            } else {
                e.Cancel = true;
            }
        }
        /// <summary>
        /// 计算一页有多少个项
        /// </summary>
        private int calc_page_count() {
            int page_hei = gridEX1.Height;
            int scrollbar_hei = SystemInformation.HorizontalScrollBarHeight;
            int item_hei = gridEX1.RootTable.RowHeight;
            return (page_hei - item_hei - scrollbar_hei) / item_hei;
        }
        /// <summary>
        /// 设置表格控件的大小
        /// </summary>
        private void setup_gridex_size() {
            int index_w = dataConfig.index_size;
            int sn_w = dataConfig.sn_size;
            int date_w = dataConfig.print_date_size;
            int scuss_w = dataConfig.print_success_size;
            gridEX1.RootTable.Columns[0].Width = index_w;
            gridEX1.RootTable.Columns[1].Width = sn_w;
            gridEX1.RootTable.Columns[2].Width = date_w;
            gridEX1.RootTable.Columns[3].Width = scuss_w;
            int log_grid_width = gridEX2.Width;
            int time_w = (int)(log_grid_width * 0.2);
            int log_w = (int)(log_grid_width * 0.8);
            gridEX2.RootTable.Columns[0].Width = time_w;
            gridEX2.RootTable.Columns[1].Width = log_w;
        }
        private void insert_test() {
            SQLiteTransaction ts =  sqlite_conn.BeginTransaction();
            int i = 1;
            string sql = "insert into sn (sn, availed_sn, print_time, success) values (\'{0}\', \'{1}\', \'{2}\', 'OK')";
            printer_dev dev = new printer_dev(dataConfig.code_printer_template_path);
            string sql_exec = "";
            for (; i <= 2000000; i++) {
                //解析打印数据
                if (!dev.parase_key_code(i.ToString())) {
                    if (dataConfig.lang == "zh")
                        MessageBox.Show("打印值超出设定范围!");
                    else
                        MessageBox.Show("print value up to max!");
                    break;
                }
                string true_sn = dev.TrueSN;
                sql_exec = string.Format(sql, i.ToString(), dev.TrueSN, DateTime.Now.ToLongTimeString());
                SQLiteHelper.ExecuteNonQuery(ts, sql_exec, null);
            }
            ts.Commit();
            MessageBox.Show("success!");
        }
        private void load_data() {
            if (dataConfig.to_print_data_path == "")
                return;
            //修改打开另外一个标签的问题
            pre_print_sn = 0;
            pre_print_id = 0;
            //_now = DateTime.Now;//del17/9/22
            product_list_table.Clear();
            sqlite_conn.Close();
            sqlite_conn.ConnectionString = "Data Source = " + dataConfig.to_print_data_path;
            sqlite_conn.Open();
            //insert_test();
            //得到所有的条码数目
            string sql_query_all = "select count(*) from sn;";
            DataSet ds = SQLiteHelper.ExecuteDataSet(sqlite_conn, sql_query_all, null);
            if (ds != null) {
                print_all_counts = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            sqlite_conn.Close();
            sqlite_conn.Open();
            log_all_page_count = (print_all_counts + 999) / 1000; //总的页数
            //计算出offset
            //page_offset = (print_all_counts - 1) / 1000;//起始地址
            page_offset = log_all_page_count - 1;
            //fix 15.10.13
            if (page_offset < 0) {
                page_offset = 0;
            }
            string query_later_page_data = "select * from sn order by id limit 1000 offset {0};";
            query_later_page_data = string.Format(query_later_page_data, page_offset * 1000);
            //查询
            adp_product = new SQLiteDataAdapter(query_later_page_data, sqlite_conn);
            adp_product.Fill(product_list_table);
            if (product_list_table.Rows.Count > 0) {
                int _i = product_list_table.Rows.Count - 1;
                pre_print_sn = Convert.ToInt32(product_list_table.Rows[_i]["sn"]);
                pre_print_id = Convert.ToInt32(product_list_table.Rows[_i]["id"]);
                string t = product_list_table.Rows[_i]["print_time"].ToString();
                _now = Convert.ToDateTime(t);
            }
            gridEX1.DataSource = product_list_table;
            gridEX1.MoveLast();
            //获取当前的开始sn
            DataTable tbl_sn_property = new DataTable("tmp_tbl");
            SQLiteDataAdapter adp_sn_property = new SQLiteDataAdapter("select * from sn_property;", sqlite_conn);
            adp_sn_property.Fill(tbl_sn_property);
            start_sn = Convert.ToInt32(tbl_sn_property.Rows[0]["start_sn"]);
            //Console.WriteLine("start_sn"+start_sn.ToString());
            //日志
            log_table.Clear();
            adp_log = new SQLiteDataAdapter("select * from log;", sqlite_conn);
            cmdbuilder_log = new SQLiteCommandBuilder(adp_log);
            adp_log.Fill(log_table);
            gridEX2.DataSource = log_table;
            gridEX2.MoveLast();
            _log("打开标签 " + Path.GetFileName(dataConfig.code_printer_template_path));
            bDatabaseOpen = true; 
        }
        private static mainFrm _mainform = null;
        public static mainFrm getMainForm()
        {
            return _mainform;
        }
        //显示一个对话框
        public void setWarnningDlg()
        {
            this.Invoke((EventHandler)delegate { warnDlg d = new warnDlg(); d.Show(); });
        }
        private bool CheckRegistData(string key)
        {
            if (RegistFileHelper.ExistRegistInfofile() == false)
            {
                return false;
            }
            else
            {
                string info = RegistFileHelper.ReadRegistFile();
                if (info == key)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private bool CheckRegist(string encryptComputer)
        {
            return CheckRegistData(encryptComputer);
        }
        private bool check_register_info()
        {
            string computer = ComputerInfo.GetComputerInfo();
            EncryptionHelper help = new EncryptionHelper(EncryptionKeyEnum.KeyB);
            string md5String = help.GetMD5String(computer);
            string registInfo = help.EncryptString(md5String);
            return CheckRegist(registInfo);
        }

        private void mainFrm_Load(object sender, EventArgs e){
           
            create_labels_dir("c:\\labels");
            //initLog();
            dataConfig.loadConfig();
            if (!check_register_info())
            {
                if (dataConfig.lang == "zh")
                {
                    MessageBox.Show("软件没有注册,请注册!");
                }
                else if (dataConfig.lang == "en")
                {
                    MessageBox.Show("software is not register!");
                }
                //Application.Exit();
                System.Environment.Exit(0);
            }
            if (dataConfig.lang == "zh")
            {
                this.Text = "标签打印软件";
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            }
            else if (dataConfig.lang == "en")
            {
                this.Text = "label print application";
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            }
            ApplyResource();
            setup_gridex_size();
            this.Text += " " + Properties.Resources.version;
            //从io板接收到数据并处理
            _proto_parase.on_packet_data += new proto_parase.proto_parase.procedure_packet(_proto_parase_on_packet_data);
            //模板文件还是加载,但是数据从数据库中取
            if (File.Exists(dataConfig.code_printer_template_path) && File.Exists(dataConfig.to_print_data_path)) {
                string cap = "";
                if (dataConfig.lang == "zh")
                {
                    cap = "  当前标签:";
                }
                else
                {
                    cap = "  current label:";
                }
                Text += cap + Path.GetFileNameWithoutExtension(dataConfig.code_printer_template_path);
                //判断是是否需添加表的结构
                check_and_fix_db(dataConfig.to_print_data_path);
                sn_property property = new sn_property(dataConfig.to_print_data_path);
                //---------------------------------------------------------------------
                //复初始值
                /*if (prev_min_val == 0) */{
                    prev_min_val = int.Parse(property.sn_min);
                }
                /*if (prev_max_val == 0) */{
                    prev_max_val = int.Parse(property.sn_max);
                }
                //---------------------------------------------------------------------
                if (property.sn_print_magic_code == "") {
                    //加入自动载入打印脚本数据
                    string prn_file = dataConfig.code_printer_template_path;
                    FileStream fs = new FileStream(prn_file, FileMode.Open);
                    StreamReader rd = new StreamReader(fs);
                    string content = rd.ReadToEnd();
                    property.sn_print_magic_code = content;
                    fs.Close();
                    property.update();
                }
                load_data();
            } else {
                dataConfig.code_printer_template_path = "";
                dataConfig.to_print_data_path = "";
                dataConfig.savePrintDataPath();
                dataConfig.savePrintTemplatePath();
            }
            if (dataConfig.io_dev != "") {
                try {
                    serialport_iotrans.PortName = dataConfig.io_dev;
                    serialport_iotrans.Open();
                    bIOOpen = true;
                } catch{
                    bIOOpen = false;
                }
            }
            string path = dataConfig.code_printer_template_path;
            string tmp_name = "";
            if (path != "") {
                tmp_name = Path.GetFileNameWithoutExtension(path);
                path = Path.GetDirectoryName(path);
            }
            string db_file_name = path + "\\" + tmp_name + ".db3";
            sn_property sn_pro = null;
            if (File.Exists(db_file_name))
            {
                sn_pro = new sn_property(db_file_name);
            }
            else
            {
                if (dataConfig.lang == "zh")
                    MessageBox.Show("配置文件对应的打印模板或数据库为空，请手动选择标签命令！");
                else
                    MessageBox.Show("config file is empty，please select lable file cmd！");
                return;
            }

            //new 
            manual_print.Enabled = true;//test
            bScanerOpen = true;
            /*
            if (dataConfig.scan_serial_name!=""){
                _scanner = new scanner(dataConfig.scan_serial_name);
                _scanner.BarCodeEvent = new scanner.BarCodeDelegate(barcode_event);
                if (sn_pro.scan_check && _scanner.openScanner()) {
                    MessageBox.Show("打开标签扫描仪成功!");
                    toolstrip_codecheck.Text = "关闭标签扫描仪";
                    manual_print.Enabled = true;//test
                    bScanerOpen = true;
                } else if (sn_pro.scan_check && !_scanner.openScanner()) {
                    manual_print.Enabled = false;//test
                    MessageBox.Show("打开标签扫描仪失败!");
                    bScanerOpen = false;
                }
            }
             * */
            _mainform = this;
          
            if (sn_pro.scan_check&&bScanerOpen)
                //开启串口打印业务
                start_print_content();
            else if (!sn_pro.scan_check && !bScanerOpen)
                start_print_content();
            //开启定时检查日期
            init_date_change();
        }
        /*
        private void toolstrip_codecheck_Click(object sender, EventArgs e)
        {
            //fix bug 在扫描的时候不要让它有效
            if (toolstrip_codecheck.Text == "关闭标签扫描仪" && timer_cheksn.Enabled) {
                return;
            }
            bScanerOpen = !bScanerOpen;
            if (bScanerOpen){
                dataConfig.scanner_is_autoload = true;
               
                _scanner = new scanner(dataConfig.scan_serial_name);
                _scanner.BarCodeEvent = new scanner.BarCodeDelegate(barcode_event);
                if (_scanner.openScanner()) {
                    MessageBox.Show("打开标签扫描仪成功!");
                    toolstrip_codecheck.Text = "关闭标签扫描仪";
                } else {
                    MessageBox.Show("打开标签扫描仪失败!");
                    bScanerOpen = false;
                }
            }else{
                //barcode.Stop();
                _scanner.closeScanner();
                dataConfig.scanner_is_autoload = false;
                MessageBox.Show("关闭标签扫描仪!");
                toolstrip_codecheck.Text = "打开标签扫描仪";
            }
            dataConfig.saveScanAutoLoad();
        }
        */
        //日志log函数
        private void _log(string msg)
        {
            DataRow r = log_table.NewRow();
            r["log_time"] = DateTime.Now.ToShortTimeString();
            r["log_msg"] = msg;
            log_table.Rows.Add(r);
            adp_log.Update(log_table.GetChanges(DataRowState.Added));
            log_table.AcceptChanges();
        }
        //添加到表中的行
        private DataRow added_row = null;
        //
        //扫描仪读取触发事件
        //
        //ReadBarCode.BarCodes barCode
        private void barcode_event(string  barCode){
           // this.Text = barCode;
            //log("trigger barcode_event " + barCode + "\r\n");
            //echo_msg1.msg = barCode;
            if (added_row != null && barCode!="") 
            {
                //不要重复扫描
                if (Convert.ToString(added_row["success"]) == "OK" || Convert.ToString(added_row["success"]) == "NS") {
                    //log("in barcode_event :待扫描的已经OK\r\n");
                    return;
                }
                string print_sn = Convert.ToString(added_row["availed_sn"]);
                //edit 12.24
                //防止有回车符号，产生不匹配
                int lastChar = print_sn[print_sn.Length - 1];
                if (lastChar == '\r') {
                    print_sn = print_sn.Remove(print_sn.Length - 1, 1);
                }
                //edit 12.24
                //防止有回车符号，产生不匹配
                if (print_sn.Trim() != barCode) {
                    added_row["success"] = barCode;
                    bIsScannerOK = false;//失败
                    //log("in barcode_event: 扫描失败!\r\n");
                } else {
                    //log("in barcode_event:扫描成功!\n\n");
                    added_row["success"] = "OK";
                    bIsScannerOK = true;//成功
                    set_opt_btn(true);
                }

                if (bIsScannerOK)
                    send_plc_scan_result(true);
                else
                    send_plc_scan_result(false);

                string sql = "update sn set  success = \'{0}\' where sn = \'{1}\';";
                sql = string.Format(sql, Convert.ToString(added_row["success"]), Convert.ToString(added_row["sn"]));
                SQLiteHelper.ExecuteNonQuery(sqlite_conn, sql, null);
                //扫描成功了把这个新添加的置为null
                //add 2015.9.24
                if (bIsScannerOK) {
                    if (added_row != null)
                        added_row = null;//clear data
                }
            }
            //if (added_row == null) {
                //log("没有添加的行可以扫描!\r\n");
           // }
            //if (barCode == "") {
                //log("扫描不到序号号!\r\n");
            //}
            //log("没有添加的行可以扫描!\r\n");
        }
        //之前是2w
        private const int query_availed_sn_length = 10000*100;
        /// <summary>
        /// 查询前面一万条记录是否有重复的
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        private bool query_sn_is_validate(string sn) {
            //System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
            //w.Start();
            bool isValidate = true;
            int offset = 0;
            if (print_all_counts < query_availed_sn_length) {
                offset = 0;
            } else {
                offset = print_all_counts - query_availed_sn_length - 1;
            }
            string query_later_page_data = "select availed_sn from sn where id > \'{0}\' and availed_sn = \'{1}\'";
            query_later_page_data = string.Format(query_later_page_data, offset, sn);
            DataSet ds =  SQLiteHelper.ExecuteDataSet(sqlite_conn, query_later_page_data, null);
            //w.Stop();
            //Console.WriteLine(w.ElapsedMilliseconds);
            if (ds != null&&ds.Tables[0].Rows.Count>0) {
                isValidate = false;
                if (dataConfig.lang == "zh")
                    MessageBox.Show("!!!序列号重复!");
                else
                    MessageBox.Show("!!!serial number is used!");
            }
            return isValidate;
        }
        //用来记录初始设定最大最小值
        private int prev_min_val = 0;
        private int prev_max_val = 0;

        //打印逻辑分离
        private Thread _print_content_thread;
        private void _print_content_proc() {
            serialport_printer.PortName = dataConfig.code_printer_dev;
            serialport_printer.BaudRate = dataConfig.code_printer_dev_speed;
            //serialport_printer.WriteBufferSize = 1000000;
            try {
                serialport_printer.Open();
            } catch (Exception e) {
                MessageBox.Show(e.Message);
                System.Environment.Exit(-1);
            }
            while (_print_content_flag) {
                if (_print_content_queue.Count > 0) {
                    //MessageBox.Show("have print content message!");
                    byte[] data = null;
                    lock (_lock_print_queue) {
                        data = _print_content_queue.Dequeue();
                    }
                    String conten = Encoding.ASCII.GetString(data);
                    serialport_printer.Write(conten);
                    //Thread.Sleep(500);
                    Thread.Sleep(100);
                } 
                Thread.Sleep(10);
            }
            serialport_printer.Close();
        }
        //--------------------------------------------------------------------------------------
        private Thread _date_chaneg_work = null;
        private bool _date_change_work_flag = true;
        private bool _change_flag = false;
        private void init_date_change() {
            _date_chaneg_work = new Thread(new ThreadStart(_date_chaneg_proc));
            _date_chaneg_work.Start();
        }
        private void stop_date_change() {
            _date_change_work_flag = false;
        }
        private void _date_chaneg_proc() {
            while (_date_change_work_flag) {
                if (DateTime.Now.Year != _now.Year || DateTime.Now.Month != _now.Month || DateTime.Now.Day != _now.Day) {
                    _change_flag = true;
                }
                Thread.Sleep(10);
            }
        }
        
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private object _lock_print_queue = new object();
        //private Mutex _mutex = new Mutex(true);
        private Queue<byte[]> _print_content_queue = new Queue<byte[]>();

        private bool _print_content_flag = true;

        private void start_print_content() {
            _print_content_thread = new Thread(new ThreadStart(_print_content_proc));
            _print_content_thread.Start();
        }

        private void stop_print_content() {
            _print_content_flag = false;
        }

        //0:没有打印
        //1:打印成功
        //2:打印失败
        private int print_result = 0;
        //响应打印
        private bool post_print_code()
        {
            lbWarningMsg.Text = "";//先清掉
            bool min_val_changed = false;
            //bool max_val_changed = false;
            //!4.5
            sn_property pro = new sn_property(dataConfig.to_print_data_path);
            int print_min_val = Convert.ToInt32(pro.sn_min);
            int print_max_val = Convert.ToInt32(pro.sn_max);
            //设定的最小值改变了
            if (prev_min_val != print_min_val) {
                min_val_changed = true;
                prev_min_val = print_min_val;
            }
            /*
            //设定的最大值改变
            if (prev_max_val != print_max_val) {
                max_val_changed = true;
                prev_max_val = print_max_val;
            }
            */
            bool print_ok = false;
            int new_index = 1;
            int sn = start_sn;
            
            //这里会有问题,修改了起始打印点
            if (print_all_counts > 0 &&
                pre_print_sn >= print_min_val) {
                if (!min_val_changed) {
                    new_index = pre_print_id + 1;
                    sn = pre_print_sn + 1;
                } else {
                    new_index = pre_print_id + 1;
                    sn = print_min_val;
                }
            }
            //如果设置大小值配置发生变化了
            if (pre_print_sn < print_min_val) {
                new_index = pre_print_id + 1;
                sn = print_min_val;//从最小开始
            }
            /*
            if (print_min_val > 0 && sn < print_min_val) {
                sn = print_min_val;
            }
             * */
            //有日期变动，要把序号变成开始值
            if (_change_flag) {
                sn = start_sn;
                if (print_min_val > 0 && print_max_val > 0) {
                    sn = print_min_val;
                }
            }
           
            //解析打印数据
            //printer_dev dev = new printer_dev(dataConfig.code_printer_template_path);
            //sn_property property = new sn_property(dataConfig.to_print_data_path);
            if (string.IsNullOrEmpty(pro.sn_print_magic_code))
                return false;
            printer_dev dev = new printer_dev(pro.sn_print_magic_code);
            //超过设定数据
            /*
            if (dataConfig.print_max_val > 0) {
                if (sn > dataConfig.print_max_val) {
                    MessageBox.Show("打印值超出设定的上限!");
                    return false;
                }
            } 
             * */
            if (Convert.ToInt32(pro.sn_max) > 0) {
                if (sn > Convert.ToInt32(pro.sn_max)) {
                    if (dataConfig.lang == "zh")
                        MessageBox.Show("打印值超出设定的上限!");
                    else
                        MessageBox.Show("print value up to max number!");
                    return false;
                }
            } 
            if (!dev.parase_key_code(sn.ToString())) {
                if (dataConfig.lang == "zh")
                    MessageBox.Show("打印值超出设定范围!");
                else
                    MessageBox.Show("print value up to max number!");
                return false;
            }
            string true_sn = dev.TrueSN;
            //Console.WriteLine(true_sn);
            //debug for test
            //true_sn = "*A3II 5ZH A P 6002*";

            //查询前2万条记录是否有重复
            if ((pro.sn_check_repeat && query_sn_is_validate(true_sn)) /*|| !pro.sn_check_repeat*/)
            {
                added_row = product_list_table.NewRow();
                //fix bug 2015.10.14
                if (product_list_table.Rows.Count >= 1000)
                    product_list_table.Clear();
                //-----------------------------------------------------------------------------------------------------------------
                added_row["id"] = new_index.ToString();
                //Console.WriteLine(added_row["id"].ToString());
                added_row["availed_sn"] = true_sn.ToString();
                added_row["sn"] = sn.ToString();
                added_row["print_time"] = DateTime.Now.ToLongTimeString();
                added_row["success"] = !bScanerOpen?"NS":"";//fix bug 在没有打开的情况下直接把打印结果更新成NS
                cmdbuilder_product = new SQLiteCommandBuilder(adp_product);
                product_list_table.Rows.Add(added_row);
                adp_product.Update(product_list_table.GetChanges(DataRowState.Added));
                product_list_table.AcceptChanges();
                //更新下
                pre_print_id=new_index;
                pre_print_sn = sn ;
                gridEX1.MoveLast();
                try {
                    /*
                    serialport_printer.PortName = dataConfig.code_printer_dev;
                    serialport_printer.BaudRate = dataConfig.code_printer_dev_speed;
                    serialport_printer.Open();
                    Thread.Sleep(500);
                    serialport_printer.Write(dev.PrintContent);
                    serialport_printer.Close();
                     * */
                    //这里需要把打印逻辑分离
                    
                    byte[] data = Encoding.ASCII.GetBytes( dev.PrintContent);
                    lock (_lock_print_queue) {
                        _print_content_queue.Enqueue(data);
                    }
                    //-----------------------------------------------
                    to_print_sn_num--;//将待打印个数--
                    print_ok = true;
                    //更新日期和改变标志
                    if (_change_flag){
                        _now = DateTime.Now;
                        _change_flag = false;
                    }
                } catch(Exception err) {
                    print_result = 0;//没有打印
                    //串口异常了
                    //toolStripButton1_Click(null, null);
                    //为了测试，注释掉了
                    if (dataConfig.lang == "zh")
                        MessageBox.Show("标签打印机操作失败!" + err.Message);
                    else
                        MessageBox.Show("print error!" + err.Message);
                    goto _error;
                }
            } else {
                //异常
                if (dataConfig.lang == "zh")
                {
                    lbWarningMsg.Text = "生成条码异常，请检查系统日期是否被修改!";
                }
                else
                {
                    lbWarningMsg.Text = "gerator serial number error, please check the system date is changed!";
                }
                goto _error;
            }
            if (print_ok)
            {
                print_result = 1;//成功
            }
            else
            {
                print_result = 2;//失败
                //log("打印失败！\r\n");
            }
            _error:
            return print_ok;
        }
        /*
        //弹出选择打印多少个打印数据
        private void mutil_print_Click(object sender, EventArgs e)
        {
            bool r = check_serial_dev(dataConfig.code_printer_dev, 1) && 
                (dataConfig.to_print_data_path != "" && File.Exists(dataConfig.to_print_data_path));
            if (r) {
                frmSelectPrintSN sn_num_select = new frmSelectPrintSN();
                if (sn_num_select.ShowDialog() == DialogResult.OK) {
                    to_print_sn_num = sn_num_select.to_print_sn_num;
                    print_sn();
                }
            }
        }
         * */
        /// <summary>
        /// 检测串口设备
        /// </summary>
        /// <returns></returns>
        private bool check_serial_dev(string name, int type) {
            bool r = false;
            bool have_dev = false;
            string[] all_pors = SerialPort.GetPortNames();
            foreach (string p in all_pors) {
                if (p.Trim() == name.Trim()) {
                    have_dev = true;
                }
            }
            if (!have_dev) {
                string msg = "";
                if (type == 0) {
                    if (dataConfig.lang == "zh")
                        msg = "设备对应的PLC不通讯板存在，请检查是否连接!";
                    else
                        msg = "link to plc serial com can not find, please check ";
                    dataConfig.io_dev = "";
                } else if (type == 1) {
                    if (dataConfig.lang == "zh")
                        msg = "设备对应的打印机不存在，请检查是否连接!";
                    else
                        msg = "link to printer serial com can not find, please check ";
                    dataConfig.code_printer_dev = "";
                }/* else if (type == 2) {
                    msg = "设备对应的扫描枪不存在，请检查是否连接!";
                    dataConfig.scan_serial_name = "";
                }*/
                MessageBox.Show(msg);
                dataConfig.saveIO();
            }
            r = have_dev;
            return r;
        }
        //fix 2016.4.23
        private void set_print_ns() {
            if (added_row != null) {
                string sql = "update sn set  success = \'{0}\' where sn = \'{1}\';";
                added_row["success"] = "NS";
                sql = string.Format(sql, "NS", Convert.ToString(added_row["sn"]));
                added_row = null;//取消打印
                SQLiteHelper.ExecuteNonQuery(sqlite_conn, sql, null);
            }
        }

        /// <summary>
        /// 打印序列号
        /// </summary>
        //之前是200w
        private void print_sn() {
            if (print_all_counts >= 2000000) {
                if (dataConfig.lang == "zh")
                    MessageBox.Show("数据达到200万条，请关闭软件，修改数据库名称（加日期）然后重新打开软件并加载标签文件，提示输入起始值时要为最新的。", "重要提示");
                else
                    MessageBox.Show("database upto max limit，please close the application and rename database name, reopen the application.", "warning");
                return;
            }
            if (to_print_sn_num > 0) {
                ++print_all_counts;
                //计算出offset
                log_all_page_count = (print_all_counts+999) / 1000;
                //如果当前的offset不是最后
                if (page_offset != log_all_page_count-1) {
                    page_offset = log_all_page_count-1;
                    product_list_table.Clear();
                    string query_later_page_data = "select * from sn order by id limit 1000 offset {0};";
                    query_later_page_data = string.Format(query_later_page_data, page_offset * 1000);
                    //查询
                    adp_product = new SQLiteDataAdapter(query_later_page_data, sqlite_conn);
                    adp_product.Fill(product_list_table);
                    cmdbuilder_product = new SQLiteCommandBuilder(adp_product);
                    gridEX1.DataSource = product_list_table;
                    gridEX1.MoveLast();
                }
                //发送一个打印信号到串口，并在列表里面加入一行
                print_result = 0;//先初始化一下打印结果
                if (post_print_code()) {
                    //log("打印机开始打印!\r\n");
                    //如果日期改变，并且打印ok，那就把当前的日期改成目前的日期，这样可以继续打下去
                    //_now = DateTime.Now;
                    //加入从数据库中载入自动扫描的选项
                    sn_property pro = new sn_property(dataConfig.to_print_data_path);
                    if (pro.scan_check) {
                        set_opt_btn(false);
                        //初始化扫描结果为false
                        bIsScannerOK = false;
                        //开启定时器，用来等待正确的sn扫描值

                        print_period = 0;//用来计时的
                        timer_cheksn.Start();

                        //让按钮显示出来
                        if (bScanerOpen)
                            btnCancelScan.Visible = true;
                    } else {
                        //把结果置为ns
                        set_print_ns();
                    }
                } else {
                    set_opt_btn(true);
                    //返回操作
                    --print_all_counts;
                }
            }
        }
        //自动打印时候用
        private void auto_print(object sender, EventArgs e)
        {
            string path = dataConfig.code_printer_template_path;
            string tmp_name = "";
            if (path != "")
            {
                tmp_name = Path.GetFileNameWithoutExtension(path);
                path = Path.GetDirectoryName(path);
            }
            string db_file_name = path + "\\" + tmp_name + ".db3";
            sn_property sn_pro = new sn_property(db_file_name);
            /*
            bool t = false;
            if (sn_pro.scan_check)
                t = check_serial_dev(dataConfig.scan_serial_name, 2);
            else
                t=true;
            */
            bool r = check_serial_dev(dataConfig.code_printer_dev, 1) /*&&
                      t*/
                           &&
                    (dataConfig.to_print_data_path != "" &&
                    File.Exists(dataConfig.to_print_data_path)
                );
            if (r)
            {
                to_print_sn_num = 1;
                //fix 2107.4.23.16.50
                this.Invoke((EventHandler)(delegate { print_sn(); }));

            }
            /*
            to_print_sn_num = 1;
            print_sn();
             * */

        }

        //打印按钮事件
        private void manual_print_Click(object sender, EventArgs e) {
            //insert_test();
            //return;
            //dataConfig.loadConfig();
             frmLogin login = new frmLogin();
             if (login.ShowDialog() == DialogResult.OK)
             {
                 string path = dataConfig.code_printer_template_path;
                 string tmp_name = "";
                 if (path != "")
                 {
                     tmp_name = Path.GetFileNameWithoutExtension(path);
                     path = Path.GetDirectoryName(path);
                 }
                 string db_file_name = path + "\\" + tmp_name + ".db3";
                 sn_property sn_pro = new sn_property(db_file_name);
                 /*
                 bool t = false;
                 if (sn_pro.scan_check)
                     t = check_serial_dev(dataConfig.scan_serial_name, 2);
                 else
                     t=true;
                 */
                 bool r = check_serial_dev(dataConfig.code_printer_dev, 1) /*&&
                      t*/
                                &&
                         (dataConfig.to_print_data_path != "" &&
                         File.Exists(dataConfig.to_print_data_path)
                     );
                 if (r)
                 {
                     to_print_sn_num = 1;
                     //fix 2107.4.23.16.50
                     this.Invoke((EventHandler)(delegate { print_sn(); }));

                 }
                 /*
                 to_print_sn_num = 1;
                 print_sn();
                  * */
             }
        }
        
        //io发送数据
        private void send2iotranslate(byte[] data) {
            if (serialport_iotrans.IsOpen)
            {
                serialport_iotrans.Write(data, 0, data.Length);
            }
        }

        private delegate void _setLabelImage(Label l, bool isSucess);

        private void setLabelImage(Label l, bool isSucess) {
            if (l.InvokeRequired) {
                l.Invoke(new _setLabelImage(setLabelImage), new object[] { l, isSucess });
            } else {
                if (isSucess) {
                    l.Image = Properties.Resources.green;
                } else {
                    l.Image = Properties.Resources.gray;
                }
            }
        }

        //设置out_led的颜色
        private void set_in_led(int led, bool s) {
            Label inLabel = null;
            switch (led) {
                case 0:
                    inLabel = lbIN0;
                    break;
                case 1:
                    inLabel = lbIN1;
                    break;
                case 2:
                    inLabel = lbIN2;
                    break;
                case 3:
                    inLabel = lbIN3;
                    break;
                case 4:
                    inLabel = lbIN4;
                    break;
            }
            /*
            if (s) {
                this.Invoke((EventHandler)(delegate {
                    inLabel.Image = Properties.Resources.green;
                }));
            } else {
                this.Invoke((EventHandler)(delegate {
                    inLabel.Image = Properties.Resources.gray;
                }));
            }
             * */
            setLabelImage(inLabel, s);
        }
        //设置out_led的颜色
        private void set_out_led(int led, bool s) {
            Label outLabel = null;
            switch (led) {
                case 0:
                    outLabel = lbOUT0;
                    break;
                case 1:
                    outLabel = lbOUT1;
                    break;
                case 2:
                    outLabel = lbOUT2;
                    break;
                case 3:
                    outLabel = lbOUT3;
                    break;
                case 4:
                    outLabel = lbOUT4;
                    break;
            }
            setLabelImage(outLabel, s);
            /*
            if (s) {
                this.Invoke((EventHandler)(delegate {
                    outLabel.Image = Properties.Resources.red;
                }));
            } else {
                this.Invoke((EventHandler)(delegate {
                    outLabel.Image = Properties.Resources.gray;
                }));
            }
             * */
        }
        private delegate void _log_io_msg(string msg);
        /// <summary>
        /// 用来显示io口来的命令信息
        /// </summary>
        /// <param name="msg"></param>
        private void log_io_msg(string msg) {
            if (echo_msg1.InvokeRequired) {
                echo_msg1.Invoke(new _log_io_msg(log_io_msg), new object[] { msg });
            } else {
                echo_msg1.msg = msg;
            }
        }
        //用来记录之前的数据
        private byte in_status_data = 0;
        private byte out_status_data = 0;
        //记录之前的命令字节
        private byte pre_cmd_data = 0;
        //bug fix 
        private bool is_ready = false;//用来显示ready的状态

        private delegate void _do_print_sn();
        /*
        private void do_print_sn() {
            if (this.InvokeRequired) {
                this.Invoke(new _do_print_sn(do_print_sn), null);
            } else {
                to_print_sn_num = 1;
                print_sn();
                sn_property pro = new sn_property(dataConfig.to_print_data_path);
                if (pro.scan_check)
                    bIsScannerOK = false;
                else
                    bIsScannerOK = true;
            }
        }*/
        /*
        private Thread _check_time_chaneg_work = null;
        private bool check_time_flag = true;
        private object _lock_check_time_change = new object();
        private void entery_check_time_change() {
            _check_time_chaneg_work = new Thread(new ThreadStart(delegate {
                while (check_time_flag) {
                    lock (_lock_check_time_change) {
                        if (DateTime.Now.Year != _now.Year || DateTime.Now.Month != _now.Month || DateTime.Now.Day != _now.Day) {
                            _datatime_chaneged = true;
                        }
                    }
                }
            }));
        }
        private void exit_check_time_change() {
            check_time_flag = false;
        }
        */
        /// <summary>
        /// 计时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private UInt32 _clock_count = 0;
        private void timerClock_Tick(object sender, EventArgs e) {
            /*
            if (DateTime.Now.Year != _now.Year || DateTime.Now.Month != _now.Month || DateTime.Now.Day != _now.Day) {
                _datatime_chaneged = true;
            }
             * */
        }
        private void send_plc_res(byte[] data)
        {
            serialport_iotrans.Write(data, 0, data.Length);
        }
        //返回打印控制
        private void send_plc_res()
        {
            byte[] res = new byte[6];
            res[0] = 0x5B;
            res[1] = 0x50;
            res[2] = 0x52;
            res[3] = 0x5d;
            res[4] = 0x0D;
            res[5] = 0x0a;
            send_plc_res(res);
        }
        //返回扫描结果
        private void send_plc_scan_result(bool isOK)
        {
            byte[] res = new byte[6];
            res[0] = 0x5B;
            if (isOK)
            {
                res[1] = 0x4F;
                res[2] = 0x4B;
            }
            else
            {
                res[1] = 0x4E;
                res[2] = 0x47;
            }
            res[3] = 0x5d;
            res[4] = 0x0D;
            res[5] = 0x0a;
            send_plc_res(res);
        }
        //返回标签
        private void send_plc_chg_lab_result(string lab)
        {
            byte[] res = new byte[6 + lab.Length];
            res[0] = 0x5B;
            res[1] = 0x46;
            res[2] = 0x49;
            res[3] = 0x5D;
            byte[] lab_buf = Encoding.ASCII.GetBytes(lab);
            Array.Copy(lab_buf, 0, res, 4, lab_buf.Length);
            res[6 + lab.Length - 1 - 1] = 0x0D;
            res[6 + lab.Length - 1] = 0x0D;
            send_plc_res(res);
        }
        //fix bug 只有接收到扫描ok才继续扫描
        //第一次操作
       //private UInt32 _pre_time = 0, _cur_time = 0;
       private void _proto_parase_on_packet_data(int cmd, string param){
           //收到打印命令
           if (cmd == 0x0)
           {
               if (manual_print.Enabled && bIsScannerOK)
               {
                   bool r = check_serial_dev(dataConfig.code_printer_dev, 1) && (dataConfig.to_print_data_path != "" && File.Exists(dataConfig.to_print_data_path));
                   if (r)
                   {
                       //打印
                       //do_print_sn();
                       auto_print(null, null);//fix 2017/8/25
                       //manual_print_Click(null, null);
                       //发送plc状态
                       send_plc_res();
                   }
               }

           }
           else if (cmd == 0x1)
           {
               barcode_event(param);
           }
           else if (cmd == 0x2)
           {
               //改变便签
               this.Invoke((EventHandler)(delegate {
                   string path = "c:\\labels";
                   chanegPrintTemp(param, false, path);
               }));

           }
           /*
           if (msg.cmd == 0x02 && msg.len == 1) {
               byte data = msg.data[0];
               if (data == 0xA0) {
                   if (manual_print.Enabled && bIsScannerOK) {
                       bool r = check_serial_dev(dataConfig.code_printer_dev, 1) && (dataConfig.to_print_data_path != "" && File.Exists(dataConfig.to_print_data_path));
                       
                       if (r ) {
                           do_print_sn();
                       }

                       string path = dataConfig.code_printer_template_path;
                       string tmp_name = "";
                       if (path != "") {
                           tmp_name = Path.GetFileNameWithoutExtension(path);
                           path = Path.GetDirectoryName(path);
                       }
                       string db_file_name = path + "\\" + tmp_name + ".db3";
                       //sn_property sn_pro = new sn_property(db_file_name);
                       if (!bScanerOpen)
                           send2iotranslate(req_data_helper.make_print_res(0x13));
                   }
               } else if (data == 0xA1) {
                   send2iotranslate(req_data_helper.make_scanner_res((byte)(bIsScannerOK ? 2 : 1)));
               } else if (data == 0xA2) {
                   send2iotranslate(req_data_helper.make_reset_res());
               } else if (data == 0xA3) {
                   send2iotranslate(req_data_helper.make_ready_res((byte)(bIOOpen && bDatabaseOpen && bScanerOpen ? 2 : 1)));
               }
               //fix bug ready 命令
               if (data == 0xA3)
               {
                   is_ready = true;
                   ready_timeout = 0;
               }
               //用来显示信息
               if (data != pre_cmd_data) {
                   pre_cmd_data = data;
                   if (data == 0xA0)
                   {
                       log_io_msg("收到打印命令!");
                   }
                   else if (data == 0xA1) {
                       log_io_msg("收到查询扫描结果命令!");
                   } else if (data == 0xA2) {
                       log_io_msg("收到复位命令!");
                   } 
               }
           //状态数据
           } else if (msg.cmd == 0x01 && msg.len == 2) {
               byte data_status0 = msg.data[0];
               byte data_status1 = msg.data[1];
               if (data_status0 != in_status_data) {
                   in_status_data = data_status0;
                   if ((data_status0 & 1) == 1) {
                       set_in_led(0, true);
                   } else if ((data_status0 & 1) == 0) {
                       set_in_led(0, false);
                   }
                   if ((data_status0 & 2) == 2) {
                       set_in_led(1, true);
                   } else if ((data_status0 & 2) == 0) {
                       set_in_led(1, false);
                   }
                   if ((data_status0 & 4) == 4) {
                       set_in_led(2, true);
                   } else if ((data_status0 & 4) == 0) {
                       set_in_led(2, false);
                   }
                   if ((data_status0 & 8) == 8) {
                       set_in_led(3, true);
                   } else if ((data_status0 & 8) == 0) {
                       set_in_led(3, false);
                   }
                   if ((data_status0 & 16) == 16) {
                       set_in_led(4, true);
                   } else if ((data_status0 & 16) == 0) {
                       set_in_led(4, false);
                   }
               }
               if (out_status_data != data_status1) {
                   out_status_data = data_status1;
                   if ((data_status1 & 1) == 1) {
                       set_out_led(0, true);
                   } else if ((data_status1 & 1) == 0) {
                       set_out_led(0, false);
                   }
                   if ((data_status1 & 2) == 2) {
                       set_out_led(1, true);
                   } else if ((data_status1 & 2) == 0) {
                       set_out_led(1, false);
                   }
                   if ((data_status1 & 4) == 4) {
                       set_out_led(2, true);
                   } else if ((data_status1 & 4) == 0) {
                       set_out_led(2, false);
                   }
                   if ((data_status1 & 8) == 8) {
                       set_out_led(3, true);
                   } else if ((data_status1 & 8) == 0) {
                       set_out_led(3, false);
                   }
                   if ((data_status1 & 16) == 16) {
                       set_out_led(4, true);
                   } else if ((data_status1 & 16) == 0) {
                       set_out_led(4, false);
                   }
               }
           }
            * */
       }
        private delegate void BarCodeDelegate(string barCode);
        private BarCodeDelegate BarCodeEvent;
        //PLC口接收到数据
        private void serialport_iotrans_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            if (bIOOpen) {
                serialport_iotrans.ReceivedBytesThreshold = 5000;
                byte data = (byte)serialport_iotrans.ReadByte();
                _proto_parase.push_byte(data);
                serialport_iotrans.ReceivedBytesThreshold = 1;
            }
        }
        private void gridEX1_FormattingRow(object sender, Janus.Windows.GridEX.RowLoadEventArgs e) {
            if (e.Row.Cells["success"].Text == "NS") {
                e.Row.Cells["success"].FormatStyle = new Janus.Windows.GridEX.GridEXFormatStyle();
                e.Row.Cells["success"].FormatStyle.ForeColor = Color.Red;
                e.Row.Cells["success"].FormatStyle.FontBold = Janus.Windows.GridEX.TriState.True;
            } else if (e.Row.Cells["success"].Text == "OK") {
                e.Row.Cells["success"].FormatStyle = new Janus.Windows.GridEX.GridEXFormatStyle();
                e.Row.Cells["success"].FormatStyle.ForeColor = Color.Green;
                e.Row.Cells["success"].FormatStyle.FontBold = Janus.Windows.GridEX.TriState.True;
            } else {
                e.Row.Cells["success"].FormatStyle = new Janus.Windows.GridEX.GridEXFormatStyle();
                e.Row.Cells["success"].FormatStyle.ForeColor = Color.Red;
                e.Row.Cells["success"].FormatStyle.FontBold = Janus.Windows.GridEX.TriState.True;
            }
        }
        /// <summary>
        /// 跳转到指定页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void jump_pageoffset_Click(object sender, EventArgs e) {
            if (tbJumpoffset.Text != ""&&!btnCancelScan.Visible) {
                try {
                    int jump_offset = 0;
                    jump_offset = int.Parse(tbJumpoffset.Text.Trim());
                    page_offset = (jump_offset  + 999)/ 1000 - 1;
                    string query_later_page_data = "select * from sn order by id limit 1000 offset {0};";
                    query_later_page_data = string.Format(query_later_page_data, page_offset * 1000);
                    //查询
                    product_list_table.Clear();
                    adp_product = new SQLiteDataAdapter(query_later_page_data, sqlite_conn);
                    adp_product.Fill(product_list_table);
                    gridEX1.MoveTo(jump_offset-1);
                    tbJumpoffset.Text = "";
                    //gridEX1.MoveFirst();    
                } catch {
                    if (dataConfig.lang == "zh")
                        MessageBox.Show("请输入准确的数字!");
                    else
                        MessageBox.Show("please input the correct number!");
                }
            }
        }
        /// <summary>
        /// 向上翻页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void page_up_Click(object sender, EventArgs e) {
            if (btnCancelScan.Visible)
                return;
            if (page_offset >=0) {
                string query_later_page_data = "select * from sn order by id limit 1000 offset {0};";
                query_later_page_data = string.Format(query_later_page_data, page_offset * 1000);
                //查询
                product_list_table.Clear();
                adp_product = new SQLiteDataAdapter(query_later_page_data, sqlite_conn);
                adp_product.Fill(product_list_table);
                --page_offset;
                if (page_offset <= 0)
                    page_offset = 0;
                gridEX1.MoveFirst();
            }
        }
        /// <summary>
        /// 向下翻页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void page_down_Click(object sender, EventArgs e) {
            if (btnCancelScan.Visible)
                return;
            if (page_offset <=log_all_page_count-1) {
                string query_later_page_data = "select * from sn order by id limit 1000 offset {0};";
                query_later_page_data = string.Format(query_later_page_data, page_offset * 1000);
                //查询
                product_list_table.Clear();
                adp_product = new SQLiteDataAdapter(query_later_page_data, sqlite_conn);
                adp_product.Fill(product_list_table);
                ++page_offset;
                if (page_offset >= log_all_page_count) {
                    page_offset = log_all_page_count - 1;
                }
                gridEX1.MoveLast();
            }
        }

        private void cancel_scan_res(){
            if (added_row != null)
            {
                string sql = "update sn set  success = \'{0}\' where sn = \'{1}\';";
                added_row["success"] = "NS";
                sql = string.Format(sql, "NS", Convert.ToString(added_row["sn"]));
                added_row = null;//取消打印
                SQLiteHelper.ExecuteNonQuery(sqlite_conn, sql, null);
            }
            //使得打印按钮有效，继续打印
            btnCancelScan.Visible = false;
            bIsScannerOK = true;//假设已经扫描ok
            set_opt_btn(true);
            lbWarningMsg.Text = "";
        }

        /// <summary>
        /// 检测sn定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private int dot_count = 1;
        private int print_period = 0;//打印周期
        private const int PRINT_PERIOD = 30;//30s
        private void timer_cheksn_Tick(object sender, EventArgs e) {
            //打印周期
            print_period++;
            //如果扫描枪打开的
            if (bScanerOpen)
            {
                if (print_result == 1)//fix bug 如果没有开始一个打印事件
                {
                    if (!bIsScannerOK)
                    {
                        if (print_period <= PRINT_PERIOD)
                        {
                            if (dot_count >= 10)
                            {
                                dot_count = 1;
                            }
                            //lbWarningMsg.Text = "等待扫描结果";
                            for (int i = 0; i < dot_count; i++)
                            {
                                //lbWarningMsg.Text += ".";
                                if (dataConfig.lang == "zh")
                                    lbWarningMsg.Text = "等待扫描结果 " + print_period + "S";
                                else
                                    lbWarningMsg.Text = "wait scaner " + print_period + "S";
                                
                            }
                            dot_count++;
                        }
                            //周期到
                        else
                        {
                            cancel_scan_res();
                            send_plc_scan_result(false);
                            print_period = 0;
                            dot_count = 1;
                            timer_cheksn.Stop();//结束打印
                        }
                    }
                    //扫描成功
                    else
                    {
                        print_period = 0;
                        dot_count = 1;
                        lbWarningMsg.Text = "";
                        btnCancelScan.Visible = false;
                        set_opt_btn(true);
                        timer_cheksn.Stop();//结束打印
                    }
                }
                    //打印不成功
                else
                {
                    print_period = 0;
                    dot_count = 1;
                    lbWarningMsg.Text = "";
                    btnCancelScan.Visible = false;
                    set_opt_btn(true);
                    timer_cheksn.Stop();//结束打印
                }

            }
                /*
                else
                {
                    //开始新的扫描
                    if (to_print_sn_num > 0)
                    {
                        if (print_period > PRINT_PERIOD)
                        {
                            print_period = 0;
                            bIsScannerOK = false;
                            print_sn();
                            dot_count = 1;
                        }
                    }
                    else
                    {
                        lbWarningMsg.Text = "";
                        btnCancelScan.Visible = false;
                        set_opt_btn(true);
                        timer_cheksn.Stop();//结束打印
                    }
                }
            }else {
                //开始新的扫描
                if (to_print_sn_num > 0) {
                    if (print_period > PRINT_PERIOD)
                    {
                        print_period = 0;
                        bIsScannerOK = false;
                        print_sn();
                        dot_count = 1;
                    }
                } else {
                    lbWarningMsg.Text = "";
                    btnCancelScan.Visible = false;
                    set_opt_btn(true);
                    timer_cheksn.Stop();//结束打印
                }
            }*/
        }
        /// <summary>
        /// 屏蔽掉alt + F4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainFrm_KeyDown(object sender, KeyEventArgs e) {
            if ((e.KeyCode == Keys.F4) && (e.Alt == true)) {
                e.Handled = true;
            }
        }

        private void mainFrm_Resize(object sender, EventArgs e) {
            setup_gridex_size();
        }
        private void tbJumpoffset_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 13 && !btnCancelScan.Visible)
                jump_pageoffset_Click(sender, null);
        }
        /*
        /// <summary>
        /// 停止连续打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e) {
            if (added_row != null) {
                string sql = "update sn set  success = \'{0}\' where sn = \'{1}\';";
                added_row["success"] = "NS";
                sql = string.Format(sql, "NS", Convert.ToString(added_row["sn"]));
                added_row = null;//取消打印
                SQLiteHelper.ExecuteNonQuery(sqlite_conn, sql, null);
            }
            to_print_sn_num = 0;
            //使得打印按钮有效，继续打印
            btnCancelScan.Visible = false;
            bIsScannerOK = true;//假设已经扫描ok
            set_opt_btn(true);
            lbWarningMsg.Text = "";
        }
         * */
        private int ready_timeout = 0;
        /// <summary>
        /// ready信号显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_check_ready_Tick(object sender, EventArgs e)
        {
            if (++ready_timeout > 5)
            {
                ready_timeout = 0;
                if (!is_ready)
                {
                    lbLink.ForeColor = Color.White;
                    lbLink.BackColor = Color.Gray;
                }
                is_ready = false;
            }
            if (is_ready)
            {
                lbLink.ForeColor = Color.White;
                lbLink.BackColor = Color.Green;
            }
        }
        private void gridEX1_SizingColumn(object sender, Janus.Windows.GridEX.SizingColumnEventArgs e) {
            bool changed = false;
            if (dataConfig.index_size != gridEX1.RootTable.Columns[0].Width) {
                dataConfig.index_size = gridEX1.RootTable.Columns[0].Width;
                changed = true;
            }
            if (dataConfig.sn_size != gridEX1.RootTable.Columns[1].Width) {
                dataConfig.sn_size = gridEX1.RootTable.Columns[1].Width;
                changed = true;
            }
            if (dataConfig.print_date_size != gridEX1.RootTable.Columns[2].Width) {
                dataConfig.print_date_size = gridEX1.RootTable.Columns[2].Width;
                changed = true;
            }
            if ( dataConfig.print_success_size != gridEX1.RootTable.Columns[3].Width){
                 dataConfig.print_success_size = gridEX1.RootTable.Columns[3].Width;
                 changed = true;
            }
            if (changed) {
                dataConfig.saveCellSize();
            }
        }
        //修改
        private void print_minmax_Click(object sender, EventArgs e) {
            /*
              frmPrintMinMax dlg = new frmPrintMinMax();
              dlg.ShowDialog();
               * */
            string msg = "";
            if (dataConfig.lang == "zh")
            {
                msg = "设置打印参数";
            }
            else
            {
                msg = "set the printer";
            }
            frmCreatedatabase set = new frmCreatedatabase(msg, true);
            set.ShowDialog();
            string path = dataConfig.code_printer_template_path;
            string tmp_name = "";
            if (path != "") {
                tmp_name = Path.GetFileNameWithoutExtension(path);
                path = Path.GetDirectoryName(path);
            }
            string db_file_name = path + "\\" + tmp_name + ".db3";
            sn_property sn_pro = new sn_property(db_file_name);
            if (!sn_pro.scan_check) {
                manual_print.Enabled = true;
            } else {
                //new add
                bScanerOpen = true;
                manual_print.Enabled = true;//test
                //扫描
                /*
                if (dataConfig.scan_serial_name != "") {
                    _scanner = new scanner(dataConfig.scan_serial_name);
                    _scanner.BarCodeEvent = new scanner.BarCodeDelegate(barcode_event);
                    if (_scanner.openScanner()) {
                        toolstrip_codecheck.Text = "关闭标签扫描仪";
                        bScanerOpen = true;
                        manual_print.Enabled = true;//test
                    } else {
                        if (sn_pro.scan_check) {
                            manual_print.Enabled = false;//test
                            bScanerOpen = false;
                        }
                    }
                }
                 * */
            }
        }
        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager res = new ComponentResourceManager(typeof(mainFrm));
            foreach (Control ctl in Controls)
            {
                res.ApplyResources(ctl, ctl.Name);
                if (ctl is Panel)
                {
                    foreach (Control sub in ctl.Controls)
                    {
                        res.ApplyResources(sub, sub.Name);
                    }
                }
            }
            //菜单
            foreach (ToolStripItem item in main_toolstrip.Items)
            {
                res.ApplyResources(item, item.Name);
                /*
                foreach (ToolStripItem subItem in item.DropDownItems)
                {
                    res.ApplyResources(subItem, subItem.Name);
                }
                 * */
            }
            //Caption
            res.ApplyResources(this, "$this");
        }
        private void btnLanguageSwitch_Click(object sender, EventArgs e)
        {
            string msg = "";
            if (btnLanguageSwitch.Text == "change language")
            {
                //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
                dataConfig.lang = "zh";
                msg = "set the langue require reboot application, please reboot application!";
            }
            else
            {
                //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                dataConfig.lang = "en";
                msg = "设置语言需要重新启动程序,请重新启动软件!";
            }
            dataConfig.saveLanguage();
            MessageBox.Show(msg);
        }
    }
}