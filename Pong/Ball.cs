using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong
{
    public class Ball : Shape
    {
        Vector3 velocity { get; set; }
        float radius;
        public Ball(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, float scale, Vector3 velocity, Effect effect) : base(graphics, model, position, color, scale, effect)
        {
            this.velocity = velocity;
            this.radius = scale;
        }

        public void Update(float elapsedMilliseconds, Vector3 fieldDimentions)
        {
            Vector3 deltaPosition = elapsedMilliseconds / 1000 * velocity;
            float buffer = .2f;
            // Detect collisions
            if (position.X + deltaPosition.X + radius > fieldDimentions.X / 2 - buffer || position.X + deltaPosition.X - radius < -fieldDimentions.X / 2 + buffer)
                velocity = Vector3.Multiply(velocity, new Vector3(-1, 1, 1));

            if (position.Y + deltaPosition.Y + radius > fieldDimentions.Y / 2 - buffer || position.Y + deltaPosition.Y - radius < -fieldDimentions.Y / 2 + buffer)
                velocity = Vector3.Multiply(velocity, new Vector3(1, -1, 1));

            if (position.Z + deltaPosition.Z + radius > fieldDimentions.Z / 2 - buffer || position.Z + deltaPosition.Z - radius < -fieldDimentions.Z / 2 + buffer)
                velocity = Vector3.Multiply(velocity, new Vector3(1, 1, -1));

            deltaPosition = elapsedMilliseconds / 1000 * velocity;
            position += deltaPosition;
        }




    }
}
