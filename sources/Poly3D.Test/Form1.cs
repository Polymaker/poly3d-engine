using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Poly3D.Engine.OpenGL;
using Poly3D.Engine;
using Poly3D.Maths;
using Poly3D.Engine.Data;

namespace Poly3D.Test
{
    public partial class Form1 : Form
    {

        private SceneObject modelRootObj;
        private ObjectMesh modelObject;
        private ObjectMesh modelObject2;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            modelRootObj = poly3DControl1.Scene.AddObject<SceneObject>();

            var model = WavefrontMeshLoader.LoadWavefrontObj(@"32495.obj");
            var modelBounds = model.BoundingBox;
            var modelScale = 5f / modelBounds.Extents.Length;

            modelObject = modelRootObj.AddObject<ObjectMesh>();
            modelObject.Mesh = model;
            modelObject.Transform.WorldScale = new Vector3(modelScale, modelScale, modelScale);

            model = WavefrontMeshLoader.LoadWavefrontObj(@"32496.obj");

            modelObject2 = modelObject.AddObject<ObjectMesh>();
            modelObject2.Mesh = model;
            
            var offset = 13.3f * modelScale;
            modelObject2.Transform.Translate(modelObject2.Transform.Forward * offset, Space.World);
            modelObject2.Transform.Rotation = new Rotation(0, 180f, 0);
            

            var rotater = modelObject2.AddComponent<AnonymousBehaviour>();
            rotater.Update = (ob, dt) =>
             {
                 ob.SceneObject.Transform.Rotate(new Rotation(0f, 0f, 90f * (float)dt));
             };

            var mainCam = poly3DControl1.Scene.ActiveCameras.First();
            mainCam.AddComponent<PanOrbitCamera>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            poly3DControl1.SetGraphicsMode(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 4));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var mainCam = poly3DControl1.Scene.ActiveCameras.First();
            if (mainCam.Projection == ProjectionType.Perspective)
                mainCam.Projection = ProjectionType.Orthographic;
            else
                mainCam.Projection = ProjectionType.Perspective;

            Trace.WriteLine(poly3DControl1.RenderFrequency.ToString());
        }

        private void poly3DControl1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = string.Format("{0:0.##} FPS", poly3DControl1.RenderFrequency);
        }
    }
}
