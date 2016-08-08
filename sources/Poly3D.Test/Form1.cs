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
        public Form1()
        {
            InitializeComponent();
        }
        private SceneObject modelRootObj;
        private ObjectMesh modelObject;
        private ObjectMesh modelObject2;

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
            //modelRootObj.Transform.Rotate(new Rotation(22.5f, 0f, 0f));

            if (poly3DControl1.Scene != null)
            {
                var mainCam = poly3DControl1.Scene.ActiveCameras.First();
                var raycast = mainCam.RaycastFromScreen(new Vector2(e.X, e.Y));
                var selectedObject = mainCam.RaySelect(raycast);
                if (selectedObject != null)
                {
                    Trace.WriteLine("Selected object id " + selectedObject.GetInstanceID());
                }
                //var rayAngle = Rotation.FromDirection(raycast.Direction);
                //Trace.WriteLine("rayAngle = " + rayAngle);

                //var plane = new Plane(Vector3.UnitY, 0f);
                //float hitDist = 0;
                //if (plane.Raycast(raycast, out hitDist))
                //{
                //    var hitPos = raycast.GetPoint(hitDist);
                //    Trace.WriteLine("clicked pos = " + hitPos);
                //}
                //Trace.WriteLine("=======================\r\n");
            }
        }

        private void poly3DControl1_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (modelRootObj != null)
            {
                //modelRootObj.Transform.Rotate(new Rotation(0f, 0f, 45f * (float)e.Time));
                modelObject.Transform.Rotate(new Rotation(0f, 45f * (float)e.Time, 0f));
                modelObject2.Transform.Rotate(new Rotation(0f, 0f, 90f * (float)e.Time));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = string.Format("{0:0.##} FPS", poly3DControl1.RenderFrequency);
        }

    }
}
