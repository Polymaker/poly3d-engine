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

namespace Poly3D.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            poly3DControl1.SetGraphicsMode(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8));
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

            //if (poly3DControl1.Scene != null)
            //{
            //    var raycast = poly3DControl1.Scene.ActiveCameras.First().RaycastFromScreen(new Vector2(e.X, e.Y));
            //    var rayAngle = Rotation.FromDirection(raycast.Direction);
            //    Trace.WriteLine("rayAngle = " + rayAngle);

            //    var plane = new Plane(Vector3.UnitY, 0f);
            //    float hitDist = 0;
            //    if (plane.Raycast(raycast, out hitDist))
            //    {
            //        var hitPos = raycast.GetPoint(hitDist);
            //        Trace.WriteLine("clicked pos = " + hitPos);
            //    }
            //    Trace.WriteLine("=======================\r\n");
            //}
        }
    }
}
