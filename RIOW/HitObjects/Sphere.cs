using RIOW.Materials;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    internal class Sphere : HitObject
    {
        public Sphere(Vector3 center, float radius, Material material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public Vector3 Center { get; }
        public float Radius { get; }
        public Material Material { get; }

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            outputBox = new AABB(Center - new Vector3(Radius, Radius, Radius), Center + new Vector3(Radius, Radius, Radius));
            return true;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            Vector3 oc = ray.Origin - Center;
            var a = ray.Direction.LengthSquared();
            var halfB = Vector3.Dot(oc, ray.Direction);
            var c = oc.LengthSquared() - Radius * Radius;

            var discriminant = halfB * halfB - a * c;
            if (discriminant < 0) return false;

            var sqrtd = MathF.Sqrt(discriminant);

            var root = (-halfB - sqrtd) / a;
            if(root > tMax || root < tMin)
            {
                root = (-halfB + sqrtd) / a;
                if (root > tMax || root < tMin)
                {
                    return false;
                }
            }

            record.t = root;
            record.p = ray.At(record.t);
            Vector3 outwardNormal = (record.p - Center) / Radius;
            record.SetFaceNormal(ray, outwardNormal);
            GetSphereUV(outwardNormal, ref record.u, ref record.v);
            record.material = Material;
            return true;
        }

        static void GetSphereUV(Vector3 p, ref float u, ref float v)
        {
            var theta = MathF.Acos(-p.Y);
            var phi = MathF.Atan2(-p.Z, p.X) + MathF.PI;

            u = phi / (2 * MathF.PI);
            v = theta / MathF.PI;
        }
    }
}
