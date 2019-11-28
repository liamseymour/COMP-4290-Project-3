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
    class Field : Shape
    {
        public float height;
        public float width;
        public float depth;

        public Field(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, float scale, Vector3 dimentions, Effect effect) 
            : base(graphics, model, position, color, scale, effect)
        {
            this.height = dimentions.Y;
            this.width = dimentions.X;
            this.depth = dimentions.Z;
        }
        public void Draw(Matrix view, Matrix projection)
        {
            // Store previous rasterizer state so as not to cause any side effects
            // when method is exited. Create a new RasterizerState to create wireframe effect
            // and stop culling (in order to see all wires).
            RasterizerState oldRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState newRasterizerState = new RasterizerState();
            newRasterizerState.CullMode = CullMode.None;
            newRasterizerState.FillMode = FillMode.WireFrame;
            graphics.GraphicsDevice.RasterizerState = newRasterizerState;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(Matrix.CreateScale(this.scale * new Vector3(width, height, depth) / 2) * Matrix.CreateTranslation(position));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["Color"].SetValue(color.ToVector3());
                }
                mesh.Draw();
            }
            graphics.GraphicsDevice.RasterizerState = oldRasterizerState;
        }
    }
}
