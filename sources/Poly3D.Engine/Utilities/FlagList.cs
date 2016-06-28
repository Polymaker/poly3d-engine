using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Utilities
{
    public class FlagList
    {
        private Dictionary<string, bool> innerDict;

        public bool this[string flagName]
        {
            get
            {
                return innerDict.ContainsKey(flagName) && innerDict[flagName];
            }
            set
            {
                innerDict[flagName] = value;
            }
        }

        public FlagList()
        {
            innerDict = new Dictionary<string, bool>();
        }

        public void Set(string flagName)
        {
            innerDict[flagName] = true;
        }

        public void UnSet(string flagName)
        {
            innerDict[flagName] = false;
        }

        public bool Get(string flagName)
        {
            return this[flagName];
        }
    }
}
