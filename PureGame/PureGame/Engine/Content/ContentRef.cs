using System;

namespace PureGame.Engine.Content
{
    public readonly struct ContentRef<T> : IDisposable where T : class
    {
        public T Asset { get; }
        private readonly Action _onDispose;

        public ContentRef(T asset, Action onDispose)
        {
            Asset = asset;
            _onDispose = onDispose;
        }

        public void Dispose() => _onDispose?.Invoke();
    }
}