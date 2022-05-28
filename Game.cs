using System;
using SFML.System;
using SFML.Window;
using System.Threading;
using SFML.Graphics;

namespace AirHockey
{
    public class Game
    {
        private const uint width = 960;
        private const uint height = 540;

        private const int pointCount = 5;

        private RenderWindow window = new RenderWindow(new VideoMode(960, 540), "Game window");

        private CircleShape coin;
        private RectangleShape centerLine;

        private Drawable[] objectsToDraw;

        private bool isCoinSpawned = false;

        private readonly Font font = new Font("Data/OpenSans-Bold.ttf");
        private Text text1;
        private Text text2;

        private Player player1;
        private Player player2;

        private Ball ball;

        private Random rnd = new Random();

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
            centerLine = new RectangleShape(new Vector2f(width / 100, height))
            {
                Position = new Vector2f(width / 2, 0),
                FillColor = Color.White
            };

            coin = new CircleShape(25)
            {
                FillColor = Color.Yellow,
                OutlineColor = Color.Black,
                OutlineThickness = 3,
            };

            Color gray = new Color(58, 58, 58);

            ball = new Ball(gray, new Vector2f(width / 2 - 20, height / 2 - 25));

            Vector2f player1Position = new Vector2f(30 * 2, window.Size.Y / 2 - 30);
            Vector2f player2Position = new Vector2f(window.Size.X - 30 * 4, window.Size.Y / 2 - 30);

            player1 = new Player(Color.Blue, 30, player1Position);
            player2 = new Player(Color.Red, 30, player2Position);

            text1 = new Text(player1.score.ToString(), font)
            {
                Position = new Vector2f(80, 100),
            };

            text2 = new Text(player2.score.ToString(), font)
            {
                Position = new Vector2f(width - 100, 100),
            };

            objectsToDraw = new Drawable[7] { player1, player2, centerLine, ball, coin, text1, text2 };
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
            player1.TryMove(player1.Position.Y + Input(), player1, height);

            player2.TryMove(ball.Position.Y + 50, player2, height);       
        }

        private void MoveBall()
        {
            ball.Move(height, player1, player2);

            if (ball.TryGoal(width) == 1) player1.score += 1;
            else if (ball.TryGoal(width) == 2) player2.score += 1;
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
                position.X = player1.Radius * 2;
            }
            else 
            {
                position.X = width - player1.Radius * 4;
            }           

            isCoinSpawned = true;
            coin.Position = position;
        }

        private void CalculateCoin()
        {
            if (VectorExtensions.isColliding(coin, player1))
            {
                player1.score += 1;
                isCoinSpawned = false;
            }
            else if (VectorExtensions.isColliding(coin, player2))
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
