#if UNITY_5
using UnityEngine;
#endif

namespace FixedPointy
{
	public static class FixUtil
	{
		public static float Floating(Fix val)
        {
            return (float)val;
        }

        public static Fix Fixed(float val)
        {
            return (Fix)(FixConst)val;
        }

		#if UNITY_5
		public static Bounds Floating(FixBounds val)
		{
			return new Bounds(Floating(val.center), Floating(val.size));
		}

		public static Ray Floating(FixRay val)
		{
			return new Ray(Floating(val.origin), Floating(val.direction));
		}

		public static Rect Floating(FixRect val)
		{
			return new Rect(Floating(val.x), Floating(val.y), Floating(val.width), Floating(val.height));
		}

        public static Vector2 Floating(FixVec2 val)
        {
            return new Vector2(Floating(val.X), Floating(val.Y));
        }
        
        public static Vector3 Floating(FixVec3 val)
        {
            return new Vector3(Floating(val.X), Floating(val.Y), Floating(val.Z));
        } 

		public static FixBounds Fixed(Bounds val)
		{
			return new FixBounds(Fixed(val.center), Fixed(val.size));
		}

		public static FixRay Floating(Ray val)
		{
			return new FixRay(Fixed(val.origin), Fixed(val.direction));
		}

		public static FixRect Fixed(Rect val)
		{
			return new FixRect(Fixed(val.x), Fixed(val.y), Fixed(val.width), Fixed(val.height));
		}

        public static FixVec2 Fixed(Vector2 val)
        {
            return new FixVec2(Fixed(val.x), Fixed(val.y));
        }

        public static FixVec3 Fixed(Vector3 val)
        {
            return new FixVec3(Fixed(val.x), Fixed(val.y), Fixed(val.z));
        }
        #endif
	}
}
