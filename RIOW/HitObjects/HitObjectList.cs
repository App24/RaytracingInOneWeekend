using System;
using System.Collections.Generic;
using System.Text;

namespace RIOW.HitObjects
{
    internal class HitObjectList : HitObject
    {
        public List<HitObject> HitObjects { get; } = new List<HitObject>();

        public HitObjectList()
        {

        }

        public HitObjectList(HitObject hitObject)
        {
            Add(hitObject);
        }

        public void Clear()
        {
            HitObjects.Clear();
        }

        public void Add(HitObject hitObject)
        {
            HitObjects.Add(hitObject);
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            HitRecord tempRec = new HitRecord();
            bool hitAnything = false;
            var closestSoFar = tMax;

            foreach(HitObject hitObject in HitObjects)
            {
                if(hitObject.Hit(ray, tMin, closestSoFar, ref tempRec, utils))
                {
                    hitAnything = true;
                    closestSoFar = tempRec.t;
                    record = tempRec;
                }
            }

            return hitAnything;
        }

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            outputBox = null;
            if (HitObjects.Count <= 0) return false;

            AABB tempBox;
            bool firstBox = true;

            foreach (HitObject hitObject in HitObjects)
            {
                if (!hitObject.BoundingBox(time0, time1, out tempBox)) return false;
                outputBox = firstBox ? tempBox : Utils.SurroundingBox(outputBox, tempBox);
                firstBox = false;
            }

            return true;
        }
    }
}
