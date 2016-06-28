using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poly3D.Engine.Maths;

namespace Poly3D.Engine
{
    public class Transform
    {
        private Quaternion _Rotation;//local
        private Vector3 _Scale;//local
        private Vector3 _Position;//local

        /// <summary>
        /// The blue axis of the transform in world space. (Z Axis)
        /// </summary>
        public Vector3 Forward
        {
            get { return Rotation.Mult(Vector3.UnitZ); }
            //set
            //{
            //}
        }

        /// <summary>
        /// The green axis of the transform in world space. (Y Axis)
        /// </summary>
        public Vector3 Up
        {
            get { return Rotation.Mult(Vector3.UnitY); }
            //set
            //{
            //}
        }

        /// <summary>
        /// The red axis of the transform in world space. (X Axis)
        /// </summary>
        public Vector3 Right
        {
            get { return Rotation.Mult(Vector3.UnitX); }
            //set
            //{
            //}
        }

        /// <summary>
        /// The rotation of the transform in world space stored as a Quaternion.
        /// </summary>
        public Quaternion Rotation
        {
            get { return _Rotation; }
            set
            {
                _Rotation = value;
            }
        }

        public Vector3 EulerAngles
        {
            get { return GLMath.EulerAnglesFromQuaternion(Rotation) * GLMath.TO_DEG; }
            set
            {
                Rotation = GLMath.QuaternionFromEulerAngles(value * GLMath.TO_RAD);
            }
        }

        public Vector3 Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
            }
        }

        public Vector3 Scale
        {
            get { return _Scale; }
            set
            {
                _Scale = value;
            }
        }

        public Transform()
        {
            _Rotation = Quaternion.Identity;
            _Scale = Vector3.One;
            _Position = Vector3.Zero;
        }
    }
}
