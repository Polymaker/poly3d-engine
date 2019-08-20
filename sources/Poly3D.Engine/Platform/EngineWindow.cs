using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using Poly3D.Utilities;
using System.ComponentModel;
using System.Threading;
using Poly3D.Platform.Native;
using Poly3D.Engine;
using Poly3D.Maths;
using System.Drawing;

namespace Poly3D.Platform
{
    public class EngineWindow : NativeWindow, IGLSurface, IEngineDisplay, IDisposable
    {
        private bool _IsExiting;
        private VSyncMode _VSync;
        private IGraphicsContext _Context;
        private DisplayEngineSettings _Configuration;
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

        /// <summary>
        /// Gets or states the state of the NativeWindow.
        /// </summary>
        public override WindowState WindowState
        {
            get
            {
                return base.WindowState;
            }
            set
            {
                base.WindowState = value;
                if (Context != null)
                {
                    Context.Update(WindowInfo);
                }
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

        /// <summary>
        /// Gets the aspect ratio of this EngineWindow.
        /// </summary>
        public float AspectRatio
        {
            get
            {
                EnsureUndisposed();
                return (float)ClientSize.Width / (float)ClientSize.Height;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DisplayEngineSettings Configuration
        {
            get { return _Configuration; }
        }

        public new bool Visible
        {
            get { return base.Visible; }
            set
            {
                SetVisible(value);
            }
        }

        #endregion

        #region Events

        public event EventHandler Load;

        public event EventHandler Unload;

        public event EventHandler DisplayChanged;

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
            _Configuration = new DisplayEngineSettings();
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
        }

        #endregion

        #region IGLSurface

        public IntPtr Handle
        {
            get { return WindowInfo.Handle; }
        }

        public IGraphicsContext Context
        {
            get
            {
                EnsureUndisposed();
                return _Context;
            }
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

        public void MakeCurrent()
        {
            EnsureUndisposed();
            Context.MakeCurrent(WindowInfo);
        }

        public void SwapBuffers()
        {
            EnsureUndisposed();
            Context.SwapBuffers();
        }

        #endregion

        #region IEngineDisplay

        public bool IsIdle
        {
            get { return GetIdle(); }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            OnDisplayChanged(e);
        }

        protected virtual void OnDisplayChanged(EventArgs e)
        {
            var handler = DisplayChanged;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnLoad(EventArgs e)
        {
            var handler = Load;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnUnload(EventArgs e)
        {
            var handler = Unload;
            if (handler != null)
                handler(this, e);
        }

        public void LoadScene(Scene scene)
        {
            LoadScene(scene, true);
        }

        public void LoadScene(Scene scene, bool autostart)
        {
            scene.AssignDisplay(this);
            if (autostart)
                scene.Start();
        }

        public Vector2 GetMousePosition()
        {
            return new Vector2(0, 0);
        }

        public Rectangle GetDisplayBounds()
        {
            return Bounds;
        }

        #endregion

        #region Window/Form methods & functions

        private void SetVisible(bool value)
        {
            EnsureUndisposed();
            if (value != Visible)
            {
                if (value)
                    Show();
                else
                    Hide();
            }
        }

        public void Show()
        {
            base.Visible = true;
            RunMessageLoop();
        }

        public void Hide()
        {
            base.Visible = false;
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
            OnLoad(EventArgs.Empty);
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

        internal bool GetIdle()
        {
            if (Utility.CurrentPlatform == GLPlatforms.Windows)
                return WinAPI.IsIdle(Handle);
            else if (Utility.CurrentPlatform == GLPlatforms.SDL2)
                return Sdl2API.SDL_HasEvents(0, 65535);
            
            return true;
        }

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
