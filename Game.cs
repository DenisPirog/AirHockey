using System;
using SFML;
using SFML.Window;
using SFML.Graphics;

namespace AirHockey
{
    internal class Game
    {
        private RenderWindow window = new RenderWindow(new VideoMode(960, 540), "Game window");

        private CircleShape ball;

        private CircleShape player1;
        private CircleShape player2;

        private RectangleShape centerLine;

        private double dx = 0.3;
        private double dy = 0.3;

        public void Start()
        {
            window.Closed += WindowClosed;

            CreateField();
            CreatePlayers();
            CreateBall();

            while (window.IsOpen)
            {
                UpdateField();
            }
        }

        private void CreateField()
        {
            uint lineWidth = window.Size.X / 100;
            uint lineHeight = window.Size.Y;

            uint lineX = window.Size.X / 2;
            uint lineY = 0;

            centerLine = new RectangleShape(new SFML.System.Vector2f(lineWidth, lineHeight))
            {
                Position = new SFML.System.Vector2f(lineX, lineY),
                FillColor = Color.White
            };
        }

        private void CreatePlayers()
        {
            uint playerRadius = 30;

            uint player1X = playerRadius * 2;
            uint playerY = window.Size.Y / 2 - playerRadius;

            player1 = new CircleShape(playerRadius)
            {
                FillColor = Color.Blue,
                OutlineColor = Color.Black,
                OutlineThickness = 2,
                Position = new SFML.System.Vector2f(player1X, playerY)
            };
           
            uint player2X = window.Size.X - playerRadius * 4;

            player2 = new CircleShape(playerRadius)
            {
                FillColor = Color.Red,
                OutlineColor = Color.Black,
                OutlineThickness = 2,
                Position = new SFML.System.Vector2f(player2X, playerY),
            };           
        }

        private void CreateBall()
        {
            Color gray = new Color(58, 58, 58);

            ball = new CircleShape(25)
            {
                FillColor = gray,
                OutlineColor = Color.Black,
                OutlineThickness = 3,
                Position = new SFML.System.Vector2f(window.Size.X / 2 - 20, window.Size.Y / 2 - 25),
            };
        }

        private void UpdateField()
        {
            ClearField();
            CalculatePlayers();
            DrawField();
            CalculateBall();
        }

        private void ClearField()
        {
            window.DispatchEvents();
            Color gray = new Color(107, 107, 107);
            window.Clear(gray);
        }

        private void CalculatePlayers()
        {
            SFML.System.Vector2i mousePosition = Mouse.GetPosition(window);

            if (mousePosition.Y < window.Size.Y - player1.Radius * 2 && mousePosition.Y >= 0)
            {
                player1.Position = new SFML.System.Vector2f(player1.Position.X, mousePosition.Y);
            }

            player2.Position = new SFML.System.Vector2f(player2.Position.X, ball.Position.Y + 10);
        }

        private void CalculateBall()
        {
            double x = ball.Position.X;
            double y = ball.Position.Y;
            double r = ball.Radius;

            x += dx;
            y += dy;

            if (y + r > window.Size.Y || (y - r < 0))
            {
                dy = -dy;
            }

            double Dx = x - player1.Position.X;
            double Dy = y - player1.Position.Y;
            double d = Math.Sqrt(Dx * Dx + Dy * Dy);
            if (d == 0) d = 0.01f;

            double Dx1 = x - player2.Position.X;
            double Dy1 = y - player2.Position.Y;
            double d1 = Math.Sqrt(Dx1 * Dx1 + Dy1 * Dy1);
            if (d1 == 0) d1 = 0.01f;

            if (d < r + player1.Radius || d1 < r + player2.Radius)
            {
                dx = -dx;
                dy = -dy;
            }

            if (x + r > window.Size.X || (x - r < 0))
            {
                x = window.Size.X / 2 - 20;
                y = window.Size.Y / 2 - 25;
            }

            ball.Position = new SFML.System.Vector2f((float)x, (float)y);           
        }

        private void DrawField()
        {
            window.Draw(player1);
            window.Draw(player2);
            window.Draw(centerLine);
            window.Draw(ball);
            window.Display();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            RenderWindow w = (RenderWindow)sender;
            w.Close();
        }
    }
}
