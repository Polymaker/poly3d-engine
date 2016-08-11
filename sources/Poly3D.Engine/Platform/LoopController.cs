using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Poly3D.Platform
{
    public class LoopController
    {
        private bool _IsRunning;
        private Stopwatch updateTimer;
        private Stopwatch renderTimer;
        private double nextRender;
        private double nextUpdate;
        private IEngineDisplay _Target;

        public bool IsRunning
        {
            get { return _IsRunning; }
        }

        public IEngineDisplay Target
        {
            get { return _Target; }
        }

        public event EventHandler<FrameEventArgs> UpdateFrame;

        public event EventHandler<FrameEventArgs> RenderFrame;

        public LoopController(IEngineDisplay target)
        {
            _Target = target;
            _IsRunning = false;
            updateTimer = new Stopwatch();
            renderTimer = new Stopwatch();
            nextRender = 0d;
            nextUpdate = 0d;
            target.DisplayChanged += Target_SizeChanged;
            target.Unload += Target_Unload;
        }

        private void Target_Unload(object sender, EventArgs e)
        {
            Stop();
        }

        private void Target_SizeChanged(object sender, EventArgs e)
        {
            if (IsRunning)
                DispatchUpdateAndRenderFrame();
            else
                ForceRender();
        }

        public void Start()
        {
            if (!IsRunning)
            {
                _IsRunning = true;
                Application.Idle += Application_Idle;
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                Application.Idle -= Application_Idle;
                _IsRunning = false;
                updateTimer.Reset();
                renderTimer.Reset();
                nextRender = nextUpdate = 0;
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (IsRunning && Target.IsIdle)
            {
                DispatchUpdateAndRenderFrame();
            }
        }

        private void DispatchUpdateAndRenderFrame()
        {
            RaiseRenderFrame();
            RaiseUpdateFrame();
        }

        private void RaiseRenderFrame()
        {
            double elapsedSeconds = renderTimer.Elapsed.TotalSeconds;
            if (elapsedSeconds <= 0.0)
            {
                renderTimer.Restart();
                return;
            }
 
            elapsedSeconds = Math.Min(elapsedSeconds, 1.0);

            double renderDelay = nextRender - elapsedSeconds;

            if (renderDelay <= 0.0 && elapsedSeconds > 0.0)
            {
                nextRender = renderDelay + _Target.Configuration.RenderPeriod;
                nextRender = Math.Max(-1.0, nextRender);

                renderTimer.Restart();
                if (elapsedSeconds > 0.0)
                {
                    //this.render_period = (render_args.Time = num);
                    DispatchRender(elapsedSeconds);
                    //this.render_time = renderTimer.Elapsed.TotalSeconds;
                }
            }
        }

        private void RaiseUpdateFrame()
        {
            int updateCount = 0;
            //double num2 = 0.0;
            double elapsedSeconds = updateTimer.Elapsed.TotalSeconds;

            if (elapsedSeconds <= 0.0)
            {
                updateTimer.Restart();
                return;
            }

            elapsedSeconds = Math.Min(elapsedSeconds, 1.0);

            while (nextUpdate - elapsedSeconds <= 0.0 && elapsedSeconds > 0.0)
            {
                nextUpdate -= elapsedSeconds;
                DispatchUpdate(elapsedSeconds);
                elapsedSeconds = (/*this.update_time = */Math.Max(updateTimer.Elapsed.TotalSeconds, 0.0) - elapsedSeconds);
                updateTimer.Restart();
                nextUpdate += Target.Configuration.UpdatePeriod;
                nextUpdate = Math.Max(nextUpdate, -1.0);
                //num2 += this.update_time;
                if (++updateCount >= 10 || Target.Configuration.UpdateFrequency == 0.0)
                {
                    break;
                }
            }
            if (updateCount > 0)
            {
                //this.update_period = num2 / (double)num;
            }
        }

        private void DispatchRender(double elapsedSeconds)
        {
            if ((Target as IGLSurface).Exists)
            {
                (Target as IGLSurface).MakeCurrent();
                OnRenderFrame(new FrameEventArgs(elapsedSeconds));
                (Target as IGLSurface).SwapBuffers();
            }
        }

        private void DispatchUpdate(double elapsedSeconds)
        {
            if ((Target as IGLSurface).Exists)
                OnUpdateFrame(new FrameEventArgs(elapsedSeconds));
        }

        private void OnRenderFrame(FrameEventArgs e)
        {
            var handler = RenderFrame;
            if (handler != null)
                handler(Target, e);
        }

        private void OnUpdateFrame(FrameEventArgs e)
        {
            var handler = UpdateFrame;
            if (handler != null)
                handler(Target, e);
        }

        internal void ForceRender()
        {
            DispatchRender(0.001);
        }
    }
}
