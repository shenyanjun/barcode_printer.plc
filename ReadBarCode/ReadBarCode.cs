using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;

namespace barcode_printer {
    public class ReadBarCode {
        //安装钩子原型  
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowsHookEx(
                int hookid,
                HookPro pfnhook,
                IntPtr hinst,
                int threadid
            );
        //卸载钩子原型  
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool UnhookWindowsHookEx
            (
               IntPtr hhook
            );
        //回调钩子原型  
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool CallNextHookEx
            (
                IntPtr hhook,
                int code,
                IntPtr wparam,
                IntPtr lparam
            );
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(
                ref KBDLLHOOKSTRUCT Source,
                IntPtr Destination, int Length
            );
        //键盘Hook结构函数  
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT {
            public int vkCode;//message
            public int scanCode;//paramL
            public int flags;//paramH
            public int time;//Time
            public int dwExtraInfo;//hwnd
        }
        [DllImport("user32", EntryPoint = "GetKeyNameText")]
        private static extern int GetKeyNameText(int lParam, StringBuilder lpBuffer, int nSize);

        [DllImport("user32", EntryPoint = "GetKeyboardState")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32", EntryPoint = "ToAscii")]
        private static extern bool ToAscii(int VirtualKey, int ScanCode, byte[] lpKeyState, ref uint lpChar, int uFlags);
        private delegate bool HookPro(int nCode, IntPtr wParam, IntPtr lParam); //创建委托，进行回调  
        private static IntPtr hHook = IntPtr.Zero; //创建钩子编号
        const int WH_KEYBOARD_LL = 13; //创建键盘钩子类型  
        GCHandle _hookProcHandle;
        private DateTime prev_keyboard_time = DateTime.Now;//先前的时间
        private delegate void keypresscode(int keycode, int modifierkeys);//创建按健委托
        public delegate void BarCodeDelegate(string barCode);
        public BarCodeDelegate BarCodeEvent;
        //private ArrayList alNum = new ArrayList();
        private string scan_bar_code = "";
        public bool Start() {
            return (SET_WINDOWS_KEYBOARD_HOOK());
        }
        public void Stop() {
            UNLOAD_WINDOWS_KETBOARD_HOOK();
        }
        //private string KeyName = "";
        private uint AscII = 0;
        private char Chr;
        private uint uKey = 0;
        private bool KEYBOARD_HOOKPRO(int nCode, IntPtr wParam, IntPtr lParam){
            AscII = 0;
            Chr = Char.MaxValue;
            KBDLLHOOKSTRUCT kb = new KBDLLHOOKSTRUCT();
            CopyMemory(ref kb, lParam, 20);
            if ((int)wParam == 0x100) {
                byte[] kbArray = new byte[256];
                GetKeyboardState(kbArray);
                if (ToAscii(kb.vkCode, kb.scanCode, kbArray, ref uKey, 0)) {
                    AscII = uKey;
                    Chr = Convert.ToChar(uKey);
                    if (Chr != '\r') {
                        scan_bar_code += Chr.ToString();
                    }
                }
                if (kb.vkCode == 13) {
                    if (BarCodeEvent != null) {
                        //Console.Write(scan_bar_code);
                        BarCodeEvent(scan_bar_code);
                    }
                    scan_bar_code = "";
                } 
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }
        /// <summary>  
        /// 设置钩子....调用此方法即可装载钩子  
        /// </summary>  
        private bool SET_WINDOWS_KEYBOARD_HOOK() {
            if (hHook == IntPtr.Zero) {
                HookPro hk = new HookPro(this.KEYBOARD_HOOKPRO);
                _hookProcHandle = GCHandle.Alloc(hk);
                hHook = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    hk,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                if (hHook == IntPtr.Zero) {
                    return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>  
        /// 卸载钩子  
        /// </summary>  
        private void UNLOAD_WINDOWS_KETBOARD_HOOK() {
            if (hHook != IntPtr.Zero) {
                //如果钩子已经挂上则取消钩子，否则不用取消  
                UnhookWindowsHookEx(hHook);
                _hookProcHandle.Free();
                hHook = IntPtr.Zero;
            }
        }
    }
}
