using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine.Components;
using Project1.Engine.Systems;
using Project1.Engine.Systems.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine
{
    internal class World : GameComponent, IDrawable
    {
        public Action GameFocused;
        // public Game Game { private set; get; }
        public int EntityCount => _entities.Count;

        private SparceIndexedList<Entity> _entities;
        private Dictionary<Type, List<EntityComponent>> _components;
        private Dictionary<Type, SystemComponent> _systems;
        private bool _focused;
        private InjectionContainer _injectionContainer;

        public World(Game game) : base(game)
        {
            //Game = game;
            
            _focused = game.IsActive;
            _entities = new SparceIndexedList<Entity>();
            _components = new Dictionary<Type, List<EntityComponent>>();
            _systems = new Dictionary<Type, SystemComponent>();

            _injectionContainer = new InjectionContainer();
            _injectionContainer.RegisterClass(game);
            _injectionContainer.RegisterClass(this);
        }

        /// <summary>
        /// Hot path for getting the render, will crash if none are loaded
        /// </summary>
        public RenderingSystem Render => (RenderingSystem)_systems[typeof(RenderingSystem)];


        public void AddInjectedType(object obj)
        {
            _injectionContainer.RegisterClass(obj);
        }

        public World AddSystem<T>() where T : SystemComponent
        {
            T obj = _injectionContainer.Resolve<T>();
            _systems[obj.GetType()] = obj;
            return this;
        }

        public World AddSystem(SystemComponent system)
        {
            _injectionContainer.RegisterClass(system);
            _systems[system.GetType()] = system;
            return this;
        }

        public List<Entity> GetEntities()
        {
            var ents = new List<Entity>(_entities.GetContents());
            ents.RemoveAll(e => e == null);
            return ents;
        }

        public Entity GetEntity(int Id)
        {
            return _entities[Id];
        }

        public unsafe T[] GetEntityComponents<T> () where T : EntityComponent
        {
            if (_components.ContainsKey(typeof(T)))
            {
                // TODO : this might be too slow
                return Unsafe.As<T[]>(_components[typeof(T)].ToArray());
            }
            return null;
        }

        public T GetSystem<T>() where T : SystemComponent
        {
            if (_systems.ContainsKey(typeof(T)))
            {
                return (T)_systems[typeof(T)];
            }
            return null;
        }

        public T RegisterEntityComponent<T>(T component) where T : EntityComponent
        {
            Type t = typeof(T);
            
            //TODO : figure a way to remove this silly hack
            if (t.BaseType != typeof(EntityComponent))
                t = t.BaseType;
            
            if (!_components.ContainsKey(t))
                _components[t] = new List<EntityComponent>();
            _components[t].Add(component);
            return component;
        }

        public void UnRegisterEntityComponent(EntityComponent component)
        {
            foreach (var x in _components)
                if (x.Key.IsAssignableFrom(component.GetType()))
                    if (x.Value.Remove(component))
                        return;
            throw new Exception($"Component {component.GetType()} is not registered");
        }

        public Entity CreateEntity()
        {
            Entity entity = new Entity(this);
            entity.Id = _entities.Add(entity);
            return entity;
        }

        public void RemoveEntity(int id)
        {
            _entities.Remove(id);
        }

        public override void Update(GameTime deltaTime)
        {
            if (_focused != Game.IsActive)
            {
                _focused = Game.IsActive;
                if (Game.IsActive)
                    GameFocused?.Invoke();
            }

            var entUpdates = GetEntityComponents<EntityUpdateComponent>();
            if (entUpdates != null)
                foreach (var x in GetEntityComponents<EntityUpdateComponent>())
                    x.Update(deltaTime);

            foreach (var x in _systems.Values)
                x.Update(deltaTime);
        }

        public void Draw(GameTime deltaTime)
        {
            foreach (var x in _systems.Values)
            {
                if (x.GetType().IsAssignableTo(typeof(IDrawUpdate)))
                    ((IDrawUpdate)x).Draw(deltaTime);
            }
        }

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine($"Closing world with {_entities.Count} entities, {_systems.Count} systems and, {_components.Count} registered components");
            foreach (var x in _systems.Values)
                x.Close();
            foreach (var x in GetEntities())
                x.Close();
            _entities.Clear();
            _components.Clear();
            _systems.Clear();
            _injectionContainer = null;
            base.Dispose(disposing);
        }


        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public int DrawOrder => 1;
        public bool Visible => true;


    }
}
