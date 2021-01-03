using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MGPhysics;
using ReeGame.Components;

namespace ReeGame.Systems
{
    class MovementSystem : ISystem
    {
        public void Call(ECSManager manager, int deltaTime)
        {
            List<Task> tasks = new List<Task>();
            foreach(KeyValuePair<Entity, MovementComponent> entry in manager.ComponentManager.GetComponentArray<MovementComponent>().Array)
            {
                tasks.Add(Task.Run(() =>
                {
                    Vector velocity = Vector.Lerp(manager.ComponentManager.GetComponentArray<Transform>().Array[entry.Key].Position, 
                                                    entry.Value.target, 0.1f);

                    if (Math.Abs(velocity.X) > entry.Value.velocity)
                        velocity.X = (velocity.X / Math.Abs(velocity.X)) * entry.Value.velocity;
                    if (Math.Abs(velocity.Y) > entry.Value.velocity)
                        velocity.Y = (velocity.Y / Math.Abs(velocity.Y)) * entry.Value.velocity;

                    manager.ComponentManager.UpdateComponent(entry.Key,
                                    new Transform(manager.ComponentManager.GetComponentArray<Transform>().Array[entry.Key].Position +
                                        velocity, manager.ComponentManager.GetComponentArray<Transform>().Array[entry.Key].Size));
                }));
            }

            foreach(Task task in tasks)
            {
                task.Wait();
            }
        }
    }
}
