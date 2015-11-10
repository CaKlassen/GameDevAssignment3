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

namespace Assignment3
{
    public class MazeBuilder
    {
        private Random random;
        private bool[,] rawMaze;
        private int size;
        private Vector2 startPos = new Vector2(0, 0);
        private Vector2 exitPos = new Vector2(0, 0);

        /// <summary>
        /// The MazeBuilder constructor.
        /// </summary>
        /// <param name="size">The size of the maze (excluding outer walls)</param>
        public MazeBuilder(int mazeSize)
        {
            // Ensure that the maze is of odd length
            if (mazeSize % 2 == 0)
            {
                mazeSize++;
            }

            size = mazeSize;
            random = new Random();

            // Create the maze base
            rawMaze = new bool[size + 2, size + 2];

            // Set the maze to empty
            for (int i = 0; i < size + 2; i++)
            {
                for (int j = 0; j < size + 2; j++)
                {
                    rawMaze[i, j] = false;
                }
            }

            // Generate the maze walls
            generateMazeWalls();
        }

        /// <summary>
        /// This function builds a maze and returns it as a 2D array.
        /// </summary>
        /// <returns>The constructed maze</returns>
        public bool[,] buildMaze()
        {
            divide(1, 1, size, size, chooseOrientation(size, size));

            generateStartAndExit();

            //printDebugMaze();

            return rawMaze;
        }

        private void generateStartAndExit()
        {
            int startWall = random.Next(4);
            int endWall;
            bool horizontal;

            if (startWall == 0)
            {
                startWall = 1;
                endWall = size;
                horizontal = false;
            }
            else if (startWall == 1)
            {
                startWall = 1;
                endWall = size;
                horizontal = true;
            }
            else if (startWall == 2)
            {
                startWall = 1;
                endWall = size;
                horizontal = false;
            }
            else
            {
                startWall = 1;
                endWall = size;
                horizontal = true;
            }

            // Place the start and end
            int randStartPos, randEndPos;

            if (horizontal)
            {
                do
                {
                    randStartPos = random.Next(size);
                }
                while (rawMaze[startWall, randStartPos] == true);
                
                do
                {
                    randEndPos = random.Next(size);
                }
                while (rawMaze[endWall, randEndPos] == true);

                startPos.X = randStartPos;
                startPos.Y = startWall;

                exitPos.X = randEndPos;
                exitPos.Y = endWall;
            }
            else
            {
                do
                {
                    randStartPos = random.Next(size);
                }
                while (rawMaze[randStartPos, startWall] == true);

                do
                {
                    randEndPos = random.Next(size);
                }
                while (rawMaze[randEndPos, endWall] == true);

                startPos.Y = randStartPos;
                startPos.X = startWall;

                exitPos.Y = randEndPos;
                exitPos.X = endWall;
            }
            
        }

        public void generateWalls(ContentManager content, List<Entity> collideList)
        {
            // Generate the walls
            for (int i = 0; i < size + 2; i++)
            {
                for (int j = 0; j < size + 2; j++)
                {
                    if (rawMaze[i, j] == true)
                    {
                        collideList.Add(new Wall(content, new Vector3(j, 0, i)));
                    }
                }
            }

            // Generate the exit
            collideList.Add(new Exit(content, new Vector3(exitPos.X, 0, exitPos.Y)));
        }


        /// <summary>
        /// This function generates the maze outer walls.
        /// </summary>
        private void generateMazeWalls()
        {
            for (int i = 0; i < size + 2; i++)
            {
                rawMaze[0, i] = true;
                rawMaze[size + 1, i] = true;
                rawMaze[i, 0] = true;
                rawMaze[i, size + 1] = true;
            }
        }

        /// <summary>
        /// This function divides a subsection of the maze into two smaller subsections.
        /// </summary>
        /// <param name="x">The starting x position of the subsection</param>
        /// <param name="y">The starting y position of the subsection</param>
        /// <param name="width">The width of the subsection</param>
        /// <param name="height">The height of the subsection</param>
        /// <param name="horizontal">Whether or not the division is horizontal</param>
        private void divide(int x, int y, int width, int height, bool horizontal)
        {
            // Check if this is the target size
            if (width < 2 || height < 2)
            {
                return;
            }

            int wallX, wallY, pathX, pathY, directionX, directionY, length;

            // Determine the start point of the wall
            wallX = x + (horizontal ? 0 : random.Next(width - 2));
            if (!horizontal && wallX % 2 != 0)
            {
                wallX++;
            }

            wallY = y + (horizontal ? random.Next(height - 2) : 0);
            if (horizontal && wallY % 2 != 0)
            {
                wallY++;
            }

            // Determine the position of the path through the wall
            pathX = wallX + (horizontal ? random.Next(width) : 0);
            if (horizontal && pathX % 2 == 0)
            {
                pathX++;
            }

            pathY = wallY + (horizontal ? 0 : random.Next(height));
            if (!horizontal && pathY % 2 == 0)
            {
                pathY++;
            }

            // Determine the direction of the wall
            directionX = horizontal ? 1 : 0;
            directionY = horizontal ? 0 : 1;

            // Determine the length of the wall
            length = horizontal ? width : height;

            // Create the wall
            for (int i = 0; i < length; i++)
            {
                if (horizontal)
                {
                    if (wallX != pathX)
                    {
                        rawMaze[wallY, wallX] = true;
                    }
                }
                else
                {
                    if (wallY != pathY)
                    {
                        rawMaze[wallY, wallX] = true;
                    }
                }

                wallX += directionX;
                wallY += directionY;
            }

            //printDebugMaze();
            //Console.WriteLine();

            int newX, newY, newWidth, newHeight;

            // Recurse on one side of the wall
            newX = x;
            newY = y;
            newWidth = horizontal ? width : wallX - x;
            newHeight = horizontal ? wallY - y: height;

            divide(newX, newY, newWidth, newHeight, chooseOrientation(newWidth, newHeight));

            // Recurse on the other side of the wall
            newX = horizontal ? x : wallX + 1;
            newY = horizontal ? wallY + 1 : y;
            newWidth = horizontal ? width : x + width - wallX - 1;
            newHeight = horizontal ? y + height - wallY - 1 : height;

            divide(newX, newY, newWidth, newHeight, chooseOrientation(newWidth, newHeight));
        }

        /// <summary>
        /// This function returns whether or not the next 'cut' is horizontal.
        /// </summary>
        /// <param name="width">The width of the subsection</param>
        /// <param name="height">The height of the subsection</param>
        /// <returns>The orientation of the next cut</returns>
        private bool chooseOrientation(int width, int height)
        {
            if (width < height)
            {
                return true;
            }
            else if (height < width)
            {
                return false;
            }
            else
            {
                return random.Next(2) == 0;
            }
        }

        public Vector2 getStartPos()
        {
            return startPos;
        }

        /// <summary>
        /// This function prints the maze layout to the console.
        /// </summary>
        public void printDebugMaze()
        {
            for (int i = 0; i < size + 2; i++)
            {
                for (int j = 0; j < size + 2; j++)
                {
                    Console.Write((rawMaze[i, j] ? 'X' : '.') + " ");
                }

                Console.WriteLine();
            }
        }
    }
}
