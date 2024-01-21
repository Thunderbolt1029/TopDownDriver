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
        public static List<Hitbox> Bounds = new List<Hitbox>();
        
        public static void Initialize()
        {
            Bounds.AddRange(new[]
            {
                new Hitbox(new Rectangle(-10, -10, 20, 720), 0f),
                new Hitbox(new Rectangle(-10, 710, 1080, 20), 0f),
                new Hitbox(new Rectangle(1070, -10, 20, 720), 0f),
                new Hitbox(new Rectangle(-10, -10, 1080, 20), 0f)
            });
        }
    }
}
