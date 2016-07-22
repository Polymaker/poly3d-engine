using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using Poly3D.Engine.Utilities;
using Poly3D.Engine;
using OpenTK.Graphics;

namespace Poly3D.Control
{
    
    public partial class Poly3DControl : GLControl, IViewPort
    {
        // Fields...
        private Scene _Scene;
        private double _RenderPeriod;
        private double _UpdatePeriod;
        private RenderSettings _RenderSettings;
        private RenderBehavior _RenderBehavior;
        private Color _BackColor;
        private FlagList dirtyFlags;
        private bool parentFormHooked = false;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RenderBehavior RenderBehavior
        {
            get { return _RenderBehavior; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RenderSettings RenderSettings
        {
            get { return _RenderSettings; }
        }

        [Browsable(false)]
        public bool IsContextValid
        {
            get { return !DesignMode && Context != null && Context.IsCurrent; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scene Scene
        {
            get { return _Scene; }
            set
            {
                _Scene = value;
                _Scene.Viewport = this;
            }
        }
        

        #region Render & update Stats

        /// <summary>
        /// Gets a double representing the actual frequency of RenderFrame events, in hertz (i.e. fps or frames per second).
        /// </summary>
        [Browsable(false)]
        public double RenderFrequency
        {
            get
            {
                if (RenderPeriod == 0d)
                    return 1.0;
                return 1.0 / RenderPeriod;
            }
        }

        /// <summary>
        /// Gets a double representing the period of RenderFrame events, in seconds.
        /// </summary>
        [Browsable(false)]
        public double RenderPeriod
        {
            get { return _RenderPeriod; }
        }

        /// <summary>
        /// Gets a double representing the frequency of UpdateFrame events, in hertz.
        /// </summary>
        [Browsable(false)]
        public double UpdateFrequency
        {
            get
            {
                if (UpdatePeriod == 0d)
                    return 1.0;
                return 1.0 / UpdatePeriod;
            }
        }

        /// <summary>
        /// Gets a double representing the period of UpdateFrame events, in seconds.
        /// </summary>
        [Browsable(false)]
        public double UpdatePeriod
        {
            get { return _UpdatePeriod; }
        }

        [DefaultValue(true)]
        public bool AutoSwapBuffers { get; set; }

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

        public override Color BackColor
        {
            get
            {
                if (_BackColor.IsEmpty)
                    return Color.DarkGray;
                return _BackColor;
            }
            set
            {
                if (_BackColor != value)
                {
                    _BackColor = value;
                    if(IsHandleCreated)
                        OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when it is time to render a frame.
        /// </summary>
        public event EventHandler<FrameEventArgs> RenderFrame;

        /// <summary>
        /// Occurs when it is time to update a frame.
        /// </summary>
        public event EventHandler<FrameEventArgs> UpdateFrame;

        #endregion

        #region Ctors

        public Poly3DControl()
            : this(GraphicsMode.Default) { }

        public Poly3DControl(GraphicsMode mode)
            : this(mode, 1, 0, GraphicsContextFlags.Default) { }

        public Poly3DControl(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags)
            : base(mode, major, minor, flags)
        {
            _RenderBehavior = new RenderBehavior();
            _RenderSettings = new RenderSettings();
            _BackColor = Color.Empty;
            AutoSwapBuffers = true;
            SetStyle(ControlStyles.ResizeRedraw, false);
            dirtyFlags = new FlagList();

            InitializeComponent();

            SetStyle(ControlStyles.Selectable, true);

            _RenderBehavior.PropertyChanged += Behavior_PropertyChanged;
            _RenderSettings.PropertyChanged += RenderSettings_PropertyChanged;

            _Scene = new Scene(this);

            dirtyFlags.Set("BackColor");
        }

        #endregion

        #region Settings management

        private void Behavior_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "CaptureMouse")
            //{

            //}
        }

        private void RenderSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Strategy")
            {
                SetupUpdateRenderStrategy();
            }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
           
            HookParentForm();

            SetupUpdateRenderStrategy();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            HookParentForm();
        }

        private void HookParentForm()
        {
            if (parentFormHooked)
                return;
            var parentForm = FindForm();
            if (parentForm != null)
            {
                Trace.WriteLine("found parent!");
                parentFormHooked = true;
                parentForm.Move += ParentForm_Move;
                parentForm.Resize += ParentForm_Move;
            }
        }

        private void ParentForm_Move(object sender, EventArgs e)
        {
            if (UpdateTimer != null && 
                UpdateTimer.IsRunning && 
                RenderSettings.Strategy == RenderStrategy.OnIdle)
            {
                RaiseUpdateFrame(UpdateTimer, ref NextUpdate);
                RaiseRenderFrame(RenderTimer, ref NextRender);
            }
        }

        public void SetGraphicsMode(GraphicsMode mode)
        {
            var formatField = typeof(GLControl).GetField("format", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            formatField.SetValue(this, mode);
            if (IsHandleCreated)
            {
                RecreateHandle();
                InitializeComponent();
                dirtyFlags.Set("BackColor");
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!DesignMode && Context != null)
            {
                UpdateViewPort();
                OnRenderFrameInternal(new FrameEventArgs());
            }
        }

        #region BackColor

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            dirtyFlags.Set("BackColor");
            CheckSetGLBackColor();
        }

        private void CheckSetGLBackColor()
        {
            if (IsContextValid && dirtyFlags["BackColor"])
            {
                GL.ClearColor(BackColor);
                GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
                dirtyFlags.UnSet("BackColor");
            }
        }

        #endregion

        #region ViewPort

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateViewPort();
        }

        private void UpdateViewPort()
        {
            if (!IsContextValid)
                return;
            GL.Viewport(Size);
        }

        #endregion

        #region Render & Update threads

        private Stopwatch UpdateTimer;
        private Stopwatch RenderTimer;
        private double NextUpdate;
        private double NextRender;
        private bool loopRunning = true;

        private void SetupUpdateRenderStrategy()
        {
            if (DesignMode)
                return;

            loopRunning = false;

            if (UpdateTimer == null)
            {
                UpdateTimer = Stopwatch.StartNew();
                RenderTimer = Stopwatch.StartNew();
            }
            
            Application.Idle -= Application_Idle;

            if (RenderSettings.Strategy == RenderStrategy.OnIdle)
            {
                loopRunning = true;
                Application.Idle += Application_Idle;
            }
            else
            {

            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {

            while (IsIdle && loopRunning)
            {
                RaiseUpdateFrame(UpdateTimer, ref NextUpdate);
                RaiseRenderFrame(RenderTimer, ref NextRender);
            }
        }

        //private void UpdateRenderLoop()
        //{
        //    var update_watch = new Stopwatch();
        //    var render_watch = new Stopwatch();
        //    double next_render = 0;
        //    double next_update = 0;

        //    while (true)
        //    {
        //        //Application.DoEvents();
        //        RaiseUpdateFrame(update_watch, ref next_update);
        //        RaiseRenderFrame(render_watch, ref next_render);

        //    }
        //}

        private void RaiseRenderFrame(Stopwatch render_watch, ref double next_render)
        {
            double elapsed = render_watch.Elapsed.TotalSeconds;

            if (elapsed <= 0.0)
            {
                render_watch.Restart();
                return;
            }

            elapsed = elapsed > 1d ? 1.0 : elapsed;

            double delta = next_render - elapsed;
            if (delta <= 0.0 && elapsed > 0.0)
            {

                next_render = delta + RenderSettings.RenderPeriod;
                next_render = Math.Max(next_render, -1.0);
                render_watch.Restart();

                if (elapsed > 0.0)
                {
                    _RenderPeriod = elapsed;
                    OnRenderFrameInternal(new FrameEventArgs(elapsed));
                }
            }
        }

        private void RaiseUpdateFrame(Stopwatch update_watch, ref double next_update)
        {
            int num = 0;
            double num2 = 0.0;
            double num3 = update_watch.Elapsed.TotalSeconds;

            if (num3 <= 0.0)
            {
                update_watch.Restart();
                return;
            }

            num3 = num3 > 1d ? 1d : num3;

            while (next_update - num3 <= 0.0 && num3 > 0.0)
            {
                next_update -= num3;
                OnUpdateFrameInternal(new FrameEventArgs(num3));
                num3 = Math.Max(update_watch.Elapsed.TotalSeconds, 0.0) - num3;
                update_watch.Restart();
                next_update += RenderSettings.UpdatePeriod;
                next_update = Math.Max(next_update, -1.0);
                num2 += num3;
                if (++num >= 10 || RenderSettings.UpdateFrequency == 0.0)
                {
                    break;
                }
            }
            if (num > 0)
            {
                _UpdatePeriod = num2 / (double)num;
            }
        }

        private void OnRenderFrameInternal(FrameEventArgs e)
        {
            if (!IsDisposed && IsHandleCreated)
            {
                if (Width == 0 || Height == 0)
                    return;
                if (!Context.IsCurrent)
                    MakeCurrent();

                CheckSetGLBackColor();
                if (Scene != null)
                    Scene.RenderScene();
                else
                    OnRenderFrame(e);
                if(AutoSwapBuffers)
                    SwapBuffers();
            }
        }

        /// <summary>
        /// Called when the frame is rendered.
        /// </summary>
        /// <param name="e">Contains information necessary for frame rendering.</param>
        /// <remarks>
        /// Subscribe to the <see cref="E:OpenTK.GameWindow.RenderFrame" /> event instead of overriding this method.
        /// </remarks>
        protected virtual void OnRenderFrame(FrameEventArgs e)
        {
            var handler = RenderFrame;
            if (handler != null)
                handler(this, e);
        }

        private void OnUpdateFrameInternal(FrameEventArgs e)
        {
            if (!IsDisposed && IsHandleCreated)
                OnUpdateFrame(e);
        }

        /// <summary>
        /// Called when the frame is updated.
        /// </summary>
        /// <param name="e">Contains information necessary for frame updating.</param>
        /// <remarks>
        /// Subscribe to the <see cref="E:OpenTK.GameWindow.UpdateFrame" /> event instead of overriding this method.
        /// </remarks>
        protected virtual void OnUpdateFrame(FrameEventArgs e)
        {
            var handler = UpdateFrame;
            if (handler != null)
                handler(this, e);
        }

        #endregion

        #region Designer stuff

        public bool ShouldSerializeBackColor()
        {
            return !_BackColor.IsEmpty;
        }

        public override void ResetBackColor()
        {
            _BackColor = Color.Empty;
            OnBackColorChanged(EventArgs.Empty);
        }

        #endregion
    }
}