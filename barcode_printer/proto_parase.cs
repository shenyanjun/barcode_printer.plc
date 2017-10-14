using System;
using System.Collections.Generic;
using System.Text;

namespace proto_parase
{
    /*
     public class packet_msg
     {
         public byte addr;
         public byte cmd;
         public byte len;
         public byte[] data;
         public DateTime time;
     }
     * */
    public class proto_parase
    {
        public const int packet_recv_data_len = 100;
        public delegate void procedure_packet(int msg, string param);
        public event procedure_packet on_packet_data;
        //private packet_msg _msg = new packet_msg();
        private void clear()
        {
            Array.Clear(recv_data, 0, packet_recv_data_len);
            recv_count = 0;
            pre_recv_data = 0;
        }
        //fix when plc send to me code less than 20 chars, and left buffer will be filled '#', so want be filter '#' code!  
        private string convert_error_code(string p)
        {
            if (p.Length < 20)
            {
                int i = p.Length - 1;
                for (; p[i] == '*'; i--) ;
                return p.Substring(0, i + 1);
            }
            return p;
        }

        private bool start_flag = false;
        private byte[] recv_data = new byte[packet_recv_data_len];
        private int recv_count = 0;
        private byte pre_recv_data = 0x0;
        public void push_byte(byte data)
        {
            recv_data[recv_count++] = data;
            if (recv_count >= packet_recv_data_len)
            {
                clear();
            }
            if (data == 0x0A)
            {
                if (pre_recv_data == 0x0D)
                {
                    if (recv_count > 4)
                    {
                        string cmd = Encoding.ASCII.GetString(recv_data, 0, 4).ToUpper().Trim();
                        int c = -1;
                        string p = "";
                        switch (cmd)
                        {
                            case "[TR]":
                                c = 0;
                                p = "";
                                break;
                            case "[SC]":
                                c = 1;
                                p = Encoding.ASCII.GetString(recv_data, 4, recv_count - 2 - 4).ToUpper().Replace("\0", "").Trim();
                                break;
                            case "[OP]":
                                c = 2;
                                p = Encoding.ASCII.GetString(recv_data, 4, recv_count - 2 - 4).ToUpper().Replace("\0", "").Trim();
                                break;
                            default: break;
                        }
                        on_packet_data(c, p);
                    }
                    clear();
                }
            }
            pre_recv_data = data;
        }
    }
}
