using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ReeGame.Components;

namespace ReeGame
{
    public class Camera2D
    {
        Vector position;
        float zoom;

        /// <summary>
        /// New 2D Camera object
        /// </summary>
        /// <param name="Position">Position of the camera</param>
        /// <param name="Zoom">Camera zoom amount</param>
        public Camera2D(Vector Position, float Zoom)
        {
            this.position = Position;
            this.zoom = Zoom;
        }
        
        /// <summary>
        /// Gets the Transform Matrix, wich can be used in spritebatch
        /// </summary>
        /// <param name="viewport">Viewport of the game</param>
        /// <returns></returns>
        public Matrix GetTransformationMatrix(Viewport viewport)
        {
            return Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                                            Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                                            Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));
        }

        public Vector Position { get => position; set => position = value; }
        public float Zoom { get => zoom; set => zoom = value; }
    }
}
