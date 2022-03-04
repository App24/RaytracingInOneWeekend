using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    internal class Translate : HitObject
    {
        public Translate(HitObject p, Vector3 displacement)
        {
            Ptr = p;
            Displacement = displacement;
        }

        public HitObject Ptr { get; }
        public Vector3 Displacement { get; }

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            if (!Ptr.BoundingBox(time0, time1, out outputBox))
                return false;

            outputBox = new AABB(outputBox.Minimum + Displacement, outputBox.Maximum + Displacement);

            return true;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            Ray movedRay = new Ray(ray.Origin - Displacement, ray.Direction, ray.Time);
            if (!Ptr.Hit(movedRay, tMin, tMax, ref record, utils))
                return false;

            record.p += Displacement;
            record.SetFaceNormal(movedRay, record.normal);

            return true;
        }
    }
}
