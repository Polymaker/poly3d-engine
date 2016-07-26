using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using Poly3D.Maths;

namespace Poly3D.Engine
{
    public sealed class Transform : ObjectComponent
    {
        private Rotation _Rotation;//local
        private Vector3 _Scale;//local
        private Vector3 _Position;//local
        internal bool isWorldMatrixDirty;
        private Matrix4 _LocalToWorldMatrix;

        /// <summary>
        /// The blue axis of the transform in world space. (Z Axis)
        /// </summary>
        public Vector3 Forward
        {
            get 
            {
                return Vector3.Transform(Vector3.UnitZ, WorldRotation.Quaternion);
                //return WorldRotation.Quaternion.Mult(Vector3.UnitZ); 
            }
        }

        /// <summary>
        /// The green axis of the transform in world space. (Y Axis)
        /// </summary>
        public Vector3 Up
        {
            get { return WorldRotation.Quaternion.Mult(Vector3.UnitY); }
        }

        /// <summary>
        /// The red axis of the transform in world space. (X Axis)
        /// </summary>
        public Vector3 Right
        {
            get { return WorldRotation.Quaternion.Mult(Vector3.UnitX); }
        }

        /// <summary>
        /// Gets or sets the position of the transform in local space.
        /// </summary>
        public Vector3 Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the transform in local space.
        /// </summary>
        public Rotation Rotation
        {
            get { return _Rotation; }
            set
            {
                _Rotation = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale of the transform in local space.
        /// </summary>
        public Vector3 Scale
        {
            get { return _Scale; }
            set
            {
                _Scale = value;
            }
        }

        /// <summary>
        /// Get a matrix that transforms a point from local space into world space.
        /// </summary>
        public Matrix4 LocalToWorldMatrix
        {
            get
            {
                if (isWorldMatrixDirty)
                    BuildConvertionMatrices();
                return _LocalToWorldMatrix;
            }
        }

        /// <summary>
        /// Gets or sets the position of the transform in world space.
        /// </summary>
        public Vector3 WorldPosition
        {
            get
            {
                return Vector3.Transform(Position, LocalToWorldMatrix);
            }
            set
            {
                _Position = Vector3.Transform(value, LocalToWorldMatrix.Inverted());
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the transform in world space.
        /// </summary>
        public Rotation WorldRotation
        {
            get
            {
                var rot = LocalToWorldMatrix.ExtractRotation();
                return Quaternion.Multiply(Rotation.Quaternion, rot);
            }
            //set
            //{
            //    //_Position = 
            //}
        }

        /// <summary>
        /// Gets or sets the scale of the transform in world space.
        /// </summary>
        public Vector3 WorldScale
        {
            get
            {
                return Vector3.Multiply(LocalToWorldMatrix.ExtractScale(), Scale);
            }
            set
            {
                _Scale = Vector3.Divide(value, LocalToWorldMatrix.ExtractScale());
            }
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

        public void LookAt(Vector3 target, bool localPos = false)
        {
            //in opengl Z+ (forward) is away from camera, but our forward is toward camera so we need to invert Z;
            //var worldPos = WorldPosition;
            ////worldPos.Z *= -1f;
            ////target.Z *= -1f;
            //Rotation = Matrix4.LookAt(worldPos, target, Up).Inverted();
            //rotation.Invert();
            //Rotation = rotation;
            Rotation = Rotation.FromDirection((target - WorldPosition).Normalized());
        }

        public Matrix4 GetLocalTransformMatrix()
        {
            var finalMat = Matrix4.Identity;

            finalMat = Matrix4.Mult(finalMat, Matrix4.CreateScale(Scale));
            finalMat = Matrix4.Mult(finalMat, (Matrix4)Rotation);
            finalMat = Matrix4.Mult(finalMat, Matrix4.CreateTranslation(Position));
            
            return finalMat;
        }

        public Matrix4 GetTransformMatrix()
        {
            return Matrix4.Mult(LocalToWorldMatrix, GetLocalTransformMatrix());
        }

        public void BuildConvertionMatrices()
        {
            if (SceneObject == null)
                return;
            _LocalToWorldMatrix = Matrix4.Identity;

            foreach (var node in SceneObject.GetHierarchy(false).Reverse())
            {
                var transformMatrix = node.Transform.GetLocalTransformMatrix();
                _LocalToWorldMatrix = Matrix4.Mult(_LocalToWorldMatrix, transformMatrix);
            }

            isWorldMatrixDirty = false;
        }

        public Transform Clone()
        {
            return new Transform()
            {
                _Rotation = Rotation,
                _Position = Position,
                _Scale = Scale
            };
        }
    }
}
