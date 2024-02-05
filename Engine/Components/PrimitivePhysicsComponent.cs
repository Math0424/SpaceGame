﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Project1.Engine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project1.Engine.Systems;
using Project1.Engine.Systems.RenderMessages;
using BulletSharp;

namespace Project1.Engine.Components
{
    public enum RigidBodyFlags
    {
        Static,
        Dynamic,
        Kinematic,
    }

    public enum PhysicsLayer
    {
        Default,
        Trigger,
    }

    internal enum RigidBodyType
    {
        Sphere,
        Capsule,
        Cylinder,
        Box,
        Plane,
    }

    internal class PrimitivePhysicsComponent : EntityComponent
    {
        public PhysicsLayer PhysicsLayer;
        public bool IsEnabled;
        public bool IsSleeping => !_rigidBody.IsActive;
        public int EntityId => _entity.Id;

        private float _userRadius;
        private float _userMass;

        private RigidBodyType _type;
        private RigidBodyFlags _flags;

        private RigidBody _rigidBody;

        /// <summary>
        /// Entity intersecting with [entity] at [position]
        /// </summary>
        public Action<int, Vector3> Intersecting;
        /// <summary>
        /// Entity collided with [entity] at [position] [velocity] [normal] with [force]
        /// </summary>
        public Action<int, Vector3, Vector3, Vector3, float> Collision;

        public PrimitivePhysicsComponent(RigidBodyType type)
        {
            _type = type;
            _flags = RigidBodyFlags.Static;
            _userRadius = -1;
            _userMass = 0;
        }

        public PrimitivePhysicsComponent(RigidBodyType type, RigidBodyFlags flags, float mass, float radius = -1)
        {
            _type = type;
            _flags = flags;
            _userMass = mass;
            _userRadius = radius;
        }

        public override void Initalize()
        {
            var sim = _entity.World.GetSystem<PhysicsSystem>().World;

            Matrix transform = _entity.Position.WorldMatrix;

            float length = transform.Right.Length();
            float width = transform.Forward.Length();
            float height = transform.Up.Length();

            ConvexInternalShape shape = null;
            switch (_type)
            {
                case RigidBodyType.Box:
                    shape = new BoxShape(transform.HalfExtents().ToBullet());
                    break;
                case RigidBodyType.Sphere:
                    shape = new SphereShape(_userRadius == -1 ? length : _userRadius);
                    break;
                case RigidBodyType.Capsule:
                    shape = new CapsuleShape(width, height);
                    break;
                case RigidBodyType.Cylinder:
                    shape = new CylinderShape(transform.HalfExtents().ToBullet());
                    break;
            }

            Quaternion quat;
            Vector3 pos;
            transform.Decompose(out _, out quat, out pos);
            transform = Matrix.CreateFromQuaternion(quat) * Matrix.CreateTranslation(pos);

            //figure out what to do if its kinematic
            if (_flags == RigidBodyFlags.Static)
            {
                using (var bodyInfo = new RigidBodyConstructionInfo(0, null, shape)
                {
                    AngularDamping = 0.5,
                    LinearDamping = 0.1,
                    StartWorldTransform = transform.ToBullet(),
                })
                {
                    _rigidBody = new RigidBody(bodyInfo);
                }
            } 
            else
            {
                using (var bodyInfo = new RigidBodyConstructionInfo(_userMass, new DefaultMotionState(transform.ToBullet()), shape, shape.CalculateLocalInertia(_userMass))
                {
                    AngularDamping = 0.5,
                    LinearDamping = 0.1,
                })
                {
                    _rigidBody = new RigidBody(bodyInfo);
                }
            }
            sim.AddRigidBody(_rigidBody);
        }

        public void SetWorldMatrix(Matrix matrix)
        {
            _rigidBody.WorldTransform = matrix.ToBullet();
        }

        public void UpdateWorldMatrix()
        {
            if (_flags == RigidBodyFlags.Static)
                return;
            _entity.Position.SetWorldMatrix(_rigidBody.WorldTransform.ToXNA());
        }

        public Vector3 VelocityAtPoint(Vector3 point)
        {
            return _rigidBody.GetVelocityInLocalPoint(point.ToBullet()).ToXNA();
        }

        public void AddTorque(Vector3 torque)
        {
            _rigidBody.ActivationState = ActivationState.ActiveTag;
            _rigidBody.ApplyTorqueImpulse(torque.ToBullet());
        }

        public void AddForce(Vector3 force)
        {
            _rigidBody.ActivationState = ActivationState.ActiveTag;
            _rigidBody.ApplyCentralImpulse(force.ToBullet());
        }

        public void AddImpulse(Vector3 force, Vector3 relPos)
        {
            _rigidBody.ActivationState = ActivationState.ActiveTag;
            _rigidBody.ApplyImpulse(force.ToBullet(), relPos.ToBullet());
        }

        public void DebugDraw(RenderingSystem render)
        {
            var pos = _entity.Position;
            
            DrawingUtils.DrawMatrix(render, pos.WorldMatrix);

            BulletSharp.Math.Vector3 bMin;
            BulletSharp.Math.Vector3 bMax;
            _rigidBody.GetAabb(out bMin, out bMax);

            Vector3 size = bMax.ToXNA() - bMin.ToXNA();

            Matrix aabb = Matrix.CreateTranslation(pos.WorldMatrix.Translation);
            aabb.Forward = new Vector3(size.X, 0, 0);
            aabb.Up = new Vector3(0, size.Y, 0);
            aabb.Right = new Vector3(0, 0, size.Z);
            render.EnqueueMessage(new RenderMessageDrawBox(aabb));

            if (_flags != RigidBodyFlags.Static)
            {
                DrawingUtils.DrawLine(render, pos.Position, _rigidBody.AngularVelocity.ToXNA(), Color.Orange);
                DrawingUtils.DrawLine(render, pos.Position, _rigidBody.LinearVelocity.ToXNA(), Color.Pink);
                DrawingUtils.DrawWorldText(render, $"ID: {_entity.Id}\n AngV: {Math.Round(_rigidBody.AngularVelocity.Length, 2)}\nLinV: {Math.Round(_rigidBody.LinearVelocity.Length, 2)}", pos.Position, IsSleeping ? Color.Blue : Color.Orange);
            }
            else
                DrawingUtils.DrawWorldText(render, $"ID: {_entity.Id}", pos.Position, Color.Yellow);
        }

        public void Stop()
        {
            _rigidBody.AngularVelocity = BulletSharp.Math.Vector3.Zero;
            _rigidBody.LinearVelocity = BulletSharp.Math.Vector3.Zero;
        }

    }
}
