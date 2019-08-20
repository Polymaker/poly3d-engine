using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public interface IEngineBehavior
    {
        void Render(Camera camera);
        void Update(float deltaTime);
    }
}
