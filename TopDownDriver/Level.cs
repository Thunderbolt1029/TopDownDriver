using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TopDownDriver
{
    internal class Level
    {
        public string Name;
        public List<Hitbox> Bounds;
        public List<Vector2> GrapplePoints;

        public static string ToJson(Level level) => JsonSerializer.Serialize(level, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        public static Level FromJson(string json) => JsonSerializer.Deserialize<Level>(json, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
    }
}
