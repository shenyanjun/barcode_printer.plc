using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace barcode_printer
{
    class dataConfig
    {

        public static bool diaglog_warn_flag = false;

        //打印机配置
        public static string code_printer_dev = "";
        public static string code_printer_dev_detail = "";
        public static int code_printer_dev_speed = 9600;
        public static bool code_printer_is_autoload = true;

        //io
        public static string io_dev = "";
        public static string io_dev_detail = "";
        public static int io_dev_speed = 115200;
        public static bool io_dev_is_autoload = true;

        //标签扫描仪
        public static bool scanner_is_autoload = true;
        public static string code_printer_template_path = "";

        //打印参数
        public static int autoprint_timeout = 3000;//知道打印的超时

        public static int index_size = 80;
        public static int sn_size = 200;
        public static int print_date_size = 200;
        public static int print_success_size = 200;

        //标签扫描
        public static string scan_serial_name = "";

        //打印上限和下限
        public static int print_max_val = 9999;
        public static int print_min_val = 1;
        //待打印数据的路径
        public static string to_print_data_path = "";
        public static  ini_cls ini = new ini_cls(@".\cfg.ini");
        public static dataConfig _config = null;
        //加入权限
        #region private manager
        public static bool isSuperAccess = false;
        #endregion
        //加入中英文选项
        public static string lang = "zh";

        public static void loadConfig()
        {
            code_printer_dev = ini.IniReadValue("set", "code_printer_dev");
            code_printer_dev_detail = ini.IniReadValue("set", "code_printer_dev_detail");
            code_printer_dev_speed = Convert.ToInt32(ini.IniReadValue("set", "code_printer_dev_speed"));
            code_printer_is_autoload = ini.IniReadValue("set", "code_printer_is_autoload") == "true" ? true : false ;

            io_dev = ini.IniReadValue("set", "io_dev");
            io_dev_detail = ini.IniReadValue("set", "io_dev_detail");
            io_dev_speed = Convert.ToInt32(ini.IniReadValue("set", "io_dev_speed"));
            io_dev_is_autoload = ini.IniReadValue("set", "io_dev_is_autoload") == "true" ? true : false;

            scanner_is_autoload = ini.IniReadValue("set", "scanner_is_autoload") == "true"?true:false;
            to_print_data_path = ini.IniReadValue("set", "to_print_data_path");
            code_printer_template_path = ini.IniReadValue("set", "code_printer_template_path");

            autoprint_timeout =int.Parse( ini.IniReadValue("params", "autoprint_timeout"));

            index_size = int.Parse(ini.IniReadValue("print_cell_size", "index_size"));
            sn_size = int.Parse(ini.IniReadValue("print_cell_size", "sn_size"));
            print_date_size = int.Parse(ini.IniReadValue("print_cell_size", "print_date_size"));
            print_success_size = int.Parse(ini.IniReadValue("print_cell_size", "print_success_size"));
            print_min_val = int.Parse(ini.IniReadValue("print_max_min_val", "min"));
            print_max_val = int.Parse(ini.IniReadValue("print_max_min_val", "max"));

            //标签扫描
            scan_serial_name = ini.IniReadValue("set", "scan_serial_name");

            lang = ini.IniReadValue("set", "lang");
        }

        public static void saveLanguage()
        {
            ini.IniWriteValue("set", "lang", lang);
        }

        public static void saveCellSize() {
            ini.IniWriteValue("print_cell_size", "index_size", index_size.ToString());
            ini.IniWriteValue("print_cell_size", "sn_size", sn_size.ToString());
            ini.IniWriteValue("print_cell_size", "print_date_size", print_date_size.ToString());
            ini.IniWriteValue("print_cell_size", "print_success_size", print_success_size.ToString());
        }

        public static void savePrintDataPath() {
            ini.IniWriteValue("set", "to_print_data_path", to_print_data_path);
        }

        public static void savePrintTemplatePath() {
            ini.IniWriteValue("set", "code_printer_template_path", code_printer_template_path);
        }
        public static void saveIO() {
            ini.IniWriteValue("set", "code_printer_dev", code_printer_dev);
            ini.IniWriteValue("set", "code_printer_dev_detail", code_printer_dev_detail);
            ini.IniWriteValue("set", "code_printer_dev_speed", code_printer_dev_speed.ToString());
            ini.IniWriteValue("set", "io_dev", io_dev);
            ini.IniWriteValue("set", "io_dev_detail", io_dev_detail);
            ini.IniWriteValue("set", "io_dev_speed", io_dev_speed.ToString());
            //标签扫描scan_serial_name = ini.IniReadValue("scan_serial_name", "name");
            ini.IniWriteValue("set", "scan_serial_name", scan_serial_name);
        }
        public static void saveParamMinMax() {
            ini.IniWriteValue("print_max_min_val", "max", print_max_val.ToString());
            ini.IniWriteValue("print_max_min_val", "min", print_min_val.ToString());
        }
        public static void saveScanAutoLoad() {
            ini.IniWriteValue("set", "scanner_is_autoload", scanner_is_autoload == true ? "true" : "false");
        }
        public static void savePrinterAutoLoad() {
            ini.IniWriteValue("set", "code_printer_is_autoload", code_printer_is_autoload == true ? "true" : "false");
        }
        public static void saveIOAutoLoad() {
            ini.IniWriteValue("set", "io_dev_is_autoload", io_dev_is_autoload == true ? "true" : "false");
        }
    }
}
