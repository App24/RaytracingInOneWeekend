using RIOW.Materials;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    struct HitRecord
    {
        public Vector3 p;
        public Vector3 normal;
        public Material material;
        public float t;
        public float u;
        public float v;
        public bool frontFace;

        public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
        {
            frontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0;
            normal = frontFace ? outwardNormal : -outwardNormal;
        }
    }

    internal abstract class HitObject
    {
        public abstract bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils);
        public abstract bool BoundingBox(float time0, float time1, out AABB outputBox);
    }
}
