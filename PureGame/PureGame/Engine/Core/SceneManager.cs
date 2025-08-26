using System;

namespace PureGame.Engine.Core
{
    public static class SceneManager
    {
        public static Scene? Current { get; private set; }
        private static Scene? _next;

        public static void ChangeScene(Scene newScene)
        {
            _next = newScene;
        }

        internal static void Update(double dt)
        {
            // pending switch
            if (_next != null)
            {
                Current?._Exit();
                Current?.Dispose();
                Current = _next;
                _next = null;
                Current._Enter();
            }

            Current?._Update(dt);
        }

        internal static void Draw()
        {
            Current?._Draw();
        }
    }
}