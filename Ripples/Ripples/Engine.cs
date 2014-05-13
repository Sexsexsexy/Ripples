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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Ripples ripples;
        Point watersize;

        Matrix scalematrix;
        Vector2 scale;

        Vector2 mousedelta;


        int counter;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            spriteBatch = new SpriteBatch(GraphicsDevice);
         
            Input.Initialize();

            ripples.Initialize();

            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            Point resolution = new Point(1280, 720);
            watersize = new Point(256, 144);
            ripples = new Ripples(GraphicsDevice, watersize, resolution);
            ripples.LoadContent(Content);


            graphics.PreferredBackBufferHeight = resolution.Y;
            graphics.PreferredBackBufferWidth = resolution.X;
            graphics.ApplyChanges();

            scale = new Vector2((float)resolution.X / watersize.X, (float)resolution.Y / watersize.Y);
            scalematrix = Matrix.CreateScale(scale.X, scale.Y, 1);
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            if(Input.MouseLeftDown())
            {
                Vector2 mousepos = Input.MouseXY();
                mousepos.X /= scale.X;
                mousepos.Y /= scale.Y;

                int dropwidth = 5;
                int dropheight = 5;

                for (int x = 0; x < dropwidth; x++)
                    for (int y = 0; y < dropheight; y++)
                        ripples.ripple(0.1f, (int)mousepos.X + x, (int)mousepos.Y + y);
            }
            if (Input.MouseRightDown())
            {
                Vector2 mousepos = Input.MouseXY();
                mousepos.X /= scale.X;
                mousepos.Y /= scale.Y;

                int brushwidth = 5;
                int brushheight = 5;

                for (int x = 0; x < brushwidth;x++)
                    for (int y = 0; y < brushheight; y++)
                        ripples.addcoll((int)mousepos.X + x, (int)mousepos.Y + y);

            }

            mousedelta = Input.MouseXY();

            counter++;

            ripples.Update();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue);

            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, scalematrix);

            //ripples.Draw(spriteBatch);

            //spriteBatch.End();
            ripples.Draw3D();

            base.Draw(gameTime);
        }
    }
}
