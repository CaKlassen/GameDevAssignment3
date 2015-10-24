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
        public MenuScene()
        {

        }

        public override void onLoad(ContentManager content)
        {

        }

        public override void update(GameTime gameTime, GamePadState gamepad, KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Enter))
            {
                // Go to the maze
                BaseGame.instance.changeScene(SceneType.MAZE);
            }
        }

        public override void draw(SpriteBatch sb)
        {

        }
    }
}
