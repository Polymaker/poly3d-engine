using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Poly3D.Platform.Native
{
    internal static class Sdl2API
    {
        [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SDL_HasEvents(int minType, int maxType);
    }
}
