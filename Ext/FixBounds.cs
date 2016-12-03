namespace FixedPointy
{
	/// <summary>
	/// Fixed point version of UnityEngine.Bounds.
	/// <summary>
	[System.Serializable]
	public struct FixBounds
	{
		private enum Quadrant
		{
			LEFT,
			RIGHT,
			MIDDLE
		}

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec3 _center;

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec3 _extents;

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec3 _max;

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec3 _min;

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec3 _size;

		public FixVec3 center{ get{ return _center; } }

		public FixVec3 extents{ get{ return _extents; } }

		public FixVec3 max{ get{ return _max; } }

		public FixVec3 min{ get{ return _min; } }

		public FixVec3 size{ get{ return _size; } }

		public FixBounds(FixVec3 center, FixVec3 size)
		{
			_center = center;
			_extents = size / 2;
			_min = _center - _extents;
			_max = _center + _extents;
			_size = size;
		}

		public FixVec3 ClosestPoint(FixVec3 point)
		{
			// Clamp point to bounds
			Fix x = FixMath.Min(FixMath.Max(_min.X, point.X), _max.X);
			Fix y = FixMath.Min(FixMath.Max(_min.Y, point.Y), _max.Y);
			Fix z = FixMath.Min(FixMath.Max(_min.Z, point.Z), _max.Z);

			// Return clamped point
			return new FixVec3(x, y, z);
		}

		public bool Contains(FixVec3 point)
		{
			if ((point.X < _min.X) ||
			    (point.X > _max.X) ||
				(point.Y < _min.Y) ||
				(point.Y > _max.Y) ||
				(point.Z < _min.Z) ||
				(point.Z > _max.Z))
			{
				return false;
			}

			return true;
		}

		public bool Contains(FixBounds other)
		{
			return this.min.X <= other.max.X && this.max.X >= other.min.X && 
					this.min.Y <= other.max.Y && this.max.Y >= other.min.Y && 
					this.min.Z <= other.max.Z && this.max.Z >= other.min.Z;
		}

		public void Encapsulate(FixVec3 point)
		{
			Fix x;
			Fix y;
			Fix z;

			// Expand min
			x = FixMath.Min(_min.X, point.X);
			y = FixMath.Min(_min.Y, point.Y);
			z = FixMath.Min(_min.Z, point.Z);
			_min = new FixVec3(x, y, z);

			// Expand max
			x = FixMath.Max(_max.X, point.X);
			y = FixMath.Max(_max.Y, point.Y);
			z = FixMath.Max(_max.Z, point.Z);
			_max = new FixVec3(x, y, z);

			// Update other values
			UpdateFromMinMax();
		}

		public void Encapsulate(FixBounds other)
		{
			Fix x;
			Fix y;
			Fix z;

			// Expand min
			x = FixMath.Min(_min.X, other._min.X);
			y = FixMath.Min(_min.Y, other._min.Y);
			z = FixMath.Min(_min.Z, other._min.Z);
			_min = new FixVec3(x, y, z);

			// Expand max
			x = FixMath.Max(_max.X, other._max.X);
			y = FixMath.Max(_max.Y, other._max.Y);
			z = FixMath.Max(_max.Z, other._max.Z);
			_max = new FixVec3(x, y, z);

			// Update other values
			UpdateFromMinMax();
		}

		public void Expand(Fix amount)
		{
			Expand(new FixVec3(amount, amount, amount));
		}

		public void Expand(FixVec3 amount)
		{
			_size += amount;
			_extents = _size / 2;
			_min = _center - _extents;
			_max = _center + _extents;
		}

		// TODO - ED: Test
		public bool IntersectRay(FixRay ray)
		{
			Fix dummy;
			return IntersectRay(ray, out dummy);
		}

		// TODO - ED: Test
		// Source: https://github.com/erich666/GraphicsGems/blob/master/gems/RayBox.c
		public bool IntersectRay(FixRay ray, out Fix distance)
		{
			int whichPlane;
			bool inside = true;
			Quadrant[] quadrant = new Quadrant[3];
			Fix[] maxT = new Fix[3];
			Fix[] candidatePlane = new Fix[3];
			Fix[] coord = new Fix[3];
			FixVec3 intersection;
			
			Fix[] minB = {_min.X, _min.Y, _min.Z};
			Fix[] maxB = {_max.X, _max.Y, _max.Z};
			Fix[] rayOrigin = {ray.origin.X, ray.origin.Y, ray.origin.Z};
			Fix[] rayDirection = {ray.direction.X, ray.direction.Y, ray.direction.Z};

			// Find candidate planes; this loop can be avoided if
			// rays cast all from the eye(assume perpsective view).
			for (int i = 0; i < 3; i++)
			{
				if (rayOrigin[i] < minB[i])
				{
					quadrant[i] = Quadrant.LEFT;
					candidatePlane[i] = minB[i];
					inside = false;
				}
				else if (rayOrigin[i] > maxB[i])
				{
					quadrant[i] = Quadrant.RIGHT;
					candidatePlane[i] = maxB[i];
					inside = false;
				}
				else
				{
					quadrant[i] = Quadrant.MIDDLE;
				}
			}

			// Ray origin inside bounding box.
			if (inside)
			{
				distance = Fix.Zero;
				return true;
			}


			// Calculate T distances to candidate planes.
			for (int i = 0; i < 3; i++)
			{
				if (quadrant[i] != Quadrant.MIDDLE && rayDirection[i] != Fix.Zero)
				{
					maxT[i] = (candidatePlane[i] - rayOrigin[i]) / rayDirection[i];
				}
				else
				{
					maxT[i] = -Fix.One;
				}
			}

			// Get largest of the maxT's for final choice of intersection.
			whichPlane = 0;
			for (int i = 1; i < 3; i++)
			{
				if (maxT[whichPlane] < maxT[i])
				{
					whichPlane = i;
				}
			}

			// Check final candidate actually inside box.
			if (maxT[whichPlane] < Fix.Zero)
			{
				distance = Fix.Zero;
				return false;
			}
			
			for (int i = 0; i < 3; i++)
			{
				if (whichPlane != i)
				{
					coord[i] = rayOrigin[i] + maxT[whichPlane] * rayDirection[i];
					if (coord[i] < minB[i] || coord[i] > maxB[i])
					{
						distance = Fix.Zero;
						return false;
					}
				}
				else
				{
					coord[i] = candidatePlane[i];
				}
			}

			intersection = new FixVec3(coord[0], coord[1], coord[2]);
			
			distance = (intersection - ray.origin).GetMagnitude();
			return true;
		}

		public bool Intersects(FixBounds bounds)
		{
			if ((bounds._max.X < _min.X) ||
				(bounds._min.X > _max.X) ||
				(bounds._max.Y < _min.Y) ||
				(bounds._min.Y > _max.Y) ||
				(bounds._max.Z < _min.Z) ||
				(bounds._min.Z > _max.Z))
			{
				return false;
			}

			return true;
		}

		public void SetMinMax(FixVec3 min, FixVec3 max)
		{
			_min = min;
			_max = max;

			UpdateFromMinMax();
		}

		public Fix SqrDistance(FixVec3 point)
		{
			FixVec3 clampedPoint = ClosestPoint(point);
			return (point - clampedPoint).GetSqrMagnitude();
		}

		private void UpdateFromMinMax()
		{
			_size = _max - _min;
			_extents = _size / 2;
			_center = _min + extents;
		}
	}
}
