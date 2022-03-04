using RIOW.Materials;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    internal class MovingSphere : HitObject
    {
        public MovingSphere(Vector3 center0, Vector3 center1, float time0, float time1, float radius, Material material)
        {
            Center0 = center0;
            Center1 = center1;
            Time0 = time0;
            Time1 = time1;
            Radius = radius;
            Material = material;
        }

        public Vector3 Center0 { get; }
        public Vector3 Center1 { get; }
        public float Time0 { get; }
        public float Time1 { get; }
        public float Radius { get; }
        public Material Material { get; }

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            AABB box0 = new AABB(Center(time0) - new Vector3(Radius, Radius, Radius), Center(time0) + new Vector3(Radius, Radius, Radius));
            AABB box1 = new AABB(Center(time1) - new Vector3(Radius, Radius, Radius), Center(time1) + new Vector3(Radius, Radius, Radius));
            outputBox = Utils.SurroundingBox(box0, box1);
            return true;
        }

        public Vector3 Center(float time)
        {
            return Center0 + ((time - Time0) / (Time1 - Time0)) * (Center1 - Center0);
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            Vector3 oc = ray.Origin - Center(ray.Time);
            var a = ray.Direction.LengthSquared();
            var halfB = Vector3.Dot(oc, ray.Direction);
            var c = oc.LengthSquared() - Radius * Radius;

            var discriminant = halfB * halfB - a * c;
            if (discriminant < 0) return false;

            var sqrtd = MathF.Sqrt(discriminant);

            var root = (-halfB - sqrtd) / a;
            if (root > tMax || root < tMin)
            {
                root = (-halfB + sqrtd) / a;
                if (root > tMax || root < tMin)
                {
                    return false;
                }
            }


            record.t = root;
            record.p = ray.At(record.t);
            Vector3 outwardNormal = (record.p - Center(ray.Time)) / Radius;
            record.SetFaceNormal(ray, outwardNormal);
            record.material = Material;
            return true;
        }
    }
}
