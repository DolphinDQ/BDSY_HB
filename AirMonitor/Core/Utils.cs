using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace AirMonitor.Core
{
    public static class Utils
    {
        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        /// <summary>
        /// 截取一个win32窗口。
        /// </summary>
        /// <param name="win32View">Win32窗口</param>
        /// <returns></returns>
        public static ImageSource SnapShot(HwndHost win32View)
        {
            // 获取宽高
            int screenWidth = (int)win32View.ActualWidth;
            int screenHeight = (int)win32View.ActualHeight;

            var myIntptr = win32View.Handle;
            var hwndInt = myIntptr.ToInt32();
            var hwnd = myIntptr;
            //创建图形
            var bm = new Bitmap(screenWidth, screenHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            var g = Graphics.FromImage(bm);
            var hdc = g.GetHdc();
            //调用api 把hwnd的内容用图形绘制到hdc 如果你有代码洁癖 可以不使用api 使用g.CopyFromScreen，请自行研究
            bool result = PrintWindow(hwnd, hdc, 0);
            g.ReleaseHdc(hdc);
            g.Flush();
            if (result == true) //成功 转换并返回ImageSource
            {
                var imageSourceConverter = new ImageSourceConverter();
                var stream = new MemoryStream();
                bm.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return (ImageSource)imageSourceConverter.ConvertFrom(stream);
            }
            return null;
        }
    }
}
