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
    public static class Input
    {
        static KeyboardState currState;
        static KeyboardState prevState;

        static MouseState curmouseState;
        static MouseState prevmouseState;

        public static bool IsKeyDown(Keys key)
        {
            return currState.IsKeyDown(key);
        }
        public static bool WasKeyPressed(Keys key)
        {
            return currState.IsKeyUp(key) && prevState.IsKeyDown(key);
        }

        static List<GamePadState> currGStates = new List<GamePadState>();
        static List<GamePadState> prevGStates = new List<GamePadState>();

        public static void Initialize()
        {
            for (int i = 0; i < 4; i++)
            {
                currGStates.Add(new GamePadState());
                prevGStates.Add(new GamePadState());
            }
        }

        public static void Update()
        {
            prevState = currState;
            currState = Keyboard.GetState();

            //prevGState = currGState;
            //currGState = GamePad.GetState(PlayerIndex.One);

            for (int i = 0; i < currGStates.Count; i++)
            {
                prevGStates[i] = currGStates[i];
                currGStates[i] = GamePad.GetState((PlayerIndex)i);
            }

            prevmouseState = curmouseState;
            curmouseState = Mouse.GetState();
        }

        public static Vector2 MouseXY()
        {
            return new Vector2(curmouseState.X, curmouseState.Y);
        }

        public static bool MouseLeftDown()
        {
            if (curmouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                return true;

            return false;
        }

        public static bool MouseRightDown()
        {
            if (curmouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                return true;

            return false;
        }

        public static bool MouseLeftPressed()
        {
            if (curmouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && prevmouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                return true;

            return false;
        }

        public static bool MouseRightPressed()
        {
            if (curmouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released && prevmouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                return true;

            return false;
        }
    }
}
