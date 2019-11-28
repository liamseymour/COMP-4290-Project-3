using Microsoft.Xna.Framework;
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

        Vector3 cameraPosition = new Vector3(0, 0, 50);
        Vector3 cameraUp = new Vector3(0, 1, 0);
        Vector3 cameraForward = new Vector3(0, 0, -1);
        Vector3 cameraRight = new Vector3(1, 0, 0);

        private Hashtable shapes; // Reference for shapes i.e. all objects composing the game. 
        private Hashtable models; // Model reference
        private Hashtable textures; // Texture reference
        private Hashtable effects; // Effect / Shader reference

        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 50), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75), 800f / 480f, 0.01f, 1000f);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        /* ||||||| TEMP CAMERA CONTROLS ||||||| */
        float yaw = 0; // angle that camera has rotated on the y-axis
        float pitch = 0; // angle that camera has rotated on the x-axis
        /* ||||||| END TEMP CAMERA CONTROLS ||||||| */

        public PongGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferMultiSampling = true;
            // TODO Dynamic Screen dimentions
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Camera
            cameraPosition = new Vector3(0, 0, 50);
            cameraForward = -Vector3.UnitZ;
            cameraUp = Vector3.UnitY;

            base.Initialize();
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
            effects.Add("directional", Content.Load<Effect>("Directional"));

            // Shapes
            shapes = new Hashtable();
            shapes.Add("skybox", new Skybox(graphics, (Model)models["cube"], cameraPosition,
                new Color(1f, 1f, 1f, 1f), 500, (TextureCube)textures["skybox-ocean"], (Effect)effects["skybox"]));
            shapes.Add("ball", new Ball(graphics, (Model)models["sphere"], new Vector3(0, 0, 0), new Color(1, 1, 1, 1), 1/3f, new Vector3(0, 0, 10), null));
            shapes.Add("field", new Field(graphics, (Model)models["cube"], new Vector3(0), new Color(1, 1, 1, 1), 1, fieldDimentions, (Effect)effects["simple"]));
            Vector3 paddleDimentions = new Vector3(2, 2, .2f);
            shapes.Add("player_paddle", new Paddle(graphics, (Model)models["cube"], new Vector3(0, 0, fieldDimentions.Z / 2 + paddleDimentions.Z / 2), 
                new Color(1, 1, 1, 1), 1, paddleDimentions, (Effect)effects["directional"]));
            shapes.Add("opponent_paddle", new Paddle(graphics, (Model)models["cube"], new Vector3(0, 0, -fieldDimentions.Z / 2 -paddleDimentions.Z / 2),
                new Color(1, 1, 1, 1), 1, paddleDimentions, (Effect)effects["directional"]));

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

            /* ||||||| TEMP CAMERA CONTROLS ||||||| */
            // Camera rotation using arrow keys
            float rotationFactor = 0.02f; // Rotation "sensitivity"

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                pitch += rotationFactor;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                pitch -= rotationFactor;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                yaw -= rotationFactor;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                yaw += rotationFactor;

            // Apply Rotations
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);

            Vector3 newForward = Vector3.Transform(cameraForward, cameraRotation);
            Vector3 newUp = Vector3.Transform(cameraUp, cameraRotation);
            Vector3 newRight = Vector3.Transform(cameraRight, cameraRotation);

            // Movement on WASD
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateTranslation(newForward));
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateTranslation(-newForward));
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateTranslation(-newRight));
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateTranslation(newRight));

            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + newForward, newUp);
            /* ||||||| END TEMP CAMERA CONTROLS ||||||| */

            ((Ball)shapes["ball"]).Update(gameTime.ElapsedGameTime.Milliseconds);

            base.Update(gameTime);
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
            ((Paddle)shapes["player_paddle"]).Draw(view, projection, new Vector3(1.4142f, 1.4142f, 1.4142f), new Color(0.9921875f, 0.9921875f, 0.8359375f));
            ((Paddle)shapes["opponent_paddle"]).Draw(view, projection, new Vector3(1.4142f, 1.4142f, 1.4142f), new Color(0.9921875f, 0.9921875f, 0.8359375f));

            base.Draw(gameTime);
        }
    }
}
