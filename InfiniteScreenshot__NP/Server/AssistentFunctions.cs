using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server
{
    public class AssistentFunctions
    {
        public static byte[] ImageToByteArray(Bitmap bitmap)
        {
            using (Bitmap bmp = bitmap)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    return ms.ToArray();
                }
            }
        }

        public static Bitmap CaptureScreen(int width, int height)
        {
            // Create a bitmap to hold the screenshot
            Bitmap screenshot = new Bitmap(width, height);

            // Create a graphics object from the bitmap
            using (Graphics graphics = Graphics.FromImage(screenshot))
            {
                // Capture the screen into the bitmap
                graphics.CopyFromScreen(0, 0, 0, 0, screenshot.Size);
            }

            return screenshot;
        }
    }
}
