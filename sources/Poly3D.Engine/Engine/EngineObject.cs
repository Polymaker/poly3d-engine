using Poly3D.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Poly3D.Engine
{
    public abstract class EngineObject
    {
        private static long CurrentID = 0;
        private readonly long InstanceID = Interlocked.Increment(ref CurrentID);
        //public static TSDictionary<long, EngineObject> InstancesList;

        static EngineObject()
        {
            //InstancesList = new TSDictionary<long, EngineObject>();
        }

        public long GetInstanceID()
        {
            return InstanceID;
        }
    }
}
