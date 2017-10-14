using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Management;
using System.Timers;
using System.ComponentModel;

namespace barcode_printer {
    public class scanner {
        public static SerialPort _port;
        private byte[] _recv_buf = new byte[100];
        private int _recv_idx = 0;
        private string port_name = "";
        public delegate void BarCodeDelegate(string barCode);
        public BarCodeDelegate BarCodeEvent;

        //���������Ķ�ʱ��
        private System.Timers.Timer recv_timer;
        //recv handle counts
        private int recv_counts = 0;
        //timeout flags
        private bool recv_timeoutflag = false;
        //find 0x0D flag
        private bool find_0x0D_flag = false;
        //��������
        private void recv_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //1000ms��
            if (recv_counts++ > 50)
            {
                recv_timer.Enabled = false;
                recv_counts = 0;
                recv_timeoutflag = true;//��ʱ��
                if (!find_0x0D_flag)
                {
                    if (!dataConfig.diaglog_warn_flag)
                    {
                        dataConfig.diaglog_warn_flag = true;
                        mainFrm.getMainForm().setWarnningDlg();
                    }
                }
            }
        }

        public scanner(string p) {
            port_name = p;
            recv_timer = new System.Timers.Timer(10);
            recv_timer.Elapsed += new System.Timers.ElapsedEventHandler(recv_timer_Elapsed);
        }

        public bool openScanner() {
            /*
            if (_port != null && _port.IsOpen) {
                _port.Close();
                _port.Dispose();
            }
             * */
            if (_port != null) {
                _port.Close();
            }
            if (_port == null) {
                _port = new SerialPort(port_name);
                _port.BaudRate = 9600;
                _port.Parity = Parity.None;
                _port.ReceivedBytesThreshold = 1;
                _port.StopBits = StopBits.One;
                _port.DataBits = 8;
                _port.DataReceived += _dataReceive;
            } else {
                _port.PortName = port_name;
            }
            try {
                _port.Open();
            } catch(Exception ex) {
                return false;
            }
            return true;
        }

        private void _dataReceive(object sender, SerialDataReceivedEventArgs e) {
            _port.ReceivedBytesThreshold = 5000;
            if (!recv_timer.Enabled)
            {
                recv_timer.Enabled = true;//������ʱ��
                recv_timeoutflag = false;//��λ���ܳ�ʱ��־
                recv_counts = 0;//���ܶ�ʱ��
                find_0x0D_flag = false;
            }
            byte d = (byte)_port.ReadByte();
            recv_timeoutflag = false;//�յ�����������λ���ܳ�ʱ��־
            recv_counts = 0;//��λ��ʱ��
            if (d == 0x0D) {
                if (_recv_idx > 0) {
                    string sn = Encoding.ASCII.GetString(_recv_buf, 0, _recv_idx);
                    _recv_idx = 0;
                    if (BarCodeEvent != null) {
                        BarCodeEvent(sn);
                        find_0x0D_flag = true;
                        //��ֹ���ݹ���
                        System.Threading.Thread.Sleep(50);
                    }
                }
            } else {
                //�޸���ɨ��ǹû�����ûس�������
                if (_recv_idx >= 100)
                {
                    _recv_idx = 0;
                    //System.Windows.Forms.MessageBox.Show("ɨ�����ݳ���100���ַ�,����ɨ��ǹ�Ƿ�������ȷ!");
                }
                _recv_buf[_recv_idx++] = d;
                if (_recv_idx > 100) {
                    _recv_idx = 0;
                }
            }
            _port.ReceivedBytesThreshold = 1;
        }
        //s = 0 : box handle
        //s = 1 : product handle
        public void closeScanner() {
            if (_port != null && _port.IsOpen) {
                _port.Close();
            }
        }
    }
}
