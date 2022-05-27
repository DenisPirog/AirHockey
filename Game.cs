using System;
using SFML.System;
using SFML.Window;
using System.Threading;
using SFML.Graphics;

namespace AirHockey
{
    internal class Game
    {
        private int width = 960;
        private int height = 540;

        private RenderWindow window = new RenderWindow(new VideoMode(960, 540), "Game window");

        private CircleShape ball;
        private RectangleShape centerLine;

        private Player player1;
        private Player player2;

        private int pointCount = 5;

        private double dx = 6;
        private double dy = 6;

        public void Start()
        {
            window.Closed += WindowClosed;

            window.SetFramerateLimit(60);

            CreateField();

            while (!isEndGame())
            {
                ClearField();
                CalculatePlayers();
                CalculateBall();
                DrawField();
            }

            window.Clear(Color.Cyan);
            window.Display();
        }

        private bool isEndGame()
        {
            if (player1.score == pointCount)
            {
                GameResult(Color.Blue);
                return true;
            }
            else if (player2.score == pointCount)
            {
                GameResult(Color.Red);
                return true;
            }

            return false;
        }

        private void GameResult(Color color)
        {
            window.Clear(color);
        }

        private void CreateField()
        {
            uint lineWidth = window.Size.X / 100;
            uint lineHeight = window.Size.Y;

            uint lineX = window.Size.X / 2;
            uint lineY = 0;

            centerLine = new RectangleShape(new Vector2f(lineWidth, lineHeight))
            {
                Position = new Vector2f(lineX, lineY),
                FillColor = Color.White
            };

            Color gray = new Color(58, 58, 58);

            ball = new CircleShape(25)
            {
                FillColor = gray,
                OutlineColor = Color.Black,
                OutlineThickness = 3,
                Position = new Vector2f(window.Size.X / 2 - 20, window.Size.Y / 2 - 25),
            };

            player1 = new Player(Color.Blue, 30, window, 1);
            player2 = new Player(Color.Red, 30, window, 2);
        }

        private void ClearField()
        {
            window.DispatchEvents();
            Color gray = new Color(107, 107, 107);
            window.Clear(gray);
        }

        private void CalculatePlayers()
        {
            float newPlayerX = player1.mesh.Position.Y + Input();

            if (newPlayerX <= height - player1.mesh.Radius * 2 && newPlayerX >= 0)
            {
                player1.mesh.Position = new Vector2f(player1.mesh.Position.X, newPlayerX);
            }
            
            player2.mesh.Position = new Vector2f(player2.mesh.Position.X, ball.Position.Y);
        }

        private int Input()
        {
            int speed = 9;

            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                return -speed;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                return speed;
            }

            return 0;
        }

        private void CalculateBall()
        {
            double r = ball.Radius;
            double x = ball.Position.X;
            double y = ball.Position.Y;

            x += dx;
            y += dy;

            if (y + r > window.Size.Y - ball.Radius || y - r < 0 - ball.Radius)
            {
                dy = -dy;
            }

            double distance1 = CalculateDistance(x, y, player1);
            double distance2 = CalculateDistance(x, y, player2);

            if (distance1 < r + player1.mesh.Radius || distance2 < r + player2.mesh.Radius)
            {
                dx = -dx;
                dy = -dy;
            }

            if (x + r > window.Size.X - ball.Radius || x - r < 0 - ball.Radius)
            {
                dx = -dx;
            }

            ball.Position = new Vector2f((float)x, (float)y);           
        }

        private void RespawnBall()
        {
            ball.Position = new Vector2f(window.Size.X / 2 - 20, window.Size.Y / 2 - 25);
        }

        private double CalculateDistance(double x, double y, Player player)
        {
            double Dx = x - player.mesh.Position.X;
            double Dy = y - player.mesh.Position.Y;

            double distance = Math.Sqrt(Dx * Dx + Dy * Dy);

            if (distance == 0)
            {
                distance = 0.01f;
            }

            return distance;
        }

        private void DrawField()
        {
            window.Draw(player1.mesh);
            window.Draw(player2.mesh);
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
