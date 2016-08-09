using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using Poly3D.Utilities;

namespace Poly3D.Platform
{
    [System.ComponentModel.ToolboxItem(true)]
    public class EngineControl : GLControl, IGLSurface, IDisposable
    {
        private GLPlatforms _NativePlatform;
        private bool _Exists;

        #region Ctors

        public EngineControl()
            : base()
        {
            Initialize();
        }

        public EngineControl(GraphicsMode mode)
            : base(mode)
        {
            Initialize();
        }

        public EngineControl(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags)
            : base(mode, major, minor, flags)
        {
            Initialize();
        }

        private void Initialize()
        {
            DetectPlatform();
            _Exists = true;
        }

        #endregion

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _Exists = false;
            base.OnHandleDestroyed(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _Exists = true;
        }

        #region IGLSurface

        GLPlatforms IGLSurface.Platform
        {
            get { return _NativePlatform; }
        }

        protected object Implementation
        {
            get
            {
                return ReflectionHelper.GetField(typeof(GLControl), this, 
                    "implementation", 
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.NonPublic);
            }
        }

        IntPtr IGLSurface.Handle
        {
            get { return Handle; }
        }

        bool IGLSurface.Exists
        {
            get { return _Exists; }
        }

        private void DetectPlatform()
        {
            var impTypeName = Implementation.GetType().Name;
            if (impTypeName.Equals("WinGLControl"))
                _NativePlatform = GLPlatforms.Windows;
            else if (impTypeName.Equals("CarbonGLControl"))
                _NativePlatform = GLPlatforms.OSX;
            else if (impTypeName.Equals("Sdl2GLControl"))
                _NativePlatform = GLPlatforms.SDL2;
            else if (impTypeName.Equals("X11GLControl"))
                _NativePlatform = GLPlatforms.X11;
            else
                _NativePlatform = GLPlatforms.Unknown;
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            _Exists = false;
            base.Dispose(disposing);
        }
    }
}
