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
using Assignment3.Scenes;

namespace Assignment3.Entities
{
    public class Wall : Entity
    {
        public static int WALL_LENGTH = 200;

        public Model model;
        private Vector3 pos;

        private float scale = 0.02f;

        public Wall(ContentManager content, Vector3 position)
        {
            pos = position;
            pos.X *= (WALL_LENGTH * scale);
            pos.Y *= (WALL_LENGTH * scale);
            pos.Z *= (WALL_LENGTH * scale);

            model = content.Load<Model>("Models/Wall");
        }
        
        public override void update(GameTime gameTime, GamePadState gamepad, KeyboardState keyboard)
        {

        }

        public override void draw(SpriteBatch sb)
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {

                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();//lighting

                    //effect.DirectionalLight0.Enabled = true;
                    //effect.DirectionalLight0.DiffuseColor = Color.Red.ToVector3();
                    //effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, -1.5f, 0));

                    //effect.DirectionalLight1.Enabled = true;
                    //effect.DirectionalLight1.DiffuseColor = Color.Red.ToVector3();
                    //effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(1, -1.5f, -1));

                    //effect.DirectionalLight2.Enabled = true;
                    //effect.DirectionalLight2.DiffuseColor = Color.Red.ToVector3();
                    //effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(-1, -1.5f, -1));

                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale)
                        * Matrix.CreateTranslation(pos);
                    effect.View = MazeScene.instance.camera.View;
                    effect.Projection = MazeScene.instance.camera.Projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        public Vector3 getPosition()
        {
            return pos;
        }
    }
}
