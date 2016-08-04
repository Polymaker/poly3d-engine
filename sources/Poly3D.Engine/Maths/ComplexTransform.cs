using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Maths
{
    public struct ComplexTransform
    {
        // Fields...
        private Vector3 _Scale;
        private Vector3 _Translation;
        private Rotation _Rotation;

        public Rotation Rotation
        {
            get { return _Rotation; }
            set
            {
                _Rotation = value;
            }
        }

        public Vector3 Translation
        {
            get { return _Translation; }
            set
            {
                _Translation = value;
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

        public Matrix4 GetTransformMatrix()
        {
            var finalMat = Matrix4.Identity;

            finalMat = Matrix4.Mult(finalMat, Matrix4.CreateScale(Scale));
            finalMat = Matrix4.Mult(finalMat, (Matrix4)Rotation);
            finalMat = Matrix4.Mult(finalMat, Matrix4.CreateTranslation(Translation));

            return finalMat;
        }

        private void SetTransformMatrix(Matrix4 matrix)
        {
            _Rotation = matrix.ExtractRotation();
            _Scale = matrix.ExtractScale();
            _Translation = matrix.ExtractTranslation();
        }
    }
}
