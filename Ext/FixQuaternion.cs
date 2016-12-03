#region --- License ---
/*
Copyright (c) 2006 - 2008 The Open Toolkit library.
Copyright 2013 Xamarin Inc

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
#endregion

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace FixedPointy
{
    /// <summary>
    /// Represents a Quaternion.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct FixQuaternion : IEquatable<FixQuaternion>
    {
        #region Fields

        private FixVec3 xyz;

        private Fix w;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Quaternion from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public FixQuaternion(FixVec3 v, Fix w)
        {
            this.xyz = v;
            this.w = w;
        }

        /// <summary>
        /// Construct a new Quaternion
        /// </summary>
        /// <param name="x">The x component</param>
        /// <param name="y">The y component</param>
        /// <param name="z">The z component</param>
        /// <param name="w">The w component</param>
        public FixQuaternion(Fix x, Fix y, Fix z, Fix w)
            : this(new FixVec3(x, y, z), w)
        { }

        public FixQuaternion (ref FixTrans3 matrix)
        {
			Fix determinant = (matrix.M11 * (matrix.M22 * matrix.M33 - matrix.M32 * matrix.M23)) -
							  (matrix.M12 * (matrix.M21 * matrix.M33 - matrix.M31 * matrix.M23)) +
							  (matrix.M13 * (matrix.M21 * matrix.M32 - matrix.M31 * matrix.M22));
			
            Fix scale = FixMath.Pow(determinant, Fix.One / 3);
            Fix x, y, z;
  
            w = (FixMath.Sqrt(FixMath.Max(0, scale + matrix.M11 + matrix.M22 + matrix.M33)) / 2);
            x = (FixMath.Sqrt(FixMath.Max(0, scale + matrix.M11 - matrix.M22 - matrix.M33)) / 2);
            y = (FixMath.Sqrt(FixMath.Max(0, scale - matrix.M11 + matrix.M22 - matrix.M33)) / 2);
            z = (FixMath.Sqrt(FixMath.Max(0, scale - matrix.M11 - matrix.M22 + matrix.M33)) / 2);

            xyz = new FixVec3 (x, y, z);

            if (matrix.M32 - matrix.M23 < 0) X = -X;
            if (matrix.M13 - matrix.M31 < 0) Y = -Y;
            if (matrix.M21 - matrix.M12 < 0) Z = -Z;
        }

        #endregion

        #region Public Members

        #region Properties

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Y and Z components of this instance.
        /// </summary>
        [Obsolete("Use Xyz property instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public FixVec3 XYZ { get { return Xyz; } set { Xyz = value; } }

        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Y and Z components of this instance.
        /// </summary>
        public FixVec3 Xyz { get { return xyz; } set { xyz = value; } }

        /// <summary>
        /// Gets or sets the X component of this instance.
        /// </summary>
        public Fix X { get { return xyz.X; } set { xyz = new FixVec3(value, xyz.Y, xyz.Z); } }

        /// <summary>
        /// Gets or sets the Y component of this instance.
        /// </summary>
        public Fix Y { get { return xyz.Y; } set { xyz = new FixVec3(xyz.X, value, xyz.Z); } }

        /// <summary>
        /// Gets or sets the Z component of this instance.
        /// </summary>
        public Fix Z { get { return xyz.Z; } set { xyz = new FixVec3(xyz.X, xyz.Y, value); } }

        /// <summary>
        /// Gets or sets the W component of this instance.
        /// </summary>
        public Fix W { get { return w; } set { w = value; } }

        #endregion

        #region Instance

        #region ToAxisAngle

        // /// <summary>
        // /// Convert the current quaternion to axis angle representation
        // /// </summary>
        // /// <param name="axis">The resultant axis</param>
        // /// <param name="angle">The resultant angle</param>
        // public void ToAxisAngle(out FixVec3 axis, out float angle)
        // {
        //     Vector4 result = ToAxisAngle();
        //     axis = result.Xyz;
        //     angle = result.W;
        // }

        // /// <summary>
        // /// Convert this instance to an axis-angle representation.
        // /// </summary>
        // /// <returns>A Vector4 that is the axis-angle representation of this quaternion.</returns>
        // public Vector4 ToAxisAngle()
        // {
        //     FixQuaternion q = this;
        //     if (FixMath.Abs(q.W) > Fix.One)
        //         q.Normalize();

        //     Vector4 result = new Vector4();

        //     result.W = 2 * FixMath.Acos(q.W); // angle
        //     Fix den = FixMath.Sqrt(Fix.One - q.W * q.W);
        //     if (den > Fix.One / 10000)
        //     {
        //         result.Xyz = q.Xyz / den;
        //     }
        //     else
        //     {
        //         // This occurs when the angle is zero. 
        //         // Not a problem: just set an arbitrary normalized axis.
        //         result.Xyz = FixVec3.UnitX;
        //     }

        //     return result;
        // }

        #endregion

        #region public float Length

        /// <summary>
        /// Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        public Fix Length
        {
            get
            {
                return FixMath.Sqrt(W * W + Xyz.GetSqrMagnitude());
            }
        }

        #endregion

        #region public float LengthSquared

        /// <summary>
        /// Gets the square of the quaternion length (magnitude).
        /// </summary>
        public Fix LengthSquared
        {
            get
            {
                return W * W + Xyz.GetSqrMagnitude();
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the Quaternion to unit length.
        /// </summary>
        public void Normalize()
        {
            Fix scale = Fix.One / this.Length;
            Xyz *= scale;
            W *= scale;
        }

        #endregion

        #region public void Conjugate()

        /// <summary>
        /// Convert this quaternion to its conjugate
        /// </summary>
        public void Conjugate()
        {
            Xyz = -Xyz;
        }

        #endregion

        #endregion

        #region Static

        #region Fields

        /// <summary>
        /// Defines the identity quaternion.
        /// </summary>
        public static FixQuaternion Identity = new FixQuaternion(0, 0, 0, 1);

        #endregion

        #region Add

        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <returns>The result of the addition</returns>
        public static FixQuaternion Add(FixQuaternion left, FixQuaternion right)
        {
            return new FixQuaternion(
                left.Xyz + right.Xyz,
                left.W + right.W);
        }

        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <param name="result">The result of the addition</param>
        public static void Add(ref FixQuaternion left, ref FixQuaternion right, out FixQuaternion result)
        {
            result = new FixQuaternion(
                left.Xyz + right.Xyz,
                left.W + right.W);
        }

        #endregion

        #region Sub

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>The result of the operation.</returns>
        public static FixQuaternion Sub(FixQuaternion left, FixQuaternion right)
        {
            return  new FixQuaternion(
                left.Xyz - right.Xyz,
                left.W - right.W);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Sub(ref FixQuaternion left, ref FixQuaternion right, out FixQuaternion result)
        {
            result = new FixQuaternion(
                left.Xyz - right.Xyz,
                left.W - right.W);
        }

        #endregion

        #region Mult

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        [Obsolete("Use Multiply instead.")]
        public static FixQuaternion Mult(FixQuaternion left, FixQuaternion right)
        {
            return new FixQuaternion(
                right.W * left.Xyz + left.W * right.Xyz + left.Xyz.Cross(right.Xyz),
                left.W * right.W - left.Xyz.Dot(right.Xyz));
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        [Obsolete("Use Multiply instead.")]
        public static void Mult(ref FixQuaternion left, ref FixQuaternion right, out FixQuaternion result)
        {
            result = new FixQuaternion(
                right.W * left.Xyz + left.W * right.Xyz + left.Xyz.Cross(right.Xyz),
                left.W * right.W - left.Xyz.Dot(right.Xyz));
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static FixQuaternion Multiply(FixQuaternion left, FixQuaternion right)
        {
            FixQuaternion result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref FixQuaternion left, ref FixQuaternion right, out FixQuaternion result)
        {
            result = new FixQuaternion(
                right.W * left.Xyz + left.W * right.Xyz + left.Xyz.Cross(right.Xyz),
                left.W * right.W - left.Xyz.Dot(right.Xyz));
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref FixQuaternion quaternion, Fix scale, out FixQuaternion result)
        {
            result = new FixQuaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        [Obsolete ("Use the overload without the ref float scale")]
        public static void Multiply(ref FixQuaternion quaternion, ref Fix scale, out FixQuaternion result)
        {
            result = new FixQuaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);    
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static FixQuaternion Multiply(FixQuaternion quaternion, Fix scale)
        {
            return new FixQuaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        #endregion

        #region Conjugate

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <returns>The conjugate of the given quaternion</returns>
        public static FixQuaternion Conjugate(FixQuaternion q)
        {
            return new FixQuaternion(-q.Xyz, q.W);
        }

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <param name="result">The conjugate of the given quaternion</param>
        public static void Conjugate(ref FixQuaternion q, out FixQuaternion result)
        {
            result = new FixQuaternion(-q.Xyz, q.W);
        }

        #endregion

        #region Invert

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The inverse of the given quaternion</returns>
        public static FixQuaternion Invert(FixQuaternion q)
        {
            FixQuaternion result;
            Invert(ref q, out result);
            return result;
        }

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <param name="result">The inverse of the given quaternion</param>
        public static void Invert(ref FixQuaternion q, out FixQuaternion result)
        {
            Fix lengthSq = q.LengthSquared;
            if (lengthSq != Fix.Zero)
            {
                Fix i = Fix.One / lengthSq;
                result = new FixQuaternion(q.Xyz * -i, q.W * i);
            }
            else
            {
                result = q;
            }
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static FixQuaternion Normalize(FixQuaternion q)
        {
            FixQuaternion result;
            Normalize(ref q, out result);
            return result;
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <param name="result">The normalized quaternion</param>
        public static void Normalize(ref FixQuaternion q, out FixQuaternion result)
        {
            Fix scale = Fix.One / q.Length;
            result = new FixQuaternion(q.Xyz * scale, q.W * scale);
        }

        #endregion

        #region FromAxisAngle

        /// <summary>
        /// Build a quaternion from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns></returns>
        public static FixQuaternion FromAxisAngle(FixVec3 axis, Fix angle)
        {
            if (axis.GetSqrMagnitude() == Fix.Zero)
                return Identity;

            FixQuaternion result = Identity;

            angle *= Fix.One / 2;
            axis.Normalize();
            result.Xyz = axis * FixMath.Sin(angle);
            result.W = FixMath.Cos(angle);

            return Normalize(result);
        }

        #endregion

        #region Slerp

        /// <summary>
        /// Do Spherical linear interpolation between two quaternions 
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given quaternions</returns>
        public static FixQuaternion Slerp(FixQuaternion q1, FixQuaternion q2, Fix blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared == Fix.Zero)
            {
                if (q2.LengthSquared == Fix.Zero)
                {
                    return Identity;
                }
                return q2;
            }
            else if (q2.LengthSquared == Fix.Zero)
            {
                return q1;
            }


            Fix cosHalfAngle = q1.W * q2.W + q1.Xyz.Dot(q2.Xyz);

            if (cosHalfAngle >= Fix.One || cosHalfAngle <= -Fix.One)
            {
                // angle = 0.0f, so just return one input.
                return q1;
            }
            else if (cosHalfAngle < Fix.Zero)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            Fix blendA;
            Fix blendB;
            if (cosHalfAngle < (Fix.One / 100) * 99)
            {
                // do proper slerp for big angles
                Fix halfAngle = FixMath.Acos(cosHalfAngle);
                Fix sinHalfAngle = FixMath.Sin(halfAngle);
                Fix oneOverSinHalfAngle = Fix.One / sinHalfAngle;
                blendA = FixMath.Sin(halfAngle * (Fix.One - blend)) * oneOverSinHalfAngle;
                blendB = FixMath.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = Fix.One - blend;
                blendB = blend;
            }

            FixQuaternion result = new FixQuaternion(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            if (result.LengthSquared > Fix.Zero)
                return Normalize(result);
            else
                return Identity;
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static FixQuaternion operator +(FixQuaternion left, FixQuaternion right)
        {
            left.Xyz += right.Xyz;
            left.W += right.W;
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static FixQuaternion operator -(FixQuaternion left, FixQuaternion right)
        {
            left.Xyz -= right.Xyz;
            left.W -= right.W;
            return left;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static FixQuaternion operator *(FixQuaternion left, FixQuaternion right)
        {
            Multiply(ref left, ref right, out left);
            return left;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static FixQuaternion operator *(FixQuaternion quaternion, Fix scale)
        {
            Multiply(ref quaternion, scale, out quaternion);
            return quaternion;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static FixQuaternion operator *(Fix scale, FixQuaternion quaternion)
        {
            return new FixQuaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(FixQuaternion left, FixQuaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(FixQuaternion left, FixQuaternion right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Quaternion.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("V: {0}, W: {1}", Xyz, W);
        }

        #endregion

        #region public override bool Equals (object o)

        /// <summary>
        /// Compares this object instance to another object for equality. 
        /// </summary>
        /// <param name="other">The other object to be used in the comparison.</param>
        /// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object other)
        {
            if (other is FixQuaternion == false) return false;
               return this == (FixQuaternion)other;
        }

        #endregion

        #region public override int GetHashCode ()

        /// <summary>
        /// Provides the hash code for this object. 
        /// </summary>
        /// <returns>A hash code formed from the bitwise XOR of this objects members.</returns>
        public override int GetHashCode()
        {
            return Xyz.GetHashCode() ^ W.GetHashCode();
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Quaternion> Members

        /// <summary>
        /// Compares this Quaternion instance to another Quaternion for equality. 
        /// </summary>
        /// <param name="other">The other Quaternion to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(FixQuaternion other)
        {
            return (Xyz.X == other.Xyz.X) &&
				   (Xyz.Y == other.Xyz.Y) &&
				   (Xyz.Z == other.Xyz.Z) &&
				   (W == other.W);
        }

        #endregion
    }
}
