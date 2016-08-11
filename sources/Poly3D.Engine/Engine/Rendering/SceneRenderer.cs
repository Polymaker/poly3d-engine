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
            PhongShader = Shader.LoadPrefab("phong");
            //PhongShader = new Shader(System.IO.File.ReadAllText("phong.vsh"), System.IO.File.ReadAllText("phong.psh"));
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

            foreach (var rootObj in scene.RootObjects.Where(o => o.IsActive))
            {
                OnRenderObject(camera, rootObj);
            }

            //RenderHelper.RenderAxes(5, 0.25f);
            RenderHelper.RenderManipulator(camera, Vector3.Zero, TransformType.Scale);
        }

        private static void OnRenderObject(Camera camera, SceneObject sceneObject)
        {
            GL.PushMatrix();
            var transMat = sceneObject.Transform.GetLocalTransformMatrix();
            GL.MultMatrix(ref transMat);

            if (sceneObject is ObjectMesh)
                RenderMeshObject(camera, (ObjectMesh)sceneObject);

            if (sceneObject.Childs.Count > 0)
            {
                foreach (var childObj in sceneObject.Childs.Where(o => o.IsActive))
                    OnRenderObject(camera, childObj);
            }

            GL.PopMatrix();
        }

        private static void RenderMeshObject(Camera camera, ObjectMesh meshObj)
        {
            GL.PushAttrib(AttribMask.LightingBit | AttribMask.LineBit | AttribMask.StencilBufferBit | AttribMask.DepthBufferBit);

            //SetupStencil();

            PhongShader.Bind();
            RenderHelper.DrawMesh(Color.Gray, meshObj.Mesh, meshObj.Transform.WorldScale);
            Shader.Bind(null);

            //ApplyStencil();
            GL.Disable(EnableCap.Lighting);

            RenderHelper.DrawWireMesh(Color.DarkBlue, meshObj.Mesh);

            //RenderHelper.DrawBox(Color.Yellow, meshObj.Mesh.BoundingBox);

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
