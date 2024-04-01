using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Utils
{
    public class ImageUtils
    {
        public static string BitmapToBase64(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // 保存为JPEG格式，也可以选择Png，Gif等等
                bitmap.Save(memoryStream, ImageFormat.Jpeg);

                // 获取内存流的字节数组
                byte[] imageBytes = memoryStream.ToArray();

                // 将字节转换为Base64字符串
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        public static List<string> BitmapListToBase64(Bitmap[] bitmaps)
        {
            List<string> base64Strings = new List<string>();

            foreach (Bitmap bitmap in bitmaps)
            {
                base64Strings.Add(BitmapToBase64(bitmap));
            }
            return base64Strings;
        }
    }
}
