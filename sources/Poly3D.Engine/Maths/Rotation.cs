using OpenTK;

using System;

namespace Poly3D.Maths
{
    public class Rotation
    {
        private Quaternion _Quaternion;
        private Vector3 _EulerAngles;
        private bool isEulerDirty;

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

        public Vector3 EulerAngles
        {
            get
            {
                if (isEulerDirty)
                {
                    _EulerAngles = (GLMath.EulerAnglesFromQuaternion(_Quaternion) * GLMath.TO_DEG);
                    NormalizeEulers();
                    isEulerDirty = false;
                }
                return _EulerAngles;
            }
            set
            {
                
                _EulerAngles = value;
                NormalizeEulers();
                _Quaternion = GLMath.QuaternionFromEulerAngles(_EulerAngles * GLMath.TO_RAD);
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
                isEulerDirty = true;
            }
        }

        public Rotation()
        {
            _Quaternion = Quaternion.Identity;
            _EulerAngles = Vector3.Zero;
            isEulerDirty = false;
        }

        public Rotation(Quaternion quaternion)
        {
            _Quaternion = quaternion;
            _EulerAngles = Vector3.Zero;
            isEulerDirty = Quaternion.Identity != quaternion;
        }


        public Rotation(Vector3 eulerAngles)
        {
            _EulerAngles = eulerAngles;
            _Quaternion = GLMath.QuaternionFromEulerAngles(_EulerAngles * GLMath.TO_RAD);
            isEulerDirty = false;
        }

        public Rotation(Angle pitch, Angle yaw, Angle roll)
            : this(new Vector3(pitch.Degrees, yaw.Degrees, roll.Degrees)) { }


        public static implicit operator Rotation(Quaternion quat)
        {
            return new Rotation(quat);
        }

        public static implicit operator Rotation(Vector3 euler)
        {
            return new Rotation(euler);
        }

        public static explicit operator Quaternion(Rotation rot)
        {
            return rot.Quaternion;
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
