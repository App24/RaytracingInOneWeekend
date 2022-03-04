using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Textures
{
    internal class NoiseTexture : Texture
    {
        Perlin perlin=new Perlin();

        public NoiseTexture(float scale)
        {
            Scale = scale;
        }

        public float Scale { get; }

        public override Vector3 Value(float u, float v, Vector3 p)
        {
            return Vector3.One * 0.5f * (1f + MathF.Sin(Scale * p.Z + 10f * perlin.Turb(p)));
        }
    }
}
