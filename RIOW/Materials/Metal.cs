using RIOW.HitObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Materials
{
    internal class Metal : Material
    {
        public Metal(Vector3 color, float fuzz)
        {
            Color = color;
            Fuzz = fuzz < 1 ? fuzz : 1;
        }

        public Vector3 Color { get; }
        public float Fuzz { get; }

        public override bool Scatter(Ray ray, HitRecord record, out Vector3 attenuation, out Ray scattered, Utils utils)
        {
            Vector3 reflected = Vector3.Reflect(Vector3.Normalize(ray.Direction), record.normal);
            scattered = new Ray(record.p, reflected+Fuzz*utils.RandomInUnitSphere(), ray.Time);
            attenuation = Color;
            return Vector3.Dot(scattered.Direction, record.normal) > 0;
        }
    }
}
