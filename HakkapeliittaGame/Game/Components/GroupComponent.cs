using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MGPhysics;
using MGPhysics.Components;

namespace ReeGame.Components
{
    public struct GroupComponent : IComponent
    {
        public Entity LeaderEntity { get; set; }
        public List<Entity> Members { get; set; }
        public int RowLenght { get; set; }
        public float Spacing { get; set; }


        public GroupComponent(Entity leaderEntity, int rowLenght = 5, float spacing = 0f)
        {
            LeaderEntity = leaderEntity;
            Members = new List<Entity>();
            RowLenght = rowLenght;
            Spacing = spacing;
        }

        /// <summary>
        /// Check if entity is a member or leader
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <param name="group">Affected Group</param>
        /// <returns>True if group contains entity and false if not</returns>
        public bool ContainsEntity(Entity entity)
        {
            if (LeaderEntity == entity)
                return true;

            return Members.Contains(entity);
        }
    }
}
