using RIOW.HitObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Materials
{
    internal class Dielectric : Material
    {
        public Dielectric(float refractionIndex)
        {
            RefractionIndex = refractionIndex;
        }

        public float RefractionIndex { get; }

        public override bool Scatter(Ray ray, HitRecord record, out Vector3 attenuation, out Ray scattered, Utils utils)
        {
            attenuation = Vector3.One;
            float refractionRatio = record.frontFace ? (1f / RefractionIndex) : RefractionIndex;

            Vector3 unitDirection = Vector3.Normalize(ray.Direction);

            float cosTheta = MathF.Min(Vector3.Dot(-unitDirection, record.normal), 1);
            float sinTheta = MathF.Sqrt(1f - cosTheta * cosTheta);

            bool cannotRefract = (refractionRatio * sinTheta) > 1f;
            Vector3 direction;

            if (cannotRefract || Utils.Reflectance(cosTheta, refractionRatio) > utils.RandomFloat())
                direction = Vector3.Reflect(unitDirection, record.normal);
            else
                direction = Utils.Refract(unitDirection, record.normal, refractionRatio);

            scattered = new Ray(record.p, direction, ray.Time);
            return true;
        }
    }
}
