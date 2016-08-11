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
        private Scene MyScene;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MyScene = Scene.CreateDefault();
            engineControl1.LoadScene(MyScene);
            
            modelRootObj = MyScene.AddObject<SceneObject>();

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
            modelObject2.Name = "WheelHub";

            var rotater = modelObject2.AddComponent<AnonymousBehaviour>();
            rotater.Update = (ob, dt) =>
             {
                 (ob.EngineObject as SceneObject).Transform.Rotate(new Rotation(0f, 0f, 30f * (float)dt), Space.Parent);
                 //Trace.WriteLine("Rotation = " + (ob.EngineObject as SceneObject).Transform.Rotation);

             };
            var mainCam = MyScene.ActiveCameras.First();
            var testObj = mainCam.AddObject<SceneObject>();
            testObj.Transform.Position = new Vector3(0, 0, 10);
            MyScene.Resume();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MyScene.IsRunning)
                MyScene.Pause();
            else
                MyScene.Resume();
            //poly3DControl1.SetGraphicsMode(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 4));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var mainCam = MyScene.ActiveCameras.First();
            
            var testObj = mainCam.Childs.First();

            testObj.Transform.SetRotation(RotationComponent.Roll, testObj.Transform.Rotation.Roll + 45f, Space.Self);
            //mainCam.Transform.Rotate(new Rotation(0, 0, 45f), Space.Self);

            Trace.WriteLine("testObj forward = " + mainCam.Transform.Forward);
            Trace.WriteLine("testObj forward = " + testObj.Transform.Forward);
            //var mainCam = MyScene.ActiveCameras.First();
            //if (mainCam.Projection == ProjectionType.Perspective)
            //    mainCam.Projection = ProjectionType.Orthographic;
            //else
            //    mainCam.Projection = ProjectionType.Perspective;

            //Trace.WriteLine("obj forward = " + mainCam.Childs.First().Transform.Forward);
            //mainCam.Transform.SetRotation(RotationComponent.Roll, 45f, Space.Self);
            //Trace.WriteLine(poly3DControl1.RenderFrequency.ToString());
        }

        private void poly3DControl1_MouseClick(object sender, MouseEventArgs e)
        {
            //if (poly3DControl1.Scene == null)
            //    return;
            ////modelRootObj.Transform.Rotate(new Rotation(22.5f, 0f, 0f));

            //if (e.Button == MouseButtons.Left)
            //{
            //    var mainCam = poly3DControl1.Scene.ActiveCameras.First();
            //    var raycast = mainCam.RaycastFromScreen(new Vector2(e.X, e.Y));
            //    var selectedObject = mainCam.RaySelect(raycast);
            //    if (selectedObject != null)
            //    {
            //        Trace.WriteLine("Selected object id " + selectedObject.Name);
            //    }
            //}
            //else if (e.Button == MouseButtons.Right)
            //{
                
            //    //var mainCam = poly3DControl1.Scene.ActiveCameras.First();
            //    //var objPos = mainCam.WorldPointToScreen(modelObject.Transform.WorldPosition);
            //    //Trace.WriteLine(objPos);
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //label1.Text = string.Format("{0:0.##} FPS", poly3DControl1.RenderFrequency);
        }

    }
}
