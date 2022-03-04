using RIOW.Materials;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    internal class YZRect : HitObject
    {
        public YZRect(float y0, float y1, float z0, float z1, float k, Material material)
        {
            Y0 = y0;
            Y1 = y1;
            Z0 = z0;
            Z1 = z1;
            K = k;
            Material = material;
        }

        public float Y0 { get; }
        public float Y1 { get; }
        public float Z0 { get; }
        public float Z1 { get; }
        public float K { get; }
        public Material Material { get; }

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            outputBox = new AABB(new Vector3(K-0.0001f, Y0, Z0), new Vector3(K+0.0001f, Y1, Z1));
            return true;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            var t = (K - ray.Origin.X) / ray.Direction.X;
            if (t < tMin || t > tMax)
                return false;

            var y = ray.Origin.Y + t * ray.Direction.Y;
            var z = ray.Origin.Z + t * ray.Direction.Z;

            if (y < Y0 || y > Y1 || z < Z0 || z > Z1)
                return false;

            record.u = (y - Y0) / (Y1 - Y0);
            record.v = (z - Z0) / (Z1 - Z0);
            record.t = t;
            var outwardNormal = new Vector3(1, 0, 0);
            record.SetFaceNormal(ray, outwardNormal);
            record.material = Material;
            record.p = ray.At(t);
            return true;
        }
    }
}
