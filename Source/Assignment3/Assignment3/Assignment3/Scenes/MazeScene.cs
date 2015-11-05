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
    public enum MazeDifficulty
    {
        EASY, MEDIUM, HARD
    }

    public class MazeScene : Scene
    {
        public static MazeScene instance;

        public Camera camera;
        public Player mazeRunner;

        private Vector3 MazeStartPos;
        
        private float AspectRatio;
        
        public List<Entity> collideList = new List<Entity>();
        private bool[,] rawMaze;
        private Floor floor;

        private KeyboardState prevKB;
        private GamePadState prevGP;

        public Matrix Projection;
        public Matrix View;
        public Matrix World;

        public Effect HLSLeffect;


        public MazeScene()
        {
            instance = this;
        }

        public override void onLoad(ContentManager content)
        {
            HLSLeffect = content.Load<Effect>("Effects/Shader");

            MazeDifficulty difficulty = MazeCommunication.getDifficulty();

            // Generate the maze
            MazeBuilder builder = new MazeBuilder(((int)difficulty + 1) * 10);
            rawMaze = builder.buildMaze();

            builder.generateWalls(content, collideList);
            Vector2 startPos = builder.getStartPos();
            MazeStartPos = new Vector3(startPos.X * 4, 2f, startPos.Y * 4);

            // Create the floor
            Vector3 floorPos = new Vector3(rawMaze.GetLength(0) / 2f, 0, rawMaze.GetLength(0) / 2f);
            floor = new Floor(content, floorPos, rawMaze.GetLength(0));

            // Create the camera/player
            AspectRatio = BaseGame.instance.GraphicsDevice.Viewport.AspectRatio;
            camera = new Camera(new Vector3(startPos.X * 4, 2f, startPos.Y * 4), new Vector3(0f, 180f, 0f), 10f, AspectRatio,
                BaseGame.instance.GraphicsDevice.Viewport.Height, BaseGame.instance.GraphicsDevice.Viewport.Width);

            mazeRunner = new Player();
            mazeRunner.Load(content);

            

            prevKB = Keyboard.GetState();

            World = Matrix.Identity;

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
                prevGP = GamePad.GetState(PlayerIndex.One);

            //effect = new BasicEffect(BaseGame.instance.GraphicsDevice);
        }

        public override void update(GameTime gameTime, GamePadState gamepad, KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                // Go to the maze
                BaseGame.instance.changeScene(SceneType.MENU);
            }

            //turn walking through walls on/off
            if (!gamepad.IsConnected)//no controller
            {
                //walking through walls feature
                if (keyboard.IsKeyDown(Keys.P) && !prevKB.IsKeyDown(Keys.P))
                {
                    if (camera.walkThroughWalls)
                        camera.walkThroughWalls = false;
                    else
                        camera.walkThroughWalls = true;
                }

                //return to beginning of maze
                if(keyboard.IsKeyDown(Keys.Home) && !prevKB.IsKeyDown(Keys.Home))
                {
                    camera.Position = MazeStartPos;
                    camera.UpdateFOV(90f);
                }

                //zoom in
                if(Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    float fov = camera.getFOV();
                    if(fov > 45f)
                        fov--;
       
                    camera.UpdateFOV(fov);
                }
                //zoom out
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    float fov = camera.getFOV();
                    if(fov < 110f)
                        fov++;
                    
                    camera.UpdateFOV(fov);
                }

                //reset back to original FOV
                if(Mouse.GetState().MiddleButton == ButtonState.Pressed)
                {
                    camera.UpdateFOV(90f);
                }
            }
            else
            {
                //walking through walls feature
                if(gamepad.IsButtonDown(Buttons.Y) && !prevGP.IsButtonDown(Buttons.Y))
                {
                    if (camera.walkThroughWalls)
                        camera.walkThroughWalls = false;
                    else
                        camera.walkThroughWalls = true;
                }

                //return to beginning of maze
                if(gamepad.IsButtonDown(Buttons.Start) && !prevGP.IsButtonDown(Buttons.Start))
                {
                    camera.Position = MazeStartPos;
                    camera.UpdateFOV(90f);
                }

                //zoom in
                if (gamepad.Triggers.Right > 0)
                {
                    float fov = camera.getFOV();
                    if (fov > 45f)
                        fov--;

                    camera.UpdateFOV(fov);
                }
                //zoom out
                if (gamepad.Triggers.Left > 0)
                {
                    float fov = camera.getFOV();
                    if (fov < 110f)
                        fov++;

                    camera.UpdateFOV(fov);
                }

                //reset back to original FOV
                if (gamepad.IsButtonDown(Buttons.RightStick) && !prevGP.IsButtonDown(Buttons.RightStick))
                {
                    camera.UpdateFOV(90f);
                }
            }

            

            mazeRunner.update(gameTime, gamepad, keyboard);
            camera.Update(gameTime);

            // Update the model list
            foreach (Entity e in collideList)
            {
                e.update(gameTime, gamepad, keyboard);
            }
        }

        public override void draw(SpriteBatch sb)
        {
            // Reset the render state
            BaseGame.instance.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            BaseGame.instance.GraphicsDevice.BlendState = BlendState.Opaque;
            BaseGame.instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            BaseGame.instance.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // Update the shader parameters
            Vector3 viewVector = Vector3.Transform(camera.getLookAt() - camera.Position, Matrix.CreateRotationY(0));
            viewVector.Normalize();

            //test
            Vector3 position = camera.Position;
            Vector3 LAt = camera.getLookAt();
            //LAt.X *= -1;
            //LAt.Z *= -1;
            //Vector3 LminPos = position - LAt;
            //LminPos.Normalize();
            //LminPos = Vector3.TransformNormal(LminPos, Matrix.Invert(World));
            //Console.Write("LookAt: " + LAt + "\n");
            //Console.Write("Position - LookAt: " + LminPos + "\n");

            //Console.Write("LookAt: " + LAt + "\nPosition: " + position + "\n");

            HLSLeffect.CurrentTechnique = HLSLeffect.Techniques["ShaderTech"];

            HLSLeffect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            HLSLeffect.Parameters["AmbientIntensity"].SetValue(0.1f);

           
            HLSLeffect.Parameters["spotlightDirection"].SetValue(Vector3.Normalize(position - LAt));
            HLSLeffect.Parameters["spotlightPosition"].SetValue(Vector3.Transform(position, Projection * View));
            HLSLeffect.Parameters["lightColor"].SetValue(Color.White.ToVector3());

            HLSLeffect.Parameters["View"].SetValue(camera.View);
            HLSLeffect.Parameters["Projection"].SetValue(camera.Projection);
            HLSLeffect.Parameters["ViewVector"].SetValue(viewVector);

            floor.draw(sb, HLSLeffect);

            //// Render the model list
            foreach (Entity e in collideList)
            {
                e.draw(sb, HLSLeffect);
            }
        }
    }
}
