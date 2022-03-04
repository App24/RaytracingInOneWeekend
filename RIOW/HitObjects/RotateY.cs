using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    internal class RotateY : HitObject
    {
        public RotateY(HitObject p, float angle)
        {
            Ptr = p;

            var radians=Utils.DegreesToRadians(angle);
            sinTheta = MathF.Sin(radians);
            cosTheta = MathF.Cos(radians);
            hasBox = Ptr.BoundingBox(0, 1, out bbox);

            Vector3 min = new Vector3(Utils.Infinity);
            Vector3 max = new Vector3(-Utils.Infinity);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        var x = i * bbox.Maximum.X + (1f - i) * bbox.Minimum.X;
                        var y = j * bbox.Maximum.Y + (1f - j) * bbox.Minimum.Y;
                        var z = k * bbox.Maximum.Z + (1f - k) * bbox.Minimum.Z;

                        var newX = cosTheta * x + sinTheta * z;
                        var newZ = -sinTheta * x + cosTheta * z;

                        Vector3 tester = new Vector3(newX, y, newZ);

                        for (int c = 0; c < 3; c++)
                        {
                            min.SetAxis(c, MathF.Min(min.GetAxis(c), tester.GetAxis(c)));
                            max.SetAxis(c, MathF.Max(max.GetAxis(c), tester.GetAxis(c)));
                        }
                    }
                }
            }

            bbox = new AABB(min, max);
        }

        public HitObject Ptr { get; }
        float sinTheta;
        float cosTheta;
        bool hasBox;
        AABB bbox;

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            outputBox = bbox;
            return true;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            var origin = ray.Origin;
            var direction = ray.Direction;

            origin.X = cosTheta * ray.Origin.X - sinTheta * ray.Origin.Z;
            origin.Z = sinTheta * ray.Origin.X + cosTheta * ray.Origin.Z;

            direction.X = cosTheta * ray.Direction.X - sinTheta * ray.Direction.Z;
            direction.Z = sinTheta * ray.Direction.X + cosTheta * ray.Direction.Z;

            Ray rotateRay = new Ray(origin, direction, ray.Time);

            if (!Ptr.Hit(rotateRay, tMin, tMax, ref record, utils))
                return false;

            var p = record.p;
            var normal = record.normal;

            p.X = cosTheta * record.p.X + sinTheta * record.p.Z;
            p.Z = -sinTheta * record.p.X + cosTheta * record.p.Z;

            normal.X = cosTheta * record.normal.X + sinTheta * record.normal.Z;
            normal.Z = -sinTheta * record.normal.X + cosTheta * record.normal.Z;

            record.p = p;
            record.SetFaceNormal(rotateRay, normal);

            return true;
        }
    }
}
