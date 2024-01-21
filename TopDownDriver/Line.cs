using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownDriver
{
    internal struct Line
    {
        public Vector2 Start, End;

        // ax + by = c
        internal float A => End.Y - Start.Y;
        internal float B => Start.X - End.X;
        internal float C => A * Start.X + B * End.Y;

        public Line(Vector2 Start, Vector2 End)
        {
            this.Start = Start;
            this.End = End;
        }

        public bool Intersects(Line line2, out Vector2? intersectionPoint) => Intersects(this, line2, out intersectionPoint);

        public static bool Intersects(Line line1, Line line2, out Vector2? intersectionPoint)
        {
            intersectionPoint = null;

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
