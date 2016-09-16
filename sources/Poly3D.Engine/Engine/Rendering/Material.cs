using Poly3D.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class Material
    {
        public Color DiffuseColor { get; set; }

        public Texture DiffuseTexture { get; set; }

        public Texture NormalMap { get; set; }

        public Material()
        {
            DiffuseColor = Color.FromArgb(128, 128, 128);
            DiffuseTexture = null;
            NormalMap = null;
        }
    }
}
