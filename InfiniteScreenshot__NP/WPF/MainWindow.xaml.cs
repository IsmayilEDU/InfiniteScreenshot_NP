
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Drawing;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var client = new UdpClient();

            var connectEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27001);
            string ScreenWidth = SystemParameters.VirtualScreenWidth.ToString();
            string ScreenHeight = SystemParameters.VirtualScreenHeight.ToString();

            var str = ScreenWidth + ':' + ScreenHeight;
            var bytes = Encoding.Default.GetBytes(str);
            client.Send(bytes, bytes.Length, connectEP);

            Task.Run(async () =>
            {
                while (true)
                {
                    var result = await client.ReceiveAsync();
                    var ReceivedImageBytes = result.Buffer;
                    var CurrentImageBytes = new byte[ReceivedImageBytes.Length];
                    var CurrentImageBytesCount = 0;
                    var bufferSize = ushort.MaxValue - 28;
                    while (ReceivedImageBytes.Length > CurrentImageBytesCount)
                    {
                        if (ReceivedImageBytes.Length - CurrentImageBytesCount < bufferSize)
                        {
                            bufferSize = ReceivedImageBytes.Length - CurrentImageBytesCount;
                        }
                        Array.Copy(ReceivedImageBytes, CurrentImageBytesCount, CurrentImageBytes, CurrentImageBytesCount, bufferSize);
                        CurrentImageBytesCount += bufferSize;
                    }

                    using (MemoryStream ms = new MemoryStream(CurrentImageBytes))
                    {
                        Bitmap image = new Bitmap(ms);
                        image_Screenshot.Dispatcher.Invoke(() => image_Screenshot.Source = ToBitmapImage(image));
                    }
                }
            });
        }

        private BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

    }
}
