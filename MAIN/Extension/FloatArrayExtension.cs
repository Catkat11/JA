using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MAIN.Extension
{
    static class FloatArrayExtension
    {
        public static BitmapSource ConvertBGRArrayToImage(
                this float[] pixels, int width, int height, PixelFormat pixelFormat)
        {
            byte[] byteArray = pixels.ConvertToByteArray();
            return byteArray.ConvertBGRArrayToImage(width, height, pixelFormat);
        }

        public static byte[] ConvertToByteArray(this float[] floatArray)
        {
            var newByteArray = new byte[floatArray.Length];
            for (int i = 0; i < newByteArray.Length; i++)
            {
                newByteArray[i] = (byte)floatArray[i];
            }
            return newByteArray;
        }
    }
}
