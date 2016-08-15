using OpenTK;
using Poly3D.Engine;
using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Prefabs.Scripts
{
    public class Animator : ObjectBehaviour
    {
        public ComplexTransform Target { get; set; }
        public float Time { get; set; }

        private ComplexTransform Origin;
        private float TotalTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Animator"/> class.
        /// </summary>
        public Animator()
        {
            Time = 0f;
            TotalTime = 0f;
            Enabled = false;
        }

        protected override void OnInitialize()
        {
            //Enabled = false;
            Origin = ComplexTransform.FromMatrix(EngineObject.Transform.LocalToWorldMatrix);
            TotalTime = 0;
        }

        protected override void OnUpdate(float deltaTime)
        {
            TotalTime += deltaTime;
            var blend = (float)Math.Min(TotalTime / Time, 1);
            EngineObject.Transform.WorldRotation = Rotation.Slerp(Origin.Rotation, Target.Rotation, blend);

            EngineObject.Transform.WorldPosition = Vector3.Lerp(Origin.Translation, Target.Translation, blend);
            
            if (blend >= 1.0)
                Destroy();
        }
    }
}
