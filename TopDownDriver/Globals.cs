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

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            /*
            Levels.Add(new Level()
            {
                Name = "Level0",
                Bounds = new List<Hitbox>()
                {
                    new Hitbox(new Vector2(540, 0), new Vector2(1080, 20), 0f),
                    new Hitbox(new Vector2(540, 720), new Vector2(1080, 20), 0f),
                    new Hitbox(new Vector2(0, 360), new Vector2(20, 720), 0f),
                    new Hitbox(new Vector2(1080, 360), new Vector2(20, 720), 0f),

                    new Hitbox(new Vector2(540, 360), new Vector2(100, 100), MathHelper.PiOver4 / 2)
                },
                GrapplePoints = new List<Vector2>()
                {
                    new Hitbox(new Vector2(540, 360), new Vector2(100, 100), MathHelper.PiOver4 / 2).Corners[0],
                    new Hitbox(new Vector2(540, 360), new Vector2(100, 100), MathHelper.PiOver4 / 2).Corners[1],
                    new Hitbox(new Vector2(540, 360), new Vector2(100, 100), MathHelper.PiOver4 / 2).Corners[2],
                    new Hitbox(new Vector2(540, 360), new Vector2(100, 100), MathHelper.PiOver4 / 2).Corners[3],
                }
            });
            */
        }
    }
}
