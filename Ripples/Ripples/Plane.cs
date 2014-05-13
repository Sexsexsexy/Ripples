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
    class Plane
    {
        const string textureName = "water";
        public const int TOP_LEFT = 0;
        public const int TOP_RIGHT = 1;
        public const int BOTTOM_RIGHT = 2;
        public const int BOTTOM_LEFT = 3;


        Texture2D polytexture;
        VertexPositionColorNormal[] vertexData;
        int[] indexData;

        RasterizerState WIREFRAME_RASTERIZER_STATE = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.WireFrame };

        private Vector2 position;
        private Vector2 size;

        public Plane(Vector2 pos, Vector2 size)
        {
            position = pos;
            this.size = size;
        }

        public void LoadData(ContentManager manager)
        {
            polytexture = manager.Load<Texture2D>(textureName);

            SetUpVertices(Color.White);
            SetUpIndices();

        }

        private void SetUpVertices(Color color)
        {
            const float Z = 0.0f;

            vertexData = new VertexPositionColorNormal[4];
            vertexData[TOP_LEFT] = new VertexPositionColorNormal(new Vector3(position.X, position.Y + size.Y, Z), color);
            vertexData[TOP_RIGHT] = new VertexPositionColorNormal(new Vector3(position.X + size.X, position.Y + size.Y, Z), color);
            vertexData[BOTTOM_RIGHT] = new VertexPositionColorNormal(new Vector3(position.X + size.X, position.Y, Z), color);
            vertexData[BOTTOM_LEFT] = new VertexPositionColorNormal(new Vector3(position.X, position.Y, Z), color);
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

        public void setHeight(int vert, float height)
        {
            vertexData[vert].Position.Z = height;
            calculateNormals();
        }

        private void calculateNormals()
        {
            for (int i = 0; i < vertexData.Length; i++)
                vertexData[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indexData.Length / 3; i++)
            {
                int index1 = indexData[i * 3];
                int index2 = indexData[i * 3 + 1];
                int index3 = indexData[i * 3 + 2];

                Vector3 side1 = vertexData[index1].Position - vertexData[index3].Position;
                Vector3 side2 = vertexData[index1].Position - vertexData[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertexData[index1].Normal += normal;
                vertexData[index2].Normal += normal;
                vertexData[index3].Normal += normal;
            }
        }

        public void Draw(GraphicsDevice gd, Matrix view, Matrix projection, BasicEffect effect)
        {
            // Draw textured box
            gd.RasterizerState = RasterizerState.CullNone;  // vertex order doesn't matter
            gd.BlendState = BlendState.NonPremultiplied;    // use alpha blending
            gd.DepthStencilState = DepthStencilState.DepthRead;  // don't bother with the depth/stencil buffer

            effect.View = view;
            effect.Projection = projection;
            effect.CurrentTechnique.Passes[0].Apply();

            //gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexData, 0, 4, indexData, 0, 2, VertexPositionColorNormal.VertexDeclaration);

            // Draw wireframe box
            gd.RasterizerState = WIREFRAME_RASTERIZER_STATE;    // draw in wireframe
            gd.BlendState = BlendState.Opaque;                  // no alpha this time

            effect.TextureEnabled = false;
            effect.DiffuseColor = Color.White.ToVector3();
            effect.CurrentTechnique.Passes[0].Apply();

            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexData, 0, 4, indexData, 0, 2, VertexPositionColorNormal.VertexDeclaration);
        }
    }

    struct VertexPositionColorNormal
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public VertexPositionColorNormal(Vector3 pos, Color c)
        {
            Position = pos;
            Color = c;
            Normal = Vector3.Zero;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
     (
         new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
         new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
         new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
     );
    }
}
