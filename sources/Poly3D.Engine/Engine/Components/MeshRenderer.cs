using Poly3D.Engine.Meshes;
using Poly3D.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    [ComponentBehavior(IsSingleton = true)]
    public class MeshRenderer : ObjectComponent
    {
        // Fields...
        private Mesh _Mesh;

        public Mesh Mesh
        {
            get { return _Mesh; }
            set
            {
                _Mesh = value;
            }
        }

        public RenderMode Mode { get; set; }

        public Material[] Materials { get; set; }

        public bool DrawWireframe { get; set; }

        public Color WireframeColor { get; set; }

        public bool Outlined { get; set; }

        public Color OutlineColor { get; set; }

        public float OutlineSize { get; set; }

        public MeshRenderer()
        {
            _Mesh = null;
            Materials = new Material[] { new Material() };
            Mode = RenderMode.Opaque;
            WireframeColor = Color.DarkBlue;
            DrawWireframe = false;
            Outlined = false;
            OutlineColor = Color.Black;
            OutlineSize = 2f;
        }

        
    }
}
