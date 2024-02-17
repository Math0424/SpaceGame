using BulletSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Engine.Components;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems
{
    internal class PhysicsSystem : SystemComponent
    {
        public DiscreteDynamicsWorld World;
        /// <summary>
        /// [ent] collided with [ent] at [pos] with [normal] and [force]
        /// </summary>
        public Action<int, int, Vector3, Vector3, float> Collision;

        private World _world;
        private double _accumulatedTime;
        private const float physicsTimeStep = 0.005f;

        private bool _debugMode;
        private Camera _camera;

        private CollisionConfiguration _collisionConfiguration;
        private CollisionDispatcher _dispatcher;
        private DbvtBroadphase _broadphase;

        public override void Close()
        {
            World.Dispose();
            _collisionConfiguration.Dispose();
            _dispatcher.Dispose();
            _broadphase.Dispose();
            Render.PreDraw -= DebugDraw;
        }

        public PhysicsSystem(World world, Camera camera)
        {
            _world = world;
            _debugMode = false;
            _camera = camera;

            _collisionConfiguration = new DefaultCollisionConfiguration();
            _dispatcher = new CollisionDispatcher(_collisionConfiguration);
            _broadphase = new DbvtBroadphase();
            World = new DiscreteDynamicsWorld(_dispatcher, _broadphase, null, _collisionConfiguration);
            World.Gravity = new BulletSharp.Math.Vector3(0, 0, 0);

            Render.PreDraw += DebugDraw;
        }

        public void DebugDraw()
        {
            if (_debugMode)
            {
                DrawingUtils.DrawWorldText(_camera, $"{World.NumCollisionObjects} physics objects", Vector3.Zero, Color.Thistle);

                var physicsObjects = _world.GetEntityComponents<PrimitivePhysicsComponent>();
                if (physicsObjects == null)
                    return;

                foreach (var physicObject in physicsObjects)
                    physicObject.DebugDraw();
            }
        }

        public override void Update(GameTime delta)
        {
            if (Input.IsNewKeyDown(Keys.F12))
                _debugMode = !_debugMode;

            _accumulatedTime += delta.ElapsedGameTime.TotalSeconds;
            while (_accumulatedTime >= physicsTimeStep)
            {
                World.StepSimulation(physicsTimeStep);
                _accumulatedTime -= physicsTimeStep;
            }

            var physicsObjects = _world.GetEntityComponents<PrimitivePhysicsComponent>();
            if (physicsObjects == null)
                return;
            foreach (var x in physicsObjects)
                if (x.EntityId != -1)
                    x.UpdateWorldMatrix();

            for (int i = 0; i < World.Dispatcher.NumManifolds; i++)
            {
                PersistentManifold contactManifold = World.Dispatcher.GetManifoldByIndexInternal(i);
                if (contactManifold.NumContacts != 0)
                {
                    CollisionObject obA = contactManifold.Body0;
                    CollisionObject obB = contactManifold.Body1;
                    ManifoldPoint pt = contactManifold.GetContactPoint(0);
                    Collision?.Invoke(obA.UserIndex, obB.UserIndex, pt.PositionWorldOnA.ToXNA(), pt.NormalWorldOnB.ToXNA(), (float)pt.AppliedImpulse);
                }
            }

        }
    }
}
