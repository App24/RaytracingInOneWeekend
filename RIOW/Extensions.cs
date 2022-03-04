using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace RIOW
{
    internal static class Extensions
    {
        public static Color ToColor(this Vector3 vec)
        {
            int ir = (int)(255 * vec.X);
            int ig = (int)(255 * vec.Y);
            int ib = (int)(255 * vec.Z);

            return Color.FromArgb(Math.Clamp(ir, 0, 255), Math.Clamp(ig, 0, 255), Math.Clamp(ib, 0, 255));
        }

        public static float GetAxis(this Vector3 vec, int axis)
        {
            if (axis == 0)
                return vec.X;
            else if (axis == 1)
                return vec.Y;
            else if (axis == 2)
                return vec.Z;
            return 0;
        }

        public static void SetAxis(this Vector3 vec, int axis, float value)
        {
            if (axis == 0)
                vec.X = value;
            else if (axis == 1)
                vec.Y = value;
            else if (axis == 2)
                vec.Z = value;
        }
    }
}
