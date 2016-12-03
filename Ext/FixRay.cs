namespace FixedPointy
{
	/// <summary>
	/// Fixed point version of UnityEngine.Bounds.
	/// <summary>
	[System.Serializable]
	public struct FixRay
	{
		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec3 _direction;

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		private FixVec3 _origin;

		public FixVec3 direction{ get{ return _direction; } }

		public FixVec3 origin{ get{ return _origin; } }

		public FixRay(FixVec3 origin, FixVec3 direction)
		{
			_origin = origin;
			_direction = direction.Normalize();
		}

		public FixVec3 GetPoint(Fix distance)
		{
			return _origin + _direction * distance;
		}
	}
}
