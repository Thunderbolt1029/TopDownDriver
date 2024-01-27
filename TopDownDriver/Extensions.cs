using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TopDownDriver
{
    internal static class Extensions
    {
        public static Vector2 DirectionVector(this Vector2 vector) => vector.Length() > 0 ? Vector2.Normalize(vector) : Vector2.Zero;
    }
}
