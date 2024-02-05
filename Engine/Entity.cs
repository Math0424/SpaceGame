using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine
{
    internal class Entity
    {
        private static FieldInfo entityField = typeof(EntityComponent).GetField("_entity", BindingFlags.Instance | BindingFlags.NonPublic);

        public int Id { get; set; }
        public World World { get; private set; }
        private Dictionary<Type, EntityComponent> _components;
        // I dont like this, figure another way - but its so much faster~
        private PositionComponent _cachedPos;

        public Entity(World world)
        {
            World = world;
            _components = new Dictionary<Type, EntityComponent>();
        }

        /// <summary>
        /// Used often enough to justify this shorthand
        /// !! will crash if the entity does not have a PositionComponent !!
        /// </summary>
        public PositionComponent Position
        {
            get
            {
                if (_cachedPos == null)
                    _cachedPos = (PositionComponent)_components[typeof(PositionComponent)];
                return _cachedPos;
            }
        }

        public Entity AddComponent<T>(T obj) where T : EntityComponent
        {
            _components[typeof(T)] = obj;
            World.RegisterEntityComponent(obj);
            entityField.SetValue(obj, this);
            obj.Initalize();
            return this;
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            if (!_components.ContainsKey(typeof(T)))
                return null;
            return (T)_components[typeof(T)];
        }

        public void RemoveComponent(Type t)
        {
            if (_components.ContainsKey(t))
            {
                World.UnRegisterEntityComponent(_components[t]);
                _components.Remove(t);
                _cachedPos = null;
            }
        }

        public void Close() {
            foreach(var x in _components)
            {
                World.UnRegisterEntityComponent(x.Value);
                x.Value.Close();
            }
            _components.Clear();
            _cachedPos = null;
            World.RemoveEntity(Id);
            Id = -1;
        }
    }
}
