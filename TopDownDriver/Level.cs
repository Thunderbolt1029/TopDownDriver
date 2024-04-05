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
        public Vector2 SpawnPoint;

        public static string ToJson(Level level) => JsonSerializer.Serialize(level, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        public static Level FromJson(string json) => JsonSerializer.Deserialize<Level>(json, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
    }

    internal class EditingLevel
    {
        public string Name;
        public List<(Hitbox, bool)> Bounds;
        public List<(Vector2, bool)> GrapplePoints;
        public (Vector2, bool) SpawnPoint;

        public static EditingLevel LoadLevel(Level level)
        {
            EditingLevel editingLevel = new EditingLevel()
            {
                Name = level.Name,
                Bounds = new List<(Hitbox, bool)>(),
                GrapplePoints = new List<(Vector2, bool)>(),
                SpawnPoint = (level.SpawnPoint, false)
            };

            foreach (Hitbox hitbox in level.Bounds)
                editingLevel.Bounds.Add((hitbox, false));

            foreach (Vector2 grapplePoint in level.GrapplePoints)
                editingLevel.GrapplePoints.Add((grapplePoint, false));

            return editingLevel;
        }

        public Level GetLevel()
        {
            return new Level()
            {
                Name = Name,
                Bounds = Bounds.ConvertAll(x => x.Item1),
                GrapplePoints = GrapplePoints.ConvertAll(x => x.Item1),
                SpawnPoint = SpawnPoint.Item1
            };
        }
    }
}
