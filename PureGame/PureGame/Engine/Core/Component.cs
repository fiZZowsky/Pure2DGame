using System;

namespace PureGame.Engine.Core
{
    public abstract class Component : IUpdatable, IDrawable
    {
        public Entity Entity { get; internal set; } = null!;
        public Scene Scene => Entity.Scene;

        public bool Enabled { get; set; } = true;
        public int UpdateOrder { get; set; } = 0;

        public bool Visible { get; set; } = true;
        public int DrawOrder { get; set; } = 0;

        internal bool IsInitialized { get; private set; }

        // Lifecycle
        internal void _Initialize()
        {
            if (!IsInitialized)
            {
                Initialize();
                IsInitialized = true;
            }
        }

        protected virtual void Initialize() { }
        protected virtual void OnAddedToEntity() { }
        protected virtual void OnRemovedFromEntity() { }
        protected virtual void OnEnabled() { }
        protected virtual void OnDisabled() { }

        public virtual void Update(double dt) { }
        public virtual void Draw() { }

        internal void _NotifyAdded() => OnAddedToEntity();
        internal void _NotifyRemoved() => OnRemovedFromEntity();

        public void SetEnabled(bool enabled)
        {
            if (Enabled == enabled) return;
            Enabled = enabled;
            if (Enabled) OnEnabled(); else OnDisabled();
        }
    }
}