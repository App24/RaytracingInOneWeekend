using RIOW.HitObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW
{
    internal class Utils
    {
        public static float Infinity => float.MaxValue;
        public Random random = new Random();

        public static float DegreesToRadians(float degrees)
        {
            return degrees * MathF.PI / 180f;
        }

        public static Vector3 Reflect(Vector3 v, Vector3 n)
        {
            return v - 2 * Vector3.Dot(v, n) * n;
        }

        public static Vector3 Refract(Vector3 uv, Vector3 n, float etaiOverEtat)
        {
            var cosTheta = MathF.Min(Vector3.Dot(-uv, n), 1);
            Vector3 rOutPerp = etaiOverEtat * (uv + cosTheta * n);
            Vector3 rOutParallel = -MathF.Sqrt(MathF.Abs(1f-rOutPerp.LengthSquared())) * n;
            return rOutPerp + rOutParallel;
        }

        public static float Reflectance(float cosince, float refIdx)
        {
            var r0 = (1 - refIdx) / (1 + refIdx);
            r0 = r0 * r0;
            return r0 + (1 - r0) * MathF.Pow((1f - cosince), 5);
        }

        public static AABB SurroundingBox(AABB box0, AABB box1)
        {
            Vector3 small = new Vector3(
                MathF.Min(box0.Minimum.X, box1.Minimum.X),
                MathF.Min(box0.Minimum.Y, box1.Minimum.Y),
                MathF.Min(box0.Minimum.Z, box1.Minimum.Z)
                );

            Vector3 big = new Vector3(
                MathF.Max(box0.Maximum.X, box1.Maximum.X),
                MathF.Max(box0.Maximum.Y, box1.Maximum.Y),
                MathF.Max(box0.Maximum.Z, box1.Maximum.Z)
                );

            return new AABB(small, big);
        }

        public static bool BoxCompare(HitObject a, HitObject b, int axis)
        {
            AABB boxA;
            AABB boxB;

            if (!a.BoundingBox(0, 0, out boxA) || !b.BoundingBox(0, 0, out boxB))
            {
                throw new Exception();
            }

            return boxA.Minimum.GetAxis(axis) < boxB.Minimum.GetAxis(axis);
        }

        public static bool BoxXCompare(HitObject a, HitObject b)
        {
            return BoxCompare(a, b, 0);
        }

        public static bool BoxYCompare(HitObject a, HitObject b)
        {
            return BoxCompare(a, b, 1);
        }

        public static bool BoxZCompare(HitObject a, HitObject b)
        {
            return BoxCompare(a, b, 2);
        }

        public float RandomFloat()
        {
            return RandomFloat(0, 1);
        }

        public float RandomFloat(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        public Vector3 RandomVector3()
        {
            return new Vector3(RandomFloat(), RandomFloat(), RandomFloat());
        }

        public Vector3 RandomVector3(float min, float max)
        {
            return new Vector3(RandomFloat(min, max), RandomFloat(min, max), RandomFloat(min, max));
        }

        public Vector3 RandomInUnitSphere()
        {
            Vector3 p = RandomVector3(-1, 1);
            if (p.LengthSquared() >= 1)
                return RandomInUnitSphere();
            return p;
        }

        public Vector3 RandomInUnitDisk()
        {
            Vector3 p = new Vector3(RandomFloat(-1, 1), RandomFloat(-1, 1), 0);
            if(p.LengthSquared()>=1)
                return RandomInUnitDisk();
            return p;
        }

        public Vector3 RandomUnitVector()
        {
            return Vector3.Normalize(RandomInUnitSphere());
        }

        public Vector3 RandomInHemisphere(Vector3 normal)
        {
            Vector3 inUnitSphere=RandomInUnitSphere();
            if(Vector3.Dot(inUnitSphere, normal) > 0)
                return inUnitSphere;
            return -inUnitSphere;
        }
    }
}
