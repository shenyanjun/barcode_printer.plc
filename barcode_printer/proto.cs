using System;
using System.Collections.Generic;
using System.Text;

namespace proto
{
    public class proto
    {
        //地址
        private byte _addr = 0;
        //长度
        private byte _len = 0;
        //命令
        private byte _cmd = 0;
        //数据
        private byte[] _tmp_data = null;
        public proto()
        {
            _addr = 0;
            _len = 0;
            _cmd = 0;
        }
        public byte addr
        {
            set { _addr = value; }
            get { return _addr; }
        }
        public byte len
        {
            set { _len = value; }
            get { return _len; }
        }
        public byte cmd
        {
            set { _cmd = value; }
            get { return _cmd; }
        }
        public byte[] data
        {
            set 
            {
                _tmp_data = value;
            }
            get { return _tmp_data; }
        }
        public proto(byte addr, byte cmd, byte[] data)
        {
            _addr = addr;
            _cmd = cmd;
            _tmp_data = data;
            _len = (byte)data.Length;
        }
        public byte[] export_proto_data()
        {
            byte cs = 0;
            byte[] tmp = new byte[8] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
            int offset = 0;
            tmp[offset++] = 0x02;
            cs ^= _addr;
            tmp[offset++] = _addr;
            cs ^= _cmd;
            tmp[offset++] = _cmd;
            cs ^= _len;
            tmp[offset++] = _len;
            for (int n = 0; n < _len; n++)
            {
                tmp[offset++] = _tmp_data[n];
                cs ^= _tmp_data[n];
            }
            if (_len < 2)
                cs ^= 0xFF;
            //加入校验
            tmp[6] = cs;
            tmp[7] = 0x03;
            return tmp;
        }
    }
}
