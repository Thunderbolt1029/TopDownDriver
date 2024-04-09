using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal static class Globals
    {
        public static List<Level> Levels = new List<Level>();
        public static string CurrentLevelName = "Level0";
        public static Level CurrentLevel => Levels.First(x => x.Name == CurrentLevelName);

        public static EditingLevel CurrentEditingLevel;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            
        }
    }
}
