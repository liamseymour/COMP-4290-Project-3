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
        protected Model model;
        protected Effect effect;
        protected Vector3 position { get; set; }
        protected int Scale { get; set; }
        protected Color color { get; set; }

        public Shape(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, Effect effect)
        {
            this.graphics = graphics;
            this.model = model;
            this.position = position;
            this.color = color;
            this.effect = effect;
            this.Scale = 1;
        }

        public void Draw()
        {

        }
        
    }
}
