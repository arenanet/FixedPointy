/* FixedPointy - A simple fixed-point math library for C#.
 * 
 * Copyright (c) 2013 Jameson Ernst
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;

namespace FixedPointy {
	[Serializable]
	public struct FixVec3 {
		public static readonly FixVec3 Zero = new FixVec3();
		public static readonly FixVec3 One = new FixVec3(1, 1, 1);
		public static readonly FixVec3 UnitX = new FixVec3(1, 0, 0);
		public static readonly FixVec3 UnitY = new FixVec3(0, 1, 0);
		public static readonly FixVec3 UnitZ = new FixVec3(0, 0, 1);

		public static implicit operator FixVec3 (FixVec2 value) {
			return new FixVec3(value.X, value.Y, 0);
		}

		public static FixVec3 operator + (FixVec3 rhs) {
			return rhs;
		}
		public static FixVec3 operator - (FixVec3 rhs) {
			return new FixVec3(-rhs._x, -rhs._y, -rhs._z);
		}

		public static bool operator == (FixVec3 lhs, FixVec3 rhs) {
			return lhs._x == rhs._x && lhs._y == rhs._y && lhs._z == rhs._z;
		}
		public static bool operator != (FixVec3 lhs, FixVec3 rhs) {
			return lhs._x != rhs._x || lhs._y != rhs._y || lhs._z != rhs._z;
		}

		public static FixVec3 operator + (FixVec3 lhs, FixVec3 rhs) {
			return new FixVec3(lhs._x + rhs._x, lhs._y + rhs._y, lhs._z + rhs._z);
		}
		public static FixVec3 operator - (FixVec3 lhs, FixVec3 rhs) {
			return new FixVec3(lhs._x - rhs._x, lhs._y - rhs._y, lhs._z - rhs._z);
		}

		public static FixVec3 operator + (FixVec3 lhs, Fix rhs) {
			return lhs.ScalarAdd(rhs);
		}
		public static FixVec3 operator + (Fix lhs, FixVec3 rhs) {
			return rhs.ScalarAdd(lhs);
		}
		public static FixVec3 operator - (FixVec3 lhs, Fix rhs) {
			return new FixVec3(lhs._x - rhs, lhs._y - rhs, lhs._z - rhs);
		}
		public static FixVec3 operator * (FixVec3 lhs, Fix rhs) {
			return lhs.ScalarMultiply(rhs);
		}
		public static FixVec3 operator * (Fix lhs, FixVec3 rhs) {
			return rhs.ScalarMultiply(lhs);
		}
		public static FixVec3 operator / (FixVec3 lhs, Fix rhs) {
			return new FixVec3(lhs._x / rhs, lhs._y / rhs, lhs._z / rhs);
		}

		public static FixVec3 MoveTowards(FixVec3 current, FixVec3 target, Fix maxDistanceDelta)
        {
            FixVec3 vector = target - current;
            Fix magnitude = vector.GetMagnitude();
            if ((magnitude > maxDistanceDelta) && (magnitude != Fix.Zero))
            {
                return (current + ((FixVec3) ((vector / magnitude) * maxDistanceDelta)));
            }
            return target;
        }

		public static FixVec3 Lerp(FixVec3 a, FixVec3 b, Fix t)
        {
            t = t < Fix.Zero ? Fix.Zero : (t > Fix.One ? Fix.One : t);
            return new FixVec3(a.X + ((b.X - a.X) * t), a.Y + ((b.Y - a.Y) * t), a.Z + ((b.Z - a.Z) * t));
        }

		#if UNITY_5
		[UnityEngine.SerializeField]
		#endif
		Fix _x, _y, _z;

		public FixVec3 (Fix x, Fix y, Fix z) {
			_x = x;
			_y = y;
			_z = z;
		}

		public Fix X { get { return _x; } }
		public Fix Y { get { return _y; } }
		public Fix Z { get { return _z; } }

		public Fix Dot (FixVec3 rhs) {
			return _x * rhs._x + _y * rhs._y + _z * rhs._z;
		}

		public FixVec3 Cross (FixVec3 rhs) {
			return new FixVec3(
				_y * rhs._z - _z * rhs._y,
				_z * rhs._x - _x * rhs._z,
				_x * rhs._y - _y * rhs._x
			);
		}

		FixVec3 ScalarAdd (Fix value) {
			return new FixVec3(_x + value, _y + value, _z + value);
		}
		FixVec3 ScalarMultiply (Fix value) {
			return new FixVec3(_x * value, _y * value, _z * value);
		}

		public FixVec3 WithX(Fix x)
		{
			return new FixVec3(x, _y, _z);
		}

		public FixVec3 WithY(Fix y)
		{
			return new FixVec3(_x, y, _z);
		}

		public FixVec3 WithZ(Fix z)
		{
			return new FixVec3(_x, _y, z);
		}

		public Fix GetSqrMagnitude()
		{
			if (X == Fix.Zero && Y == Fix.Zero && Z == Fix.Zero)
			{
				return Fix.Zero;
			}
			
			return Dot(this);
		}

		public static FixVec3 ClampMagnitude(FixVec3 vector, Fix maxLength)
		{
			if (vector.GetSqrMagnitude() > maxLength * maxLength)
			{
				return vector.Normalize() * maxLength;
			}

			return vector;
		}

		public static FixVec3 SmoothDamp(FixVec3 current, FixVec3 target, ref FixVec3 currentVelocity, Fix smoothTime, Fix maxSpeed, Fix deltaTime)
		{
			smoothTime = FixMath.Max(Fix.Ratio(1, 10000), smoothTime);
			Fix num = Fix.Ratio(2, 1) / smoothTime;
			Fix num2 = num * deltaTime;
			Fix d = Fix.One  / (Fix.One + num2 + Fix.Ratio(48, 100) * num2 * num2 + Fix.Ratio(235, 1000) * num2 * num2 * num2);
			FixVec3 vector = current - target;
			FixVec3 vector2 = target;
			Fix maxLength = maxSpeed * smoothTime;
			vector = FixVec3.ClampMagnitude(vector, maxLength);
			target = current - vector;
			FixVec3 vector3 = (currentVelocity + num * vector) * deltaTime;
			currentVelocity = (currentVelocity - num * vector3) * d;
			FixVec3 vector4 = target + (vector + vector3) * d;
			if ((vector2 - current).Dot(vector4 - vector2) > Fix.Zero)
			{
				vector4 = vector2;
				currentVelocity = (vector4 - vector2) / deltaTime;
			}
			return vector4;
		}

		public Fix GetMagnitude () {
			Fix sqrMagnitude = GetSqrMagnitude();

			if (sqrMagnitude == Fix.Zero)
			{
				return Fix.Zero;
			}

			return FixMath.Sqrt(sqrMagnitude);
		}

		public FixVec3 Normalize () {
			if (_x == 0 && _y == 0 && _z == 0)
				return FixVec3.Zero;

			var m = GetMagnitude();
			if (m == Fix.Zero)
			{
				// Magnitude was too small for fixed precision, approximate with a 0-length vector
				return FixVec3.Zero;
			}

			return new FixVec3(_x / m, _y / m, _z / m);
		}

		public FixVec3 NormalizeReturnMagnitude (out Fix oldMagnitude) {
			if (_x == 0 && _y == 0 && _z == 0)
            {
                oldMagnitude = Fix.Zero;
				return FixVec3.Zero;
            }

			oldMagnitude = GetMagnitude();
			if (oldMagnitude == Fix.Zero)
			{
				// Magnitude was too small for fixed precision, approximate with a 0-length vector
				return FixVec3.Zero;
			}

			return new FixVec3(_x / oldMagnitude, _y / oldMagnitude, _z / oldMagnitude);
		}

		public bool Equals(FixVec3 other)
		{
			return (this == other);
		}

		public override bool Equals(object o)
		{
			if (o is FixVec3)
			{
				return Equals((FixVec3)o);
			}

			return false;
		}

		public override string ToString () {
			return string.Format("({0}, {1}, {2})", _x, _y, _z);
		}

		public FixVec2 WithXZAsXY()
		{
			return new FixVec2(_x, _z);
		}
	}
}
