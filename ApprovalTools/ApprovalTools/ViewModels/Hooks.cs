using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ApprovalTools.Approve.ViewModels
{
    public sealed class Hooks : IDisposable
    {
        private const uint OutOfContext = 0x0000;

        private WinEventDelegate _dEvent;

        private IntPtr _pHook;

        public Action<IntPtr> OnWindowDestroy;

        public Hooks(IntPtr idProcess)
        {
            _dEvent = WinEvent;

            _pHook = SetWinEventHook(
                (uint)SystemEvents.SystemDestroy,
                (uint)SystemEvents.SystemDestroy,
                IntPtr.Zero,
                _dEvent,
                IntPtr.Zero, 
                0,
                OutOfContext);

            if (IntPtr.Zero.Equals(_pHook)) throw new Win32Exception();
        }

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(
            uint eventMin, uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            IntPtr idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);


        private void WinEvent(IntPtr hWinEventHook,
            uint eventType, IntPtr hWnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            switch (eventType)
            {
                case (uint)SystemEvents.SystemDestroy:
                    if (OnWindowDestroy != null) OnWindowDestroy(hWnd);
                    break;
            }
        }

        public void Dispose()
        {
            if (!IntPtr.Zero.Equals(_pHook)) 
                UnhookWinEvent(_pHook);
            _pHook = IntPtr.Zero;
        }

        private enum SystemEvents : uint
        {
            SystemDestroy = 0x8001,
            //EVENT_SYSTEM_MINIMIZESTART = 0x0016,
            //EVENT_SYSTEM_MINIMIZEEND = 0x0017,
            //EVENT_SYSTEM_FOREGROUND = 0x0003,
            Min = 0x00000001,
            Max = 0x7FFFFFFF,
        }

        private delegate void WinEventDelegate(
            IntPtr hWinEventHook, uint eventType, IntPtr hWnd,
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
    }
}