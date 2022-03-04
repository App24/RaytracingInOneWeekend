using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace RIOW.Textures
{
    internal class ImageTexture : Texture
    {
        int width;
        int height;
        int depth;

        byte[] buffer;

        public ImageTexture(string fileName)
        {
            Bitmap image = new Bitmap(Image.FromFile(fileName));
            width = image.Width;
            height = image.Height;

            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var data = image.LockBits(rect, ImageLockMode.ReadWrite, image.PixelFormat);
            depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel

            buffer = new byte[data.Width * data.Height * depth];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

        }

        public override Vector3 Value(float u, float v, Vector3 p)
        {
            if (buffer == null)
                return new Vector3(0, 1, 1);

            u = Math.Clamp(u, 0, 1);
            v = 1f - Math.Clamp(v, 0, 1);

            var i = (int)(u * width);
            var j = (int)(v * height);

            if (i >= width) i = width - 1;
            if (j >= height) j = height - 1;

            float colorScale = 1 / 255f;
            int offset = ((j * width) + i) * depth;
            var pixel = Color.FromArgb(buffer[offset], buffer[offset + 1], buffer[offset + 2]);

            return new Vector3(colorScale * pixel.B, colorScale * pixel.G, colorScale * pixel.R);
        }
    }
}
