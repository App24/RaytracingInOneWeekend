using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW
{
    internal class Ray
    {
        public Ray(Vector3 origin, Vector3 direction, float time=0)
        {
            Origin = origin;
            Direction = direction;
            Time = time;
        }

        public Vector3 Origin { get; }
        public Vector3 Direction { get; }
        public float Time { get; }

        public Vector3 At(float t)
        {
            return Origin + t * Direction;
        }
    }
}
