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
    public class Shape
    {
        protected GraphicsDeviceManager graphics;
        public Model model;
        public Effect effect;
        public Vector3 position { get; set; }
        public float scale { get; set; }
        public Color color { get; set; }

        public Shape(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, float scale, Effect effect)
        {
            this.graphics = graphics;
            this.model = model;
            this.position = position;
            this.color = color;
            this.effect = effect;
            this.scale = scale;
        }

        /// <summary>
        /// By default, render shapes using basicEffect. Uses the shapes scale and position for the world matrix.
        /// </summary>
        /// <param name="view">View Matrix</param>
        /// <param name="projection">Projection Matrix</param>
        public void Draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.Direction = new Vector3(1.4142f, 1.4142f, 1.4142f);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.9921875f, 0.9921875f, 0.8359375f); // Pale yellow
                    effect.DirectionalLight0.SpecularColor = new Vector3(1);
                    effect.AmbientLightColor = new Vector3(0.1484375f, 0.1484375f, 0.12890625f); // Dark yellow
                    
                    effect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }   
        }
    }
}
