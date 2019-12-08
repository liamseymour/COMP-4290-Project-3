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
        public Vector3 cpuLocation { get; set; }
        private const float SPEED = 10f;
        private const float CPU_SPEED = 3f;
        private Texture2D texture;

        public Paddle(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, Vector3 dimensions, Effect effect, Texture2D texture)
                : base(graphics, model, position, color, 1, effect)
        {
            this.dimensions = dimensions;
            this.texture = texture;
            cpuLocation = position;  
        }


        public void Update(float elapsedMilliseconds)
        {
            float x = 0;
            float y = 0;

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

            // Gamepad input
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                x = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X;
                y = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y;
            }


            if (x != 0 && y != 0) // Normalizes speed if two directional buttons are held at once
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

        public void CpuNewLocation(float elapsedMilliseconds, Vector3 ballVelocity, Vector3 ballContact, Vector3 fieldDimensions, float radius)
        {
            Vector3 topWallNormal = -Vector3.UnitY;

            Vector3 botWallNormal = Vector3.UnitY;

            Vector3 leftWallNormal = Vector3.UnitX;

            Vector3 rightWallNormal = -Vector3.UnitX;

            Vector3 backWallNormal = Vector3.UnitZ;

            cpuLocation = NextAIPosition(ballVelocity, topWallNormal, botWallNormal, leftWallNormal,  rightWallNormal, backWallNormal,  ballContact, fieldDimensions, radius);

        }

        public void UpdateCPU(float elapsedMilliseconds)
        {
            Vector3 cpuMoveVector = cpuLocation - position;
            cpuMoveVector.Normalize();

            if (position != cpuLocation)
            {
                position += new Vector3(cpuMoveVector.X, cpuMoveVector.Y, 0) * elapsedMilliseconds * CPU_SPEED;
            }

            if (position.X >= 20 - dimensions.X)
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

        private Vector3 NextAIPosition(Vector3 velocity, Vector3 topNormal, Vector3 botNormal,  Vector3 leftNormal, Vector3 rightNormal, Vector3 backNormal, Vector3 startingPoint, Vector3 fieldDimensions, float radius)
        {
            float t = 0;
            float tZ = 0;
            bool foundPosition = false;
            bool useTemp = false;

            Vector3 tempPoint = Vector3.Zero;
            Vector3 nextPoint = Vector3.Zero;
            float buffer = .2f;
            while (!foundPosition)
            {
                tZ = ((-fieldDimensions.Z + buffer + radius) - startingPoint.Z) / velocity.Z;
                tempPoint = startingPoint + velocity * tZ;

                if(Vector3.Dot(velocity, topNormal) < 0 && Vector3.Dot(velocity,leftNormal) < 0)
                {
                    float tTop = ((fieldDimensions.Y - buffer - radius) - startingPoint.Y) / velocity.Y;
                    float tLeft = t = ((-fieldDimensions.X + buffer + radius) - startingPoint.X) / velocity.X;
                    if(tTop <= tLeft)
                    {
                        nextPoint = startingPoint + (tTop * velocity);
                        if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                        {
                            velocity.X *= -1;
                            velocity.Y *= -1;
                        }
                        else
                        {
                            velocity.Y = -Math.Abs(velocity.Y);
                        }
                    }
                    else
                    {
                        nextPoint = startingPoint + (tLeft * velocity);
                        if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                        {
                            velocity.X *= -1;
                            velocity.Y *= -1;
                        }
                        else
                        {
                            velocity.X = Math.Abs(velocity.X);
                        }    
                    }
                }else if(Vector3.Dot(velocity, topNormal) < 0 && Vector3.Dot(velocity, rightNormal) < 0)
                {
                    float tTop = ((fieldDimensions.Y - buffer - radius) - startingPoint.Y) / velocity.Y;
                    float tRight = ((fieldDimensions.X - buffer - radius) - startingPoint.X) / velocity.X;
                    if (tTop <= tRight)
                    {
                        nextPoint = startingPoint + (tTop * velocity);
                        if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                        {
                            velocity.X *= -1;
                            velocity.Y *= -1;
                        }
                        else
                        {
                            velocity.Y = -Math.Abs(velocity.Y);
                        }
                    }
                    else
                    {
                        nextPoint = startingPoint + (tRight * velocity);
                        if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                        {
                            velocity.X *= -1;
                            velocity.Y *= -1;
                        }
                        else
                        {
                            velocity.X = -Math.Abs(velocity.X);
                        }
                    }
                }else if (Vector3.Dot(velocity, botNormal) < 0 && Vector3.Dot(velocity, leftNormal) < 0){
                    float tBot = ((-fieldDimensions.Y + buffer + radius) - startingPoint.Y) / velocity.Y;
                    float tLeft = t = ((-fieldDimensions.X + buffer + radius) - startingPoint.X) / velocity.X;
                    if (tBot <= tLeft)
                    {
                        nextPoint = startingPoint + (tBot * velocity);
                        if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                        {
                            velocity.X *= -1;
                            velocity.Y *= -1;
                        }
                        else
                        {
                            velocity.Y = Math.Abs(velocity.Y);
                        }
                    }
                    else
                    {
                        nextPoint = startingPoint + (tLeft * velocity);
                        if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                        {
                            velocity.X *= -1;
                            velocity.Y *= -1;
                        }
                        else
                        {
                            velocity.X = Math.Abs(velocity.X);
                        }
                    }
                }else if (Vector3.Dot(velocity, botNormal) < 0 && Vector3.Dot(velocity, rightNormal) < 0)
                {
                    float tBot = ((-fieldDimensions.Y + buffer + radius) - startingPoint.Y) / velocity.Y;
                    float tRight = ((fieldDimensions.X - buffer - radius) - startingPoint.X) / velocity.X;
                    if (tBot <= tRight)
                    {
                        nextPoint = startingPoint + (tBot * velocity);
                        if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                        {
                            velocity.X *= -1;
                            velocity.Y *= -1;
                        }
                        else
                        {
                            velocity.Y = Math.Abs(velocity.Y);
                        }
                    }
                    else
                    {
                        nextPoint = startingPoint + (tRight * velocity);
                        if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                        {
                            velocity.X *= -1;
                            velocity.Y *= -1;
                        }
                        else
                        {
                            velocity.X = -Math.Abs(velocity.X);
                        }
                    }
                }
                else if (Vector3.Dot(velocity, topNormal) < 0)
                {
                    t = ((fieldDimensions.Y - buffer - radius) -startingPoint.Y) / velocity.Y;
                    nextPoint = startingPoint + (t * velocity);
                    if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                    {
                        velocity.X *= -1;
                        velocity.Y *= -1;
                    }
                    else
                    {
                        velocity.Y = -Math.Abs(velocity.Y);
                    }     
                    Console.WriteLine("ggggg");
                }
                else if (Vector3.Dot(velocity, botNormal) < 0)
                {  
                    t = ((-fieldDimensions.Y + buffer + radius) - startingPoint.Y) / velocity.Y;
                    nextPoint = startingPoint + (t * velocity);
                    if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                    {
                        velocity.X *= -1;
                        velocity.Y *= -1;
                    }
                    else
                    {
                        velocity.Y = Math.Abs(velocity.Y);
                    } 
                }
                else if (Vector3.Dot(velocity, leftNormal) < 0)
                {
                    t = ((-fieldDimensions.X + buffer + radius) - startingPoint.X) / velocity.X;
                    nextPoint = startingPoint + (t * velocity);
                    if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                    {
                        velocity.X *= -1;
                        velocity.Y *= -1;
                    }
                    else
                    {
                        velocity.X = Math.Abs(velocity.X);
                    }  
                }
                else if (Vector3.Dot(velocity, rightNormal) < 0)
                {
                    t = ((fieldDimensions.X - buffer - radius) - startingPoint.X) / velocity.X;
                    nextPoint = startingPoint + (t * velocity);
                    if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
                    {
                        velocity.X *= -1;
                        velocity.Y *= -1;
                    }
                    else
                    {
                        velocity.X = -Math.Abs(velocity.X);
                    }               
                }
                else
                {
                    foundPosition = true;
                }
                if (nextPoint.Z <= -fieldDimensions.Z)
                {
                    foundPosition = true;
                    useTemp = true;
                }             
                startingPoint = nextPoint;
            }
            if(useTemp != true)
            {
                return nextPoint;
            }
            else
            {
                return tempPoint;
            }
            
           
            
        }

        private Vector3 CheckVelocity(Vector3 velocity)
        {
            if (Math.Abs(velocity.X) == Math.Abs(velocity.Y))
            {
                velocity.X *= -1;
                velocity.Y *= -1;
            }
            return velocity;
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
                    effect.Parameters["ModelTexture"].SetValue(texture);
                }
                mesh.Draw();
            }
        }
    }
}
