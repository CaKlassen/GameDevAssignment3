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

namespace Assignment3.Scenes
{
    public class MenuScene : Scene
    {
        private SpriteFont font;

        public MenuScene()
        {

        }

        public override void onLoad(ContentManager content)
        {
            font = content.Load<SpriteFont>("MenuFont");
        }

        public override void update(GameTime gameTime, GamePadState gamepad, KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.D1) || gamepad.IsButtonDown(Buttons.A))
            {
                // Go to the maze
                MazeCommunication.setDifficulty(MazeDifficulty.EASY);
                BaseGame.instance.changeScene(SceneType.MAZE);
            }
            else if (keyboard.IsKeyDown(Keys.D2) || gamepad.IsButtonDown(Buttons.B))
            {
                // Go to the maze
                MazeCommunication.setDifficulty(MazeDifficulty.MEDIUM);
                BaseGame.instance.changeScene(SceneType.MAZE);
            }
            else if (keyboard.IsKeyDown(Keys.D3) || gamepad.IsButtonDown(Buttons.X))
            {
                // Go to the maze
                MazeCommunication.setDifficulty(MazeDifficulty.HARD);
                BaseGame.instance.changeScene(SceneType.MAZE);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Begin();

            sb.DrawString(font, "MAZE GAME!!!!", new Vector2(100, 100), Color.White);
            sb.DrawString(font, "1/A: Easy Maze", new Vector2(100, 200), Color.White);
            sb.DrawString(font, "2/B: Medium Maze", new Vector2(100, 240), Color.White);
            sb.DrawString(font, "3/X: Hard Maze", new Vector2(100, 280), Color.White);

            sb.End();
        }
    }
}
