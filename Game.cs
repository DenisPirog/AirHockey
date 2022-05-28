using System;
using SFML.System;
using SFML.Window;
using System.Threading;
using SFML.Graphics;

namespace AirHockey
{
    internal class Game
    {
        private const uint width = 960;
        private const uint height = 540;

        private RenderWindow window = new RenderWindow(new VideoMode(960, 540), "Game window");

        private CircleShape ball;
        private RectangleShape centerLine;
        private CircleShape coin;
        private bool isCoinSpawned = false;
        private Font font = new Font("Data/OpenSans-Bold.ttf");

        private Player player1;
        private Player player2;

        private Drawable[] objectsToDraw;

        private const int pointCount = 5;
        private Random rnd = new Random();

        private double dx = 6;
        private double dy = 6;

        public void Start()
        {
            window.Closed += WindowClosed;
            window.SetFramerateLimit(60);

            CreateField();

            while (!isEndGame())
            {
                CalculatePlayers();
                CalculateBall();
                TrySpawnCoin();
                if (isCoinSpawned) CalculateCoin();
                DrawField();
            }
        }

        private void CreateField()
        {
            uint lineWidth = width / 100;
            uint lineHeight = height;

            uint lineX = width / 2;
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
                Position = new Vector2f(width / 2 - 20, height / 2 - 25),
            };

            coin = new CircleShape(25)
            {
                FillColor = Color.Yellow,
                OutlineColor = Color.Black,
                OutlineThickness = 3,
            };

            player1 = new Player(Color.Blue, 30, window, 1);
            player2 = new Player(Color.Red, 30, window, 2);

            objectsToDraw = new Drawable[5] { player1.mesh, player2.mesh, centerLine, ball, coin };
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
            window.Display();
        }

        private void CalculatePlayers()
        {
            TryMovePlayer(player1.mesh.Position.Y + Input(), player1);

            TryMovePlayer(ball.Position.Y + 50, player2);       
        }

        private void TryMovePlayer(float newPlayerX, Player player)
        {
            if (newPlayerX <= height - player.mesh.Radius * 2 && newPlayerX >= 0)
            {
                player.mesh.Position = new Vector2f(player.mesh.Position.X, newPlayerX);
            }
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

            dx += 0.02;
            dy += 0.02;

            x += dx;
            y += dy;


            if (y + r > window.Size.Y - ball.Radius || y - r < 0 - ball.Radius)
            {
                dy = -dy;
            }

            if (x + r > window.Size.X - ball.Radius)
            {
                (x, y) = Goal(player1);
            }
            else if (x - r < 0 - ball.Radius)
            {
                (x, y) = Goal(player2);
            }

            ball.Position = new Vector2f((float)x, (float)y);

            if (isColliding(ball, player1.mesh) || isColliding(ball, player2.mesh))
            {
                dx = -dx;
                dy = -dy;
            }          
        }     

        private bool isColliding(CircleShape object1, CircleShape object2)
        {
            double distance = CalculateDistance(object1, object2);

            if (distance < object1.Radius + object2.Radius)
            {
                return true;
            }

            return false;
        }    

        private double CalculateDistance(CircleShape object1, CircleShape object2)
        {
            double Dx = object1.Position.X - object2.Position.X;
            double Dy = object1.Position.Y - object2.Position.Y;

            double distance = Math.Sqrt(Dx * Dx + Dy * Dy);

            return distance;
        }

        private (double, double) Goal(Player player)
        {
            dx = -dx;
            dy = -dy;

            double x = width / 2 - 20;
            double y = height / 2 - 25;

            player.score += 1;

            return (x, y);
        }

        private void TrySpawnCoin()
        {
            if (rnd.Next(0, 700) == 1)
            {
                SpawnCoin();
            }       
        }

        private void SpawnCoin()
        {
            float randomX;
            float randomY = rnd.Next(0, (int)height - (int)coin.Radius * 2);

            int fieldPart = rnd.Next(1, 3);

            if (fieldPart == 1)
            {
                randomX = player1.mesh.Radius * 2;
            }
            else 
            {
                randomX = width - player1.mesh.Radius * 4;
            }           

            isCoinSpawned = true;
            coin.Position = new Vector2f(randomX, randomY);
        }

        private void CalculateCoin()
        {
            if (isColliding(coin, player1.mesh))
            {
                player1.score += 1;
                isCoinSpawned = false;
            }
            else if (isColliding(coin, player2.mesh))
            {
                player2.score += 1;
                isCoinSpawned = false;
            }
        }

        private void DrawField()
        {
            window.DispatchEvents();

            Color gray = new Color(107, 107, 107);
            window.Clear(gray);

            

            Text text1 = new Text(player1.score.ToString(), font);
            Text text2 = new Text(player2.score.ToString(), font);

            text1.Position = new Vector2f(80, 100);
            text2.Position = new Vector2f(width - 100, 100);

            window.Draw(text1);
            window.Draw(text2);

            foreach (Drawable shape in objectsToDraw)
            {
                if(shape == coin && isCoinSpawned == true)
                {
                    window.Draw(shape);
                }
                else if (shape != coin)
                {
                    window.Draw(shape);
                }              
            }

            window.Display();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            RenderWindow w = (RenderWindow)sender;
            w.Close();
        }
    }
}
