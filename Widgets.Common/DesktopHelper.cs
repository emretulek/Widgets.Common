using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows;

namespace Widgets.Common
{
    public static class DeskTopHelper
    {
        private static readonly string FormWindowCaption = "WidgetWindowContainer";
        private static IntPtr _formHwnd;
        private static TaskCompletionSource<bool>? WorkerwContainerLoaded;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        [STAThread]
        private static async Task<IntPtr> CreateWorkerwContainer()
        {
            if (WorkerwContainerLoaded != null)
            {
                await WorkerwContainerLoaded.Task;
                return _formHwnd;
            }
 
            WorkerwContainerLoaded = new();

            Thread formThread = new(() =>
            {
                Form form = new()
                {
                    Text = FormWindowCaption,
                    Width = (int)SystemParameters.VirtualScreenWidth,
                    Height = (int)SystemParameters.VirtualScreenHeight,
                    AllowTransparency = false,
                    BackColor = Color.Black,
                    TransparencyKey = Color.Black,
                    StartPosition = FormStartPosition.Manual,
                    Location = new System.Drawing.Point(0, 0),
                    FormBorderStyle = FormBorderStyle.None
                };

                form.Load += (s, e) =>
                {
                    _formHwnd = form.Handle;
                    SetWindowStyles(_formHwnd);
                    SetWindowExStyles(_formHwnd);
                    //wiat for form window
                    WorkerwContainerLoaded.SetResult(true);
                };

                form.Show();

                System.Windows.Forms.Application.Run(form);
            });

            formThread.SetApartmentState(ApartmentState.STA);
            formThread.IsBackground = true;
            formThread.Start();

            await WorkerwContainerLoaded.Task;

            return _formHwnd;
        }

        /// <summary>
        /// Workerw Widgets Container
        /// </summary>
        /// <returns></returns>
        public static async Task<IntPtr> GetWorkerwContainer()
        {
            var workerw = GetWorkerW();

            if (workerw == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            try
            {
                await _semaphore.WaitAsync();

                _formHwnd = FindWindowEx(workerw, IntPtr.Zero, null, FormWindowCaption);

                if (_formHwnd == IntPtr.Zero)
                {
                    _formHwnd = await CreateWorkerwContainer();
                }

            }catch(Exception e) {
                Logger.Error(e);
            }finally
            {
                _semaphore.Release();
            }

            var bounds = new Rectangle(0, 0, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight);
            DeskTopHelper.SetParentExtended(_formHwnd, workerw, bounds);
            //DeskTopHelper.Refresh();

            if (Marshal.GetLastWin32Error() > 0)
            {
                Logger.Info("WidgetWindowContainer desktop integration error: " + Marshal.GetLastWin32Error());
            }

            return _formHwnd;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void CreateWorkerW()
        {
            var _progman = FindWindow("Progman", null);

            if (_progman == IntPtr.Zero)
            {
                return;
            }
     
            SendMessageTimeout(_progman, 0x052C, new IntPtr(0xD), new IntPtr(0x1), 0x0000, 1000, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IntPtr GetWorkerW()
        {
            IntPtr Workerw = IntPtr.Zero;
            CreateWorkerW();

            //win11
            var progman = FindWindow("Progman", null);
            if (progman != IntPtr.Zero)
            {
                Workerw = FindWindowEx(progman, IntPtr.Zero, "WorkerW", null);
            }

            if (Workerw == IntPtr.Zero)
            {
                //win10
                var enumWindowResult = EnumWindows(((hwnd, handle) =>
                {
                    var shelldll_defview = FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);

                    if (shelldll_defview != IntPtr.Zero)
                    {
                        Workerw = FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);

                        if (Workerw != IntPtr.Zero)
                            return false;
                    }

                    return true;
                }), 0);
            }

            return Workerw;
        }

        /// <summary>
        /// Set parent of window 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static bool SetParentExtended(IntPtr child, IntPtr parent, Rectangle? bounds)
        {
            if (child == IntPtr.Zero || parent == IntPtr.Zero || bounds == null)
                return false;

            SetWindowExStyles(child);
            SetWindowStyles(child);
            _ = SetWindowPos(child, IntPtr.Zero, -10000, 0, 0, 0, SWP_NOACTIVATE);

            var res = IntPtr.Zero;
            int attempts = 0;
            const int maxAttempts = 50;

            while (res == IntPtr.Zero && attempts < maxAttempts)
            {
                res = SetParent(child, parent);
                if (res == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                }
                attempts++;
            }

            Span<System.Drawing.Point> points = new System.Drawing.Point[2];
            points[0] = new System.Drawing.Point(bounds.Value.X, bounds.Value.Y);
            points[1] = new System.Drawing.Point(bounds.Value.Right, bounds.Value.Bottom);
            _ = MapWindowPoints(IntPtr.Zero, parent, ref points[0], 2);

            var tmpBounds = new Rectangle(points[0].X, points[0].Y, points[1].X - points[0].X, points[1].Y - points[0].Y);

            //After setting the parent on Windows 10, resizing fails if the window size has not been changed. I couldn't understand the reason.
            _ = SetWindowPos(child, IntPtr.Zero, tmpBounds.X, tmpBounds.Y, tmpBounds.Width - 1, tmpBounds.Height - 1, SWP_NOACTIVATE);
            _ = SetWindowPos(child, IntPtr.Zero, tmpBounds.X, tmpBounds.Y, tmpBounds.Width, tmpBounds.Height, SWP_NOACTIVATE);
            return true;
        }

        /// <summary>
        /// Refresh Desktop
        /// </summary>
        public static void Refresh()
        {
            _ = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, null, SPIF_UPDATEINIFILE);
        }

        private static void SetWindowStyles(IntPtr hwnd)
        {
            var style = GetWindowLongPtr(hwnd, GWL_STYLE);

            style &= ~(int)WS_CAPTION;
            style &= ~(int)WS_THICKFRAME;
            style &= ~(int)WS_MINIMIZEBOX;
            style &= ~(int)WS_MAXIMIZEBOX;
            style &= ~(int)WS_SYSMENU;
            SetWindowLongPtr(hwnd, GWL_STYLE, style);
        }

        private static void SetWindowExStyles(IntPtr hwnd)
        {
            var tmp = (GetWindowLongPtr(hwnd, GWL_EXSTYLE)
                | (int)WS_EX_TOOLWINDOW)
                & ~(int)WS_EX_APPWINDOW
                & ~(int)WS_EX_COMPOSITED;
            SetWindowLongPtr(hwnd, GWL_EXSTYLE, tmp);
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        private const int GWL_OWNER = -8;
        private const uint GW_OWNER = 4;

        private const int WS_CLIPCHILDREN = 0x02000000;
        private const int WS_CLIPSIBLINGS = 0x04000000;
        private const int WS_SYSMENU = 0x00080000;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;
        private const uint WS_OVERLAPPED = 0x00000000;
        private const uint WS_CHILD = 0x40000000;
        private const uint WS_VISIBLE = 0x10000000;
        private const uint WS_MINIMIZEBOX = 0x20000;
        private const uint WS_MAXIMIZEBOX = 0x10000;

        private const int WS_EX_COMPOSITED = 0x02000000;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_APPWINDOW = 0x00040000;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_TOOLWINDOW = 0x00000080;

        private const uint SPI_SETDESKWALLPAPER = 20;
        private const uint SPIF_UPDATEINIFILE = 0x01;

        private const uint RDW_INVALIDATE = 0x0001;
        private const uint RDW_UPDATENOW = 0x0100;
        private const uint RDW_ALLCHILDREN = 0x0080;

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref System.Drawing.Point lpPoints, int cPoints);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string? className, string? winName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlage, uint timeout, IntPtr result);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String? pvParam, UInt32 fWinIni);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetClassLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateSolidBrush(IntPtr crColor);

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
