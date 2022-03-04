using RIOW.HitObjects;
using RIOW.Textures;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Materials
{
    internal class Isotropic : Material
    {
        public Isotropic(Vector3 color):this(new SolidColor(color))
        {

        }

        public Isotropic(Texture a)
        {
            Albedo = a;
        }

        public Texture Albedo { get; }

        public override bool Scatter(Ray ray, HitRecord record, out Vector3 attenuation, out Ray scattered, Utils utils)
        {
            scattered = new Ray(record.p, utils.RandomInUnitSphere(), ray.Time);
            attenuation = Albedo.Value(record.u, record.v, record.p);
            return true;
        }
    }
}
