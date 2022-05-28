using SFML.System;
using SFML.Graphics;

namespace AirHockey
{
    public class Player : CircleShape
    {
        public int score = 0;

        public Player(Color meshColor, uint radius, Vector2f position)
        {
            FillColor = meshColor;
            Radius = radius;
            OutlineColor = Color.Black;
            OutlineThickness = 2;
            Position = position;
        }

        public void TryMove(float newPlayerX, Player player, uint height)
        {
            if (newPlayerX <= height - player.Radius * 2 && newPlayerX >= 0)
            {
                player.Position = new Vector2f(player.Position.X, newPlayerX);
            }
        }
    }
}
