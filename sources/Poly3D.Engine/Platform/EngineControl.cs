using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using Poly3D.Utilities;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using Poly3D.Engine;

namespace Poly3D.Platform
{
    [System.ComponentModel.ToolboxItem(true)]
    public class EngineControl : GLControl, IGLSurface, IEngineDisplay, IDisposable
    {
        private bool _Exists;
        private DisplayEngineSettings _Configuration;

        #region Properties

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DisplayEngineSettings Configuration
        {
            get { return _Configuration; }
        }

        #endregion

        #region Base properties overrides

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }
        }

        #endregion

        #region Events

        public event EventHandler Unload;

        public event EventHandler DisplayChanged;

        #endregion

        #region Ctors

        public EngineControl()
            : this(new GraphicsMode(32,24,2,4))
        {
            //Initialize();
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
            _Configuration = new DisplayEngineSettings();
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected void OnDisplayChanged(EventArgs e)
        {
            var handler = DisplayChanged;
            if (handler != null)
                handler(this, e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            OnDisplayChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            OnDisplayChanged(e);
        }

        public Rectangle GetDisplayBounds()
        {
            return RectangleToScreen(Bounds);
        }

        #region IGLSurface

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

        #endregion

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

        protected virtual void OnUnload(EventArgs e)
        {
            var handler = Unload;
            if (handler != null)
                handler(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            _Exists = false;
            OnUnload(EventArgs.Empty);
            base.Dispose(disposing);
        }
    }
}
