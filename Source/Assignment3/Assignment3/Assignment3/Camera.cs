using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment3
{
    class Camera
    {
        //Attributes (variables)
        private Vector3 cameraPos;//camera postion (x,y,z)
        private Vector3 cameraRot;//rotation
        private float cameraSpeed;//movement speed
        private Vector3 LookAt;//What the camera is looking at
        private Vector3 mouseRotationBuffer;
        private Vector3 RS;
        private MouseState curMS;
        private MouseState prevMS;

        private int VPH;
        private int VPW;
        private float AspectRatio;


        //properties

        public Vector3 Position
        {
            get { return cameraPos; }
            set
            {
                cameraPos = value;
                UpdateLookAt();
            }
        }

        public Vector3 Rotation
        {
            get { return cameraRot; }
            set
            {
                cameraRot = value;
                UpdateLookAt();
            }
        }


        public Matrix Projection
        {
            get;
            protected set;
        }

        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(cameraPos, LookAt, Vector3.Up);
            }
        }


        /// <summary>
        /// Constructor.
        /// sets up the camera.
        /// </summary>
        /// <param name="position">camera's position to set</param>
        /// <param name="rotation">camera's rotation to set</param>
        /// <param name="speed">camera's movement speed</param>
        /// <param name="aspectRatio">the game's aspect ratio.</param>
        /// <param name="VPH">the game's ViewPort Height.</param>
        /// <param name="VPW">the game's ViewPort Width.</param>
        public Camera(Vector3 position, Vector3 rotation, float speed, float aspectRatio, int VPH, int VPW)
        {
            cameraSpeed = speed;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.05f, 1000f);

            this.VPH = VPH;
            this.VPW = VPW;

            mouseRotationBuffer.X = rotation.Y;
            RS.X = rotation.Y;
            //set cam pos and rot
            MoveTo(position, rotation);

            prevMS = Mouse.GetState();
        }

        //set camera Pos and Rot
        private void MoveTo(Vector3 Pos, Vector3 Rot)
        {
            Position = Pos;
            Rotation = Rot;
        }

        //update look at
        private void UpdateLookAt()
        {
            //build rot Matrix
            Matrix rotationMatrix = Matrix.CreateRotationX(cameraRot.X) * Matrix.CreateRotationY(cameraRot.Y);

            //create lookat offset (change in lookAt)
            Vector3 LookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);

            //Update camera's lookAt vector
            LookAt = cameraPos + LookAtOffset;
        }

        //Move camera
        public void Move(Vector3 amount)
        {
            Matrix rotate = Matrix.CreateRotationY(cameraRot.Y);
            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotate);

            MoveTo((cameraPos + movement), Rotation);
        }
        
        //update method
        public void Update(GameTime gameTime)
        {
            float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            curMS = Mouse.GetState();

            KeyboardState kbs = Keyboard.GetState();
            GamePadState gps = GamePad.GetState(PlayerIndex.One);

            Vector3 moveVector = Vector3.Zero; //which direction are we going?

            //no controller, so use KB+M
            if (!gps.IsConnected)
            {
                //keyboard
                if (kbs.IsKeyDown(Keys.W))
                {
                    moveVector.Z = 1;
                }

                if (kbs.IsKeyDown(Keys.S))
                {
                    moveVector.Z = -1;
                }

                if (kbs.IsKeyDown(Keys.A))
                {
                    moveVector.X = 1;
                }

                if (kbs.IsKeyDown(Keys.D))
                {
                    moveVector.X = -1;
                }

                //if we are moving
                if (moveVector != Vector3.Zero)
                {
                    moveVector.Normalize();
                    moveVector *= DeltaTime * cameraSpeed;

                    //move camera
                    Move(moveVector);
                }

                //Mouse Movement
                float deltaX;
                float deltaY;

                if (curMS != prevMS)
                {
                    //cache mouse location (to always be relative to middle of screen)
                    deltaX = curMS.X - (VPW / 2);
                    deltaY = curMS.Y - (VPH / 2);

                    //smooths mouse movement; creates rotation
                    mouseRotationBuffer.X -= 0.01f * deltaX * DeltaTime;
                    mouseRotationBuffer.Y -= 0.01f * deltaY * DeltaTime;

                    if (mouseRotationBuffer.Y < MathHelper.ToRadians(-75f))
                    {
                        mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(-75f));
                    }

                    if (mouseRotationBuffer.Y > MathHelper.ToRadians(75f))
                    {
                        mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(75f));
                    }

                    //limit Y to only 75 for looking up or down; wrap X (make it 360)
                    Rotation = new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75f), MathHelper.ToRadians(75f)), MathHelper.WrapAngle(mouseRotationBuffer.X), 0);

                    deltaX = 0;
                    deltaY = 0;

                }


                //set cursor back to the middle of screen
                Mouse.SetPosition(VPW / 2, VPH / 2);
                prevMS = curMS;
            }



            //There is a controller, so don't use M+KB; Only use Controller
            if (gps.IsConnected)
            {
                //Controller camera movement
                moveVector.X = gps.ThumbSticks.Left.X * -1;
                moveVector.Z = gps.ThumbSticks.Left.Y;

                //if we are moving
                if (moveVector != Vector3.Zero)
                {
                    moveVector.Normalize();
                    moveVector *= DeltaTime * cameraSpeed;

                    //move camera
                    Move(moveVector);
                }

                //Controller camera rotation
                float rotX = gps.ThumbSticks.Right.X;
                float rotY = gps.ThumbSticks.Right.Y * -1;

                RS.X -= rotX * DeltaTime;
                RS.Y -= rotY * DeltaTime;

                if (RS.Y < MathHelper.ToRadians(-75f))
                {
                    RS.Y = RS.Y - (RS.Y - MathHelper.ToRadians(-75f));
                }

                if (RS.Y > MathHelper.ToRadians(75f))
                {
                    RS.Y = RS.Y - (RS.Y - MathHelper.ToRadians(75));
                }

                Rotation = new Vector3(-MathHelper.Clamp(RS.Y, MathHelper.ToRadians(-75f), MathHelper.ToRadians(75f)), MathHelper.WrapAngle(RS.X), 0);
            }

        }


    }
}
