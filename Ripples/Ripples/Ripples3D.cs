using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Ripples
{
    partial class Ripples
    {
        const string textureName = "water";
        const int TOP_LEFT = 0;
        const int TOP_RIGHT = 1;
        const int BOTTOM_RIGHT = 2;
        const int BOTTOM_LEFT = 3;

        private SimpleCamera2D camera;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        RasterizerState WIREFRAME_RASTERIZER_STATE = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.WireFrame };

        BasicEffect effect;
        Texture2D polytexture;
        VertexPositionColorTexture[] vertexData;
        int[] indexData;

        public void Initialize()
        {
            SetUpVertices(Color.White);
            SetUpIndices();
        }

        public void LoadContent(ContentManager manager)
        {
            viewMatrix = Matrix.Identity;
            projectionMatrix = Matrix.CreateOrthographic(-1280, -720, -1.0f, 1.0f);

            polytexture = manager.Load<Texture2D>(textureName);
        }

        private void SetUpVertices(Color color)
        {
            const float HALF_SIDE = 200.0f;
            const float Z = 0.0f;

            vertexData = new VertexPositionColorTexture[4];
            vertexData[TOP_LEFT] = new VertexPositionColorTexture(new Vector3(-HALF_SIDE, HALF_SIDE, Z), color, new Vector2(0, 0));
            vertexData[TOP_RIGHT] = new VertexPositionColorTexture(new Vector3(HALF_SIDE, HALF_SIDE, Z), color, new Vector2(1, 0));
            vertexData[BOTTOM_RIGHT] = new VertexPositionColorTexture(new Vector3(HALF_SIDE, -HALF_SIDE, Z), color, new Vector2(1, 1));
            vertexData[BOTTOM_LEFT] = new VertexPositionColorTexture(new Vector3(-HALF_SIDE, -HALF_SIDE, Z), color, new Vector2(0, 1));
        }

        private void SetUpIndices()
        {
            indexData = new int[6];
            indexData[0] = TOP_LEFT;
            indexData[1] = BOTTOM_RIGHT;
            indexData[2] = BOTTOM_LEFT;

            indexData[3] = TOP_LEFT;
            indexData[4] = TOP_RIGHT;
            indexData[5] = BOTTOM_RIGHT;
        }

        public void Draw3D()
        {
            // Draw textured box

            // Draw textured box
            gd.RasterizerState = RasterizerState.CullNone;  // vertex order doesn't matter
            gd.BlendState = BlendState.NonPremultiplied;    // use alpha blending
            gd.DepthStencilState = DepthStencilState.None;  // don't bother with the depth/stencil buffer

            effect.View = camera.GetView();
            effect.Projection = projectionMatrix;
            effect.Texture = polytexture;
            effect.TextureEnabled = true;
            effect.DiffuseColor = Color.White.ToVector3();
            effect.CurrentTechnique.Passes[0].Apply();

            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexData, 0, 4, indexData, 0, 2);

            // Draw wireframe box
            gd.RasterizerState = WIREFRAME_RASTERIZER_STATE;    // draw in wireframe
            gd.BlendState = BlendState.Opaque;                  // no alpha this time

            effect.TextureEnabled = false;
            effect.DiffuseColor = Color.Black.ToVector3();
            effect.CurrentTechnique.Passes[0].Apply();

            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexData, 0, 4, indexData, 0, 2);
        }
    }
}
