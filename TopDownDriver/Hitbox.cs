using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal struct Hitbox
    {
        public Vector2 Centre;
        public Vector2 Size;
        public float Rotation;

        public Hitbox(Vector2 centre, Vector2 size, float rotation)
        {
            Centre = centre;
            Size = size;
            Rotation = rotation;
        }

        [JsonIgnore]
        public Rectangle DisplayRectangle => new Rectangle(Vector2.Round(new Vector2(Centre.X, Centre.Y)).ToPoint(), Size.ToPoint());
        [JsonIgnore]
        public Rectangle AxisAlignedBoundingBox
        {
            get
            {
                Rectangle rectangle = new Rectangle()
                {
                    X = (int)Corners.Min(x => x.X),
                    Y = (int)Corners.Min(x => x.Y),
                };

                rectangle.Width = (int)Corners.Max(x => x.X) - rectangle.X;
                rectangle.Height = (int)Corners.Max(x => x.Y) - rectangle.Y;

                return rectangle;
            }
        }

        [JsonIgnore]
        public Vector2[] Corners
        {
            get
            {
                Vector2[] corners = new Vector2[4];
                Vector2[] transformedCorners = new Vector2[4];

                corners[0] = new Vector2(-Size.X, -Size.Y) / 2;
                corners[1] = new Vector2(-Size.X, Size.Y) / 2;
                corners[2] = new Vector2(Size.X, Size.Y) / 2;
                corners[3] = new Vector2(Size.X, -Size.Y) / 2;

                for (int i = 0; i < 4; i++)
                    transformedCorners[i] = Vector2.Transform(corners[i], Matrix.CreateRotationZ(Rotation) * Matrix.CreateTranslation(new Vector3(Centre, 0)));
                
                return transformedCorners;
            }
        }

        [JsonIgnore]
        public Line[] Sides
        {
            get
            {
                Line[] lines = new Line[4];

                lines[0] = new Line(Corners[0], Corners[1]);
                lines[1] = new Line(Corners[1], Corners[2]);
                lines[2] = new Line(Corners[2], Corners[3]);
                lines[3] = new Line(Corners[3], Corners[0]);

                return lines;
            }
        }

        static bool QuickIntersect(Hitbox hitbox1, Hitbox hitbox2) => hitbox1.AxisAlignedBoundingBox.Intersects(hitbox2.AxisAlignedBoundingBox);
        public readonly bool Intersects(Hitbox hitbox2, out List<Vector2> intersectionPoints) => Intersects(this, hitbox2, out intersectionPoints);
        public static bool Intersects(Hitbox hitbox1, Hitbox hitbox2, out List<Vector2> intersectionPoints)
        {
            intersectionPoints = new List<Vector2>();
            if (!QuickIntersect(hitbox1, hitbox2))
                return false;

            foreach (Line line1 in hitbox1.Sides)
                foreach (var line2 in hitbox2.Sides)
                    if (Line.Intersects(line1, line2, out Vector2 intersectionPoint))
                        intersectionPoints.Add(intersectionPoint);

            return intersectionPoints.Count != 0;
        }

        public readonly Vector2 FindNormal(Vector2 pointOnRectangle) => FindNormal(this, pointOnRectangle);
        public static Vector2 FindNormal(Hitbox hitbox, Vector2 pointOnRectangle) 
        {
            Line line = Line.Empty;
            float MinDistance = float.MaxValue;
            foreach (Line l in hitbox.Sides)
            {
                float distance = l.LineToPointDistance(pointOnRectangle);
                if (distance < MinDistance)
                {
                    line = l;
                    MinDistance = distance;
                }
            }
            
            return Vector2.Normalize(line.Centre - hitbox.Centre);
        }
    }
}
