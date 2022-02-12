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
        private float direction;

        public Entity LeaderEntity { get; set; }
        public List<Entity> Members { get; set; }
        public int RowLenght { get; set; }
        public float Spacing { get; set; }
        public float RowVariance { get; set; }
        public float Direction {
            get { return direction; }
            set {
                if (value < -360 || value > 360)
                    throw new ArgumentOutOfRangeException("Direction", "Value must be between 360 degrees and -360 degrees");
                direction = value;
            }
        }


        public GroupComponent(Entity leaderEntity, int rowLenght = 1, float spacing = 0f, float direction = 0f, float rowVariance = 0f)
        {
            LeaderEntity = leaderEntity;
            Members = new List<Entity>();
            RowLenght = rowLenght;
            Spacing = spacing;
            RowVariance = rowVariance;

            if (direction < -360 || direction > 360)
                throw new ArgumentOutOfRangeException("Direction", "Value must be between 360 degrees and -360 degrees");
            this.direction = direction;
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
