using RIOW.Materials;
using RIOW.Textures;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.HitObjects
{
    internal class ConstantMedium : HitObject
    {
        public ConstantMedium(HitObject b, float d, Texture a)
        {
            Boundary = b;
            NegInvDensity = -1f / d;
            PhaseFunction = new Isotropic(a);
        }
        public ConstantMedium(HitObject b, float d, Vector3 c)
        {
            Boundary = b;
            NegInvDensity = -1f / d;
            PhaseFunction = new Isotropic(c);
        }

        public HitObject Boundary { get; }
        public float NegInvDensity { get; }
        public Material PhaseFunction { get; }

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            return Boundary.BoundingBox(time0, time1, out outputBox);
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            HitRecord record1 = new HitRecord();
            HitRecord record2 = new HitRecord();

            if(!Boundary.Hit(ray, -Utils.Infinity, Utils.Infinity, ref record1, utils))
                return false;

            if (!Boundary.Hit(ray, record1.t + 0.0001f, Utils.Infinity, ref record2, utils))
                return false;

            if (record1.t < tMin) record1.t = tMin;
            if (record2.t > tMax) record2.t = tMax;

            if (record1.t >= record2.t)
                return false;

            if (record1.t < 0)
                record1.t = 0;

            var rayLength = ray.Direction.Length();
            var distanceInsideBounday = (record2.t - record1.t) * rayLength;
            var hitDistance = NegInvDensity * MathF.Log(utils.RandomFloat());

            if (hitDistance > distanceInsideBounday)
                return false;

            record.t = record1.t + hitDistance / rayLength;
            record.p = ray.At(record.t);

            record.normal = new Vector3(1, 0, 0);
            record.frontFace = true;
            record.material = PhaseFunction;
            return true;
        }
    }
}
