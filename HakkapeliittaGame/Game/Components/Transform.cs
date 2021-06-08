using MGPhysics;

namespace ReeGame.Components
{
    public struct Transform : IComponent
    {
        public Vector Position { get; set; }
        public Vector Size { get; set; }

        public Transform(Vector position, Vector size)
        {
            Position = position;
            Size = size;
        }
    }
}
    