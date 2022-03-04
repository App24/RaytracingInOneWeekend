using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Textures
{
    internal class SolidColor : Texture
    {
        public SolidColor(Vector3 color)
        {
            Color = color;
        }

        public SolidColor(float red, float green, float blue) : this(new Vector3(red, green, blue))
        {

        }

        public Vector3 Color { get; }

        public override Vector3 Value(float u, float v, Vector3 p)
        {
            return Color;
        }
    }
}
