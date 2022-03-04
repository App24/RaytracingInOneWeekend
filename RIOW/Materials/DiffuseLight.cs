using RIOW.HitObjects;
using RIOW.Textures;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Materials
{
    internal class DiffuseLight : Material
    {
        public Texture Texture { get; }

        public DiffuseLight(Vector3 color):this(new SolidColor(color))
        {

        }

        public DiffuseLight(Texture texture)
        {
            Texture = texture;
        }

        public override bool Scatter(Ray ray, HitRecord record, out Vector3 attenuation, out Ray scattered, Utils utils)
        {
            attenuation = Vector3.Zero;
            scattered = ray;
            return false;
        }

        public override Vector3 Emitted(float u, float v, Vector3 p)
        {
            return Texture.Value(u, v, p);
        }
    }
}
