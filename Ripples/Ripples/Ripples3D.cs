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
        private SimpleCamera2D camera;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;
        private BasicEffect effect;

        private Plane[,] surface;

        public void Initialize()
        {
            effect = new BasicEffect(gd);

            viewMatrix = Matrix.Identity;
            projectionMatrix = Matrix.CreateOrthographic(-1280, -720, 0.0f, -2.0f);
            projectionMatrix *= Matrix.CreateRotationX((float)Math.PI / 4f);
           
            effect.EnableDefaultLighting();
            effect.LightingEnabled = true;

            effect.DirectionalLight0.Direction = new Vector3(0f, 1f, 0f);
            effect.DirectionalLight0.Direction.Normalize();
            effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
            effect.DirectionalLight0.Enabled = true;

            effect.DirectionalLight1.Direction = new Vector3(1f, 0f, 0f);
            effect.DirectionalLight1.Direction.Normalize();
            effect.DirectionalLight1.DiffuseColor = new Vector3(1f,  1f, 1f);
            effect.DirectionalLight1.Enabled = true;

            effect.CurrentTechnique.Passes[0].Apply();
        }

        public void LoadContent(ContentManager content)
        {
            surface = new Plane[width / 2, height / 2];
            Vector2 size = new Vector2(20);

            for (int x = 0; x < surface.GetLength(0); x++)
                for (int y = 0; y < surface.GetLength(1); y++)
                    surface[x, y] = new Plane(new Vector2(-x * size.X,y * size.Y), size);

            for (int x = 0; x < surface.GetLength(0); x++)
                for (int y = 0; y < surface.GetLength(1); y++)
                    surface[x, y].LoadData(content);
        }

        private void setHeight(int x, int y, Plane p)
        {
            float h;

            h = getHeight(x, y);
            p.setHeight(Plane.BOTTOM_RIGHT, h);
            h = getHeight(x + 1, y);
            p.setHeight(Plane.BOTTOM_LEFT, h);
            h = getHeight(x + 1, y + 1);
            p.setHeight(Plane.TOP_LEFT, h);
            h = getHeight(x, y + 1);
            p.setHeight(Plane.TOP_RIGHT, h);
        }

        public void Draw3D()
        {
            for (int x = 0; x < surface.GetLength(0); x++)
                for (int y = 0; y < surface.GetLength(1); y++)
                {
                    setHeight(x, y, surface[x, y]);
                    surface[x, y].Draw(gd, camera.GetView(), projectionMatrix, effect);
                }
        }
    }
}
