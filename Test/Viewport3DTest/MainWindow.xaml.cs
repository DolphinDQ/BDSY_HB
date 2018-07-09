using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Viewport3DTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        /// <summary>
        /// 对一个WebBrowser进行截图
        /// </summary>
        /// <param name="targetBrowser">我这里用的是Forms的WebBrowser，如果是wpf的，请自己改成Controls并调整参数</param>
        /// <returns></returns>
        public static ImageSource BrowserSnapShot(WebBrowser targetBrowser)
        {
            // 获取宽高
            int screenWidth = (int)targetBrowser.Width;
            int screenHeight = (int)targetBrowser.Height;

            IntPtr myIntptr = targetBrowser.Handle;
            int hwndInt = myIntptr.ToInt32();
            IntPtr hwnd = myIntptr;
            //创建图形
            Bitmap bm = new Bitmap(screenWidth, screenHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            Graphics g = Graphics.FromImage(bm);
            IntPtr hdc = g.GetHdc();

            //调用api 把hwnd的内容用图形绘制到hdc 如果你有代码洁癖 可以不使用api 使用g.CopyFromScreen，请自行研究
            bool result = PrintWindow(hwnd, hdc, 0);
            g.ReleaseHdc(hdc);
            g.Flush();


            if (result == true) //成功 转换并返回ImageSource
            {
                ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                MemoryStream stream = new MemoryStream();
                bm.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return (ImageSource)imageSourceConverter.ConvertFrom(stream);
            }
            return null;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Img2.Source = BrowserSnapShot(Broswer);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Broswer.Visibility = Visibility.Hidden;
        }
    }
}
