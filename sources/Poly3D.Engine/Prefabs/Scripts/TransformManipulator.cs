using OpenTK;
using Poly3D.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Prefabs.Scripts
{
    public class TransformManipulator : ObjectBehavior
    {
        public TransformType ManipulatorType { get; set; } = TransformType.Translation;

        public float Size { get; set; } = 1f;

        public bool AbsoluteScreenSize { get; set; }

        protected override void OnRender(Camera camera)
        {
            base.OnRender(camera);
            if (AbsoluteScreenSize)
                RenderHelper.RenderUIScaledManipulator(camera, EngineObject.Transform.WorldPosition, Size, ManipulatorType);
            else
                RenderHelper.RenderManipulator(camera, EngineObject.Transform.WorldPosition, Size, ManipulatorType);
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }
    }
}
