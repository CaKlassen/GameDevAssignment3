using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assignment3.Scenes;

namespace Assignment3
{
    public static class MazeCommunication
    {
        private static MazeDifficulty difficulty;

        public static MazeDifficulty getDifficulty()
        {
            return difficulty;
        }

        public static void setDifficulty(MazeDifficulty dif)
        {
            difficulty = dif;
        }
    }
}
