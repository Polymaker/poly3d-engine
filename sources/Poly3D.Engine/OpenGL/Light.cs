using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.OpenGL
{
    public class Light
    {
        private float _LinearAttenuation;
        private float _SpotCutoff;
        private float _SpotExponent;
        private Vector3 _SpotDirection;
        private Color4 _Diffuse;
        private Color4 _Specular;
        private Color4 _Ambient;
        private Vector4 _Position;
        private readonly int _Id;

        public int Id
        {
            get { return _Id; }
        }

        public bool Active
        {
            get
            {
                if (!InValidContext)
                    return false;

                return GL.IsEnabled(GetEnableCap());
            }
            set
            {
                if (!InValidContext)
                    return;

                if (value)
                {
                    GL.Enable(GetEnableCap());
                    ApplyAll();
                }
                else
                {
                    GL.Disable(GetEnableCap());
                }
            }
        }

        public Vector4 Position
        {
            get { return _Position; }
            set
            {
                if (value != _Position)
                {
                    _Position = value;
                    Apply(LightParameter.Position);
                }
            }
        }

        public Color4 Ambient
        {
            get { return _Ambient; }
            set
            {
                if (value != _Ambient)
                {
                    _Ambient = value;
                    Apply(LightParameter.Ambient);
                }
            }
        }

        public Color4 Diffuse
        {
            get { return _Diffuse; }
            set
            {
                if (value != _Diffuse)
                {
                    _Diffuse = value;
                    Apply(LightParameter.Diffuse);
                }
            }
        }

        public Color4 Specular
        {
            get { return _Specular; }
            set
            {
                if (value != _Specular)
                {
                    _Specular = value;
                    Apply(LightParameter.Specular);
                }
            }
        }

        public Vector3 SpotDirection
        {
            get { return _SpotDirection; }
            set
            {
                if (value != _SpotDirection)
                {
                    _SpotDirection = value;
                    Apply(LightParameter.SpotDirection);
                }
            }
        }

        public float SpotExponent
        {
            get { return _SpotExponent; }
            set
            {
                value = value.Clamp(0, 128);
                if (value != _SpotExponent)
                {
                    _SpotExponent = value;
                    Apply(LightParameter.SpotExponent);
                }
            }
        }

        public float SpotCutoff
        {
            get { return _SpotCutoff; }
            set
            {
                value = value == 180f ? value : value.Clamp(0, 90);
                if (value != _SpotCutoff)
                {
                    _SpotCutoff = value;
                    Apply(LightParameter.SpotExponent);
                }
            }
        }

        public float LinearAttenuation
        {
            get { return _LinearAttenuation; }
            set
            {
                value = value > 0 ? value : 0f;
                if (value != _LinearAttenuation)
                {
                    _LinearAttenuation = value;
                    Apply(LightParameter.LinearAttenuation);
                }
            }
        }
        

        public Light(int id)
        {
            _Id = id;
            _Ambient = new Color4(0, 0, 0, 1f);
            _Diffuse = new Color4(1f, 1f, 1f, 1f);
            _Specular = new Color4(1f, 1f, 1f, 1f);
            _Position = new Vector4(0, 0, 1f, 0);
            _SpotDirection = new Vector3(0, 0, -1);
            _SpotExponent = 0f;
            _SpotCutoff = 180f;
        }

        private void ApplyAll()
        {
            foreach (LightParameter lParam in Enum.GetValues(typeof(LightParameter)))
                Apply(lParam);
        }

        private void Apply(LightParameter lParam)
        {
            if (!InValidContext)
                return;
            var lName = GetLightName();
            switch (lParam)
            {

                case LightParameter.Ambient:
                    GL.Light(lName, lParam, Ambient);
                    break;
                case LightParameter.Diffuse:
                    GL.Light(lName, lParam, Diffuse);
                    break;
                case LightParameter.Specular:
                    GL.Light(lName, lParam, Specular);
                    break;
                case LightParameter.Position:
                    GL.Light(lName, lParam, new float[] { Position.X, Position.Y, Position.Z, Position.W });
                    break;
                case LightParameter.SpotDirection:
                    GL.Light(lName,lParam, new float[] { SpotDirection.X, SpotDirection.Y, SpotDirection.Z });
                    break;
                case LightParameter.SpotExponent:
                    GL.Light(lName, lParam, SpotExponent);
                    break;
                case LightParameter.SpotCutoff:
                    GL.Light(lName, lParam, SpotCutoff);
                    break;
                case LightParameter.ConstantAttenuation:
                    break;
                case LightParameter.LinearAttenuation:
                    GL.Light(lName, lParam, LinearAttenuation);
                    break;
                case LightParameter.QuadraticAttenuation:
                    break;
            }
        }

        private void Load(LightParameter lParam)
        {
            if (!InValidContext)
                return;
            var lName = GetLightName();
            switch (lParam)
            {
                case LightParameter.Ambient:
                    _Ambient = GetLightColor(lName, lParam);
                    break;
                case LightParameter.Diffuse:
                    _Diffuse = GetLightColor(lName, lParam);
                    break;
                case LightParameter.Specular:
                    _Specular = GetLightColor(lName, lParam);
                    break;
                case LightParameter.Position:
                    _Position = GetLightV4(lName, lParam);
                    break;
                case LightParameter.SpotDirection:
                    _SpotDirection = GetLightV3(lName, lParam);
                    break;
                case LightParameter.SpotExponent:
                    GL.GetLight(lName, lParam, out _SpotExponent);
                    break;
                case LightParameter.SpotCutoff:
                    GL.GetLight(lName, lParam, out _SpotCutoff);
                    break;
                case LightParameter.ConstantAttenuation:
                    break;
                case LightParameter.LinearAttenuation:
                    break;
                case LightParameter.QuadraticAttenuation:
                    break;
            }
        }

        private static Vector3 GetLightV3(LightName lName, LightParameter lParam)
        {
            var floatArray = new float[3];
            GL.GetLight(lName, lParam, floatArray);
            return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
        }

        private static Vector4 GetLightV4(LightName lName, LightParameter lParam)
        {
            var floatArray = new float[4];
            GL.GetLight(lName, lParam, floatArray);
            return new Vector4(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
        }

        private static Color4 GetLightColor(LightName lName, LightParameter lParam)
        {
            var floatArray = GetLightV4(lName, lParam);
            return new Color4(floatArray.X, floatArray.Y, floatArray.Z, floatArray.W);
        }

        private EnableCap GetEnableCap()
        {
            return (EnableCap)Enum.Parse(typeof(EnableCap), "Light" + Id);
        }

        private LightName GetLightName()
        {
            return (LightName)Enum.Parse(typeof(LightName), "Light" + Id);
        }

        public static bool LightingEnabled
        {
            get
            {
                if (!InValidContext)
                    return false;
                return GL.IsEnabled(EnableCap.Lighting);
            }
            set
            {
                if (!InValidContext)
                    return;

                if (value)
                    GL.Enable(EnableCap.Lighting);
                else
                    GL.Disable(EnableCap.Lighting);
            }
        }

        private static bool InValidContext
        {
            get { return GraphicsContext.CurrentContext != null; }
        }
    }
}
