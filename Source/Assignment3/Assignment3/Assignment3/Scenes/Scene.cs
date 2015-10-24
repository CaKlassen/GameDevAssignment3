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
    public abstract class Scene
    {
        public Scene()
        {

        }

        public abstract void onLoad(ContentManager content);

        public abstract void update(GameTime gameTime, GamePadState gamepad, KeyboardState keyboard);

        public abstract void draw(SpriteBatch sb);
    }
}
