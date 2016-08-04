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
            get { return Vector3.Transform(Vector3.UnitZ, WorldRotation.Quaternion); }
        }

        /// <summary>
        /// The green axis of the transform in world space. (Y Axis)
        /// </summary>
        public Vector3 Up
        {
            get { return Vector3.Transform(Vector3.UnitY, WorldRotation.Quaternion); }
        }

        /// <summary>
        /// The red axis of the transform in world space. (X Axis)
        /// </summary>
        public Vector3 Right
        {
            get { return Vector3.Transform(Vector3.UnitX, WorldRotation.Quaternion); }
        }

        /// <summary>
        /// Gets or sets the position of the transform in local space.
        /// </summary>
        public Vector3 Position
        {
            get { return _Position; }
            set
            {
                if (_Position == value)
                    return;
                _Position = value;
                NotifyParentChanged();
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
                //if (_Rotation == value)
                //    return;
                _Rotation = value;
                NotifyParentChanged();
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
                if (value == Vector3.Zero || value.X == 0f || value.Y == 0f || value.Z == 0f)
                    return;
                if (_Scale == value)
                    return;
                _Scale = value;
                NotifyParentChanged();
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
                var baseRotation = LocalToWorldMatrix.ExtractRotation();
                return Quaternion.Multiply(Rotation.Quaternion, baseRotation);
            }
            set
            {
                var baseRotation = LocalToWorldMatrix.ExtractRotation();
                _Rotation = Quaternion.Multiply(value.Quaternion, baseRotation.Inverted());
            }
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
                if (value == Vector3.Zero || value.X == 0f || value.Y == 0f || value.Z == 0f)
                    return;
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

        public Transform(Vector3 position, Rotation rotation, Vector3 scale)
        {
            _Rotation = rotation;
            _Scale = scale;
            _Position = position;
            isWorldMatrixDirty = true;
            _LocalToWorldMatrix = Matrix4.Identity;
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

        #region Matrices

        private void NotifyParentChanged()
        {
            if (SceneObject != null && SceneObject.Childs.Count > 0)
            {
                foreach (var childObj in SceneObject.AllChilds)
                    childObj.Transform.isWorldMatrixDirty = true;
            }
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
            return Matrix4.Mult(GetLocalTransformMatrix(), LocalToWorldMatrix);
            //return Matrix4.Mult(LocalToWorldMatrix, GetLocalTransformMatrix());
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

        #endregion

        #region Transform operations

        public void LookAt(Transform target)
        {
            WorldRotation = Rotation.FromDirection((target.WorldPosition - WorldPosition).Normalized());
        }

        public void LookAt(Vector3 target, bool targetInLocalSpace = false)
        {
            if (targetInLocalSpace)
                Rotation = Rotation.FromDirection((target - Position).Normalized());
            else
                WorldRotation = Rotation.FromDirection((target - WorldPosition).Normalized());
        }

        public void Translate(Vector3 translation)
        {
            Translate(translation, Space.Parent);
        }

        public void Translate(Vector3 translation, Space relativeTo)
        {
            if (relativeTo == Space.World || (relativeTo == Space.Parent && ParentTransform == null))
            {
                WorldPosition += translation;
            }
            else if (relativeTo == Space.Parent)
            {
                Position += translation;
            }
            else if (relativeTo == Space.Self)//the difference between Parent and Self is that self include local rotation
            {
                Position += Vector3.Transform(translation, Rotation.Quaternion);
            }
        }

        public void Rotate(Rotation rotation)
        {
            Rotate(rotation, Space.Self);
        }

        public void Rotate(Rotation rotation, Space relativeTo)
        {
            if (relativeTo == Space.World || (relativeTo == Space.Parent && ParentTransform == null))
            {
                
                WorldRotation = Quaternion.Multiply(rotation.Quaternion, WorldRotation.Quaternion);
            }
            else if (relativeTo == Space.Parent || relativeTo == Space.Self)
            {
                Rotation = Quaternion.Multiply(rotation.Quaternion, Rotation.Quaternion);
            }
        }

        public Vector3 ToLocalSpace(Vector3 worldPosition)
        {
            return Vector3.Transform(worldPosition, LocalToWorldMatrix.Inverted());
        }

        #endregion

        public Transform Clone()
        {
            return new Transform(Position, Rotation, Scale);
        }
    }
}
