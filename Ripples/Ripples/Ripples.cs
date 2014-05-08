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
    class Ripples
    {
        public Texture2D texture { get; private set; }

        private float[,] Buffer1;
        private float[,] Buffer2;
        private bool[,] collisiongrid;

        private Color[] heightmap;

        private float dampening;

        private int width;
        private int height;

        GraphicsDevice gd;

        public Ripples(GraphicsDevice gd, Point size)
        {
            this.gd = gd;
            width = size.X;
            height = size.Y;

            Buffer1 = new float[width, height];
            Buffer2 = new float[width, height];
            collisiongrid = new bool[width, height];

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        if (y == width / 4 || y == width * 3 / 4)
            //            collisiongrid[x, y] = true;
            //        if (x == height / 4 || x == height * 3 / 4)
            //            collisiongrid[x, y] = true;
            //    }
            //}

            heightmap = new Color[size.X * size.Y];

            texture = new Texture2D(gd, size.X, size.Y, true, SurfaceFormat.Color);
            //texture = new Texture2D(gd, size.X, size.Y, true, SurfaceFormat.Color);

            dampening = 0.96f;
        }

        public void Load(ContentManager c)
        {
            //texture = c.Load<Texture2D>("pixel");
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch sb)
        {
            //for (int x = 0; x < pixels.GetLength(0); x++)
            //    for (int y = 0; y < pixels.GetLength(1); y++)
            //        sb.Draw(texture, new Vector2(x - 1, y - 1), pixels[x, y]);
            //sliceprocess();

            process();
            sb.Draw(texture, -Vector2.One, Color.White);
        }

        private void process()
        {
            gd.Textures[0] = null;

            int width = Buffer1.GetLength(0);
            int height = Buffer1.GetLength(1);

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    Color c = Color.Transparent;

                    if (!collisiongrid[x,y])
                    {
                        //float newheight =
                        //    (Buffer1[x - 1, y]
                        //    + Buffer1[x + 1, y]
                        //    + Buffer1[x, y + 1]
                        //    + Buffer1[x, y - 1]
                        //    + Buffer1[x + 1, y - 1]
                        //    + Buffer1[x + 1, y + 1]
                        //    + Buffer1[x - 1, y - 1]
                        //    + Buffer1[x - 1, y + 1]) / 8 - Buffer2[x, y];


                        float newheight =
                            (Buffer1[x - 1, y]
                            + Buffer1[x + 1, y]
                            + Buffer1[x, y + 1]
                            + Buffer1[x, y - 1]) / 4 - Buffer2[x, y];

                        Buffer2[x, y] = newheight * dampening;


                        float val = Buffer2[x, y];

                        if (val > 0.01f)
                        {
                            c = Color.White;
                            c.A = (byte)(255 * val);
                        }
                    }
                    else
                        c = Color.Black;

                    heightmap[x + y * width] = c;
                }
            }

            texture.SetData<Color>(heightmap);

            float[,] temp = Buffer1;
            Buffer1 = Buffer2;
            Buffer2 = temp;
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
