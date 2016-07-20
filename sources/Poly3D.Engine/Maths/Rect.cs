using System;
using OpenTK;

namespace Poly3D.Maths
{
    public struct Rect
    {
        private float _X;
        private float _Y;
        private float _Width;
        private float _Height;

        #region Properties

        public float X
        {
            get { return _X; }
            set
            {
                _X = value;
            }
        }

        public float Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return new Vector2(_X, _Y);
            }
            set
            {
                _X = value.X;
                _Y = value.Y;
            }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(X + _Width / 2f, Y + _Height / 2f);
            }
            set
            {
                _X = value.X - _Width / 2f;
                _Y = value.Y - _Height / 2f;
            }
        }

        public Vector2 Min
        {
            get
            {
                return new Vector2(Left, Top);
            }
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Vector2 Max
        {
            get
            {
                return new Vector2(Right, Bottom);
            }
            set
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }

        public float Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
            }
        }

        public float Height
        {
            get { return _Height; }
            set
            {
                _Height = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(_Width, _Height);
            }
            set
            {
                _Width = value.X;
                _Height = value.Y;
            }
        }

        public float Left
        {
            get { return _X; }
            set
            {
                float xMax = Right;
                _X = value;
                _Width = xMax - _X;
            }
        }

        public float Top
        {
            get { return _Y; }
            set
            {
                float yMax = Bottom;
                _Y = value;
                _Height = yMax - _Y;
            }
        }

        public float Right
        {
            get { return _Width + _X; }
            set
            {
                _Width = value - _X;
            }
        }

        public float Bottom
        {
            get { return _Height + _Y; }
            set
            {
                _Height = value - _Y;
            }
        }


        #endregion

        #region Ctors

        public Rect(float x, float y, float width, float height)
        {
            _X = x;
            _Y = y;
            _Width = width;
            _Height = height;
        }

        public Rect(Vector2 position, Vector2 size)
        {
            _X = position.X;
            _Y = position.Y;
            _Width = size.X;
            _Height = size.Y;
        }

        public Rect(Rect source)
        {
            _X = source._X;
            _Y = source._Y;
            _Width = source._Width;
            _Height = source._Height;
        }

        #endregion

        #region Equal

        public override int GetHashCode()
        {
            return _X.GetHashCode() ^ _Width.GetHashCode() << 2 ^ _Y.GetHashCode() >> 2 ^ _Height.GetHashCode() >> 1;
        }

        public override bool Equals(object other)
        {
            if (!(other is Rect))
            {
                return false;
            }
            Rect rect = (Rect)other;
            return _X.Equals(rect.X) && _Y.Equals(rect.Y) && _Width.Equals(rect.Width) && _Height.Equals(rect.Height);
        }

        public static bool operator !=(Rect lhs, Rect rhs)
        {
            return lhs.X != rhs.X || lhs.Y != rhs.Y || lhs.Width != rhs.Width || lhs.Height != rhs.Height;
        }

        public static bool operator ==(Rect lhs, Rect rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Width == rhs.Width && lhs.Height == rhs.Height;
        }

        #endregion

        public static implicit operator Rect(System.Drawing.Rectangle rec)
        {
            return new Rect(rec.X, rec.Y, rec.Width, rec.Height);
        }

        public static implicit operator Rect(System.Drawing.RectangleF rec)
        {
            return new Rect(rec.X, rec.Y, rec.Width, rec.Height);
        }
    }
}
