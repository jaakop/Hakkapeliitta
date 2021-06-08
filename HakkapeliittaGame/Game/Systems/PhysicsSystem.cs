using System;
using System.Collections.Generic;

using MGPhysics;
using ReeGame.Components;

namespace ReeGame.Systems
{
    public static class PhysicsSystem
    {
        /// <summary>
        /// Moves an object and checks for collission on it
        /// </summary>
        /// <param name="entityKey">The entity to move</param>
        /// <param name="positions">Dictionary on entity positions</param>
        /// <param name="hitBoxes">Dictionary on entity hit boxes</param>
        /// <param name="velocity">Velocity of the entity</param>
        /// <returns>A list of the entities, that were collided with</returns>
        public static List<Entity> MoveEntity(Entity entity, Vector velocity, ref Dictionary<Entity, Transform> transforms, Dictionary<Entity, RigidBody> rigidBodies)
        {
            Vector position = transforms[entity].Position;
            Vector hitbox = rigidBodies[entity].HitBox;
            Vector adjustedPosition = transforms[entity].Position + velocity;

            List<Entity> collidedEntities = new List<Entity>();

            foreach (KeyValuePair<Entity, Transform> targetObject in transforms)
            {
                if (entity == targetObject.Key)
                    continue;

                if (!rigidBodies.ContainsKey(targetObject.Key))
                    continue;

                Vector targetPosition = targetObject.Value.Position;
                Vector targetHitBox = rigidBodies[targetObject.Key].HitBox;

                if (CheckCollissions(adjustedPosition, hitbox, targetPosition, targetHitBox))
                {
                    collidedEntities.Add(targetObject.Key);

                    int distX = (int)Math.Round(Math.Abs(position.X - targetPosition.X) - targetHitBox.X / 2);
                    int distY = (int)Math.Round(Math.Abs(position.Y - targetPosition.Y) - targetHitBox.Y / 2);

                    if (distX >= distY)
                    {
                        if (velocity.X > 0)
                            adjustedPosition.X = targetPosition.X - targetHitBox.X / 2 - hitbox.X / 2;
                        else
                            adjustedPosition.X = targetPosition.X + targetHitBox.X / 2 + hitbox.X / 2;
                    }

                    if (distX <= distY)
                    {
                        if (velocity.Y > 0)
                            adjustedPosition.Y = targetPosition.Y - targetHitBox.Y / 2 - hitbox.Y / 2;
                        else
                            adjustedPosition.Y = targetPosition.Y + targetHitBox.Y / 2 + hitbox.Y / 2;
                    }
                }
            }
            transforms[entity] = new Transform(adjustedPosition, transforms[entity].Size);
            return collidedEntities;
        }

        /// <summary>
        /// Checks for all collissions for an object
        /// </summary>
        /// <param name="entity">Entity that is colliding</param>
        /// <param name="transforms">Dictionary of transforms</param>
        /// <param name="rigidBodies">Dictionary of rigidBodies</param>
        /// <returns></returns>
        public static List<Entity> CheckCollissions(Entity entity, Dictionary<Entity, Transform> transforms, Dictionary<Entity, RigidBody> rigidBodies)
        {
            List<Entity> collidedEntities = new List<Entity>();

            Vector position = transforms[entity].Position;
            Vector hitbox = rigidBodies[entity].HitBox;

            foreach (KeyValuePair<Entity, Transform> targetObject in transforms)
            {
                if (targetObject.Key == entity)
                    continue;

                if (!rigidBodies.ContainsKey(targetObject.Key))
                    continue;

                Vector targetPosition = targetObject.Value.Position;
                Vector targetHitBox = rigidBodies[targetObject.Key].HitBox;

                if (targetPosition.X + targetHitBox.X / 2 > position.X - hitbox.X / 2
                    && targetPosition.X - targetHitBox.X / 2 < position.X + hitbox.X / 2
                    && targetPosition.Y + targetHitBox.Y / 2 > position.Y - hitbox.Y / 2
                    && targetPosition.Y - targetHitBox.Y / 2 < position.Y + hitbox.Y / 2)
                {
                    collidedEntities.Add(targetObject.Key);
                }
            }

            return collidedEntities;
        }

        /// <summary>
        /// Check for collission between two objects
        /// </summary>
        /// <param name="collider">Collider Entity</param>
        /// <param name="target">Target Entity</param>
        /// <param name="transforms">Dictionary of transforms</param>
        /// <param name="rigidBodies">Dictionary of rigidBodies</param>
        /// <returns></returns>
        public static bool CheckCollissions(Entity collider, Entity target, Dictionary<Entity, Transform> transforms, Dictionary<Entity, RigidBody> rigidBodies)
        {
            Vector colliderPosition = transforms[collider].Position;
            Vector targetPosition = transforms[target].Position;

            Vector colliderHitBox = rigidBodies[collider].HitBox;
            Vector targetHitBox = rigidBodies[target].HitBox;

            if (targetPosition.X + targetHitBox.X / 2 > colliderPosition.X - colliderHitBox.X / 2
                && targetPosition.X - targetHitBox.X / 2 < colliderPosition.X + colliderHitBox.X / 2
                && targetPosition.Y + targetHitBox.Y / 2 > colliderPosition.Y - colliderHitBox.Y / 2
                && targetPosition.Y - targetHitBox.Y / 2 < colliderPosition.Y + colliderHitBox.Y / 2)
                    return true;

            return false;
        }

        /// <summary>
        /// Check for collission between two objects
        /// </summary>
        /// <param name="colliderTransform">Transfrom of collider</param>
        /// <param name="colliderRidigBody">RigidBody of the collider</param>
        /// <param name="targetTransform">Transfrom of the target</param>
        /// <param name="targetRidigBody">RigidBody of the target</param>
        /// <returns></returns>
        public static bool CheckCollissions(Transform colliderTransform, RigidBody colliderRidigBody, Transform targetTransform, RigidBody targetRidigBody)
        {
            if (targetTransform.Position.X + targetRidigBody.HitBox.X / 2 > colliderTransform.Position.X - colliderRidigBody.HitBox.X / 2
                && targetTransform.Position.X - targetRidigBody.HitBox.X / 2 < colliderTransform.Position.X + colliderRidigBody.HitBox.X / 2
                && targetTransform.Position.Y + targetRidigBody.HitBox.Y / 2 > colliderTransform.Position.Y - colliderRidigBody.HitBox.Y / 2
                && targetTransform.Position.Y - targetRidigBody.HitBox.Y / 2 < colliderTransform.Position.Y + colliderRidigBody.HitBox.Y / 2)
                return true;

            return false;
        }

        /// <summary>
        /// Check for collission between two objects
        /// </summary>
        /// <param name="colliderPosition">collider position as a vector</param>
        /// <param name="colliderHitbox">collider hitbox as a vector</param>
        /// <param name="targetPosition">target position as a vector</param>
        /// <param name="targetHitBox">target hitbox as a vector</param>
        /// <returns></returns>
        public static bool CheckCollissions(Vector colliderPosition, Vector colliderHitbox, Vector targetPosition, Vector targetHitBox)
        {
            if (targetPosition.X + targetHitBox.X / 2 > colliderPosition.X - colliderHitbox.X / 2
                && targetPosition.X - targetHitBox.X / 2 < colliderPosition.X + colliderHitbox.X / 2
                && targetPosition.Y + targetHitBox.Y / 2 > colliderPosition.Y - colliderHitbox.Y / 2
                && targetPosition.Y - targetHitBox.Y / 2 < colliderPosition.Y + colliderHitbox.Y / 2)
                return true;

            return false;
        }
    }
}
