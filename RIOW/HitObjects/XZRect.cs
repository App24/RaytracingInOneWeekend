using RIOW.Materials;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    internal class XZRect : HitObject
    {
        public XZRect(float x0, float x1, float z0, float z1, float k, Material material)
        {
            X0 = x0;
            X1 = x1;
            Z0 = z0;
            Z1 = z1;
            K = k;
            Material = material;
        }

        public float X0 { get; }
        public float X1 { get; }
        public float Z0 { get; }
        public float Z1 { get; }
        public float K { get; }
        public Material Material { get; }

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            outputBox = new AABB(new Vector3(X0, K - 0.0001f, Z0), new Vector3(X1, K + 0.0001f, Z0));
            return true;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            var t = (K - ray.Origin.Y) / ray.Direction.Y;
            if (t < tMin || t > tMax)
                return false;

            var x = ray.Origin.X + t * ray.Direction.X;
            var z = ray.Origin.Z + t * ray.Direction.Z;

            if (x < X0 || x > X1 || z < Z0 || z > Z1)
                return false;

            record.u = (x - X0) / (X1 - X0);
            record.v = (z - Z0) / (Z1 - Z0);
            record.t = t;
            var outwardNormal = new Vector3(0, 1, 0);
            record.SetFaceNormal(ray, outwardNormal);
            record.material = Material;
            record.p = ray.At(t);
            return true;
        }
    }
}
