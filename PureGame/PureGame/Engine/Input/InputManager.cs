using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;

namespace PureGame.Engine.Input
{
    public static class InputManager
    {
        private static KeyboardState _keyboardCurrent;
        private static KeyboardState _keyboardPrevious;

        private static MouseState _mouseCurrent;
        private static MouseState _mousePrevious;

        private static Vector2 _mouseDelta;
        private static bool _hasPrevFrame = false;
        
        public static void Update(KeyboardState keyboard, MouseState mouse)
        {
            if (_hasPrevFrame)
            {
                _keyboardPrevious = _keyboardCurrent;
                _mousePrevious = _mouseCurrent;
            }

            _keyboardCurrent = keyboard;
            _mouseCurrent = mouse;

            _mouseDelta = _hasPrevFrame
                ? _mouseCurrent.Position - _mousePrevious.Position
                : Vector2.Zero;

            _hasPrevFrame = true;
        }

        // ===== Klawiatura =====
        public static bool IsKeyDown(Keys key) => _keyboardCurrent.IsKeyDown(key);
        public static bool IsKeyUp(Keys key) => !_keyboardCurrent.IsKeyDown(key);

        public static bool IsKeyPressed(Keys key) =>
            _hasPrevFrame && _keyboardCurrent.IsKeyDown(key) && !_keyboardPrevious.IsKeyDown(key);

        public static bool IsKeyReleased(Keys key) =>
            _hasPrevFrame && !_keyboardCurrent.IsKeyDown(key) && _keyboardPrevious.IsKeyDown(key);

        // ===== Mysz =====
        public static bool IsMouseButtonDown(MouseButton button) => _mouseCurrent.IsButtonDown(button);
        public static bool IsMouseButtonUp(MouseButton button) => !_mouseCurrent.IsButtonDown(button);

        public static bool IsMouseButtonPressed(MouseButton button) =>
            _hasPrevFrame && _mouseCurrent.IsButtonDown(button) && !_mousePrevious.IsButtonDown(button);

        public static bool IsMouseButtonReleased(MouseButton button) =>
            _hasPrevFrame && !_mouseCurrent.IsButtonDown(button) && _mousePrevious.IsButtonDown(button);

        public static Vector2 MousePosition => _mouseCurrent.Position;
        public static Vector2 MouseDelta => _mouseDelta;

        // Uwaga: ScrollDelta to wektor; dla pionowego – oś Y
        public static float ScrollDelta => _mouseCurrent.ScrollDelta.Y;
        
        public static void Reset()
        {
            _keyboardCurrent = default;
            _keyboardPrevious = default;
            _mouseCurrent = default;
            _mousePrevious = default;
            _mouseDelta = Vector2.Zero;
            _hasPrevFrame = false;
        }
    }
}
