using RIOW.HitObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Materials
{
    internal abstract class Material
    {
        public abstract bool Scatter(Ray ray, HitRecord record, out Vector3 attenuation, out Ray scattered, Utils utils);

        public virtual Vector3 Emitted(float u, float v, Vector3 p)
        {
            return Vector3.Zero;
        }
    }
}
