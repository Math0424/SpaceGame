using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine
{
    internal class Input
    {
        public enum MouseButtons
        {
            MiddleButton,
            LeftButton,
            RightButton,
        }

        private static Keys[] _keysLastFrame = new Keys[0];
        private static MouseState _mouseLastFrame = default(MouseState);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 MousePosition()
        {
            return Mouse.GetState().Position.ToVector2();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyDown(Keys k)
        {
            return Keyboard.GetState().IsKeyDown(k);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyUp(Keys k)
        {
            return Keyboard.GetState().IsKeyUp(k);
        }

        public static bool IsNewKeyDown(Keys k)
        {
            return !_keysLastFrame.Contains(k) && IsKeyDown(k);
        }

        public static bool IsNewKeyUp(Keys k)
        {
            return !_keysLastFrame.Contains(k) && IsKeyUp(k);
        }

        public static bool IsMouseDown(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.MiddleButton:
                    return Mouse.GetState().MiddleButton == ButtonState.Pressed;
                case MouseButtons.RightButton:
                    return Mouse.GetState().RightButton == ButtonState.Pressed;
                case MouseButtons.LeftButton:
                    return Mouse.GetState().LeftButton == ButtonState.Pressed;
            }
            return false;
        }

        public static bool IsMouseUp(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.MiddleButton:
                    return Mouse.GetState().MiddleButton == ButtonState.Released;
                case MouseButtons.RightButton:
                    return Mouse.GetState().RightButton == ButtonState.Released;
                case MouseButtons.LeftButton:
                    return Mouse.GetState().LeftButton == ButtonState.Released;
            }
            return false;
        }

        public static bool IsNewMouseDown(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.MiddleButton:
                    return _mouseLastFrame.MiddleButton == ButtonState.Released && Mouse.GetState().MiddleButton == ButtonState.Pressed;
                case MouseButtons.RightButton:
                    return _mouseLastFrame.RightButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Pressed;
                case MouseButtons.LeftButton:
                    return _mouseLastFrame.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed;
            }
            return false;
        }

        public static bool IsNewMouseUp(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.MiddleButton:
                    return _mouseLastFrame.MiddleButton == ButtonState.Pressed && Mouse.GetState().MiddleButton == ButtonState.Released;
                case MouseButtons.RightButton:
                    return _mouseLastFrame.RightButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Released;
                case MouseButtons.LeftButton:
                    return _mouseLastFrame.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MouseWheel()
        {
            return Mouse.GetState().ScrollWheelValue;
        }

        public static int MouseWheelDelta()
        {
            return Mouse.GetState().ScrollWheelValue - _mouseLastFrame.ScrollWheelValue;
        }

        public static void UpdateState()
        {
            _keysLastFrame = Keyboard.GetState().GetPressedKeys();
            _mouseLastFrame = Mouse.GetState();
        }

    }
}
