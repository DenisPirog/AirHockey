using SFML.System;
using SFML.Graphics;

namespace AirHockey
{
    public class Ball : CircleShape
    {
        private Vector2f startPosition;

        private Vector2f delta = new Vector2f(6, 6);

        public Ball(Color color, Vector2f position)
        {
            FillColor = color;
            OutlineColor = Color.Black;
            OutlineThickness = 3;
            Position = position;
            this.startPosition = position;
        }

        public void Move(uint height, Player player1, Player player2)
        {
            Vector2f position = Position;

            position += delta;

            bool isHitTheBorder = position.Y + Radius > height - Radius || position.Y - Radius < 0 - Radius;

            if (isHitTheBorder)
            {
                delta.Y = -delta.Y;
            }

            CheckIfCollidingWith(player1, player2);

            Position = position;         
        }

        private void CheckIfCollidingWith(Player player1, Player player2)
        {
            if (VectorExtensions.isColliding(this, player1) || VectorExtensions.isColliding(this, player2))
            {
                delta = -delta;
            }
        }

        public int TryGoal(uint width)
        {
            bool isHitThe1Goal = Position.X + Radius > width - Radius;
            bool isHitThe2Goal = Position.X - Radius < Radius;

            if (isHitThe1Goal)
            {
                delta = -delta;
                Position = startPosition;
                return 1;
            }
            else if (isHitThe2Goal)
            {
                delta = -delta;
                Position = startPosition;
                return 2;
            }

            return 0;
        }
    }
}
