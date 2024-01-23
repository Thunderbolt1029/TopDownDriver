using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal struct Hitbox
    {
        public Rectangle nonRotated;
        public float rotation;

        public Hitbox(Rectangle nonRotated, float rotation)
        {
            this.nonRotated = nonRotated;
            this.rotation = rotation;
        }

        public Rectangle DisplayRectangle => new Rectangle(Vector2.Round(new Vector2(nonRotated.X + nonRotated.Size.X / 2, nonRotated.Y + nonRotated.Size.Y / 2)).ToPoint(), nonRotated.Size);
        public Rectangle axisAlignedBoundingBox
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

        public Vector2[] Corners
        {
            get
            {
                Vector2[] corners = new Vector2[4];
                Vector2[] transformedCorners = new Vector2[4];

                corners[0] = new Vector2(-nonRotated.Size.X, -nonRotated.Size.Y) / 2;
                corners[1] = new Vector2(-nonRotated.Size.X, nonRotated.Size.Y) / 2;
                corners[2] = new Vector2(nonRotated.Size.X, nonRotated.Size.Y) / 2;
                corners[3] = new Vector2(nonRotated.Size.X, -nonRotated.Size.Y) / 2;

                for (int i = 0; i < 4; i++)
                    transformedCorners[i] = Vector2.Transform(corners[i], Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(nonRotated.Location.ToVector2() + nonRotated.Size.ToVector2() / 2, 0)));
                
                return transformedCorners;
            }
        }

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

        static bool QuickIntersect(Hitbox hitbox1, Hitbox hitbox2) => hitbox1.axisAlignedBoundingBox.Intersects(hitbox2.axisAlignedBoundingBox);

        public bool Intersects(Hitbox hitbox2, out Vector2 intersectionPoint) => Intersects(this, hitbox2, out intersectionPoint);
        public static bool Intersects(Hitbox hitbox1, Hitbox hitbox2, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.Zero;
            if (!QuickIntersect(hitbox1, hitbox2))
                return false;

            foreach (Line line1 in hitbox1.Sides)
                foreach (var line2 in hitbox2.Sides)
                    if (Line.Intersects(line1, line2, out intersectionPoint))
                        return true;

            return false;
        }
    }
}
