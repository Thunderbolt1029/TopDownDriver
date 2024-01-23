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
        public Vector2 Start, End;
        public Vector2 Centre => Start + (End - Start) / 2;

        // ax + by = c
        internal float A => End.Y - Start.Y;
        internal float B => Start.X - End.X;
        internal float C => A * Start.X + B * End.Y;

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
        public void Draw(SpriteBatch spriteBatch, Color color)
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

        public bool Intersects(Line line2, out Vector2 intersectionPoint) => Intersects(this, line2, out intersectionPoint);

        public static bool Intersects(Line line1, Line line2, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.Zero;

            float w = line1.A * line2.B - line2.A * line1.B;

            if (w == 0)
                return false;

            float u = line2.B * line1.C - line1.B * line2.C;
            float v = line1.A * line2.C - line2.A * line1.C;

            Vector2 iIntersection = new Vector2(u / w, v / w);

            bool OnLine1 = line1.A * iIntersection.X + line1.B * iIntersection.Y == line1.C;
            bool OnLine2 = line2.A * iIntersection.X + line2.B * iIntersection.Y == line2.C;

            if (OnLine1 && OnLine2)
            {
                intersectionPoint = iIntersection;
                return true;
            }
            return false;
        }
    }
}
