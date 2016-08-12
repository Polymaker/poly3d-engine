
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using Poly3D.Maths;
using System.Diagnostics;

namespace Poly3D.Engine
{
    //***********************
    // Except for the Position/Rotation/Scale properties (and _LocalMatrix field), 'Local' refers to the tranform's (SceneObject's) origin.
    // In other words, the LocalToWorldMatrix and WorldToLocalMatrix excludes local transformations.
    // I compute world/final values by adding the local tranforms to its parent. 
    //***********************
    public sealed class Transform : ObjectComponent
    {
        private Rotation _Rotation;//local
        private Vector3 _Scale;//local
        private Vector3 _Position;//local
        internal bool isWorldMatrixDirty;
        private Matrix4 _LocalToWorldMatrix;
        private Matrix4 _LocalMatrix;
        private bool isLocalMatrixDirty;

        /// <summary>
        /// The blue axis of the transform in world space. (Z Axis)
        /// </summary>
        public Vector3 Forward
        {
            get { return Vector3.TransformVector(Vector3.UnitZ, (Matrix4)WorldRotation); }
        }

        /// <summary>
        /// The green axis of the transform in world space. (Y Axis)
        /// </summary>
        public Vector3 Up
        {
            get { return Vector3.TransformVector(Vector3.UnitY, (Matrix4)WorldRotation); }
        }

        /// <summary>
        /// The red axis of the transform in world space. (X Axis)
        /// </summary>
        public Vector3 Right
        {
            get { return Vector3.TransformVector(Vector3.UnitX, (Matrix4)WorldRotation); }
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
                isLocalMatrixDirty = true;
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
                if (_Rotation.Quaternion == value.Quaternion)
                    return;
                _Rotation = value;
                isLocalMatrixDirty = true;
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
                isLocalMatrixDirty = true;
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
                return GetLocalToWorldMatrix();
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
                Position = Vector3.Transform(value, LocalToWorldMatrix.Inverted());
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the transform in world space.
        /// </summary>
        public Rotation WorldRotation
        {
            get
            {
                return GetTransformMatrix().GetRotation();
            }
            set
            {
                var baseRotation = LocalToWorldMatrix.ExtractRotation().Inverted();
                Rotation = Quaternion.Multiply(baseRotation, value.Quaternion);
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
                Scale = Vector3.Divide(value, LocalToWorldMatrix.ExtractScale());
            }
        }

        private Vector3 WorldScaleInv
        {
            get { return Vector3.Divide(Vector3.One, WorldScale); }
        }

        public Matrix4 WorldToLocalMatrix
        {
            get
            {
                return GetTransformMatrix().Inverted();
            }
        }

        public Transform ParentTransform
        {
            get
            {
                if (EngineObject != null && EngineObject.Parent != null)
                    return EngineObject.Parent.Transform;
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
            isLocalMatrixDirty = true;
        }

        public Transform(Vector3 position, Rotation rotation, Vector3 scale)
        {
            _Rotation = rotation;
            _Scale = scale;
            _Position = position;
            isWorldMatrixDirty = true;
            isLocalMatrixDirty = true;
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
            isLocalMatrixDirty = true;
        }

        #region Matrices

        private void NotifyParentChanged()
        {
            if (EngineObject != null && EngineObject.Childs.Count > 0)
            {
                foreach (var childObj in EngineObject.AllChilds)
                    childObj.Transform.isWorldMatrixDirty = true;
            }
        }

        /// <summary>
        /// Returns a transformation matrix containing the local transformations (position/rotation/scale).
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetLocalTransformMatrix()
        {
            if (EngineObject == null)
                return Matrix4.Identity;

            if (!isLocalMatrixDirty)
                return _LocalMatrix;

            _LocalMatrix = Matrix4.Identity;

            _LocalMatrix = Matrix4.Mult(_LocalMatrix, Matrix4.CreateScale(Scale));
            _LocalMatrix = Matrix4.Mult(_LocalMatrix, (Matrix4)Rotation);
            _LocalMatrix = Matrix4.Mult(_LocalMatrix, Matrix4.CreateTranslation(Position));

            isLocalMatrixDirty = false;

            return _LocalMatrix;
        }

        /// <summary>
        /// Returns the final transformation matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetTransformMatrix()
        {
            return Matrix4.Mult(GetLocalTransformMatrix(), GetLocalToWorldMatrix());
        }

        /// <summary>
        /// Returns a transformation matrix 
        /// </summary>
        /// <returns></returns>
        private Matrix4 GetLocalToWorldMatrix()
        {
            if (EngineObject == null)
                return Matrix4.Identity;

            if (!isWorldMatrixDirty)
                return _LocalToWorldMatrix;

            _LocalToWorldMatrix = Matrix4.Identity;

            if (ParentTransform != null)
            {
                _LocalToWorldMatrix = Matrix4.Mult(
                    ParentTransform.GetLocalTransformMatrix(),
                    ParentTransform.GetLocalToWorldMatrix());
            }
            else
                _LocalToWorldMatrix = Matrix4.Identity;

            isWorldMatrixDirty = false;
            return _LocalToWorldMatrix;
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
                WorldRotation = Rotation.FromDirection((target - WorldPosition).Normalized(), Up);
        }

        public void LookAt(Vector3 target, bool targetInLocalSpace, Vector3 up)
        {
            if (targetInLocalSpace)
                Rotation = Rotation.FromDirection((target - Position).Normalized(), up);
            else
                WorldRotation = Rotation.FromDirection((target - WorldPosition).Normalized(), up);
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
                //var result = Quaternion.Multiply(WorldRotation.Quaternion, rotation.Quaternion);
                Rotation = Quaternion.Multiply(rotation.Quaternion, Rotation.Quaternion);
            }
            else if (relativeTo == Space.Parent)
            {
                Rotation = Quaternion.Multiply(rotation.Quaternion, Rotation.Quaternion);
            }
            else if (relativeTo == Space.Self)
            {
                var realRot = GetTransformMatrix().GetRotation();
                //var mat2 = Matrix4.Mult((Matrix4)rotation, GetTransformMatrix());
                WorldRotation = Quaternion.Multiply(realRot.Quaternion, rotation.Quaternion);
                //var currentQuat = GetTransformMatrix().ExtractRotation();
                //WorldRotation = Quaternion.Multiply(rotation.Quaternion, currentQuat);
            }
        }

        public void SetRotation(RotationComponent component, Angle value)
        {
            SetRotation(component, value, Space.Self);
        }

        public void SetRotation(RotationComponent component, Angle value, Space relativeTo)
        {
            var rotation = relativeTo == Space.World ? WorldRotation : Rotation;//boxing
            switch (component)
            {
                case RotationComponent.Pitch:
                    rotation.Pitch = value;
                    break;
                case RotationComponent.Yaw:
                    rotation.Yaw = value;
                    break;
                case RotationComponent.Roll:
                    rotation.Roll = value;
                    break;
            }

            if (relativeTo == Space.World)
                WorldRotation = rotation;
            else
                Rotation = rotation;
        }

        public Vector3 ToLocalSpace(Vector3 worldPosition)
        {
            return Vector3.Transform(worldPosition, LocalToWorldMatrix.Inverted());
        }

        #endregion

        public override IEngineComponent Clone()
        {
            return new Transform(Position, Rotation, Scale);
        }
    }
}
