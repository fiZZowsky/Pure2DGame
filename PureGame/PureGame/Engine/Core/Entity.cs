using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace PureGame.Engine.Core
{
    public class Entity
    {
        public Scene Scene { get; internal set; } = null!;
        public string Name { get; set; }
        public bool Active { get; private set; } = true;

        // Transform 2D
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Rotation { get; set; } = 0f; // radians
        public Vector2 Scale { get; set; } = Vector2.One;

        private readonly List<Component> _components = new();

        public Entity(string name = "Entity") { Name = name; }

        public T AddComponent<T>(T component) where T : Component
        {
            component.Entity = this;
            _components.Add(component);
            component._Initialize();
            component._NotifyAdded();
            _components.Sort(CompareComponents); // sort by UpdateOrder/DrawOrder
            return component;
        }

        public bool RemoveComponent<T>(T component) where T : Component
        {
            if (_components.Remove(component))
            {
                component._NotifyRemoved();
                component.Entity = null!;
                return true;
            }
            return false;
        }

        public T? GetComponent<T>() where T : Component
            => _components.OfType<T>().FirstOrDefault();

        public IEnumerable<T> GetComponents<T>() where T : Component
            => _components.OfType<T>();

        internal void Update(double dt)
        {
            if (!Active) return;
            foreach (var c in _components)
                if (c.Enabled) c.Update(dt);
        }

        internal void Draw()
        {
            if (!Active) return;
            foreach (var c in _components)
                if (c.Visible) c.Draw();
        }

        public void SetActive(bool active) => Active = active;

        private static int CompareComponents(Component a, Component b)
        {
            int u = a.UpdateOrder.CompareTo(b.UpdateOrder);
            if (u != 0) return u;
            return a.DrawOrder.CompareTo(b.DrawOrder);
        }
    }
}
