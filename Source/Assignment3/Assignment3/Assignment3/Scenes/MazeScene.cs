using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Assignment3.Entities;

namespace Assignment3.Scenes
{
    public class MazeScene : Scene
    {
        public static MazeScene instance;

        public Camera camera;
        private BasicEffect effect;
        
        private float AspectRatio;
        
        private List<Entity> collideList = new List<Entity>();
        private bool[,] rawMaze;


        public MazeScene()
        {
            instance = this;
        }

        public override void onLoad(ContentManager content)
        {
            // Generate the maze
            MazeBuilder builder = new MazeBuilder(10);
            rawMaze = builder.buildMaze();

            builder.generateWalls(content, collideList);
            Vector2 startPos = builder.getStartPos();

            // Create the camera/player
            AspectRatio = BaseGame.instance.GraphicsDevice.Viewport.AspectRatio;
            camera = new Camera(new Vector3(startPos.X * 4, 2f, startPos.Y * 4), new Vector3(0f, 180f, 0f), 10f, AspectRatio, 
                BaseGame.instance.GraphicsDevice.Viewport.Height, BaseGame.instance.GraphicsDevice.Viewport.Width);

            effect = new BasicEffect(BaseGame.instance.GraphicsDevice);

        }

        public override void update(GameTime gameTime, GamePadState gamepad, KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                // Go to the maze
                BaseGame.instance.changeScene(SceneType.MENU);
            }

            camera.Update(gameTime);

            // Update the model list
            foreach (Entity e in collideList)
            {
                e.update(gameTime, gamepad, keyboard);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            // Render the model list
            foreach (Entity e in collideList)
            {
                e.draw(sb);
            }
        }
    }
}
