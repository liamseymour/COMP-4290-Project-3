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
    
    public class Skybox : Shape
    {
        TextureCube texture { get; set; }

        public Skybox(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, TextureCube texture, Effect effect) : base(graphics, model, position, color, effect)
        {
            this.texture = texture;
        }

        public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition)
        {
            // Store previous rasterizer state so as not to cause any side effects
            // when method is exited. Create a new RasterizerState to cull in the correct
            // direction (RasterizerState cannot be mutated).
            RasterizerState oldRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState newRasterizerState = new RasterizerState();
            newRasterizerState.CullMode = CullMode.CullClockwiseFace;
            graphics.GraphicsDevice.RasterizerState = newRasterizerState;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(Matrix.CreateScale(500f) * Matrix.CreateTranslation(cameraPosition));
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["Camera"].SetValue(cameraPosition);
                    effect.Parameters["SkyBoxTexture"].SetValue(texture);
                }
                mesh.Draw();
            }
            graphics.GraphicsDevice.RasterizerState = oldRasterizerState;
        }
    }
}
