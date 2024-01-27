using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal struct Line
    {
        public static Line Empty => new Line(Vector2.Zero, Vector2.Zero);

        public Vector2 Start, End;
        public readonly Vector2 Centre => Start + (End - Start) / 2;

        // ax + by = c
        public readonly float A => End.Y - Start.Y;
        public readonly float B => Start.X - End.X;
        public readonly float C => - End.X * Start.Y + Start.X * End.Y;

        public Line(Vector2 Start, Vector2 End)
        {
            this.Start = Start;
            this.End = End;
        }

        /*
            dx = abs(x1 - x0)
            sx = x0 < x1 ? 1 : -1
            dy = -abs(y1 - y0)
            sy = y0 < y1 ? 1 : -1
            error = dx + dy
    
            while true
                plot(x0, y0)
                if x0 == x1 && y0 == y1 break
                e2 = 2 * error
                if e2 >= dy
                    if x0 == x1 break
                    error = error + dy
                    x0 = x0 + sx
                end if
                if e2 <= dx
                    if y0 == y1 break
                    error = error + dx
                    y0 = y0 + sy
                end if
            end while
        */
        public readonly void Draw(SpriteBatch spriteBatch, Color color)
        {
            Texture2D ColorTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            ColorTexture.SetData(new[] { Color.White });

            int x0 = (int)Math.Round(Start.X);
            int x1 = (int)Math.Round(End.X);
            int y0 = (int)Math.Round(Start.Y);
            int y1 = (int)Math.Round(End.Y);

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;

            while (true)
            {
                spriteBatch.Draw(ColorTexture, new Rectangle(x0, y0, 1, 1), color);

                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * error;
                if (e2 >= error)
                {
                    if (x0 == x1) break;
                    error += dy;
                    x0 += sx;
                }
                if (e2 <= dx)
                {
                    if (y0 == y1) break;
                    error += dx;
                    y0 += sy;
                }
            }
        }

        /*
        char get_line_intersection(float p0_x, float p0_y, float p1_x, float p1_y, 
            float p2_x, float p2_y, float p3_x, float p3_y, float *i_x, float *i_y)
        {
            float s1_x, s1_y, s2_x, s2_y;
            s1_x = p1_x - p0_x;     s1_y = p1_y - p0_y;
            s2_x = p3_x - p2_x;     s2_y = p3_y - p2_y;

            float s, t;
            s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = ( s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                if (i_x != NULL)
                    *i_x = p0_x + (t * s1_x);
                if (i_y != NULL)
                    *i_y = p0_y + (t * s1_y);
                return 1;
            }

            return 0; // No collision
        }
        */
        public readonly bool Intersects(Line line2, out Vector2 intersectionPoint) => Intersects(this, line2, out intersectionPoint);

        public static bool Intersects(Line line1, Line line2, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.Zero;

            float s1_x, s1_y, s2_x, s2_y;
            s1_x = line1.End.X - line1.Start.X; s1_y = line1.End.Y - line1.Start.Y;
            s2_x = line2.End.X - line2.Start.X; s2_y = line2.End.Y - line2.Start.Y;

            float s, t;
            s = (-s1_y * (line1.Start.X - line2.Start.X) + s1_x * (line1.Start.Y - line2.Start.Y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (line1.Start.Y - line2.Start.Y) - s2_y * (line1.Start.X - line2.Start.X)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                intersectionPoint.X = line1.Start.X + (t * s1_x);
                intersectionPoint.Y = line1.Start.Y + (t * s1_y);
                return true;
            }

            return false; // No collision
        }

        public readonly float LineToPointDistance(Vector2 point) => (float)(Math.Abs(A * point.X + B * point.Y - C) / Math.Sqrt(A * A + B * B));
    }
}
