using MGPhysics;

namespace ReeGame.Components
{
    public struct RigidBody : IComponent
    {
        public Vector HitBox { get; set; }

        public RigidBody(Vector hitBox)
        {
            HitBox = hitBox;
        }
    }
}
