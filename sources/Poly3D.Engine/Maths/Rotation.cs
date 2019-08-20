using OpenTK;

using System;
using System.Diagnostics;
using System.Drawing;
namespace Poly3D.Maths
{
    //Defined as struct to prevent implementing complex change detection mechanism.
    //Since you can't assign a struct field by a class property, you must assign the property to a new struct, 
    //so detecting changes can be done in the property accessor.
    public struct Rotation
    {
        private Quaternion _Quaternion;
        private Vector3 _EulerAngles;
        private Matrix3 _Matrix;
        private bool isEulerDirty;
        private bool isMatrixDirty;

        /// <summary>
        /// Gets or sets the rotation along the X axis (in degrees).
        /// </summary>
        public Angle Pitch
        {
            get { return Angle.FromDegrees(EulerAngles.X); }
            set
            {
                EulerAngles = new Vector3(value.Normalized().Degrees, EulerAngles.Y, EulerAngles.Z);
            }
        }

        /// <summary>
        /// Gets or sets the rotation along the Y axis (in degrees).
        /// </summary>
        public Angle Yaw
        {
            get { return Angle.FromDegrees(EulerAngles.Y); }
            set
            {
                EulerAngles = new Vector3(EulerAngles.X, value.Normalized().Degrees, EulerAngles.Z);
            }
        }

        /// <summary>
        /// Gets or sets the rotation along the Z axis (in degrees).
        /// </summary>
        public Angle Roll
        {
            get { return Angle.FromDegrees(EulerAngles.Z); }
            set
            {
                EulerAngles = new Vector3(EulerAngles.X, EulerAngles.Y, value.Normalized().Degrees);
            }
        }

        public Angle this[RotationComponent component]
        {
            get
            {
                switch (component)
                {
                    case RotationComponent.Roll:
                        return Roll;
                    case RotationComponent.Pitch:
                        return Pitch;
                    case RotationComponent.Yaw:
                        return Yaw;
                    default:
                        return Angle.Zero;
                }
            }
            set
            {
                switch (component)
                {
                    case RotationComponent.Roll:
                        Roll = value; break;
                    case RotationComponent.Pitch:
                        Pitch = value; break;
                    case RotationComponent.Yaw:
                        Yaw = value; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the rotation angles in degrees. Vector3(Pitch, Yaw, Roll)
        /// </summary>
        public Vector3 EulerAngles
        {
            get
            {
                if (isEulerDirty)
                {
                    _EulerAngles = GLMath.QuatToEuler(_Quaternion).ToDegrees();
                    NormalizeEulers();
                    isEulerDirty = false;
                }
                return _EulerAngles;
            }
            set
            {
                _EulerAngles = value;
                NormalizeEulers();
                _Quaternion = GLMath.EulerToQuat(_EulerAngles.ToRadians());
                isMatrixDirty = true;
                isEulerDirty = false;
            }
        }

        /// <summary>
        /// Gets or sets a quaternion representing this rotation.
        /// </summary>
        public Quaternion Quaternion
        {
            get { return _Quaternion; }
            set
            {
                if (_Quaternion == value)
                    return;
                _Quaternion = value;
                isMatrixDirty = true;
                isEulerDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets a matrix representing this rotation.
        /// </summary>
        public Matrix3 Matrix
        {
            get
            {
                if (isMatrixDirty)
                {
                    _Matrix = Matrix3.CreateFromQuaternion(Quaternion);
                    isMatrixDirty = false;
                }
                return _Matrix;
            }
            set
            {
                _Matrix = value;
                _Quaternion = Quaternion.FromMatrix(value);
                isEulerDirty = true;
                isMatrixDirty = false;
            }
        }

        public Rotation(Quaternion quaternion)
        {
            _Quaternion = quaternion.Normalized();
            _Matrix = Matrix3.CreateFromQuaternion(_Quaternion);
            _EulerAngles = Vector3.Zero;
            isEulerDirty = true;
            isMatrixDirty = false;
        }

        public Rotation(Vector3 eulerAngles)
        {
            _EulerAngles = NormalizeAngles(eulerAngles);
            _Quaternion = GLMath.EulerToQuat(_EulerAngles.ToRadians());
            _Matrix = Matrix3.Identity;
            isMatrixDirty = true;
            isEulerDirty = false;
        }

        public Rotation(Matrix3 matrix)
        {
            _EulerAngles = Vector3.Zero;
            _Matrix = matrix;
            _Quaternion = Quaternion.FromMatrix(matrix);
            isEulerDirty = true;
            isMatrixDirty = false;
        }

        public Rotation(Matrix4 matrix)
        {

            _EulerAngles = Vector3.Zero;
            _Matrix = new Matrix3(matrix);
            _Quaternion = Quaternion.FromMatrix(_Matrix);
            isEulerDirty = true;
            isMatrixDirty = false;
        }

        public Rotation(Angle pitch, Angle yaw, Angle roll)
            : this(new Vector3(pitch.Degrees, yaw.Degrees, roll.Degrees)) { }

        #region Static Ctors

        public static Rotation FromMatrix4(Matrix4 mat)
        {
            var newF = Vector3.TransformVector(Vector3.UnitZ, mat);
            newF.NormalizeFast();
            var newU = Vector3.TransformVector(Vector3.UnitY, mat);
            newU.NormalizeFast();
            return FromDirection(newF, newU);
        }

        public static Rotation FromDirection(Vector3 dir)
        {
            return FromDirection(dir, Vector3.UnitY);
        }

        public static Rotation FromDirection(Vector3 dir, Vector3 up)
        {
            if (dir == Vector3.UnitY)
                up = Vector3.UnitZ * -1f;
            else if (dir == Vector3.UnitY * -1f)
                up = Vector3.UnitZ;
            return new Rotation(Matrix4.LookAt(dir, Vector3.Zero, up));
        }

        public static Rotation LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            var dir = (target - eye).Normalized();
            if (dir == Vector3.UnitY)
                up = Vector3.UnitZ * -1f;
            else if (dir == Vector3.UnitY * -1f)
                up = Vector3.UnitZ;
            return FromDirection(dir, up);
        }

        public static Rotation FromAxisAngle(Vector3 axis, Angle angle)
        {
            return FromMatrix4(Matrix4.CreateFromAxisAngle(axis, angle.Radians));
        }

        #endregion

        #region Convertion operators

        public static implicit operator Rotation(Quaternion quat)
        {
            return new Rotation(quat);
        }

        public static implicit operator Rotation(Vector3 euler)
        {
            return new Rotation(euler);
        }

        public static implicit operator Rotation(Matrix3 mat)
        {
            return new Rotation(mat);
        }

        public static implicit operator Rotation(Matrix4 mat)
        {
            return FromMatrix4(mat);
        }

        public static explicit operator Quaternion(Rotation rot)
        {
            return rot.Quaternion;
        }

        public static explicit operator Matrix3(Rotation rot)
        {
            return rot.Matrix;
        }

        public static explicit operator Matrix4(Rotation rot)
        {
            return Matrix4.CreateFromQuaternion(rot.Quaternion);
        }

        #endregion

        public static Rotation Identity
        {
            get { return new Rotation(Quaternion.Identity); }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Pitch, Yaw, Roll);
        }

        private void NormalizeEulers()
        {
            _EulerAngles.X = Angle.NormalizeDegrees(_EulerAngles.X);
            _EulerAngles.Y = Angle.NormalizeDegrees(_EulerAngles.Y);
            _EulerAngles.Z = Angle.NormalizeDegrees(_EulerAngles.Z);
        }

        private static Vector3 NormalizeAngles(Vector3 angles)
        {
            angles.X = Angle.NormalizeDegrees(angles.X);
            angles.Y = Angle.NormalizeDegrees(angles.Y);
            angles.Z = Angle.NormalizeDegrees(angles.Z);
            return angles;
        }

        public static Rotation Slerp(Rotation rot1, Rotation rot2, float blend)
        {
            return Quaternion.Slerp(rot1.Quaternion, rot2.Quaternion, blend);
        }
    }
}
