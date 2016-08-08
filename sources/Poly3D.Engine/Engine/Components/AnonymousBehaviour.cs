using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class AnonymousBehaviour : ObjectBehaviour
    {
        public Action<AnonymousBehaviour, float> Update;

        protected override void OnUpdate(float deltaTime)
        {
            if (Update != null)
                Update(this, deltaTime);
        }
    }
}
