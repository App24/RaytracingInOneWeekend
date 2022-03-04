using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RIOW.Textures
{
    internal abstract class Texture
    {
        public abstract Vector3 Value(float u, float v, Vector3 p);
    }
}
