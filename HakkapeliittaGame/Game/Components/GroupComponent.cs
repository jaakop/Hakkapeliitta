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

        public GroupComponent(Entity leaderEntity)
        {
            LeaderEntity = leaderEntity;
            Members = new List<Entity>();
        }
    }
}
