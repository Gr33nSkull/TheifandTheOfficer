using System;
using System.Collections.Generic;
using System.Text;

namespace OfficerAndTheTheif
{
    public class Vector2
    {
        public int x;
        public int y;

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool InRectangleArea(Vector2 area_end, Vector2 area_start)
        {
            if (this.x < area_start.x || this.x > area_end.x || this.y < area_start.y || this.y > area_end.y) 
                return false;

            return true;
        }

        public bool InCircleArea(Vector2 middle, int r, Vector2 point)
        {
            //  √[(x₂ - x₁)² + (y₂ - y₁)²]
            if (Math.Pow(point.x - middle.x, 2) + Math.Pow(point.y - middle.y, 2) < Math.Pow(r, 2))
                return true;
            return false;
        }

        public int Distance(Vector2 point1, Vector2 point2)
        {
            return (int)Math.Sqrt(Math.Pow(point2.x - point1.x, 2) + Math.Pow(point2.y - point1.y, 2));
        }
    }
}
