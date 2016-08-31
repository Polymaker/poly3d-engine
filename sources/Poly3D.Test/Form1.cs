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
using Poly3D.Prefabs.Scripts;

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

            //modelRootObj.Transform.SetRotation(RotationComponent.Pitch, 45f);
            //modelRootObj.Transform.Translate(new Vector3(0, 0, 2), Space.Self);

            var model = WavefrontMeshLoader.LoadWavefrontObj(@"32495.obj");
            var modelBounds = model.BoundingBox;
            var modelScale = 5f / modelBounds.Extents.Length;

            modelObject = modelRootObj.AddObject<ObjectMesh>();
            modelObject.Mesh = model;
            modelObject.Material.DrawWireframe = true;
            modelObject.Material.Color = Color.FromArgb(150, 128, 128, 128);
            modelObject.Transform.WorldScale = new Vector3(modelScale, modelScale, modelScale);
            model = WavefrontMeshLoader.LoadWavefrontObj(@"32496.obj");

            modelObject2 = modelObject.AddObject<ObjectMesh>();
            modelObject2.Mesh = model;

            var offset = 13.3f * modelScale;

            modelObject2.Transform.Translate(modelObject2.Transform.Forward * offset, Space.World);
            modelObject2.Transform.Rotation = new Rotation(0f, 180f, 0);
            modelObject2.Name = "WheelHub";
            modelObject2.Material.Outlined = true;
            var rotater = modelObject2.AddComponent<AnonymousBehaviour>();
            
            
            rotater.Update = (ob, dt) =>
             {
                 (ob.EngineObject as SceneObject).Transform.Rotate(new Rotation(0f, 0f, 90f * (float)dt), Space.Self);

             };
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var mainCam = MyScene.ActiveCameras.First();
            if (mainCam.Projection == ProjectionType.Perspective)
                mainCam.Projection = ProjectionType.Orthographic;
            else
                mainCam.Projection = ProjectionType.Perspective;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var mainCam = MyScene.ActiveCameras.First();
            mainCam.MatchOrthoViewToPersp();
            //if (mainCam.Projection == ProjectionType.Perspective)
            //    mainCam.Projection = ProjectionType.Orthographic;
            //else
            //    mainCam.Projection = ProjectionType.Perspective;
            //var orbitScript = mainCam.GetComponent<PanOrbitCamera>();
            //var camTarget = orbitScript.CameraTarget;
            //var rotater = MyScene.ActiveCameras.First().AddComponent<Animator>();
            //var targetTransform = new ComplexTransform();
            //targetTransform.Translation = camTarget + new Vector3(0, orbitScript.TargetDistance, 0);
            //targetTransform.Rotation = Rotation.LookAt(targetTransform.Translation, camTarget, Vector3.UnitY);
            //rotater.Target = targetTransform;
            //rotater.Time = 1;
            //rotater.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //label1.Text = string.Format("{0:0.##} FPS", poly3DControl1.RenderFrequency);
        }

        private void engineControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var mainCam = MyScene.ActiveCameras.First();
                var raycast = mainCam.RaycastFromScreen(new Vector2(e.X, e.Y));
                var selectedObject = mainCam.RaySelect(raycast);
                if (selectedObject != null)
                {
                    Trace.WriteLine("Selected object id " + selectedObject.Name);
                }
            }
        }

    }
}
