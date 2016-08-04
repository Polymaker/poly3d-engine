using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Poly3D.Maths;
using Poly3D.Graphics;

namespace Poly3D.Engine.Rendering
{
    internal static class SceneRenderer
    {
        public static void RenderCamera(Camera camera)
        {
            var scene = camera.Scene;
            camera.UpdateViewport();
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

            //RenderHelper.RenderAxes(5f, 0.25f);
            RenderAxes();
            foreach (var rootObj in scene.RootObjects.Where(o => o.IsActive))
            {
                OnRenderObject(rootObj);
            }

        }

        private static void RenderAxes()
        {
            GL.PushAttrib(AttribMask.LightingBit | AttribMask.LineBit | AttribMask.StencilBufferBit);

            SetupStencil();

            RenderHelper.RenderAxes(5f, 0.25f);

            ApplyStencil();

            GL.Disable(EnableCap.Lighting);
            GL.LineWidth(2f);
            RenderHelper.RenderAxesContour(5f, 0.25f);

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
            GL.PushAttrib(AttribMask.LightingBit | AttribMask.LineBit | AttribMask.StencilBufferBit);

            //SetupStencil();

            RenderHelper.DrawMesh(Color.Gray, meshObj.Mesh, meshObj.Transform.WorldScale);

            //ApplyStencil();

            GL.Disable(EnableCap.Lighting);
            
            RenderHelper.DrawWireMesh(Color.Black, meshObj.Mesh/*, 3f*/);

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
