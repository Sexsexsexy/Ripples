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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
         

            Input.Initialize();

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Point resolution = new Point(1980, 1080);
            watersize = new Point(512, 288);
            ripples = new Ripples(GraphicsDevice, watersize);

            graphics.PreferredBackBufferHeight = resolution.Y;
            graphics.PreferredBackBufferWidth = resolution.X;
            graphics.ApplyChanges();

            scale = new Vector2((float)resolution.X / watersize.X, (float)resolution.Y / watersize.Y);
            scalematrix = Matrix.CreateScale(scale.X, scale.Y, 1);
            ripples.Load(Content);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Input.Update();

            //if (Input.MouseXY().X != mousedelta.X || Input.MouseXY().Y != mousedelta.Y)
            if(Input.MouseLeftDown())
            {
                Vector2 mousepos = Input.MouseXY();
                mousepos.X /= scale.X;
                mousepos.Y /= scale.Y;

                ripples.ripple(10f, (int)mousepos.X, (int)mousepos.Y);
                ripples.ripple(10f, (int)mousepos.X + 1, (int)mousepos.Y);
                ripples.ripple(10f, (int)mousepos.X, (int)mousepos.Y + 1);
                ripples.ripple(10f, (int)mousepos.X + 1, (int)mousepos.Y + 1);
            }
            if (Input.MouseRightDown())
            {
                Vector2 mousepos = Input.MouseXY();
                mousepos.X /= scale.X;
                mousepos.Y /= scale.Y;

                ripples.addcoll((int)mousepos.X, (int)mousepos.Y);
                ripples.addcoll((int)mousepos.X + 1, (int)mousepos.Y);
                ripples.addcoll((int)mousepos.X, (int)mousepos.Y + 1);
                ripples.addcoll((int)mousepos.X + 1, (int)mousepos.Y + 1);
            }
            mousedelta = Input.MouseXY();

            counter++;

            ripples.Update();

           

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, scalematrix);

            ripples.Draw(spriteBatch);

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
