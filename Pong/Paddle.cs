using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong
{
    class Paddle : Shape
    {

        public Vector3 dimensions { get; }
        private const float SPEED = 10;

        public Paddle(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, Vector3 dimensions, Effect effect)
                : base(graphics, model, position, color, 1, effect)
        {
            this.dimensions = dimensions;
           
        }


        public void Update(float elapsedMilliseconds)
        {
            int x = 0;
            int y = 0;
            
            if (Keyboard.GetState().IsKeyDown(Keys.A)) // Paddle Movement Left
            {
                x -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) // Paddle Movement Right
            {
                x += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W)) // Paddle Movement Up
            {
 
                y += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) // Paddle Movement Down
            {
                
                y -= 1;
            }

            if(x != 0 && y != 0) // Normalizes speed if two directional buttons are held at once
            {
                this.position += (y * Vector3.UnitY + x * Vector3.UnitX) * elapsedMilliseconds * SPEED / (float)Math.Sqrt(2);
            }else
            {
                this.position += (y * Vector3.UnitY + x * Vector3.UnitX) * elapsedMilliseconds * SPEED;
            }

            // Traps paddle within our field
            if(position.X >= 20 -dimensions.X)
            {
                position = new Vector3(20 - dimensions.X, position.Y, position.Z);
            }
            
            if (position.X <= -20 + dimensions.X)
            {
                position = new Vector3(-20 + dimensions.X, position.Y, position.Z);
            }

            if (position.Y >= 20 - dimensions.Y)
            {
                position = new Vector3(position.X, 20 - dimensions.Y, position.Z);
            }

            if (position.Y <= -20 + dimensions.Y)
            {
                position = new Vector3(position.X, -20 + dimensions.Y, position.Z);
            }
        }

        public void Draw(Matrix view, Matrix projection, Vector3 lightPosition, Color lightColor, Vector3 cameraPosition)
        {
            Matrix world = Matrix.CreateScale(this.scale * dimensions) * Matrix.CreateTranslation(position);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect; 
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["Camera"].SetValue(cameraPosition);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world)));
                    effect.Parameters["LightPosition"].SetValue(lightPosition);
                    effect.Parameters["LightColor"].SetValue(lightColor.ToVector3());
                    effect.Parameters["DiffuseColor"].SetValue(color.ToVector3());
                }
                mesh.Draw();
            }
        }
    }
}
