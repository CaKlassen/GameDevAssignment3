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

        public override void draw(SpriteBatch sb)
        {
            //nothing needed
        }

        public override void update(GameTime gameTime, GamePadState gamepad, KeyboardState keyboard)
        {
            if (playerModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[playerModel.Bones.Count];
                playerModel.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in playerModel.Meshes)
                {

                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();//lighting
                        Vector3 camRot = MazeScene.instance.camera.Rotation;
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(0.1f, 0.1f, 0.1f) * Matrix.CreateRotationY(camRot.Y)
                            * Matrix.CreateTranslation(position);
                        effect.View = MazeScene.instance.camera.View;
                        effect.Projection = MazeScene.instance.camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    //mesh.Draw();
                }
            }

            //update model position w/ camera
            //position = MazeScene.instance.camera.Position;
        }

        public Vector3 getPosition()
        {
            return position;
        }
    }
}
