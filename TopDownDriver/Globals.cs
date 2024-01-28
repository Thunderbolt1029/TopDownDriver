using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal static class Globals
    {
        public static List<Hitbox> Bounds = new List<Hitbox>();

        public static void Initialize()
        {
            Bounds.AddRange(new[]
            {
                new Hitbox(new Vector2(540, 0), new Vector2(1080, 20), 0f),
                new Hitbox(new Vector2(540, 720), new Vector2(1080, 20), 0f),
                new Hitbox(new Vector2(0, 360), new Vector2(20, 720), 0f),
                new Hitbox(new Vector2(1080, 360), new Vector2(20, 720), 0f),

                new Hitbox(new Vector2(540, 360), new Vector2(100, 100), MathHelper.PiOver4 / 2)
            });
        }        
    }
}
