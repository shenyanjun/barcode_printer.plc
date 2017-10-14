using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
namespace barcode_printer {
    class user_info {
        private SQLiteConnection _conn = new SQLiteConnection();
        private string _user_name = "";
        private string _pass_word = "";
        private bool _is_locked = false;
        private DateTime _last_login_time;
        public user_info() {
            _conn.ConnectionString = "Data Source = " + System.Environment.CurrentDirectory +"\\"+"user_info.db3";
            _conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(_conn);
            cmd.CommandText = "SELECT * FROM users;";
            SQLiteDataReader rd = cmd.ExecuteReader();
            if (rd.Read()) {
                user_name = rd["user_name"].ToString().Trim();
                pass_word = rd["user_password"].ToString().Trim();
                last_login_time = DateTime.FromFileTime(Convert.ToInt64(rd["last_login_time"].ToString()));
                locked = Convert.ToBoolean(rd["is_locked"]);
            }
            rd.Close();
            _conn.Close();
        }

        public void update(string pass, bool is_locked, DateTime login_time) {
            _conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(_conn);
            //SQLiteParameter param_user = new SQLiteParameter("@user_name", user_name);
            cmd.CommandText = string.Format("update users set user_password =  \'{0}\', is_locked = \'{1}\', last_login_time =\'{2}\'", pass, is_locked, login_time);
            cmd.ExecuteNonQuery();
            _conn.Close();
        }
        public string user_name
        {
            get{return _user_name;}
            set{_user_name = value;}
        }
        public string pass_word {
            get { return _pass_word; }
            set { _pass_word = value; }
        }
        public bool locked {
            get { return _is_locked; }
            set { _is_locked = value; }
        }
        public DateTime last_login_time {
            get { return _last_login_time; }
            set { _last_login_time = value; }
        }
    }
}
