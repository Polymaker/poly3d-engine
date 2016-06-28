using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Utilities
{
    public class TempAssign<T> : IDisposable
    {
        private Action<T> final;
        private T assignVal;

        public TempAssign(T currentValue, Action<T> onRelease)
        {
            assignVal = currentValue;
            final = onRelease;
        }

        public TempAssign(T currentValue, Action onAssign, Action<T> onRelease)
        {
            assignVal = currentValue;
            onAssign();
            final = onRelease;
        }

        ~TempAssign()
        {
            Dispose();
        }

        public void Dispose()
        {
            final(assignVal);
            GC.SuppressFinalize(this);
        }
    }
}
