using Assignment3.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment3.Utilities
{
    public static class PhysicsUtil
    {
        /// <summary>
        /// This function smoothly changes one value to another. This should be called every frame.
        /// A ratio of 10 is considered about average, 5 is fast, and 20 is slow.
        /// </summary>
        /// <param name="startVal">The value to change</param>
        /// <param name="endVal">The desired value</param>
        /// <param name="ratio">The ratio of change</param>
        /// <returns>The resulting value</returns>
        public static float smoothChange(float startVal, float endVal, int ratio)
        {
            float newVal = (startVal - endVal) / ratio;
            return -newVal;
        }

        public static bool CheckCollision(Player player, List<Entity> MazeBlocks)
        {
            if (player.playerModel != null)//make sure player model isn't null
            {
                foreach (Entity e in MazeBlocks)//go through entity list
                {
                    if (e.GetType() == typeof(Wall))//make sure it's a wall
                    {
                        Wall w = (Wall)e;//put it into a wall variable
                        for (int i = 0; i < player.playerModel.Meshes.Count; i++)
                        {
                            BoundingSphere PlayerSphere = player.playerModel.Meshes[i].BoundingSphere;
                            PlayerSphere.Center += player.getPosition();

                            for (int j = 0; j < w.model.Meshes.Count; j++)
                            {
                                BoundingSphere WallSphere = w.model.Meshes[j].BoundingSphere;
                                WallSphere.Center += w.getPosition();

                                if (PlayerSphere.Intersects(WallSphere))
                                {
                                    //collision!
                                    return true;
                                }
                            }
                        }
                    }
                    else if (e.GetType() == typeof(Exit))
                    {
                        Exit w = (Exit)e;//put it into a wall variable
                        for (int i = 0; i < player.playerModel.Meshes.Count; i++)
                        {
                            BoundingSphere PlayerSphere = player.playerModel.Meshes[i].BoundingSphere;
                            PlayerSphere.Center += player.getPosition();

                            for (int j = 0; j < w.model.Meshes.Count; j++)
                            {
                                BoundingSphere WallSphere = w.model.Meshes[j].BoundingSphere;
                                WallSphere.Center += w.getPosition();

                                if (PlayerSphere.Intersects(WallSphere))
                                {
                                    //collision!
                                    BaseGame.instance.changeScene(SceneType.MENU);
                                }
                            }
                        }

                    }
                }
            }
            return false;
        }
           
   }
}

