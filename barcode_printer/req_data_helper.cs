using System;
using System.Collections.Generic;
using System.Text;

namespace barcode_printer {
    class req_data_helper {
        //获取到打印回复
        public static byte[] make_print_res(byte scanner_state) {
            proto.proto p = new proto.proto();
            p.addr = 0xFF;
            p.cmd = 0xA0;
            p.len = 1;
            p.data = new byte[1] { scanner_state };
            return p.export_proto_data();
        }
        //获取扫描结果数据
        public static byte[] make_scanner_res(byte scanner_result) {
            proto.proto p = new proto.proto();
            p.addr = 0xFF;
            p.cmd = 0xA1;
            p.len = 1;
            p.data = new byte[1] { scanner_result };
            return p.export_proto_data();
        }
        //获取复位结果数据
        public static byte[] make_reset_res() {
            proto.proto p = new proto.proto();
            p.addr = 0xFF;
            p.cmd = 0xA2;
            p.len = 1;
            p.data = new byte[1] { 0x1 };
            return p.export_proto_data();
        }
        //获取ready数据
        public static byte[] make_ready_res(byte ready_status) {
            proto.proto p = new proto.proto();
            p.addr = 0xFF;
            p.cmd = 0xA3;
            p.len = 1;
            p.data = new byte[1] { ready_status };
            return p.export_proto_data();
        }
    }
}
