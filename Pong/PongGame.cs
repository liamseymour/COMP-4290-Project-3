﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections;

namespace Pong
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PongGame : Game
    {
        private Vector3 fieldDimentions = new Vector3(20, 20, 40);

        private Hashtable shapes; // Reference for shapes i.e. all objects composing the game. 
        private Hashtable models; // Model reference
        private Hashtable textures; // Texture reference
        private Hashtable effects; // Effect / Shader reference

        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 50), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75), 800f / 480f, 0.01f, 1000f);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Game state
        Vector3 paddleDimentions;

        // Camera
        float yaw; // angle that camera has rotated on the y-axis
        float radius;
        Vector3 cameraPosition;

        // Screen
        int defaultWidth = 800;
        int defaultHeight = 480;

        // points
        int playerPoints = 0;
        int opponentPoints = 0;

        // fonts
        private SpriteFont font;
        private BasicEffect fontEffect;


        public PongGame()
        {
            // Graphics
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferMultiSampling = true;

            // Camera
            yaw = 0;
            radius = 80f;

            // Game state
            fieldDimentions = new Vector3(20, 20, 40);
            paddleDimentions = new Vector3(2, 2, .2f);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            cameraPosition = new Vector3(0, 0, radius);
            base.Initialize();
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.ApplyChanges();

            // Matrix data
            view = Matrix.CreateLookAt(new Vector3(0, 0, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)screenWidth/ screenHeight, 0.01f, 1000f);
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Models
            models = new Hashtable();
            models.Add("sphere", Content.Load<Model>("sphere"));
            models.Add("cube", Content.Load<Model>("cube"));

            // Load Textures
            textures = new Hashtable();
            textures.Add("skybox-ocean", Content.Load<TextureCube>("Ocean"));

            // Load shaders / effects
            effects = new Hashtable();
            effects.Add("skybox", Content.Load<Effect>("Skybox"));
            effects.Add("simple", Content.Load<Effect>("Simple"));
            //effects.Add("directional", Content.Load<Effect>("Directional"));
            effects.Add("point", Content.Load<Effect>("Point"));

            // Shapes
            shapes = new Hashtable();
            shapes.Add("skybox", new Skybox(graphics, (Model)models["cube"], cameraPosition,
                new Color(1f, 1f, 1f, 1f), 500, (TextureCube)textures["skybox-ocean"], (Effect)effects["skybox"]));
            shapes.Add("ball", new Ball(graphics, (Model)models["sphere"], new Vector3(0, 0, 0), new Color(1f, 1f, 1f, 1f), 1/3f, new Vector3(0, 0, 25), (Effect)effects["simple"]));
            shapes.Add("field", new Field(graphics, (Model)models["cube"], new Vector3(0), new Color(1, 1, 1, 1), fieldDimentions, null));
            Vector3 paddleDimentions = new Vector3(2, 2, .2f);
            shapes.Add("player_paddle", new Paddle(graphics, (Model)models["cube"], new Vector3(0, 0, fieldDimentions.Z + paddleDimentions.Z), 
                new Color(.5f, 0f, 0f, 1f), paddleDimentions, (Effect)effects["point"]));
            shapes.Add("opponent_paddle", new Paddle(graphics, (Model)models["cube"], new Vector3(0, 0, -fieldDimentions.Z -paddleDimentions.Z),
                new Color(0f, 0f, .5f, 1f), paddleDimentions, (Effect)effects["point"]));

            // fonts
            font = Content.Load<SpriteFont>("Font");
            fontEffect = new BasicEffect(GraphicsDevice);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Toggle fullscreen
            if(Keyboard.GetState().IsKeyDown(Keys.F) || 
                (GamePad.GetState(PlayerIndex.One).IsConnected && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed))
            {
                int screenWidth;
                int screenHeight;
                // I don't understand why this condition should be negated but it works, and is opposite otherwise...
                if (!graphics.IsFullScreen)
                {
                    screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
                else
                {
                    screenWidth = defaultWidth;
                    screenHeight = defaultHeight;
                }
                graphics.PreferredBackBufferHeight = screenHeight;
                graphics.PreferredBackBufferWidth = screenWidth;
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)screenWidth / screenHeight, 0.01f, 1000f);
                graphics.ToggleFullScreen();
            }

            UpdateCamera(gameTime);

            Vector3 paddleDimentions = new Vector3(2, 2, .2f);
            bool ballOutOfBounds = ((Ball)shapes["ball"]).Update(gameTime.ElapsedGameTime.Milliseconds, fieldDimentions, paddleDimentions, ((Paddle)shapes["player_paddle"]).position, ((Paddle)shapes["opponent_paddle"]).position);
            Ball ball = ((Ball)shapes["ball"]);
            if (ballOutOfBounds)
            {
                if (ball.position.Z > 0) // Player goal
                {
                    ((Ball)shapes["ball"]).Velocity = new Vector3(0, 0, -25);
                    opponentPoints++;
                }
                else // Ai goal
                {
                    ball.Velocity = new Vector3(0, 0, 25);
                    playerPoints++;
                }
                ball.position = new Vector3(0);
            }
            ((Paddle)shapes["player_paddle"]).Update(gameTime.ElapsedGameTime.Milliseconds/1000f);


            base.Update(gameTime);
        }

        protected void UpdateCamera(GameTime gameTime)
        {
            // Gamepad camera controlls
            float gamePadRotationFactor = 1.7f * gameTime.ElapsedGameTime.Milliseconds / 1000f; // Rotation sensitivity
            float gamePadZoomFactor = 40f * gameTime.ElapsedGameTime.Milliseconds / 1000f; // Zoom sensitivity
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                float x = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X;
                float y = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y;
                yaw += gamePadRotationFactor * x;
                radius = MathHelper.Clamp(radius - gamePadZoomFactor * y, 20, 300);
            }
            // Camera rotation using arrow keys
            float keybRotationFactor = 1.7f * gameTime.ElapsedGameTime.Milliseconds / 1000f; // Rotation sensitivity
            float keybZoomFactor = 40f * gameTime.ElapsedGameTime.Milliseconds / 1000f; // Zoom sensitivity

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                yaw -= keybRotationFactor;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                yaw += keybRotationFactor;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                radius = MathHelper.Clamp(radius - keybZoomFactor, 20, 300);
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                radius = MathHelper.Clamp(radius + keybZoomFactor, 20, 300);

            cameraPosition = new Vector3((float)(radius * System.Math.Cos(yaw)), 0, (float)(radius * System.Math.Sin(yaw)));
            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0), Vector3.UnitY);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            ((Skybox)shapes["skybox"]).Draw(view, projection, cameraPosition);
            ((Ball)shapes["ball"]).Draw(view, projection);
            ((Field)shapes["field"]).Draw(view, projection);
            ((Paddle)shapes["player_paddle"]).Draw(view, projection, ((Ball)shapes["ball"]).position, Color.White, cameraPosition);
            ((Paddle)shapes["opponent_paddle"]).Draw(view, projection, ((Ball)shapes["ball"]).position, Color.White, cameraPosition);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);

            spriteBatch.DrawString(font, "Player Score: " + playerPoints, new Vector2(100, 100), Color.Black);
            spriteBatch.DrawString(font, "Opponent Score: " + opponentPoints, new Vector2(graphics.PreferredBackBufferWidth - 500, 100), Color.Black);
            if(playerPoints == 5)
            {
                spriteBatch.DrawString(font, "Player Won!" + playerPoints, new Vector2(100, 100), Color.Black);
            }
            if(opponentPoints == 5)
            {
                spriteBatch.DrawString(font, "Opponent Won!", new Vector2((graphics.PreferredBackBufferWidth /2) - 150, graphics.PreferredBackBufferHeight /2 - 150), Color.Black);
            }

            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            base.Draw(gameTime);
        }
    }
}
