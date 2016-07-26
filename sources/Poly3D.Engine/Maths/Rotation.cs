using OpenTK;

using System;
using System.Diagnostics;

namespace Poly3D.Maths
{
    public class Rotation
    {
        private Quaternion _Quaternion;
        private Vector3 _EulerAngles;
        private Matrix3 _Matrix;
        private bool isEulerDirty;
        private bool isMatrixDirty;


        /// <summary>
        /// Gets or sets the rotation along the X axis.
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
        /// Gets or sets the rotation along the Y axis.
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
        /// Gets or sets the rotation along the Z axis.
        /// </summary>
        public Angle Roll
        {
            get { return Angle.FromDegrees(EulerAngles.Z); }
            set
            {
                EulerAngles = new Vector3(EulerAngles.X, EulerAngles.Y, value.Normalized().Degrees);
            }
        }


        /// <summary>
        /// Gets or sets the rotation angles. Vector3(Pitch, Yaw, Roll)
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

        public Rotation()
        {
            _Quaternion = Quaternion.Identity;
            _EulerAngles = Vector3.Zero;
            _Matrix = Matrix3.Identity;
            isEulerDirty = false;
            isMatrixDirty = false;
        }

        public Rotation(Quaternion quaternion)
        {
            _Quaternion = quaternion;
            _Matrix = Matrix3.CreateFromQuaternion(_Quaternion);
            _EulerAngles = Vector3.Zero;
            isEulerDirty = true;
            isMatrixDirty = false;
        }

        public Rotation(Vector3 eulerAngles)
        {
            _EulerAngles = eulerAngles;
            NormalizeEulers();
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

        public static Rotation FromDirection(Vector3 dir)
        {

            var rotation = Matrix4.LookAt(dir, Vector3.Zero, Vector3.UnitY);

            return rotation;
        }


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
            return new Rotation(mat);
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
            return new Matrix4(
                new Vector4(rot.Matrix.Row0),
                new Vector4(rot.Matrix.Row1),
                new Vector4(rot.Matrix.Row2),
                Vector4.UnitW
                );
        }

        public static readonly Rotation Identity = new Rotation(Quaternion.Identity);


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
    }
}
