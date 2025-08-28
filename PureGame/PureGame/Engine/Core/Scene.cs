using System;
using System.Collections.Generic;

namespace PureGame.Engine.Core
{
    public abstract class Scene : IDisposable
    {
        private readonly List<Entity> _entities = new();
        public bool IsLoaded { get; private set; }

        public virtual void LoadContent() { }   // ładowanie assetów
        public virtual void UnloadContent() { } // zwalnianie assetów

        public T CreateEntity<T>(string name = "Entity") where T : Entity, new()
        {
            var e = new T { Name = name, Scene = this };
            _entities.Add(e);
            return e;
        }

        public Entity CreateEntity(string name = "Entity")
        {
            var e = new Entity(name) { Scene = this };
            _entities.Add(e);
            return e;
        }

        public bool RemoveEntity(Entity e) => _entities.Remove(e);

        internal void _Enter()
        {
            if (!IsLoaded)
            {
                LoadContent();
                IsLoaded = true;
            }
            OnEnter();
        }

        internal void _Exit()
        {
            OnExit();
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnResize(int width, int height) { }

        internal void _Update(double dt)
        {
            Update(dt);
            foreach (var e in _entities) e.Update(dt);
            LateUpdate(dt);
        }

        internal void _Draw()
        {
            Draw();
            foreach (var e in _entities) e.Draw();
            PostDraw();
        }

        public virtual void Update(double dt) { }
        public virtual void LateUpdate(double dt) { }
        public virtual void Draw() { }
        public virtual void PostDraw() { }

        public virtual void Dispose()
        {
            UnloadContent();
            _entities.Clear();
        }
    }
}