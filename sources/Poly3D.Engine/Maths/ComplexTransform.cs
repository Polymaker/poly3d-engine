using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace Poly3D.Maths
{
    public struct ComplexTransform
    {
        // Fields...
        private Vector3 _Scale;
        private Vector3 _Translation;
        private Rotation _Rotation;
        private bool isDirty;
        private Matrix4 _TransformMatrix;

        public Rotation Rotation
        {
            get { return _Rotation; }
            set
            {
                if (_Rotation.Quaternion == value.Quaternion)
                    return;
                _Rotation = value;
                isDirty = true;
            }
        }

        public Vector3 Translation
        {
            get { return _Translation; }
            set
            {
                if (_Translation == value)
                    return;
                _Translation = value;
                isDirty = true;
            }
        }

        public Vector3 Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale == value)
                    return;
                _Scale = value;
                isDirty = true;
            }
        } 
        
        public Matrix4 TransformMatrix
        {
            get
            {
                return GetTransformMatrix();
            }
            set
            {
                SetTransformMatrix(value);
            }
        }

        public static implicit operator ComplexTransform(Matrix4 transformMatrix)
        {
            return FromMatrix(transformMatrix);
        }

        public static ComplexTransform FromMatrix(Matrix4 transformMatrix)
        {
            var trans = new ComplexTransform();
            trans.SetTransformMatrix(transformMatrix);
            return trans;
        }

        public Matrix4 GetTransformMatrix()
        {
            if (_TransformMatrix == default(Matrix4))
                _TransformMatrix = Matrix4.Identity;

            if (isDirty)
            {
                _TransformMatrix = Matrix4.Identity;
                _TransformMatrix = Matrix4.Mult(_TransformMatrix, Matrix4.CreateScale(Scale));
                _TransformMatrix = Matrix4.Mult(_TransformMatrix, (Matrix4)Rotation);
                _TransformMatrix = Matrix4.Mult(_TransformMatrix, Matrix4.CreateTranslation(Translation));
                isDirty = false;
            }
            return _TransformMatrix;
        }
        
        private void SetTransformMatrix(Matrix4 matrix)
        {
            _TransformMatrix = matrix;
            _Rotation = matrix.ExtractRotation();
            _Scale = matrix.ExtractScale();
            _Translation = matrix.ExtractTranslation();
            isDirty = false;
        }
    }
}
