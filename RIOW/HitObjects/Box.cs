using RIOW.Materials;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    internal class Box : HitObject
    {
        public Box(Vector3 boxMin, Vector3 boxMax, Material material)
        {
            BoxMin = boxMin;
            BoxMax = boxMax;

            sides.Add(new XYRect(BoxMin.X, BoxMax.X, BoxMin.Y, BoxMax.Y, BoxMax.Z, material));
            sides.Add(new XYRect(BoxMin.X, BoxMax.X, BoxMin.Y, BoxMax.Y, BoxMin.Z, material));

            sides.Add(new YZRect(BoxMin.Y, BoxMax.Y, BoxMin.Z, BoxMax.Z, BoxMax.X, material));
            sides.Add(new YZRect(BoxMin.Y, BoxMax.Y, BoxMin.Z, BoxMax.Z, BoxMin.X, material));

            sides.Add(new XZRect(BoxMin.X, BoxMax.X, BoxMin.Z, BoxMax.Z, BoxMax.Y, material));
            sides.Add(new XZRect(BoxMin.X, BoxMax.X, BoxMin.Z, BoxMax.Z, BoxMin.Y, material));
        }

        public Vector3 BoxMin { get; }
        public Vector3 BoxMax { get; }

        HitObjectList sides = new HitObjectList();

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            outputBox = new AABB(BoxMin, BoxMax);
            return true;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            return sides.Hit(ray, tMin, tMax, ref record, utils);
        }
    }
}
