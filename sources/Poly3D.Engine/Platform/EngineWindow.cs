﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using Poly3D.Utilities;
using System.ComponentModel;
using System.Threading;

namespace Poly3D.Platform
{
    public class EngineWindow : NativeWindow, IGLSurface, IDisposable
    {
        private bool _IsExiting;
        private VSyncMode _VSync;
        private IGraphicsContext _Context;
        private GLPlatforms _NativePlatform;
        private bool Initialized;

        #region Properties

        /// <summary>
        /// Gets or sets the VSyncMode.
        /// </summary>
        public VSyncMode VSync
        {
            get
            {
                EnsureUndisposed();
                GraphicsContext.Assert();
                return _VSync;
            }
            set
            {
                EnsureUndisposed();
                GraphicsContext.Assert();
                switch (value)
                {
                    case VSyncMode.Off:
                        Context.SwapInterval = 0;
                        break;
                    case VSyncMode.On:
                        Context.SwapInterval = 1;
                        break;
                    case VSyncMode.Adaptive:
                        Context.SwapInterval = -1;
                        break;
                }
                _VSync = value;
            }
        }

        public bool IsExiting
        {
            get
            {
                EnsureUndisposed();
                return _IsExiting;
            }
        }

        #endregion

        #region Ctors

        public EngineWindow()
            : this(640, 480, GraphicsMode.Default, "OpenTK Game Window", GameWindowFlags.Default, DisplayDevice.Default)
        {
        }

        public EngineWindow(int width, int height)
            : this(width, height, GraphicsMode.Default, "OpenTK Game Window", GameWindowFlags.Default, DisplayDevice.Default)
        {
        }

        public EngineWindow(int width, int height, GraphicsMode mode)
            : this(width, height, mode, "OpenTK Game Window", GameWindowFlags.Default, DisplayDevice.Default)
        {
        }

        public EngineWindow(int width, int height, GraphicsMode mode, string title)
            : this(width, height, mode, title, GameWindowFlags.Default, DisplayDevice.Default)
        {
        }

        public EngineWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options)
            : this(width, height, mode, title, options, DisplayDevice.Default)
        {
        }

        public EngineWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device)
            : this(width, height, mode, title, options, device, 1, 0, GraphicsContextFlags.Default)
        {
        }

        public EngineWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device, int major, int minor, GraphicsContextFlags flags)
            : this(width, height, mode, title, options, device, major, minor, flags, null)
        {
        }

        public EngineWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device, int major, int minor, GraphicsContextFlags flags, IGraphicsContext sharedContext)
            : base(width, height, title, options, mode ?? GraphicsMode.Default, device ?? DisplayDevice.Default)
        {
            try
            {
                _Context = new GraphicsContext(mode ?? GraphicsMode.Default, base.WindowInfo, major, minor, flags);
                _Context.MakeCurrent(WindowInfo);
                (_Context as IGraphicsContextInternal).LoadAll();
                VSync = VSyncMode.On;
            }
            catch (Exception)
            {
                Dispose();
                throw;
            }
            DetectPlatform();
        }

        #endregion

        #region IGLSurface

        GLPlatforms IGLSurface.Platform
        {
            get { return _NativePlatform; }
        }

        public IntPtr Handle
        {
            get { return WindowInfo.Handle; }
        }

        public IGraphicsContext Context
        {
            get { return _Context; }
        }

        protected INativeWindow Implementation
        {
            get
            {
                return ReflectionHelper.GetField<INativeWindow>(typeof(NativeWindow), this,
                    "implementation",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);
            }
        }

        private void DetectPlatform()
        {
            var impTypeName = Implementation.GetType().Name;
            if (impTypeName.Equals("WinGLNative"))
                _NativePlatform = GLPlatforms.Windows;
            else if (impTypeName.Equals("CarbonGLNative"))
                _NativePlatform = GLPlatforms.OSX;
            else if (impTypeName.Equals("Sdl2NativeWindow"))
                _NativePlatform = GLPlatforms.SDL2;
            else if (impTypeName.Equals("X11GLNative"))
                _NativePlatform = GLPlatforms.X11;
            else
                _NativePlatform = GLPlatforms.Unknown;
        }

        public void MakeCurrent()
        {
            EnsureUndisposed();
            Context.MakeCurrent(WindowInfo);
        }

        #endregion

        #region IEngineSurface

        #endregion

        #region Window/Form methods & functions

        public void Show()
        {
            Visible = true;
            RunMessageLoop();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
            {
                _IsExiting = true;
            }
        }

        private void RunMessageLoop()
        {
            if (Initialized)
                return;
            Initialized = true;
            //MessageLoop();
            new Thread(MessageLoop).Start();
        }

        private void MessageLoop()
        {
            while (true)
            {
                Implementation.ProcessEvents();
                if (!Exists || IsExiting)
                    break;
            }
        }

        #endregion

        public override void Dispose()
        {
            try
            {
                if (_Context != null)
                {
                    _Context.Dispose();
                    _Context = null;
                }
            }
            finally
            {
                base.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
