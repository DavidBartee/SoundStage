﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static SoundStage.InterceptKeys;

namespace SoundStage {
    public class KeyboardListener : IDisposable {
        private static IntPtr hookID = IntPtr.Zero;
        private static LowLevelKeyboardProc llkp;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            try {
                return HookCallbackInner(nCode, wParam, lParam);
            }
            catch {
                throw new Exception("An error has occurred");
            }
        }

        private IntPtr HookCallbackInner(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0) {
                if (wParam == (IntPtr)InterceptKeys.WM_KEYDOWN) {
                    int vkCode = Marshal.ReadInt32(lParam);

                    if (KeyUp != null)
                        KeyUp(this, new RawKeyEventArgs(vkCode, false));
                }
            }
            return InterceptKeys.CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        public event RawKeyEventHandler KeyDown;
        public event RawKeyEventHandler KeyUp;

        public KeyboardListener() {
            hookID = SetHook(llkp);
        }

        ~KeyboardListener() {
            Dispose();
        }

        public void Dispose() {
            InterceptKeys.UnhookWindowsHookEx(hookID);
        }
    }

    internal static class InterceptKeys {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static int WH_KEYBOARD_LL = 13;
        public static int WM_KEYDOWN = 0x0100;
        public static int WM_KEYUP = 0x0101;

        public static IntPtr SetHook(LowLevelKeyboardProc proc) {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadID);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }

    public class RawKeyEventArgs : EventArgs {
        public int VKCode;
        public Key key;
        public bool IsSysKey;

        public RawKeyEventArgs(int vkc, bool syskey) {
            VKCode = vkc;
            IsSysKey = syskey;
            key = System.Windows.Input.KeyInterop.KeyFromVirtualKey(VKCode);
        }
    }

    public delegate void RawKeyEventHandler(object sender, RawKeyEventArgs args);
}
