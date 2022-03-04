using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW
{
    internal class Camera
    {
        Vector3 origin;
        Vector3 lowerLeftCorner;
        Vector3 horizontal;
        Vector3 vertical;
        Vector3 u, v, w;
        float lensRadius;

        public Camera(Vector3 lookfrom, Vector3 lookat, Vector3 vup, float vfov, float aspectRatio, float aperture, float focusDist, float time0=0, float time1=0)
        {
            float theta=Utils.DegreesToRadians(vfov);
            float viewportHeight = 2 * MathF.Tan(theta / 2f);
            float viewportWidth = aspectRatio * viewportHeight;

            w = Vector3.Normalize(lookfrom - lookat);
            u = Vector3.Normalize(Vector3.Cross(vup, w));
            v = Vector3.Cross(w, u);

            origin = lookfrom;
            horizontal = focusDist*viewportWidth * u;
            vertical = focusDist*viewportHeight * v;
            lowerLeftCorner = origin - horizontal / 2f - vertical / 2f - focusDist*w;

            lensRadius = aperture / 2f;

            Time0 = time0;
            Time1 = time1;
        }

        public float Time0 { get; }
        public float Time1 { get; }

        public Ray GetRay(float s, float t, Utils utils)
        {
            Vector3 rd = lensRadius * utils.RandomInUnitDisk();
            Vector3 offset = u * rd.X + v * rd.Y;

            return new Ray(origin+offset, lowerLeftCorner + s * horizontal + t * vertical - origin - offset, utils.RandomFloat(Time0, Time1));
        }
    }
}
