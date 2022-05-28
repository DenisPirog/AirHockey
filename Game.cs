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

        private CircleShape coin;
        private CircleShape ball;
        private RectangleShape centerLine;

        private Drawable[] objectsToDraw;

        private bool isCoinSpawned = false;

        private Font font = new Font("Data/OpenSans-Bold.ttf");
        private Text text1;
        private Text text2;

        private Player player1;
        private Player player2;

        private const int pointCount = 5;

        private Random rnd = new Random();

        private Vector2f delta = new Vector2f(6, 6);

        public void Start()
        {
            window.Closed += WindowClosed;
            window.SetFramerateLimit(60);

            CreateField();

            while (!isEndGame())
            {
                TryMovePlayers();
                MoveBall();
                TrySpawnCoin();
                if (isCoinSpawned) CalculateCoin();
                DrawField();
            }
        }

        private void CreateField()
        {
            uint lineWidth = width / 100;
            uint lineHeight = height;

            centerLine = new RectangleShape(new Vector2f(lineWidth, lineHeight))
            {
                Position = new Vector2f(width / 2, 0),
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

            text1 = new Text(player1.score.ToString(), font);
            text2 = new Text(player2.score.ToString(), font);

            text1.Position = new Vector2f(80, 100);
            text2.Position = new Vector2f(width - 100, 100);

            objectsToDraw = new Drawable[7] { player1.mesh, player2.mesh, centerLine, ball, coin, text1, text2 };
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
            Thread.Sleep(1000);
        }

        private void TryMovePlayers()
        {
            player1.TryMovePlayer(player1.mesh.Position.Y + Input(), player1, height);

            player2.TryMovePlayer(ball.Position.Y + 50, player2, height);       
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

        private void MoveBall()
        {
            double r = ball.Radius;
            Vector2f position = ball.Position;

            position += delta;

            if (position.Y + r > window.Size.Y - ball.Radius || position.Y - r < 0 - ball.Radius)
            {
                delta.Y = -delta.Y;
            }

            if (position.X + r > window.Size.X - ball.Radius)
            {
                position = Goal(player1);
            }
            else if (position.X - r < 0 - ball.Radius)
            {
                position = Goal(player2);
            }

            ball.Position = position;

            if (isColliding(ball, player1.mesh) || isColliding(ball, player2.mesh))
            {
                delta = -delta;
            }          
        }     

        private bool isColliding(CircleShape object1, CircleShape object2)
        {
            double distance = VectorExtensions.DistanceTo(object1.Position, object2.Position);

            if (distance < object1.Radius + object2.Radius)
            {
                return true;
            }

            return false;
        }    

        private Vector2f Goal(Player player)
        {
            delta = -delta;

            Vector2f position = new Vector2f(width / 2 - 20, height / 2 - 25);

            player.score += 1;

            return position;
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
            Vector2f position = new Vector2f(0, rnd.Next(0, (int)height - (int)coin.Radius * 2));

            int fieldPart = rnd.Next(1, 3);

            if (fieldPart == 1)
            {
                position.X = player1.mesh.Radius * 2;
            }
            else 
            {
                position.X = width - player1.mesh.Radius * 4;
            }           

            isCoinSpawned = true;
            coin.Position = position;
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

            text1.DisplayedString = player1.score.ToString();
            text2.DisplayedString = player2.score.ToString();

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
