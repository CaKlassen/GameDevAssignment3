using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Assignment3.Scenes;

namespace Assignment3.Entities
{
    public class Player : Entity
    {
        public Model playerModel;
        public Vector3 position;

        public Player()
        {
            position = MazeScene.instance.camera.Position;
        }

        public void Load(ContentManager cm)
        {
            playerModel = cm.Load<Model>("Models/Player");
        }

        public override void draw(SpriteBatch sb, Effect effect)
        {
            //nothing needed
        }

        public override void update(GameTime gameTime, GamePadState gamepad, KeyboardState keyboard)
        {
            if (playerModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Vector3 camRot = MazeScene.instance.camera.Rotation;
                Matrix worldMatrix = Matrix.CreateScale(0.1f, 0.1f, 0.1f) * Matrix.CreateRotationY(camRot.Y) * Matrix.CreateTranslation(position);
            }

        }

        public Vector3 getPosition()
        {
            return position;
        }
    }
}
