using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MGPhysics;

namespace ReeGame.Components
{
    public struct MovementComponent : IComponent
    {
        public float velocity;
        public Vector target;
    }
}
