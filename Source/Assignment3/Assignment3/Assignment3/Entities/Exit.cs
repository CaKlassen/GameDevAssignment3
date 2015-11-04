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
    public class Exit : Entity
    {
        public static int WALL_LENGTH = 200;

        public Model model;
        private Vector3 pos;
        private Texture2D texture;

        private float scale = 0.02f;

        public Exit(ContentManager content, Vector3 position)
        {
            pos = position;
            pos.X *= (WALL_LENGTH * scale);
            pos.Y *= (WALL_LENGTH * scale);
            pos.Z *= (WALL_LENGTH * scale);

            model = content.Load<Model>("Models/Player");
            texture = content.Load<Texture2D>("Models/StoneFloor");
        }
        
        public override void update(GameTime gameTime, GamePadState gamepad, KeyboardState keyboard)
        {

        }

        public override void draw(SpriteBatch sb, Effect effect)
        {
            // Copy any parent transforms.
            Matrix worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(pos);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * worldMatrix);
                    effect.Parameters["ModelTexture"].SetValue(texture);


                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * worldMatrix));
                    effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
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
