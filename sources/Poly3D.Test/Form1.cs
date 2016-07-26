﻿using System;
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
            

            //var cam2 = poly3DControl1.Scene.AddObject<Camera>();
            //cam2.
        }

        private void button1_Click(object sender, EventArgs e)
        {
            poly3DControl1.SetGraphicsMode(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 4));
        }

        //float rotAngle = 0f;

        private void poly3DControl1_RenderFrame(object sender, OpenTK.FrameEventArgs e)
        {
            //GL.Enable(EnableCap.DepthTest);
            //GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
            //GL.Rotate(rotAngle, Vector3.UnitY);


            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            //GL.Begin(BeginMode.Triangles);

            //GL.Color3(Color.Blue);
            //GL.Vertex3(0f, -0.5f, -1f);
            //GL.Vertex3(0.866f, -0.5f, 0.5f);
            //GL.Vertex3(0f, 1f, 0f);

            //GL.Color3(Color.Red);
            //GL.Vertex3(0.866f, -0.5f, 0.5f);
            //GL.Vertex3(-0.866f, -0.5f, 0.5f);
            //GL.Vertex3(0f, 1f, 0f);

            //GL.Color3(Color.Yellow);
            //GL.Vertex3(-0.866f, -0.5f, 0.5f);
            //GL.Vertex3(0f, -0.5f, -1f);
            //GL.Vertex3(0f, 1f, 0f);

            //GL.End();
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

        private void poly3DControl1_UpdateFrame(object sender, FrameEventArgs e)
        {
            //rotAngle += (float)(90d * e.Time);
        }

        private void poly3DControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (poly3DControl1.Scene != null)
            {
                var raycast = poly3DControl1.Scene.ActiveCameras.First().RaycastFromScreen(new Vector2(e.X, e.Y));
                var rayAngle = Rotation.FromDirection(raycast.Direction);
                Trace.WriteLine("rayAngle = " + rayAngle);

                var plane = new Plane(Vector3.UnitY, 0f);
                float hitDist = 0;
                if (plane.Raycast(raycast, out hitDist))
                {
                    var hitPos = raycast.GetPoint(hitDist);
                    Trace.WriteLine("clicked pos = " + hitPos);
                }
                Trace.WriteLine("=======================\r\n");
            }
        }
    }
}
