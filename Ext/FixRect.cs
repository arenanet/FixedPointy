/// <summary>
/// Fixed point version of UnityEngine.Rect.
/// </summary>
namespace FixedPointy
{
	[System.Serializable]
	public struct FixRect
	{
		public static FixRect MinMaxRect(Fix xmin, Fix ymin, Fix xmax, Fix ymax)
		{
			return new FixRect(new FixVec2(xmin, ymin),
			                   new FixVec2((xmax - xmin), (ymax - ymin)));
		}

		public static FixVec2 NormalizedToPoint(FixRect rectangle, FixVec2 normalizedRectCoordinates)
		{
			Fix x = FixMath.Min(FixMath.Max(Fix.Zero, normalizedRectCoordinates.X), Fix.One);
			Fix y = FixMath.Min(FixMath.Max(Fix.Zero, normalizedRectCoordinates.Y), Fix.One);

			return new FixVec2(rectangle.xMin + (rectangle.xMax - rectangle.xMin) * x,
							   rectangle.yMin + (rectangle.yMax - rectangle.yMin) * y);
		}

		public static FixVec2 PointToNormalized(FixRect rectangle, FixVec2 point)
		{
			return new FixVec2(FixMath.Min(FixMath.Max(Fix.Zero, (point.X - rectangle.xMin) / (rectangle.xMax - rectangle.xMin)), Fix.One),
			                   FixMath.Min(FixMath.Max(Fix.Zero, (point.Y - rectangle.yMin) / (rectangle.yMax - rectangle.yMin)), Fix.One));
		}

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec2 _center;

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec2 _max;

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec2 _min;

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec2 _size;

		public Fix height{ get{ return _size.Y; } }

		public Fix width{ get{ return _size.X; } }

		public Fix x{ get{ return _min.X; } }

		public Fix xMax{ get{ return _max.X; } }

		public Fix xMin{ get{ return _min.X; } }

		public Fix y{ get{ return _min.Y; } }

		public Fix yMax{ get{ return _max.Y; } }

		public Fix yMin{ get{ return _min.Y; } }

		public FixVec2 center{ get{ return _center; } }

		public FixVec2 max{ get{ return _max; } }

		public FixVec2 min{ get{ return _min; } }

		public FixVec2 position{ get{ return _min; } }

		public FixVec2 size{ get{ return _size; } }

		public FixRect(FixVec2 position, FixVec2 size)
		{
			_min = position;
			_max = position + size;
			_size = size;
			_center = _min + (_max - _min) / 2;
		}

		public FixRect(Fix x, Fix y, Fix width, Fix height)
		{
			_min = new FixVec2(x, y);
			_max = new FixVec2(x + width, y + height);
			_size = new FixVec2(width, height);
			_center = _min + (_max - _min) / 2;
		}

		public bool Contains(FixVec2 point)
		{
			if ((point.X < xMin) ||
			    (point.X > xMax) ||
				(point.Y < yMin) ||
				(point.Y > yMax))
			{
				return false;
			}

			return true;
		}

		public bool Contains(FixVec3 point)
		{
			if ((point.X < xMin) ||
			    (point.X > xMax) ||
				(point.Y < yMin) ||
				(point.Y > yMax))
			{
				return false;
			}

			return true;
		}

		public bool Contains(FixRect other)
		{
			if ((other.xMax <= xMax) &&
			    (other.xMin >= xMin) &&
				(other.yMax <= yMax) &&
				(other.yMin >= yMin))
			{
				return true;
			}

			return false;
		}

		public bool Contains(Fix x, Fix y)
		{
			if ((x < xMin) ||
			    (x > xMax) ||
				(y < yMin) ||
				(y > yMax))
			{
				return false;
			}

			return true;
		}

		public void Encapsulate(Fix x, Fix y)
		{
			_min = new FixVec2(FixMath.Min(x, _min.X),
							   FixMath.Min(y, _min.Y));
			
			_max = new FixVec2(FixMath.Max(x, _max.X),
							   FixMath.Max(y, _max.Y));
			
			_size = _max - _min;
			_center = _min + _size / 2;
		}

		public bool Overlaps(FixRect other)
		{
			if ((other.xMax < xMin) ||
			    (other.xMin > xMax) ||
				(other.yMax < xMin) ||
				(other.yMin > yMax))
			{
				return false;
			}

			return true;
		}

		public void Set(FixRect other)
		{
			_center = other._center;
			_max = other._max;
			_min = other._min;
			_size = other._size;
		}

		public void Set(FixVec2 position, FixVec2 size)
		{
			_min = position;
			_max = position + size;
			_size = size;
			_center = _min + (_max - _min) / 2;
		}

		public void Set(Fix x, Fix y, Fix width, Fix height)
		{
			_min = new FixVec2(x, y);
			_max = new FixVec2(x + width, y + height);
			_size = new FixVec2(width, height);
			_center = _min + (_max - _min) / 2;
		}
	}
}