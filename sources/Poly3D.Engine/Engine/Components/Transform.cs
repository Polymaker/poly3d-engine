
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using Poly3D.Maths;
using System.Diagnostics;

namespace Poly3D.Engine
{
    public sealed class Transform : ObjectComponent
    {
        private Rotation _Rotation;//local
        private Vector3 _Scale;//local
        private Vector3 _Position;//local
        private bool[] MatricesStates;
        private Matrix4[] Matrices;

        /// <summary>
        /// The blue axis of the transform in world space. (Z Axis)
        /// </summary>
        public Vector3 Forward
        {
            get { return GetDirection(Vector3.UnitZ); }
        }

        /// <summary>
        /// The green axis of the transform in world space. (Y Axis)
        /// </summary>
        public Vector3 Up
        {
            get { return GetDirection(Vector3.UnitY); }
        }

        /// <summary>
        /// The red axis of the transform in world space. (X Axis)
        /// </summary>
        public Vector3 Right
        {
            get { return GetDirection(Vector3.UnitX); }
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
                InvalidateMatrix(MatrixType.Local);
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
                InvalidateMatrix(MatrixType.Local);
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
                InvalidateMatrix(MatrixType.Local);
            }
        }

        private Matrix4 ParentMatrix
        {
            get { return GetMatrix(MatrixType.Parent); }
        }

        /// <summary>
        /// Get a matrix that transforms a point from local space into world space.
        /// AKA the final transformation matrix.
        /// </summary>
        public Matrix4 LocalToWorldMatrix
        {
            get
            {
                return GetMatrix(MatrixType.Final);
            }
        }

        public Matrix4 WorldToLocalMatrix
        {
            get
            {
                return LocalToWorldMatrix.Inverted();
            }
        }

        public Matrix4 LocalTransform
        {
            get { return GetMatrix(MatrixType.Local); }
        }

        /// <summary>
        /// Gets or sets the position of the transform in world space.
        /// </summary>
        public Vector3 WorldPosition
        {
            get
            {
                return Vector3.Transform(Position, ParentMatrix);
            }
            set
            {
                Position = Vector3.Transform(value, ParentMatrix.Inverted());
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the transform in world space.
        /// </summary>
        public Rotation WorldRotation
        {
            get
            {
                return LocalToWorldMatrix.GetRotation();
            }
            set
            {
                var baseRotation = ParentMatrix.ExtractRotation().Inverted();
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
                return Vector3.Multiply(ParentMatrix.ExtractScale(), Scale);
            }
            set
            {
                if (value == Vector3.Zero || value.X == 0f || value.Y == 0f || value.Z == 0f)
                    return;
                Scale = Vector3.Divide(value, ParentMatrix.ExtractScale());
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
            _Scale = Vector3.One;
            _Position = Vector3.Zero;
            InitMatrices();
        }

        public Transform(Vector3 position, Rotation rotation, Vector3 scale)
        {
            _Rotation = rotation;
            _Scale = scale;
            _Position = position;
            InitMatrices();
        }

        protected override void Initialize()
        {
            base.Initialize();
            InvalidateMatrix(MatrixType.Local);
            InvalidateMatrix(MatrixType.Parent);
            EngineObject.ParentChanged += EngineObject_ParentChanged;
        }

        private void EngineObject_ParentChanged(object sender, EventArgs e)
        {
            InvalidateMatrix(MatrixType.Parent);
        }

        #region Matrices

        internal enum MatrixType
        {
            Local = 0,
            Final = 1,
            Parent = 2//parent is second, because we store only local & final, parent = parent.final
        }

        private void InitMatrices()
        {
            MatricesStates = new bool[3];
            Matrices = new Matrix4[2];
            for (int i = 0; i < 3; i++)
            {
                MatricesStates[i] = true;
                if (i < 2)
                    Matrices[i] = Matrix4.Identity;
            }
        }

        internal void InvalidateMatrix(MatrixType type)
        {
            MatricesStates[(int)type] = true;
        }

        private bool GetMatrixState(MatrixType type)//if true = dirty/invalidated
        {
            if (MatricesStates[(int)type])
                return true;

            if (type == MatrixType.Parent)
            {
                if (ParentTransform != null)
                    MatricesStates[(int)type] = ParentTransform.GetMatrixState(MatrixType.Final);
            }
            else if (type == MatrixType.Final)
            {
                MatricesStates[(int)type] = GetMatrixState(MatrixType.Local) || GetMatrixState(MatrixType.Parent);
            }

            return MatricesStates[(int)type];
        }

        internal Matrix4 GetMatrix(MatrixType type)
        {
            if (EngineObject == null)
                return Matrix4.Identity;

            if (type == MatrixType.Parent)
            {
                return ParentTransform != null ? ParentTransform.GetMatrix(MatrixType.Final) : Matrix4.Identity;
            }

            if (!GetMatrixState(type))
                return Matrices[(int)type];

            var baseMatrix = Matrix4.Identity;

            switch (type)
            {
                case MatrixType.Local:
                    baseMatrix = Matrix4.Mult(baseMatrix, Matrix4.CreateScale(Scale));
                    baseMatrix = Matrix4.Mult(baseMatrix, (Matrix4)Rotation);
                    
                    baseMatrix = Matrix4.Mult(baseMatrix, Matrix4.CreateTranslation(Position));
                    break;

                case MatrixType.Final:
                    baseMatrix = Matrix4.Mult(GetMatrix(MatrixType.Local), GetMatrix(MatrixType.Parent));
                    break;
            }

            MatricesStates[(int)type] = false;

            return (Matrices[(int)type] = baseMatrix);
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
                var realRot = LocalToWorldMatrix.GetRotation();
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

        //public Vector3 ToLocalSpace(Vector3 worldPosition)
        //{
        //    return Vector3.Transform(worldPosition, LocalToWorldMatrix.Inverted());
        //}

        #endregion

        private Vector3 GetDirection(Vector3 direction)
        {
            var newDir = Vector3.TransformVector(direction, GetMatrix(MatrixType.Final));
            newDir.NormalizeFast();
            return newDir;
        }

        public override IEngineComponent Clone()
        {
            return new Transform(Position, Rotation, Scale);
        }

    }
}
