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
        public float height;
        public float width;
        public float depth;

        public Paddle(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, float scale, Vector3 dimentions, Effect effect)
                : base(graphics, model, position, color, scale, effect)
        {
            this.height = dimentions.Y;
            this.width = dimentions.X;
            this.depth = dimentions.Z;
        }

        public void Draw(Matrix view, Matrix projection, Vector3 lightDirection, Color lightColor)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    Matrix world = Matrix.CreateScale(this.scale * new Vector3(width, height, depth) / 2) * Matrix.CreateTranslation(position); 
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world)));
                    effect.Parameters["DiffuseLightDirection"].SetValue(lightDirection);
                    effect.Parameters["DiffuseColor"].SetValue(lightColor.ToVector3());
                    
                }
                mesh.Draw();
            }
        }
    }
}
