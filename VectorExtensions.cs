using System;
using SFML.System;
using SFML.Graphics;

namespace AirHockey
{
    public class VectorExtensions
    {
        public static double DistanceTo(Vector2f from, Vector2f to)
        {
            double Dx = from.X - to.X;
            double Dy = from.Y - to.Y;

            double distance = Math.Sqrt(Dx * Dx + Dy * Dy);

            return distance;
        }
        public static bool isColliding(CircleShape object1, CircleShape object2)
        {
            double distance = DistanceTo(object1.Position, object2.Position);

            if (distance < object1.Radius + object2.Radius)
            {
                return true;
            }

            return false;
        }
    }
}
