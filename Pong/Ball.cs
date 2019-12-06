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
        private Vector3 velocity;
        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                speed = value.X + value.Y + value.Z;
                velocity = value;
            }
        }
        float speed;
        float radius;
        public Ball(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, float scale, Vector3 velocity, Effect effect) : base(graphics, model, position, color, scale, effect)
        {
            this.velocity = velocity;
            this.radius = scale;
            this.speed = velocity.X + velocity.Y + velocity.Z;
        }

        /// <summary>
        /// Returns whether or not the ball has gone out of bounds.
        /// </summary>
        /// <param name="elapsedMilliseconds"></param>
        /// <param name="fieldDimentions"></param>
        /// <param name="paddleDimentions"></param>
        /// <param name="playerPaddlePosition"></param>
        /// <param name="enemyPaddlePosition"></param>
        /// <returns></returns>
        public bool Update(float elapsedMilliseconds, Vector3 fieldDimentions, Vector3 paddleDimentions, Vector3 playerPaddlePosition, Vector3 enemyPaddlePosition)
        {
            bool outOfBounds = false;
            Vector3 deltaPosition = elapsedMilliseconds / 1000 * velocity;
            float buffer = .2f;
            // Detect collisions
            if (position.X + deltaPosition.X + radius > fieldDimentions.X / 2 - buffer || position.X + deltaPosition.X - radius < -fieldDimentions.X / 2 + buffer)
                velocity = Vector3.Multiply(velocity, new Vector3(-1, 1, 1));

            if (position.Y + deltaPosition.Y + radius > fieldDimentions.Y / 2 - buffer || position.Y + deltaPosition.Y - radius < -fieldDimentions.Y / 2 + buffer)
                velocity = Vector3.Multiply(velocity, new Vector3(1, -1, 1));

            // Here we need to check if the ball hit the paddle and if it did how to update it's velocity
            if (position.Z + deltaPosition.Z + radius > fieldDimentions.Z / 2 - buffer || position.Z + deltaPosition.Z - radius < -fieldDimentions.Z / 2 + buffer)
            {
                // Who's paddle is relavent to check?
                // We can tell by checking what side the ball is on
                Vector3 paddlePosition;
                if (position.Z > 0) // AI
                    paddlePosition = enemyPaddlePosition;
                else // Player
                    paddlePosition = playerPaddlePosition;

                float deltaX = position.X - paddlePosition.X;
                float deltaY = position.Y - paddlePosition.Y;

                if (deltaX >= -paddleDimentions.X / 2f && deltaX <= paddleDimentions.X / 2f
                     && deltaY >= -paddleDimentions.Y / 2f && deltaY <= paddleDimentions.Y / 2f)
                {
                    Vector3 direction = Vector3.Normalize(velocity);
                    direction += new Vector3(deltaX, deltaY, 0);
                    direction = Vector3.Normalize(direction);
                    velocity = direction * speed;
                    velocity = Vector3.Multiply(velocity, new Vector3(1, 1, -1));
                }
                else
                    outOfBounds = true;
            }

            deltaPosition = elapsedMilliseconds / 1000 * velocity;
            position += deltaPosition;
            return outOfBounds;
        }

        public void Draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    Matrix world = Matrix.CreateScale(this.scale) * Matrix.CreateTranslation(position);
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world)));
                    effect.Parameters["Color"].SetValue(color.ToVector3());
                }
                mesh.Draw();
            }
        }
    }
}
