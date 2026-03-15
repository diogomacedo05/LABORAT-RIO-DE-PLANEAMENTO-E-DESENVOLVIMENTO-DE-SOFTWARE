using System;
using System.Windows;
using System.Windows.Interop;

namespace Alunos
{
    public class BaseWindow : Window
    {
        private const int WM_NCHITTEST = 0x0084;
        private const int HTCLIENT = 1;

        public BaseWindow()
        {
            SourceInitialized += BaseWindow_SourceInitialized;
            ResizeMode = ResizeMode.CanMinimize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void BaseWindow_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hwnd).AddHook(WindowProc);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCHITTEST)
            {
                handled = true;
                return new IntPtr(HTCLIENT);
            }

            return IntPtr.Zero;
        }
    }
}
