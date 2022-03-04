using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW
{
    internal class Perlin
    {
        const int PointCount = 256;

        Vector3[] ranvec;
        int[] permX, permY, permZ;

        public Perlin()
        {
            Utils utils = new Utils();
            ranvec = new Vector3[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                ranvec[i] = Vector3.Normalize(utils.RandomVector3(-1, 1));
            }

            permX = PerlinGeneratePerm();
            permY = PerlinGeneratePerm();
            permZ = PerlinGeneratePerm();
        }

        public float Noise(Vector3 p)
        {
            var u = p.X - MathF.Floor(p.X);
            var v = p.Y - MathF.Floor(p.Y);
            var w = p.Z - MathF.Floor(p.Z);

            var i = (int)MathF.Floor(p.X);
            var j = (int)MathF.Floor(p.Y);
            var k = (int)MathF.Floor(p.Z);
            Vector3[,,] c = new Vector3[2, 2, 2];

            for (int di = 0; di < 2; di++)
            {
                for (int dj = 0; dj < 2; dj++)
                {
                    for (int dk = 0; dk < 2; dk++)
                    {
                        c[di, dj, dk] = ranvec[
                            permX[(i + di) & 255] ^
                            permY[(j + dj) & 255] ^
                            permZ[(k + dk) & 255]
                            ];
                    }
                }
            }

            return PerlinInterp(c, u, v, w);
        }

        public float Turb(Vector3 p, int depth = 7)
        {
            float accum = 0f;
            Vector3 tempP = p;
            float weight = 1f;

            for (int i = 0; i < depth; i++)
            {
                accum += weight * Noise(tempP);
                weight *= 0.5f;
                tempP *= 2f;
            }

            return MathF.Abs(accum);
        }

        static int[] PerlinGeneratePerm()
        {
            int[] p = new int[PointCount];

            for (int i = 0; i < PointCount; i++)
            {
                p[i] = i;
            }

            Permute(ref p, PointCount);

            return p;
        }

        static void Permute(ref int[] p, int n)
        {
            for (int i = n-1; i > 0; i--)
            {
                int target = new Random().Next(0, i);
                int tmp = p[i];
                p[i] = p[target];
                p[target] = tmp;
            }
        }

        static float TrilinearInterp(float[,,] c, float u, float v, float w)
        {
            float accum = 0f;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        accum +=
                            (i * u + (1f - i) * (1f - u)) *
                            (j * v + (1f - j) * (1f - v)) *
                            (k * w + (1f - k) * (1f - w)) * c[i, j, k];
                    }
                }
            }

            return accum;
        }

        static float PerlinInterp(Vector3[,,] c, float u, float v, float w)
        {
            float uu = u * u * (3 - 2 * u);
            float vv = v * v * (3 - 2 * v);
            float ww = w * w * (3 - 2 * w);
            float accum = 0f;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 weightV = new Vector3(u - i, v - j, w - k);
                        accum +=
                            (i * uu + (1f - i) * (1f - uu)) *
                            (j * vv + (1f - j) * (1f - vv)) *
                            (k * ww + (1f - k) * (1f - ww)) * Vector3.Dot(c[i, j, k], weightV);
                    }
                }
            }

            return accum;
        }
    }
}
