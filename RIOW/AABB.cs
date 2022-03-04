using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW
{
    internal class AABB
    {
        public AABB(Vector3 minimum, Vector3 maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public Vector3 Minimum { get; }
        public Vector3 Maximum { get; }

        public float Index(Vector3 v, int index)
        {
            if(index==0)
                return v.X;
            else if(index==1)
                return v.Y;
            else if(index==2)
                return v.Z;
            return 0;
        }

        public bool Hit(Ray ray, float tMin, float tMax)
        {
            for (int a = 0; a < 3; a++)
            {
                var invD = 1f / ray.Direction.GetAxis(a);
                var t0 = (Minimum.GetAxis(a) - ray.Origin.GetAxis(a)) * invD;
                var t1 = (Maximum.GetAxis(a) - ray.Origin.GetAxis(a)) * invD;
                if(invD < 0f)
                {
                    var temp = t0;
                    t0 = t1;
                    t1 = temp;
                }

                tMin = t0 > tMin ? t0 : tMin;
                tMax = t1 < tMax ? t1 : tMax;
                if (tMax <= tMin)
                    return false;
            }
            return true;
        }
    }
}
