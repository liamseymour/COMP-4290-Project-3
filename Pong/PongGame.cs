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
        private const float FIELD_WIDTH = 20;  // X
        private const float FIELD_HEIGHT = 20; // Y
        private const float FIELD_DEPTH = 40;  // Z

        private Vector3 cameraPosition;
        private Vector3 cameraForward; // Looking at
        private Vector3 cameraUp;

        private Hashtable shapes; // Reference for shapes i.e. all objects composing the game. 
        private Hashtable models; // Model reference
        private Hashtable textures; // Texture reference
        private Hashtable effects; // Effect / Shader reference

        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 50), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(75), 800f / 480f, 0.01f, 1000f);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

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

            // Shapes
            shapes = new Hashtable();
            shapes.Add("skybox", new Skybox(graphics, (Model)models["cube"], cameraPosition,
                new Color(1f, 1f, 1f, 1f), (TextureCube)textures["skybox-ocean"], (Effect)effects["skybox"]));

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

            // TODO: Add your update logic here

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

            base.Draw(gameTime);
        }
    }
}
