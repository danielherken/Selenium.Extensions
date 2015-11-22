using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium.Extensions
{
    public class WindowHelper
    {
        public static IEnumerable<IntPtr> GetDialogHandles(string title)
        {
            var handles = new List<IntPtr>();

            foreach (IntPtr hWnd in GetDescendantWindows(IntPtr.Zero))
            {
                if (string.IsNullOrEmpty(title))
                {
                    if (GetWindowClass(hWnd) == "#32770")
                    {
                        handles.Add(hWnd);
                    }
                }
                else
                {
                    string actualTitle = GetWindowTitle(hWnd);
                    if (GetWindowClass(hWnd) == "#32770" && actualTitle == title)
                    {
                        handles.Add(hWnd);
                    }
                }
            }

            return handles.ToList();
        }

        public static List<IntPtr> GetDescendantWindows(IntPtr parent)
        {
            var result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumWindowProc childProc = EnumWindow;
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }

            return result;
        }

        public static string GetWindowClass(IntPtr hWnd)
        {
            int nRet;
            var className = new StringBuilder(100);
            nRet = GetClassName(hWnd, className, className.Capacity);
            if (nRet != 0)
            {
                return className.ToString();
            }

            return string.Empty;
        }

        public static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            var list = gch.Target as List<IntPtr>;
            if (list == null)
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");

            list.Add(handle);
            return true;
        }

        public static void SetWindowTitle(IntPtr handle, string title)
        {
            SetWindowText(handle, title);
        }

        public static void SendKey(IntPtr handle, int key)
        {
            PostMessage(handle, 0x100, (IntPtr)key, IntPtr.Zero);
            PostMessage(handle, 0x101, (IntPtr)key, IntPtr.Zero);
        }

        /// <summary>
        ///     Sets the active window to the handle passed.
        /// </summary>
        /// <param name="handle">
        ///     The handle.
        /// </param>
        /// <returns>
        ///     True if the operation succeeded, False if it did not.
        /// </returns>
        public static bool SetActiveWindow(IntPtr handle)
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            IntPtr dummy = IntPtr.Zero;

            uint foregroundThreadId = GetWindowThreadProcessId(foregroundWindow, dummy);
            uint thisThreadId = GetWindowThreadProcessId(handle, dummy);

            if (foregroundThreadId != thisThreadId)
            {
                if (AttachThreadInput(thisThreadId, foregroundThreadId, true))
                {
                    BringWindowToTop(handle);
                    SetForegroundWindow(handle);
                    AttachThreadInput(thisThreadId, foregroundThreadId, false);
                }
            }
            else
            {
                //IntPtr timeout = IntPtr.Zero;
                //SystemParametersInfo(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, timeout, 0);
                //SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, dummy, SPIF_SENDCHANGE);
                BringWindowToTop(handle);
                SetForegroundWindow(handle);
                //SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, timeout, SPIF_SENDCHANGE);
            }

            return true;

            //ShowWindow(handle, (int)SW_RESTORE);
            //return SetForegroundWindow(handle);
        }

        public static IntPtr GetDesktop()
        {
            return GetDesktopWindow();
        }

        private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();
    }


}
