using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RIOW.HitObjects
{
    internal class BVHNode : HitObject
    {
        public HitObject Left { get; private set; }
        public HitObject Right { get; private set; }
        public AABB Box { get; private set; }

        public BVHNode()
        {

        }

        public BVHNode(HitObjectList list, float time0, float time1):this(list.HitObjects, 0, list.HitObjects.Count, time0, time1)
        {

        }

        public BVHNode(List<HitObject> objects, int start, int end, float time0, float time1)
        {
            int axis = new Random().Next(0, 2);

            int objectSpan = end - start;

            IComparer<HitObject> comparator;
            switch (axis)
            {
                case 0:
                    comparator = new BoxComparatorX();
                    break;
                case 1:
                    comparator = new BoxComparatorY();
                    break;
                default:
                case 2:
                    comparator = new BoxComparatorZ();
                    break;
            }

            if(objectSpan == 1)
            {
                Left = Right = objects[start];
            }else if(objectSpan == 2)
            {
                if (comparator.Compare(objects[start], objects[start + 1]) <= 0)
                {
                    Left=objects[start];
                    Right=objects[start+1];
                }
                else
                {
                    Left = objects[start+1];
                    Right = objects[start];
                }
            }
            else
            {
                objects.Sort(start, objectSpan, comparator);

                var mid = start + objectSpan / 2;
                Left = new BVHNode(objects, start, mid, time0, time1);
                Right = new BVHNode(objects, mid, end, time0, time1);
            }

            AABB boxLeft;
            AABB boxRight;

            if (!Left.BoundingBox(time0, time1, out boxLeft) || !Right.BoundingBox(time0, time1, out boxRight))
            {
                throw new Exception();
            }

            Box = Utils.SurroundingBox(boxLeft, boxRight);
        }

        public override bool BoundingBox(float time0, float time1, out AABB outputBox)
        {
            outputBox = Box;
            return true;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord record, Utils utils)
        {
            if (!Box.Hit(ray, tMin, tMax))
                return false;

            bool hitLeft = Left.Hit(ray, tMin, tMax, ref record, utils);
            bool hitRight = Right.Hit(ray, tMin, hitLeft ? record.t : tMax, ref record, utils);

            return hitRight || hitLeft;
        }
    }

    class BoxComparatorX : IComparer<HitObject>
    {
        public int Compare([AllowNull] HitObject a, [AllowNull] HitObject b)
        {
            return Utils.BoxXCompare(a, b) ? -1 : 1;
        }
    }

    class BoxComparatorY : IComparer<HitObject>
    {
        public int Compare([AllowNull] HitObject a, [AllowNull] HitObject b)
        {
            return Utils.BoxYCompare(a, b) ? -1 : 1;
        }
    }

    class BoxComparatorZ : IComparer<HitObject>
    {
        public int Compare([AllowNull] HitObject a, [AllowNull] HitObject b)
        {
            return Utils.BoxZCompare(a, b) ? -1 : 1;
        }
    }
}
