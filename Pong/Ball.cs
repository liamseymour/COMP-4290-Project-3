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
        public Ball(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, Vector3 velocity, Effect effect) : base(graphics, model, position, color, effect)
        {
            this.velocity = velocity;
        }
    }
}
