using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal static  class Globals
    {
        public static bool UsingController = true;

        public static List<Hitbox> Bounds = new List<Hitbox>();
        
        public static void Initialize()
        {
            Bounds.AddRange(new[]
            {
                new Hitbox(new Rectangle(-10, -10, 20, 730), 0f),
                new Hitbox(new Rectangle(-10, 710, 1090, 20), 0f),
                new Hitbox(new Rectangle(1070, -10, 20, 730), 0f),
                new Hitbox(new Rectangle(-10, -10, 1090, 20), 0f),

                new Hitbox(new Rectangle(490, 310, 100, 100), MathHelper.PiOver4 / 2)
            });
        }
    }
}
