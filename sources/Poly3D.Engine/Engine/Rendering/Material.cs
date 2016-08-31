using Poly3D.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class Material// : EngineObject
    {
        // Fields...
        private Texture _Texture;
        private Color _Color;

        /// <summary>
        /// Gets or sets the main material's color.
        /// </summary>
        public Color Color
        {
            get { return _Color; }
            set
            {
                _Color = value;
            }
        }

        public bool DrawWireframe { get; set; }

        public Color WireframeColor { get; set; }

        public bool Outlined { get; set; }

        public Color OutlineColor { get; set; }

        public float OutlineSize { get; set; }

        /// <summary>
        /// Gets or sets the material's texture.
        /// </summary>
        public Texture Texture
        {
            get { return _Texture; }
            set
            {
                _Texture = value;
            }
        }

        public Material()
        {
            _Texture = null;
            _Color = Color.FromArgb(128,128,128);
            WireframeColor = Color.DarkBlue;
            DrawWireframe = false;
            Outlined = false;
            OutlineColor = Color.Black;
            OutlineSize = 2f;
        }
    }
}
