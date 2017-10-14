using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
namespace barcode_printer {
    class sn_property {
        private SQLiteConnection _conn = new SQLiteConnection();
        private bool _scan_check = true;
        private string _sn_min = "";
        private string _sn_max = "";
        private string _sn_print_magic_code = "";
        private bool _sn_check_repeat = true;
        private string _sn_start = "";
        public string sn_min{
            get{return _sn_min;}
            set{_sn_min = value;}
        }
        public bool scan_check {
            get { return _scan_check; }
            set { _scan_check = value; }
        }
        public string sn_max {
            get{return _sn_max;}
            set{_sn_max = value;}
        }
        public string sn_print_magic_code{
            get{return _sn_print_magic_code;}
            set{_sn_print_magic_code = value;}
        }
        public bool sn_check_repeat {
            get{return _sn_check_repeat;}
            set{_sn_check_repeat = value;}
        }
        public string sn_start{
            get {return _sn_start;}
            set {_sn_start = value;}
        }
        //获取值
        public sn_property(string db_name){
            _conn.ConnectionString = "Data Source = " + db_name;
            _conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(_conn);
            cmd.CommandText = "SELECT * FROM sn_property;";
            SQLiteDataReader rd = cmd.ExecuteReader();
            if (rd.Read()) {
                sn_min = rd["sn_min"].ToString().Trim();
                sn_max = rd["sn_max"].ToString().Trim();
                sn_start = rd["start_sn"].ToString().Trim();
                sn_print_magic_code = rd["sn_print_magic_code"].ToString();
                sn_check_repeat = Convert.ToBoolean(rd["sn_check_repeat"]);
                scan_check = Convert.ToBoolean(rd["scan_check"]);
            }
            rd.Close();
            _conn.Close();
        }
        //更新值
        public void update(){
            _conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(_conn);
            cmd.CommandText = string.Format("UPDATE sn_property SET start_sn = \'{0}\', sn_min =  \'{1}\', sn_max = \'{2}\', sn_print_magic_code = \'{3}\',sn_check_repeat = \'{4}\', scan_check = \'{5}\'", sn_start, sn_min, sn_max, sn_print_magic_code, sn_check_repeat ? "1" : "0", scan_check?"1":"0");
            cmd.ExecuteNonQuery();
            _conn.Close();
        }
    }
}
