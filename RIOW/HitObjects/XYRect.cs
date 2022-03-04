using RIOW.Materials;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    internal class XYRect : HitObject
    {
        public XYRect(float x0, float x1, float y0, float y1, float k, Material material)
        {
            X0 = x0;
            X1 = x1;
            Y0 = y0;
            Y1 = y1;
            K = k;
            Material = material;
        }

        public float X0 { get; }
        public float X1 { get; }
        public float Y0 { get; }
        public float Y1 { get; }
        public float K { get; }
        public Material Material { get; }

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            outputBox = new AABB(new Vector3(X0, Y0, K - 0.0001f), new Vector3(X1, Y1, K + 0.0001f));
            return true;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            var t = (K - ray.Origin.Z) / ray.Direction.Z;
            if (t < tMin || t > tMax)
                return false;

            var x = ray.Origin.X + t * ray.Direction.X;
            var y = ray.Origin.Y + t * ray.Direction.Y;

            if (x < X0 || x > X1 || y < Y0 || y > Y1)
                return false;

            record.u = (x - X0) / (X1 - X0);
            record.v = (y - Y0) / (Y1 - Y0);
            record.t = t;
            var outwardNormal = new Vector3(0, 0, 1);
            record.SetFaceNormal(ray, outwardNormal);
            record.material = Material;
            record.p = ray.At(t);
            return true;
        }
    }
}
