using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Platform
{
    internal static class Utility
    {
        // Fields...
        private readonly static GLPlatforms _CurrentPlatform;

        public static GLPlatforms CurrentPlatform
        {
            get { return _CurrentPlatform; }
        }
        
        static Utility()
        {
            if (Configuration.RunningOnSdl2)
            {
                _CurrentPlatform = GLPlatforms.SDL2;
            }
            else if (Configuration.RunningOnWindows)
            {
                _CurrentPlatform = GLPlatforms.Windows;
            }
            else if (Configuration.RunningOnMacOS)
            {
                _CurrentPlatform = GLPlatforms.OSX;
            }
            else if (Configuration.RunningOnX11)
            {
                _CurrentPlatform = GLPlatforms.X11;
            }
            else
            {
                _CurrentPlatform = GLPlatforms.Unknown;
            }
        }
    }
}
