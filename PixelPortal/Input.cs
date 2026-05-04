using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PixelPortal
{
    public class Input
    {
        private KeyboardState pks = Keyboard.GetState();
        private KeyboardState ks = Keyboard.GetState();
        private MouseState pms = Mouse.GetState();
        private MouseState ms = Mouse.GetState();

        public Point mousePosition => ms.Position;


        public void UpdateNoDelta()
        {
            pks = ks;
            pms = ms;
            ks = Keyboard.GetState();
            ms = Mouse.GetState();
            
        }

        public bool IsKeyDown(Keys key)
        {
            return ks.IsKeyDown(key);
        }

        
        public bool IsKeyUp(Keys key)
        {
            return ks.IsKeyUp(key);
        }

        public bool IsKeyJustPressed(Keys key)
        {
            return pks.IsKeyUp(key) && ks.IsKeyDown(key);
        }
        public bool IsKeyJustReleased(Keys key)
        {
            return ks.IsKeyUp(key) && pks.IsKeyDown(key);
        }

        //from monogame docs

        public enum MouseButton
        {
            Left,
            Middle,
            Right,
            XButton1,
            XButton2
        }

        /// <summary>
        /// Returns a value that indicates whether the specified mouse button is currently down.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>true if the specified mouse button is currently down; otherwise, false.</returns>
        public bool IsButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return ms.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return ms.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return ms.RightButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return ms.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return ms.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the specified mouse button is current up.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>true if the specified mouse button is currently up; otherwise, false.</returns>
        public bool IsButtonUp(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return ms.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return ms.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return ms.RightButton == ButtonState.Released;
                case MouseButton.XButton1:
                    return ms.XButton1 == ButtonState.Released;
                case MouseButton.XButton2:
                    return ms.XButton2 == ButtonState.Released;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the specified mouse button was just pressed on the current frame.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>true if the specified mouse button was just pressed on the current frame; otherwise, false.</returns>
        public bool WasButtonJustPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return ms.LeftButton == ButtonState.Pressed && pms.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return ms.MiddleButton == ButtonState.Pressed && pms.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return ms.RightButton == ButtonState.Pressed && pms.RightButton == ButtonState.Released;
                case MouseButton.XButton1:
                    return ms.XButton1 == ButtonState.Pressed && pms.XButton1 == ButtonState.Released;
                case MouseButton.XButton2:
                    return ms.XButton2 == ButtonState.Pressed && pms.XButton2 == ButtonState.Released;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the specified mouse button was just released on the current frame.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>true if the specified mouse button was just released on the current frame; otherwise, false.</returns>
        public bool WasButtonJustReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return ms.LeftButton == ButtonState.Released && pms.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return ms.MiddleButton == ButtonState.Released && pms.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return ms.RightButton == ButtonState.Released && pms.RightButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return ms.XButton1 == ButtonState.Released && pms.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return ms.XButton2 == ButtonState.Released && pms.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }


    }
}
