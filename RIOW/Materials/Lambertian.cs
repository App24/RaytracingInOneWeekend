using RIOW.HitObjects;
using RIOW.Textures;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Materials
{
    internal class Lambertian : Material
    {
        public Lambertian(Vector3 color):this(new SolidColor(color))
        {
        }

        public Lambertian(Texture texture)
        {
            Texture = texture;
        }

        public Texture Texture { get; }

        public override bool Scatter(Ray ray, HitRecord record, out Vector3 attenuation, out Ray scattered, Utils utils)
        {
            var scatterDirection = record.normal + utils.RandomUnitVector();
            scattered = new Ray(record.p, scatterDirection, ray.Time);
            attenuation = Texture.Value(record.u, record.v, record.p);
            return true;
        }
    }
}
