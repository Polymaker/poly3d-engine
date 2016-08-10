using Poly3D.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public abstract class BaseComponent<T> : IComponent where T : EngineObject
    {
        private T _EngineObject;

        public T EngineObject
        {
            get { return _EngineObject; }
        }

        public BaseComponent()
        {
            _EngineObject = null;
        }

        public BaseComponent(T engineObject)
        {
            _EngineObject = engineObject;
        }

        internal void SetOwner(T owner)
        {
            _EngineObject = owner;
        }

        EngineObject IComponent.EngineObject
        {
            get { return EngineObject; }
        }
    }
}
