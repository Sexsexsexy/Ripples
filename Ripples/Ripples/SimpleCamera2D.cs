    #region SimpleCamera2D
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

    class SimpleCamera2D
    {
        // Public
        public Vector2 Position { get { return pos; } 
            set 
            { 
                Movement = value - Position; Moving = true; pos = value; dirtyView = true; dirtyMinMax = true;
                    //calcMinMax();
            } 
        }
        public Vector2 ResolutionScale { get { return resScale; } private set { if (value != resScale) { resScale = value; dirtyView = true; } } }
        public float Zoom 
        { 
            get { return zoom; } 
            set 
            {
                if(value != zoom)
                {
                    zoom = value;
                    targetZoom = zoom;
                    dirtyView = true;
                } 
            }
        }
        public float Rotation { get { return rot; } set { if (value != rot) { rot = value; dirtyView = true; } } }
        public float ControlInertia = 0.15f;
        public float MoveMultiplier = 1f;
        public Vector2 Min { get { if (dirtyMinMax) calcMinMax(); return min; } private set { min = value; } }
        public Vector2 Max { get { if (dirtyMinMax) calcMinMax(); return max; } private set { max = value; } }
        public float Width { get { return Max.X - Min.X; } }
        public float Height { get { return Max.Y - Min.Y; } }
        public bool PixelClampPosition = false;
        public float ZoomIncrement = 0.8f;
        public Vector2 MouseMoveDelta { get; private set; }
        public Vector2 Movement { get; private set; }
        public bool Moving { get; private set; }
        public Vector2 temporaryuglishitfix;

        // properties
        private Vector2 min, max;
        private Vector2 pos;
        private Vector2 resScale;
        private float zoom = 1, rot = 0;
        private Matrix view, viewInverse;

        // private
        private bool dirtyView = true, dirtyInverse, dirtyMinMax;
        private Vector2 screenCenter;
        private Vector2 targetPos;
        private float targetZoom = 1;
        private MouseState mouseLast;
        private Vector2 mouseLastPos;
        private Point gameRes;
        private Point gameHalf;

        public Matrix GetView()
        {
            if (dirtyView)
            {
                var p = pos;
                if (PixelClampPosition)
                    p = new Vector2((int)pos.X, (int)pos.Y);
                view = Matrix.CreateTranslation(new Vector3(-p, 0)) // move to target
                     * Matrix.CreateScale(new Vector3(Zoom * ResolutionScale.X, Zoom * ResolutionScale.Y, 1)) // scale
                     * Matrix.CreateRotationZ(-Rotation) // rotate
                     * Matrix.CreateTranslation(new Vector3(screenCenter, 0)); // center target
                dirtyView = false;
                dirtyInverse = true;
            }
            return view;
        }
        public Matrix GetViewInverse()
        {
            if (dirtyView)
                GetView();
            if (dirtyInverse)
            {
                Matrix.Invert(ref view, out viewInverse);
                dirtyInverse = false;
            }
            return viewInverse;
        }

        public SimpleCamera2D(Point currentRes, Point gameRes)
        {
            SetResolution(currentRes, gameRes);
        }

        public void SetResolution(Point currentRes, Point gameRes)
        {
            this.gameRes = gameRes;
            this.gameHalf = new Point(gameRes.X / 2, gameRes.Y / 2);
            ResolutionScale = new Vector2((float)currentRes.X / gameRes.X, (float)currentRes.Y / gameRes.Y);
            screenCenter = new Vector2(currentRes.X / 2, currentRes.Y / 2);
        }

        /// <summary>
        /// Updates a standard set of controls that can be used for debug movement.
        /// </summary>
        /// <param name="mouse">Scroll to zoom, middle click to move</param>
        /// <param name="arrows">Arrows to move camera, pageup/pagedown to zoom</param>
        /// <param name="wasd">WASD to move camera, QE to zoom</param>
        public void UpdateControls(bool mouse = true, bool arrows = false, bool wasd = false)
        {

            if (arrows || wasd)
            {
                KeyboardState key = Keyboard.GetState();

                Vector2 v = new Vector2();
                if (arrows)
                {
                    if (key.IsKeyDown(Keys.Left)) v.X = -1;
                    else if (key.IsKeyDown(Keys.Right)) v.X = 1;
                    if (key.IsKeyDown(Keys.Down)) v.Y = 1;
                    else if (key.IsKeyDown(Keys.Up)) v.Y = -1;
                    if (key.IsKeyDown(Keys.PageDown)) targetZoom *= 0.95f;
                    else if (key.IsKeyDown(Keys.PageUp)) targetZoom *= 1.05f;
                }
                if (wasd)
                {
                    if (key.IsKeyDown(Keys.A)) v.X = -1;
                    else if (key.IsKeyDown(Keys.D)) v.X = 1;
                    if (key.IsKeyDown(Keys.S)) v.Y = 1;
                    else if (key.IsKeyDown(Keys.W)) v.Y = -1;
                    if (key.IsKeyDown(Keys.Q)) targetZoom *= 0.95f;
                    else if (key.IsKeyDown(Keys.E)) targetZoom *= 1.05f;
                }

                if (key.IsKeyDown(Keys.NumPad0))
                    Zoom = 1;

                if (v != Vector2.Zero)
                    Move( v * 30 * (1 / zoom) * MoveMultiplier);

                    //targetPos = Position +;
                //Vector2 move = Position.Inertia(targetPos, ControlInertia);
                //if (Math.Abs(move.X) + Math.Abs(move.Y) > 1)
                //    Position = move;
            }



            if (mouse)
            {
                MouseMoveDelta = Vector2.Zero;
                MouseState m = Mouse.GetState();

                if (m.ScrollWheelValue > mouseLast.ScrollWheelValue)
                    targetZoom *= ZoomIncrement;
                if (m.ScrollWheelValue < mouseLast.ScrollWheelValue)
                    targetZoom /= ZoomIncrement;

                Vector2 diff = new Vector2(-m.X + mouseLastPos.X, m.Y - mouseLastPos.Y);
                MouseMoveDelta = -diff * (1 / zoom) * 1;

                if (mouseLast.MiddleButton == ButtonState.Pressed)
                {
                    if (diff != Vector2.Zero)
                    {
                        Move(MouseMoveDelta);
                    }
                }

                if (m.LeftButton == ButtonState.Pressed && m.RightButton == ButtonState.Pressed)
                    Zoom = 1;

                mouseLastPos = new Vector2(m.X, m.Y);
                mouseLast = m;
            }

            if (Math.Abs(targetZoom - zoom) > 0.001f)
            {
                //zoom = zoom.Inertia(targetZoom, 0.25f);
                dirtyView = true;
                dirtyMinMax = true;
                //if (Math.Abs(targetZoom - zoom) <= 0.001f)
                    zoom = targetZoom;
            }

            if (dirtyMinMax)
            {
                calcMinMax();
                dirtyMinMax = false;
            }

        }

        /// <summary>
        /// Clamps the position with the set min/max game area
        /// </summary>
        public void ClampPosition(Vector2 gameMin, Vector2 gameMax)
        {
            if (dirtyView)
                GetView();
            if (dirtyMinMax)
                calcMinMax();

            Vector2 gameSize = gameMax - gameMin;
            Vector2 pos = Position;

            if (gameSize.Y < Max.Y - Min.Y)
                pos.Y = gameSize.Y - gameSize.Y / 2;
            else
            {
                if (Max.Y > gameMax.Y)
                    pos.Y += (gameMax.Y - Max.Y);
                if (Min.Y < gameMin.Y)
                    pos.Y += (gameMin.Y - Min.Y);
            }

            if (gameSize.X < Max.X - Min.X)
                pos.X = gameSize.X - gameSize.X / 2;
            else
            {
                if (Min.X < gameMin.X)
                    pos.X += (gameMin.X - Min.X);
                if (Max.X > gameMax.X)
                    pos.X += (gameMax.X - Max.X);
            }

            Position = pos;
            dirtyMinMax = true;
        }

        public Vector2 FromScreenspace(Vector2 pos)
        {
            return Vector2.Transform(pos, GetViewInverse());
        }

        public void Move(Vector2 delta)
        {
            Moving = true;
            Movement = delta;
            Position += delta;
            targetPos = Position;
        }

        private void calcMinMax()
        {
            var res = new Vector2((1 / ResolutionScale.X) * screenCenter.X / Zoom, (1 / ResolutionScale.Y) * screenCenter.Y / Zoom);

            if (PixelClampPosition)
            {
                //pos = new Vector2((int)pos.X, (int)pos.Y);
                Min = new Vector2((int)(pos.X - res.X), (int)(pos.Y - res.Y));
                Max = new Vector2((int)(pos.X + res.X), (int)(pos.Y + res.Y));
            }
            else
            {
                Min = new Vector2(pos.X - res.X, pos.Y - res.Y);
                Max = new Vector2(pos.X + res.X, pos.Y + res.Y);
            }
        }

    }

    #endregion