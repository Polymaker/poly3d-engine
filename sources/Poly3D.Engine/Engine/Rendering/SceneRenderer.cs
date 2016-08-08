using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Poly3D.Maths;
using Poly3D.Graphics;
using Poly3D.OpenGL;

namespace Poly3D.Engine.Rendering
{
    internal static class SceneRenderer
    {
        static Shader PhongShader;

        private static void InitShader()
        {
            PhongShader = new Shader(System.IO.File.ReadAllText("phong.vsh"), System.IO.File.ReadAllText("phong.psh"));
            PhongShader.SetVariable("shininess", 0.4f);
        }

        public static void RenderCamera(Camera camera)
        {
            var scene = camera.Scene;
            camera.UpdateViewport();
            if (PhongShader == null)
                InitShader();
            //SetViewport(camera.DisplayRectangle);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.Texture2D);

            GL.ClearColor(camera.BackColor);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            //Setup Projection
            GL.MatrixMode(MatrixMode.Projection);
            var projectionMatrix = camera.ProjectionMatrix;
            GL.LoadMatrix(ref projectionMatrix);

            //Setup Model view (Camera)
            GL.MatrixMode(MatrixMode.Modelview);
            var viewMatrix = camera.GetModelviewMatrix();
            GL.LoadMatrix(ref viewMatrix);
            GL.Enable(EnableCap.Light0);

            //RenderAxes();
            
            
            foreach (var rootObj in scene.RootObjects.Where(o => o.IsActive))
            {
                OnRenderObject(rootObj);
            }

            RenderAxes(camera);
        }

        private static void RenderAxes(Camera cam)
        {
            GL.PushAttrib(AttribMask.LightingBit | AttribMask.LineBit | AttribMask.StencilBufferBit);

            //SetupStencil();

            //RenderHelper.RenderAxes(5f, 0.25f);

            //ApplyStencil();
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);
            float camScale = 1;

            if (cam.Projection == ProjectionType.Perspective)
            {
                var camDist = cam.Transform.WorldPosition - Vector3.Zero;
                camScale = camDist.Length * (float)Math.Tan(cam.FieldOfView.Radians / 2f);//equals half the display size (vertially)
            }
            else
            {
                camScale = cam.OrthographicSize / 2f;//equals half the display size (vertially)
            }

            //convert to screen fixed size
            camScale = 60 * camScale / (cam.DisplayRectangle.Height / 2f);

            GL.LineWidth(2f);
            GL.Begin(BeginMode.Lines);

            GL.Color4(Color.Red);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(Vector3.UnitX * camScale);

            GL.Color4(Color.LawnGreen);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(Vector3.UnitY * camScale);

            GL.Color4(Color.FromArgb(0x00, 0x66, 0xFF));
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(Vector3.UnitZ * camScale);

            GL.End();

            //GL.LineWidth(2f);
            //RenderHelper.RenderAxesContour(5f, 0.25f);
            GL.Enable(EnableCap.DepthTest);
            GL.PopAttrib();
        }

        private static void OnRenderObject(SceneObject sceneObject)
        {
            GL.PushMatrix();
            var transMat = sceneObject.Transform.GetLocalTransformMatrix();
            GL.MultMatrix(ref transMat);

            if (sceneObject is ObjectMesh)
                RenderMeshObject((ObjectMesh)sceneObject);

            if (sceneObject.Childs.Count > 0)
            {
                foreach (var childObj in sceneObject.Childs.Where(o => o.IsActive))
                    OnRenderObject(childObj);
            }

            GL.PopMatrix();
        }

        private static void RenderMeshObject(ObjectMesh meshObj)
        {
            GL.PushAttrib(AttribMask.LightingBit | AttribMask.LineBit | AttribMask.StencilBufferBit | AttribMask.DepthBufferBit);

            //SetupStencil();

            PhongShader.Bind();
            RenderHelper.DrawMesh(Color.Gray, meshObj.Mesh, meshObj.Transform.WorldScale);
            Shader.Bind(null);

            //ApplyStencil();
            GL.Disable(EnableCap.Lighting);

            //RenderHelper.DrawWireMesh(Color.DarkBlue, meshObj.Mesh);

            RenderHelper.DrawBox(Color.Yellow, meshObj.Mesh.BoundingBox);

            GL.PopAttrib();
        }

        private static void SetupStencil()
        {
            GL.Enable(EnableCap.StencilTest);
            GL.ClearStencil(0);
            GL.Clear(ClearBufferMask.StencilBufferBit);
            GL.StencilFunc(StencilFunction.Always, 1, 0xFFFF);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        }

        private static void ApplyStencil()
        {
            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFFFF);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        }

        private static void SetViewport(Rect rectangle)
        {
            GL.Viewport((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }
    }
}
