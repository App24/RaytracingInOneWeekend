using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Textures
{
    internal class CheckerTexture : Texture
    {
        public CheckerTexture(Vector3 color1, Vector3 color2):this(new SolidColor(color1), new SolidColor(color2))
        {

        }

        public CheckerTexture(Texture even, Texture odd)
        {
            Even = even;
            Odd = odd;
        }

        public Texture Even { get; }
        public Texture Odd { get; }

        public override Vector3 Value(float u, float v, Vector3 p)
        {
            var sines = MathF.Sin(10f * p.X) * MathF.Sin(10f * p.Y) * MathF.Sin(10f * p.Z);
            if(sines < 0)
                return Odd.Value(u, v, p);
            return Even.Value(u, v, p);
        }
    }
}
