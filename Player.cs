using SFML.System;
using SFML.Graphics;

namespace AirHockey
{
    public class Player
    {
        public CircleShape mesh = new CircleShape();
        public int score = 0;

        public Player(Color meshColor, uint radius, RenderWindow window, int position)
        {
            mesh.FillColor = meshColor;
            mesh.Radius = radius;
            mesh.OutlineColor = Color.Black;
            mesh.OutlineThickness = 2;

            uint player1X = radius * 2;
            uint player2X = window.Size.X - radius * 4;
            uint playerY = window.Size.Y / 2 - radius;

            if(position == 1) mesh.Position = new Vector2f(player1X, playerY);
            if(position == 2) mesh.Position = new Vector2f(player2X, playerY);
        }

        public void TryMovePlayer(float newPlayerX, Player player, uint height)
        {
            if (newPlayerX <= height - player.mesh.Radius * 2 && newPlayerX >= 0)
            {
                player.mesh.Position = new Vector2f(player.mesh.Position.X, newPlayerX);
            }
        }
    }
}
