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
using Assignment3.Utilities;

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
        private Floor roof;

        private KeyboardState prevKB;
        private GamePadState prevGP;

        public Matrix Projection;
        public Matrix View;
        public Matrix World;

        public Effect HLSLeffect;

        private static int AMBIENT_RATE = 20;
        private static Vector4 nightColour = new Vector4(0.05f, 0, 0.1f, 1);
        private static Vector4 dayColour = new Vector4(1, 1, 0.9f, 1);
        private static Vector4 nightFogColour = new Vector4(0.02f, 0.02f, 0.02f, 1);
        private static Vector4 dayFogColour = new Vector4(0.4f, 0.4f, 0.4f, 1);
        private static float dayFlashlight = 2.0f;
        private static float nightFlashlight = 0.785398f;
        private static float nightIntensity = 0.9f;
        private static float dayIntensity = 0.2f;

        private Vector4 ambientColour = dayColour;
        private Vector4 fogColour = dayFogColour;
        bool fogEnabled = true;
        private float flashlight = dayFlashlight;
        private float ambientIntensity = dayIntensity;
        private bool day = true;

        MazeDifficulty difficulty;
        private float[] fogLevels = { 30, 20, 10 };


        public MazeScene()
        {
            instance = this;
        }

        public override void onLoad(ContentManager content)
        {
            HLSLeffect = content.Load<Effect>("Effects/Shader");

            difficulty = MazeCommunication.getDifficulty();

            // Generate the maze
            MazeBuilder builder = new MazeBuilder(((int)difficulty + 1) * 10);
            rawMaze = builder.buildMaze();

            builder.generateWalls(content, collideList);
            Vector2 startPos = builder.getStartPos();
            MazeStartPos = new Vector3(startPos.X * 4, 2f, startPos.Y * 4);

            float length = rawMaze.GetLength(0) / 2f;

            // Create the floor
            Vector3 floorPos = new Vector3(16.5f, -0.8f, 16.5f);
            Vector3 roofPos = new Vector3(rawMaze.GetLength(0) / 2f, 2.5f, rawMaze.GetLength(0) / 2f);
            floor = new Floor(content, floorPos, 30);
            roof = new Floor(content, roofPos, 30);

            // Create the camera/player
            AspectRatio = BaseGame.instance.GraphicsDevice.Viewport.AspectRatio;
            camera = new Camera(new Vector3(startPos.X * 4, 2f, startPos.Y * 4), new Vector3(0f, 0f, 0f), 10f, AspectRatio,
                BaseGame.instance.GraphicsDevice.Viewport.Height, BaseGame.instance.GraphicsDevice.Viewport.Width);

            mazeRunner = new Player();
            mazeRunner.position = camera.Position;
            mazeRunner.Load(content);
            

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

                // Switching day/night
                if (keyboard.IsKeyDown(Keys.Z) && !prevKB.IsKeyDown(Keys.Z))
                {
                    day = !day;
                }

                // Toggle fog
                if (keyboard.IsKeyDown(Keys.X) && !prevKB.IsKeyDown(Keys.X))
                {
                    fogEnabled = !fogEnabled;
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

                // Switching day/night
                if (gamepad.IsButtonDown(Buttons.LeftShoulder) && !prevGP.IsButtonDown(Buttons.LeftShoulder))
                {
                    day = !day;
                }

                // Toggle fog
                if (gamepad.IsButtonDown(Buttons.RightShoulder) && !prevGP.IsButtonDown(Buttons.RightShoulder))
                {
                    fogEnabled = !fogEnabled;
                }
            }

            // Update the ambience
            if (day)
            {
                ambientColour.X += PhysicsUtil.smoothChange(ambientColour.X, dayColour.X, AMBIENT_RATE);
                ambientColour.Y += PhysicsUtil.smoothChange(ambientColour.Y, dayColour.Y, AMBIENT_RATE);
                ambientColour.Z += PhysicsUtil.smoothChange(ambientColour.Z, dayColour.Z, AMBIENT_RATE);

                ambientIntensity += PhysicsUtil.smoothChange(ambientIntensity, dayIntensity, AMBIENT_RATE);
                flashlight += PhysicsUtil.smoothChange(flashlight, dayFlashlight, AMBIENT_RATE);

                fogColour.X += PhysicsUtil.smoothChange(fogColour.X, dayFogColour.X, AMBIENT_RATE);
                fogColour.Y += PhysicsUtil.smoothChange(fogColour.Y, dayFogColour.Y, AMBIENT_RATE);
                fogColour.Z += PhysicsUtil.smoothChange(fogColour.Z, dayFogColour.Z, AMBIENT_RATE);
            }
            else
            {
                ambientColour.X += PhysicsUtil.smoothChange(ambientColour.X, nightColour.X, AMBIENT_RATE);
                ambientColour.Y += PhysicsUtil.smoothChange(ambientColour.Y, nightColour.Y, AMBIENT_RATE);
                ambientColour.Z += PhysicsUtil.smoothChange(ambientColour.Z, nightColour.Z, AMBIENT_RATE);

                ambientIntensity += PhysicsUtil.smoothChange(ambientIntensity, nightIntensity, AMBIENT_RATE);
                flashlight += PhysicsUtil.smoothChange(flashlight, nightFlashlight, AMBIENT_RATE);

                fogColour.X += PhysicsUtil.smoothChange(fogColour.X, nightFogColour.X, AMBIENT_RATE);
                fogColour.Y += PhysicsUtil.smoothChange(fogColour.Y, nightFogColour.Y, AMBIENT_RATE);
                fogColour.Z += PhysicsUtil.smoothChange(fogColour.Z, nightFogColour.Z, AMBIENT_RATE);
            }

            mazeRunner.update(gameTime, gamepad, keyboard);
            camera.Update(gameTime);

            // Update the model list
            foreach (Entity e in collideList)
            {
                e.update(gameTime, gamepad, keyboard);
            }

            prevKB = keyboard;
            prevGP = gamepad;
        }

        public override void draw(SpriteBatch sb)
        {
            // Reset the render state
            BaseGame.instance.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            BaseGame.instance.GraphicsDevice.BlendState = BlendState.Opaque;
            BaseGame.instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            BaseGame.instance.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // Update the shader parameters
            Vector3 viewVector = camera.getLookAt() - camera.Position;
            viewVector.Normalize();



            //test
            Matrix cameraWorld = Matrix.CreateTranslation(camera.Position);
            Vector3 position = camera.Position;
            Vector3 LAt = camera.getLookAt() -position;

            Console.Write("\nLookAt: " +Vector3.Normalize( LAt )+ "\n");
            //Console.Write("\nView Vector: " + viewVector + "\n");
            //Console.Write("\nPosition: " + position + "\n");

            HLSLeffect.CurrentTechnique = HLSLeffect.Techniques["ShaderTech"];
            
            HLSLeffect.Parameters["AmbientColor"].SetValue(ambientColour);
            HLSLeffect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
            HLSLeffect.Parameters["fogColor"].SetValue(fogColour);
            HLSLeffect.Parameters["fogFar"].SetValue(fogLevels[(int) difficulty]);
            HLSLeffect.Parameters["fogEnabled"].SetValue(fogEnabled);
            HLSLeffect.Parameters["FlashlightAngle"].SetValue(flashlight);

            //HLSLeffect.Parameters["SpotlightConeAngle"].SetValue(MathHelper.ToRadians(23.5f));
            HLSLeffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(LAt));
            HLSLeffect.Parameters["EyePosition"].SetValue(position);//SetValue(Vector3.Transform(position, View));
            //HLSLeffect.Parameters["lightColor"].SetValue(Color.White.ToVector3());

            HLSLeffect.Parameters["View"].SetValue(camera.View);
            HLSLeffect.Parameters["Projection"].SetValue(camera.Projection);
            //HLSLeffect.Parameters["ViewVector"].SetValue(viewVector);

            floor.draw(sb, HLSLeffect);
            roof.draw(sb, HLSLeffect);

            //Render the model list
            foreach (Entity e in collideList)
            {
                e.draw(sb, HLSLeffect);
            }
        }
    }
}
