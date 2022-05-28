using System;
using SFML.System;

namespace AirHockey
{
    internal class VectorExtensions
    {
        public static double DistanceTo(Vector2f from, Vector2f to)
        {
            double Dx = from.X - to.X;
            double Dy = from.Y - to.Y;

            double distance = Math.Sqrt(Dx * Dx + Dy * Dy);

            return distance;
        }
    }
}
