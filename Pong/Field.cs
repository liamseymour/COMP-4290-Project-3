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

        Vector3 dimentions;

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public Field(GraphicsDeviceManager graphics, Model model, Vector3 position, Color color, Vector3 dimentions, Effect effect) 
            : base(graphics, model, position, color, 1, effect)
        {
            this.height = dimentions.Y;
            this.width = dimentions.X;
            this.depth = dimentions.Z;

            this.dimentions = dimentions;

            float x = dimentions.X;
            float y = dimentions.Y;
            float z = dimentions.Z;

            
            VertexPositionColor[] vertices = new VertexPositionColor[8];
            
            vertices[0] = new VertexPositionColor(new Vector3(x, y, z), color);
            vertices[1] = new VertexPositionColor(new Vector3(-x, y, z), color);
            vertices[2] = new VertexPositionColor(new Vector3(x, -y, z), color);
            vertices[3] = new VertexPositionColor(new Vector3(x, y, -z), color);
            vertices[4] = new VertexPositionColor(new Vector3(-x, -y, z), color);
            vertices[5] = new VertexPositionColor(new Vector3(-x, y, -z), color);
            vertices[6] = new VertexPositionColor(new Vector3(x, -y, -z), color);
            vertices[7] = new VertexPositionColor(new Vector3(-x, -y, -z), color);

            int[] indices = new int[24];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 0;
            indices[3] = 2;
            indices[4] = 0;
            indices[5] = 3;

            indices[6] = 5;
            indices[7] = 1;
            indices[8] = 5;
            indices[9] = 3;
            indices[10] = 5;
            indices[11] = 7;

            indices[12] = 6;
            indices[13] = 2;
            indices[14] = 6;
            indices[15] = 3;
            indices[16] = 6;
            indices[17] = 7;

            indices[18] = 4;
            indices[19] = 1;
            indices[20] = 4;
            indices[21] = 2;
            indices[22] = 4;
            indices[23] = 7;
            
            /*
            VertexPositionColor[] vertices = new VertexPositionColor[2];
            int[] indices = new int[2];

            vertices[0] = new VertexPositionColor(new Vector3(x, y, z), color);
            vertices[1] = new VertexPositionColor(new Vector3(-x, y, z), color);

            indices[0] = 0;
            indices[1] = 1;
            */
            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);
            indexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

        }
        public void Draw(Matrix view, Matrix projection)
        {
            BasicEffect basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;

            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 12);
            }
        }
    }
}
