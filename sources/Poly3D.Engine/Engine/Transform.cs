using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poly3D.Engine.Maths;

namespace Poly3D.Engine
{
    public sealed class Transform : ObjectComponent
    {
        private Quaternion _Rotation;//local
        private Vector3 _Scale;//local
        private Vector3 _Position;//local
        internal bool isWorldMatrixDirty;
        private Matrix4 _LocalToWorldMatrix;

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
        /// The rotation of the transform in local space stored as a Quaternion.
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

        public Matrix4 LocalToWorldMatrix
        {
            get
            {
                if (isWorldMatrixDirty)
                    BuildConvertionMatrices();
                return _LocalToWorldMatrix;
            }
        }

        public Vector3 WorldPosition
        {
            get
            {

                //return LocalToWorldMatrix.ExtractTranslation();

                return Vector3.Transform(Position, LocalToWorldMatrix);
            }
            set
            {
                _Position = Vector3.Transform(value, LocalToWorldMatrix.Inverted());
            }
        }

        public Quaternion WorldRotation
        {
            get
            {
                var rot = LocalToWorldMatrix.ExtractRotation();
                return Quaternion.Multiply(rot, Rotation.Inverted());
                //return Vector3.Multiply(LocalToWorldMatrix.ExtractScale(), Scale);
            }
            //set
            //{
            //    //_Position = 
            //}
        }

        public Vector3 WorldEulerAngles
        {
            get { return GLMath.EulerAnglesFromQuaternion(WorldRotation) * GLMath.TO_DEG; }
            set
            {
                //Rotation = GLMath.QuaternionFromEulerAngles(value * GLMath.TO_RAD);
            }
        }

        public Vector3 WorldScale
        {
            get
            {
                //return LocalToWorldMatrix.ExtractScale();
                return Vector3.Multiply(LocalToWorldMatrix.ExtractScale(), Scale);
            }
            //set
            //{
            //    //_Position = 
            //}
        }

        public Transform ParentTransform
        {
            get
            {
                if (SceneObject != null && SceneObject.Parent != null)
                    return SceneObject.Parent.Transform;
                return null;
            }
        }

        public Transform()
        {
            _Rotation = Quaternion.Identity;
            _LocalToWorldMatrix = Matrix4.Identity;
            _Scale = Vector3.One;
            _Position = Vector3.Zero;
            isWorldMatrixDirty = true;
        }

        internal Transform(SceneObject sceneObject)
            : base(sceneObject)
        {
            _Rotation = Quaternion.Identity;
            _LocalToWorldMatrix = Matrix4.Identity;
            _Scale = Vector3.One;
            _Position = Vector3.Zero;
            isWorldMatrixDirty = true;
        }

        public Matrix4 GetLocalMatrix()
        {
            var finalMat = Matrix4.Identity;
            
            finalMat = Matrix4.Mult(finalMat, Matrix4.CreateScale(Scale));
            finalMat = Matrix4.Mult(finalMat, Matrix4.CreateFromQuaternion(Rotation.Inverted()));
            finalMat = Matrix4.Mult(finalMat, Matrix4.CreateTranslation(Position));
            return finalMat;
        }

        public Matrix4 GetFinalMatrix()
        {
            return Matrix4.Mult(LocalToWorldMatrix, GetLocalMatrix());
        }

        public void BuildConvertionMatrices()
        {
            if (SceneObject == null)
                return;
            _LocalToWorldMatrix = Matrix4.Identity;
            foreach (var node in SceneObject.GetHierarchy(false).Reverse())
            {
                var transformMatrix = node.Transform.GetLocalMatrix();
                _LocalToWorldMatrix = Matrix4.Mult(_LocalToWorldMatrix, transformMatrix);
            }

            isWorldMatrixDirty = false;
        }
    }
}
