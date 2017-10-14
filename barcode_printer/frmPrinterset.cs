using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
namespace barcode_printer
{
    public partial class frmPrinterset : Form
    {
         /// <summary>
        /// ö��win32 api
        /// </summary>
        public enum HardwareEnum
        {
            // Ӳ��
            Win32_Processor, // CPU ������
            Win32_PhysicalMemory, // �����ڴ���
            Win32_Keyboard, // ����
            Win32_PointingDevice, // �������豸��������ꡣ
            Win32_FloppyDrive, // ����������
            Win32_DiskDrive, // Ӳ��������
            Win32_CDROMDrive, // ����������
            Win32_BaseBoard, // ����
            Win32_BIOS, // BIOS оƬ
            Win32_ParallelPort, // ����
            Win32_SerialPort, // ����
            Win32_SerialPortConfiguration, // ��������
            Win32_SoundDevice, // ��ý�����ã�һ��ָ������
            Win32_SystemSlot, // ������ (ISA & PCI & AGP)
            Win32_USBController, // USB ������
            Win32_NetworkAdapter, // ����������
            Win32_NetworkAdapterConfiguration, // ��������������
            Win32_Printer, // ��ӡ��
            Win32_PrinterConfiguration, // ��ӡ������
            Win32_PrintJob, // ��ӡ������
            Win32_TCPIPPrinterPort, // ��ӡ���˿�
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM �˿�
            Win32_DesktopMonitor, // ��ʾ��
            Win32_DisplayConfiguration, // �Կ�
            Win32_DisplayControllerConfiguration, // �Կ�����
            Win32_VideoController, // �Կ�ϸ�ڡ�
            Win32_VideoSettings, // �Կ�֧�ֵ���ʾģʽ��
            // ����ϵͳ
            Win32_TimeZone, // ʱ��
            Win32_SystemDriver, // ��������
            Win32_DiskPartition, // ���̷���
            Win32_LogicalDisk, // �߼�����
            Win32_LogicalDiskToPartition, // �߼��������ڷ�����ʼĩλ�á�
            Win32_LogicalMemoryConfiguration, // �߼��ڴ�����
            Win32_PageFile, // ϵͳҳ�ļ���Ϣ
            Win32_PageFileSetting, // ҳ�ļ�����
            Win32_BootConfiguration, // ϵͳ��������
            Win32_ComputerSystem, // �������Ϣ��Ҫ
            Win32_OperatingSystem, // ����ϵͳ��Ϣ
            Win32_StartupCommand, // ϵͳ�Զ���������
            Win32_Service, // ϵͳ��װ�ķ���
            Win32_Group, // ϵͳ������
            Win32_GroupUser, // ϵͳ���ʺ�
            Win32_UserAccount, // �û��ʺ�
            Win32_Process, // ϵͳ����
            Win32_Thread, // ϵͳ�߳�
            Win32_Share, // ����
            Win32_NetworkClient, // �Ѱ�װ������ͻ���
            Win32_NetworkProtocol, // �Ѱ�װ������Э��
            Win32_PnPEntity,//all device
        }

        private void enum_usb_port() {
            try{
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI","SELECT * FROM MSSerial_PortName");
                foreach (ManagementObject queryObj in searcher.Get()){
                        /*
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("MSSerial_PortName instance");
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("InstanceName: {0}", queryObj["InstanceName"]);
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("MSSerial_PortName instance");
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("PortName: {0}", queryObj["PortName"]);
                         * */
                        //If the serial port's instance name contains USB 
                        //it must be a USB to serial device
                        if (queryObj["InstanceName"].ToString().Contains("USB"))
                        {
                                Console.WriteLine(queryObj["PortName"] + "is a USB to SERIAL adapter/converter");
                        }
                }
            }
            catch (ManagementException e)
            {
                MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
            } 
        }
        /// <summary>
        /// �豸����
        /// </summary>
        private struct dev_detail {
            public string dev_name;//�豸����
            //public string dev_discript;//�豸����
            public override string ToString() {
                //return dev_discript;
                return dev_name;
            }
        }
        private string GetSerialPortName(string detail)
        {
            /*
            int idx = detail.IndexOf("COM");
            int idx_end = detail.IndexOf(")");
            return detail.Substring(idx, idx_end - idx);
             * */
            Regex r = new Regex(@"com\d{1,2}", RegexOptions.IgnoreCase);
            Match m = r.Match(detail);
            if (m.Success) {
                return detail.Substring(m.Index, m.Length);
            }
            return "";
        }
        /// <summary>
        /// WMIȡӲ����Ϣ
        /// </summary>
        /// <param name="hardType"></param>
        /// <param name="propKey"></param>
        /// <returns></returns>
        private dev_detail[] MulGetHardwareInfo(HardwareEnum hardType) {
            List<dev_detail> dev_list = new List<dev_detail>();
            try {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType)) {
                    ManagementObjectCollection hardInfos = searcher.Get();
                    foreach (ManagementBaseObject hardInfo in hardInfos) {
                        if (hardInfo.Properties["Name"].Value.ToString().Contains("COM")) {
                            dev_detail dev = new dev_detail();
                            dev.dev_name = GetSerialPortName(hardInfo.Properties["Name"].Value.ToString());
                            //dev.dev_name = hardInfo.Properties["DeviceID"].Value.ToString();
                            //dev.dev_discript = hardInfo.Properties["Name"].Value.ToString();//2016.8.6
                            dev_list.Add(dev);
                        }
                    }
                    searcher.Dispose();
                }
                return dev_list.ToArray();
            } catch {
                return null;
            } finally { dev_list = null; }
        }
        public frmPrinterset()
        {
            InitializeComponent();
            //dataConfig.loadConfig();
        }

        private int select_com_index(string com_name) {
            string names = "COM1#COM2#COM3#COM4#COM5#COM6#COM7#COM8#COM9#COM10#COM11#COM12#COM13#COM14#COM15";
            string[] all_coms = names.Split(new char[] {'#' });
            for (int i = 0; i < all_coms.Length; i++) {
                if (com_name == all_coms[i])
                    return i;
            }
            return 0;
        }

        //
        //�Զ������¼�
        //
        private void frmPrinterset_Load(object sender, EventArgs e)
        {
            bool dev_not_avail = false;
            if (dataConfig.code_printer_dev != "" && dataConfig.code_printer_dev_detail!="")
            {
                //dev_detail dev = new dev_detail();
                //dev.dev_discript = dataConfig.code_printer_dev_detail;//2016.8.6
                //dev.dev_name = dataConfig.code_printer_dev;
                //cmbComList.Items.Add(dataConfig.code_printer_dev);
                cmbComList.SelectedIndex = select_com_index(dataConfig.code_printer_dev);
                dev_not_avail = true;
            }
            /*
            if (!dev_not_avail)
            {
                
                dev_detail[] ports = MulGetHardwareInfo(HardwareEnum.Win32_SerialPort);
                foreach (dev_detail com in ports) {
                    cmbComList.Items.Add(com);
                }
                
            }
             */
            if (dataConfig.code_printer_dev_speed != 0)
            {
                switch (dataConfig.code_printer_dev_speed)
                {
                    case 4800:
                        cmbSpeedList.SelectedIndex = 0;
                        break;
                    case 9600:
                        cmbSpeedList.SelectedIndex = 1;
                        break;
                    case 14400:
                        cmbSpeedList.SelectedIndex = 2;
                        break;
                    case 19200:
                        cmbSpeedList.SelectedIndex = 3;
                        break;
                    case 38400:
                        cmbSpeedList.SelectedIndex = 4;
                        break;
                    case 56000:
                        cmbSpeedList.SelectedIndex = 5;
                        break;
                    case 57600:
                        cmbSpeedList.SelectedIndex = 6;
                        break;
                    case 115200:
                        cmbSpeedList.SelectedIndex = 7;
                        break;
                }
            }
            else
            {
                cmbSpeedList.SelectedIndex = 0;
            }
            dev_not_avail = false;
            if (dataConfig.io_dev != "" && dataConfig.io_dev_detail!="")
            {
                //dev_detail dev = new dev_detail();
                //dev.dev_discript = dataConfig.io_dev_detail;//2016.8.6
                //dev.dev_name = dataConfig.io_dev;
                //comboBox1.Items.Add(dataConfig.io_dev);
                comboBox1.SelectedIndex = select_com_index(dataConfig.io_dev) ;
                dev_not_avail = true;
            }
            /*
            if (!dev_not_avail)
            {
                dev_detail[] ports = MulGetHardwareInfo(HardwareEnum.Win32_SerialPort);
                foreach (dev_detail com in ports) {
                    comboBox1.Items.Add(com);
                }
            }
             * */
            if (dataConfig.io_dev_speed != 0)
            {
                switch (dataConfig.io_dev_speed)
                {
                    case 4800:
                        comboBox2.SelectedIndex = 0;
                        break;
                    case 9600:
                        comboBox2.SelectedIndex = 1;
                        break;
                    case 14400:
                        comboBox2.SelectedIndex = 2;
                        break;
                    case 19200:
                        comboBox2.SelectedIndex = 3;
                        break;
                    case 38400:
                        comboBox2.SelectedIndex = 4;
                        break;
                    case 56000:
                        comboBox2.SelectedIndex = 5;
                        break;
                    case 57600:
                        comboBox2.SelectedIndex = 6;
                        break;
                    case 115200:
                        comboBox2.SelectedIndex = 7;
                        break;
                }
            }
            else
            {
                comboBox2.SelectedIndex = 0;
            }
            if (dataConfig.scan_serial_name != "") {
                //dev_detail dev = new dev_detail();
                //dev.dev_discript = dataConfig.scan_serial_name;//2016.8.6
                //dev.dev_name = dataConfig.scan_serial_name;
                //comboBox3.Items.Add(dataConfig.scan_serial_name);
                comboBox3.SelectedIndex = select_com_index(dataConfig.scan_serial_name);
            }
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (cmbComList.SelectedItem == null || comboBox1.SelectedItem == null /*|| comboBox3.SelectedItem== null*/)
                return;
            this.DialogResult = DialogResult.No;
            /*
            dev_detail dev_print = (dev_detail)cmbComList.SelectedItem;
            dev_detail dev_io = (dev_detail)comboBox1.SelectedItem;
            
            dev_detail dev_scan = (dev_detail)comboBox3.SelectedItem;
             * */
            string dev_print = cmbComList.SelectedItem.ToString();
            string dev_io = comboBox1.SelectedItem.ToString();
            //string dev_scan = comboBox3.SelectedItem.ToString();
            if ((dev_print!= "" && dev_io!="" ) &&  dev_print == dev_io
                ) {
                MessageBox.Show("�벻Ҫѡ����ͬ�Ĵ���!");
                this.DialogResult = DialogResult.No;
                return;
            }
            if (dev_print != "")
            {
                dataConfig.code_printer_dev = dev_print;
                //dataConfig.code_printer_dev_detail = dev_print.dev_discript;//2016.8.6
            }
            dataConfig.code_printer_dev_speed = Convert.ToInt32(cmbSpeedList.Text.Trim());
            
            if (dev_io != "")
            {
                dataConfig.io_dev = dev_io;
                //dataConfig.io_dev_detail = dev_io.dev_discript;//2016.8.6
            }
            
            dataConfig.io_dev_speed = Convert.ToInt32(comboBox2.Text.Trim());
            /*
            if (dev_scan != "") {
                dataConfig.scan_serial_name = dev_scan;
            }
             * */
            dataConfig.saveIO();
            DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnFresh_Click(object sender, EventArgs e)
        {
            cmbComList.Items.Clear();
            dev_detail[] ports = MulGetHardwareInfo(HardwareEnum.Win32_PnPEntity);
            foreach (dev_detail com in ports)
            {
                cmbComList.Items.Add(com);
            }
            if (cmbComList.Items.Count>0)
                cmbComList.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            dev_detail[] ports = MulGetHardwareInfo(HardwareEnum.Win32_PnPEntity);
            foreach (dev_detail com in ports)
            {
                comboBox1.Items.Add(com);
            }
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        private void frmPrinterset_FormClosing(object sender, FormClosingEventArgs e) {
            if (DialogResult == DialogResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;
        }

        private void button2_Click(object sender, EventArgs e) {
            comboBox3.Items.Clear();
            dev_detail[] ports = MulGetHardwareInfo(HardwareEnum.Win32_PnPEntity);
            foreach (dev_detail com in ports) {
                comboBox3.Items.Add(com);
            }
            if (comboBox3.Items.Count > 0)
                comboBox3.SelectedIndex = 0;
        }
    }
}