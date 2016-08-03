using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Poly3D.OpenGL
{
    public class Material
    {
        public static readonly Material Default = new Material();

        public Color4 Ambient { get; set; }

        public Color4 Diffuse { get; set; }

        public Color4 Specular { get; set; }

        public Color4 Emission { get; set; }

        public float Shininess { get; set; }


        public Material()
        {
            Ambient = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
            Diffuse = new Color4(0.8f, 0.8f, 0.8f, 1.0f);
            Specular = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
            Emission = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
            Shininess = 0f;
        }

        public Material(Color4 ambient, Color4 diffuse, Color4 specular, Color4 emission, float shininess)
        {
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            Emission = emission;
            Shininess = shininess;
        }

        public void Apply(MaterialFace face)
        {
            GL.Material(face, MaterialParameter.Ambient, Ambient);
            GL.Material(face, MaterialParameter.Diffuse, Diffuse);
            GL.Material(face, MaterialParameter.Specular, Specular);
            GL.Material(face, MaterialParameter.Emission, Emission);
            GL.Material(face, MaterialParameter.Shininess, Shininess);
        }

        public static void ResetMaterial()
        {
            Default.Apply(MaterialFace.FrontAndBack);
        }
    }
}
