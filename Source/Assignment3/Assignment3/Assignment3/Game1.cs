using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Assignment3.Scenes; 

namespace Assignment3
{
    public enum SceneType
    {
        MENU,
        MAZE
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BaseGame : Microsoft.Xna.Framework.Game
    {
        public static BaseGame instance;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect effect;

        private Scene currentScene = null;

        Camera camera;
        Model test;

        public int BackBufferWidth;
        public int BackBufferHeight;
        float AspectRatio;

        public BaseGame()
        {
            instance = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            BackBufferHeight = graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            BackBufferWidth = graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            
            IsMouseVisible = false;
            graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set the start scene
            currentScene = new MenuScene();
            AspectRatio = GraphicsDevice.Viewport.AspectRatio;
            camera = new Camera(new Vector3(10f, 2f, 5f), new Vector3(0f,180f,0f), 10f, AspectRatio,GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width);
            
            effect = new BasicEffect(GraphicsDevice);
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

            // Load the start scene
            currentScene.onLoad(Content);

            test = Content.Load<Model>("Models/exitDoor");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Get the gamepad and keyboard state
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboard = Keyboard.GetState();

            // Update the current scene
            currentScene.update(gamepad, keyboard);

            camera.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Render the current scene
            currentScene.draw(spriteBatch);

            if (test != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[test.Bones.Count];
                test.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in test.Meshes)
                {

                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();//lighting
                        
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale (0.01f,0.01f,0.01f)
                            * Matrix.CreateTranslation(Vector3.Zero);
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void changeScene(SceneType scene)
        {
            switch(scene)
            {
                case SceneType.MENU:
                {
                    currentScene = new MenuScene();
                    break;
                }

                case SceneType.MAZE:
                {
                    currentScene = new MazeScene();
                    break;
                }
            }
            
            currentScene.onLoad(Content);
        }
    }
}
