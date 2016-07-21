using OpenTK;

using System;

namespace Poly3D.Maths
{
    public class Rotation
    {
        private Quaternion _Quaternion;
        private Vector3 _EulerAngles;
        private bool isEulerDirty;

        public Angle Yaw
        {
            get { return Angle.FromDegrees(EulerAngles.X); }
            set
            {
                EulerAngles = new Vector3(value.Degrees, EulerAngles.Y, EulerAngles.Z);
            }
        }

        public Angle Pitch
        {
            get { return Angle.FromDegrees(EulerAngles.Y); }
            set
            {
                EulerAngles = new Vector3(EulerAngles.X, value.Degrees, EulerAngles.Z);
            }
        }

        public Angle Roll
        {
            get { return Angle.FromDegrees(EulerAngles.Z); }
            set
            {
                EulerAngles = new Vector3(EulerAngles.X, EulerAngles.Y, value.Degrees);
            }
        }

        public Vector3 EulerAngles
        {
            get
            {
                if (isEulerDirty)
                {
                    _EulerAngles = GLMath.EulerAnglesFromQuaternion(_Quaternion) * GLMath.TO_DEG;
                    isEulerDirty = false;
                }
                return _EulerAngles;
            }
            set
            {
                _Quaternion = GLMath.QuaternionFromEulerAngles(value * GLMath.TO_RAD);
                _EulerAngles = value;
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
            _Quaternion = GLMath.QuaternionFromEulerAngles(eulerAngles * GLMath.TO_RAD);
            isEulerDirty = false;
        }

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
    }
}
