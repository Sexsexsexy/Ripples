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
        private Texture2D texture;

        private float[,] Buffer1;
        private float[,] Buffer2;
        private bool[,] collisiongrid;

        private Color[] colormap;

        private float dampening;

        private int width;
        private int height;

        private GraphicsDevice gd;

        public Ripples(GraphicsDevice gd, Point size, Point realres)
        {
            this.gd = gd;
            width = size.X;
            height = size.Y;

            Buffer1 = new float[width, height];
            Buffer2 = new float[width, height];
            collisiongrid = new bool[width, height];

            colormap = new Color[size.X * size.Y];

            texture = new Texture2D(gd, size.X, size.Y, true, SurfaceFormat.Color);

            dampening = 0.97f;

            camera = new SimpleCamera2D(realres, size);
        }

        public void Update()
        {
            camera.UpdateControls();

            process();
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(getTexture(), -Vector2.One, Color.White);
        }

        private void process()
        {
            int width = Buffer1.GetLength(0);
            int height = Buffer1.GetLength(1);

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    Color c = Color.Transparent;

                    if (!collisiongrid[x,y])
                    {
                        float newheight =
                            (Buffer1[x - 1, y]
                            + Buffer1[x + 1, y]
                            + Buffer1[x, y + 1]
                            + Buffer1[x, y - 1]) / 4 - Buffer2[x, y];

                        Buffer2[x, y] = newheight * dampening;

                        float val = Buffer2[x, y];
                        float drawlimit = 0.01f;

                        if (val > drawlimit)
                        {
                            c = Color.Azure;
                            c.A = (byte)(255 * (val - drawlimit));
                        }
                        else if (val < -drawlimit)
                        {
                            c = Color.CornflowerBlue;
                            c.A = (byte)(255 * -(val - drawlimit));
                        }
                    }
                    else
                        c = Color.Black;

                    colormap[x + y * width] = c;
                }
            }

            float[,] temp = Buffer1;
            Buffer1 = Buffer2;
            Buffer2 = temp;
        }

        public float getHeight(int x, int y)
        {
            if (!(x > 0 && x < Buffer1.GetLength(0) - 1
            && y > 0 && y < Buffer1.GetLength(1) - 1))
                return 0;

            return Buffer1[x, y];
        }

        public Texture2D getTexture()
        {
            gd.Textures[0] = null;
            texture.SetData<Color>(colormap);

            return texture;

        }

        public void ripple(float amount, int x, int y)
        {
            if (!(x > 0 && x < Buffer1.GetLength(0) - 1
                && y > 0 && y < Buffer1.GetLength(1) - 1))
                return;

            Buffer2[x, y] = amount;
        }
       
        public void addcoll(int x, int y)
        {
            if (!(x - 1 > 0 && x + 1 < collisiongrid.GetLength(0) - 1
                && y - 1 > 0 && y + 1 < collisiongrid.GetLength(1) - 1))
                return;

            //collisiongrid[x, y] = !collisiongrid[x, y];
            collisiongrid[x, y] = true;
        }
    }
}
