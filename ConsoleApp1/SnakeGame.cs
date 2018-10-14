using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGameUsingNeuralNetwork
{
    public class SnakeGame
    {
        readonly double[] Right = { 1, -1, -1, -1 };
        readonly double[] Left = { -1, 1, -1, -1 };
        readonly double[] Down = { -1, -1, 1, -1 };
        readonly double[] Up = { -1, -1, -1, 1 };

        readonly double[] LeftUp = { -0.5, 0.5, -0.5, 0.5 };
        readonly double[] RightUp = { 0.5, -0.5, -0.5, 0.5 };
        readonly double[] LeftDown = { -0.5, 0.5, 0.5, -0.5 };
        readonly double[] RightDown = { 0.5, -0.5, 0.5, -0.5 };



        public enum eBoardSize { Width = 40, Height = 20 };


        Snake m_Snake = new Snake();
        Apple m_Apple = new Apple();

        private int m_GameSpeed;
        private bool m_GameOver;
        private int m_GameScore;

        public SnakeGame()
        {
            m_GameOver = false;
            m_GameScore = 0;
            m_GameSpeed = 20;

            InitBoard();
            m_Apple.GenerateNewRandomApple((int)eBoardSize.Width, (int)eBoardSize.Height);
            UpdateScore();
            //StartNormalGame();
            StartNeuralNetworkGame();
        }

        private void InitBoard()
        {
            for (int i = 0; i < (int)eBoardSize.Height; i++)
            {
                Console.SetCursorPosition((int)eBoardSize.Width, i);
                Console.Write("│");
            }

            for (int i = 0; i < (int)eBoardSize.Width; i++)
            {
                Console.SetCursorPosition(i, (int)eBoardSize.Height);
                Console.Write("─");
            }
        }

        private void StartNeuralNetworkGame()
        {
            // Inputs: {Snake Head Location (x,y), Apple Location (x,y)}
            // Output: Array of Directions {DownValue,UpValue,RightValue,LeftValue}

            NeuralNetwork snakeNN = new NeuralNetwork(4, 4, 4);
            while (!m_GameOver)
            {
                Thread.Sleep(m_GameSpeed);
                m_Snake.Move();
                ShowXY();

                if (m_Snake.isEatingApple(m_Apple))
                {
                    m_Snake.Grow();
                    m_Apple.GenerateNewRandomApple((int)eBoardSize.Width, (int)eBoardSize.Height);
                    UpdateScore();
                }

                double[] InputArr = { m_Snake.Head.X, m_Snake.Head.Y, m_Apple.X, m_Apple.Y };
                double[] NNoutput = snakeNN.FeedForward(InputArr);
                double[] PcGuess = GetPcPMove(m_Snake.Head.X, m_Snake.Head.Y, m_Apple.X, m_Apple.Y);

                int NNGuessDirection = NNoutput.ToList().IndexOf(NNoutput.Max());

                if (PcGuess[NNGuessDirection] != 1 && PcGuess[NNGuessDirection] != 0.5)
                {
                    snakeNN.TrainNetwork(InputArr, PcGuess, 0.3);
                    NNoutput = snakeNN.FeedForward(InputArr);
                    PcGuess = GetPcPMove(m_Snake.Head.X, m_Snake.Head.Y, m_Apple.X, m_Apple.Y);
                    NNGuessDirection = NNoutput.ToList().IndexOf(NNoutput.Max());
                }

                if (NNGuessDirection == 0)
                {
                    m_Snake.Dir = Snake.eSnakeDirections.Right;
                }
                else if (NNGuessDirection == 1)
                {
                    m_Snake.Dir = Snake.eSnakeDirections.Left;
                }
                else if (NNGuessDirection == 2)
                {
                    m_Snake.Dir = Snake.eSnakeDirections.Down;
                }
                else if (NNGuessDirection == 3)
                {
                    m_Snake.Dir = Snake.eSnakeDirections.Up;
                }

                Console.SetCursorPosition(0, 21);

                Console.WriteLine(string.Format("Right  : {0:P}", NNoutput[0]));
                Console.WriteLine(string.Format("Left   : {0:P}", NNoutput[1]));
                Console.WriteLine(string.Format("Down   : {0:P}", NNoutput[2]));
                Console.WriteLine(string.Format("Up     : {0:P}", NNoutput[3]));

            }
        }

        private void TrainMySnake(NeuralNetwork snakeNN)
        {
            Random rand = new Random();

            for (int i = 0; i < 5000000; i++)
            {
                double[] InputArr = { rand.Next((int)eBoardSize.Width), rand.Next((int)eBoardSize.Height), rand.Next((int)eBoardSize.Width), rand.Next((int)eBoardSize.Height) };
                double[] NNoutput = snakeNN.FeedForward(InputArr);
                double[] PcGuess = GetPcPMove(m_Snake.Head.X, m_Snake.Head.Y, m_Apple.X, m_Apple.Y);

                int NNGuessDirection = NNoutput.ToList().IndexOf(NNoutput.Max());

                while (PcGuess[NNGuessDirection] == 0)
                {
                    Console.SetCursorPosition(0, 21);
                    Console.WriteLine("[{0}]", string.Join(", ", NNoutput));
                    Console.WriteLine("\t Right \t\t Left \t\t\t Down \t\t Up");
                    snakeNN.TrainNetwork(InputArr, PcGuess, 0.1);
                    NNoutput = snakeNN.FeedForward(InputArr);
                    PcGuess = GetPcPMove(m_Snake.Head.X, m_Snake.Head.Y, m_Apple.X, m_Apple.Y);
                }
            }
        }
        private void StartNormalGame()
        {
            while (!m_GameOver)
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(m_GameSpeed);
                    m_Snake.Move();

                    if (m_Snake.isEatingApple(m_Apple))
                    {
                        m_Snake.Grow();
                        m_Apple.GenerateNewRandomApple((int)eBoardSize.Width, (int)eBoardSize.Height);
                        UpdateScore();
                    }
                }

                KeyPressed(Console.ReadKey(true).Key);
            }
        }

        private double[] GetPcPMove(double HeadX, double HeadY, double AppleX, double AppleY)
        {
            if (HeadX == AppleX)
            {
                if (HeadY > AppleY)
                {
                    return Up;
                }
                else
                {
                    return Down;
                }
            }

            else if (HeadY == AppleY)
            {
                if (HeadX > AppleX)
                {
                    return Left;
                }
                else
                {
                    return Right;
                }
            }

            else
            {
                if (HeadX > AppleX)
                {
                    if (HeadY > AppleY)
                    {
                        return LeftUp;
                    }
                    else
                    {
                        return LeftDown;
                    }
                }
                else  //HeadX < AppleX
                {
                    if (HeadY > AppleY)
                    {
                        return RightUp;
                    }
                    else
                    {
                        return RightDown;
                    }
                }

            }
        }

        private void ShowXY()
        {
            Console.SetCursorPosition((int)eBoardSize.Width + 3, 1);
            Console.Write("                 ");

            Console.SetCursorPosition((int)eBoardSize.Width + 3, 2);
            Console.Write("                 ");

            Console.SetCursorPosition((int)eBoardSize.Width + 3, 3);
            Console.Write("                 ");

            Console.SetCursorPosition((int)eBoardSize.Width + 3, 4);
            Console.Write("                 ");

            Console.SetCursorPosition((int)eBoardSize.Width + 3, 1);
            Console.Write("HeadX:" + m_Snake.Head.X);

            Console.SetCursorPosition((int)eBoardSize.Width + 3, 2);
            Console.Write("HeadY:" + m_Snake.Head.Y);

            Console.SetCursorPosition((int)eBoardSize.Width + 3, 3);
            Console.Write("AppleX:" + m_Apple.X);

            Console.SetCursorPosition((int)eBoardSize.Width + 3, 4);
            Console.Write("AppleY:" + m_Apple.Y);
        }

        private void UpdateScore()
        {
            Console.SetCursorPosition((int)eBoardSize.Width + 1, 0);
            Console.Write("Game Score: " + m_GameScore);
            m_GameScore++;
        }

        private void KeyPressed(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.RightArrow:
                    {
                        m_Snake.Dir = Snake.eSnakeDirections.Right;
                        break;
                    }
                case ConsoleKey.LeftArrow:
                    {
                        m_Snake.Dir = Snake.eSnakeDirections.Left;
                        break;
                    }
                case ConsoleKey.DownArrow:
                    {
                        m_Snake.Dir = Snake.eSnakeDirections.Down;
                        break;
                    }

                case ConsoleKey.UpArrow:
                    {
                        m_Snake.Dir = Snake.eSnakeDirections.Up;
                        break;
                    }
                case ConsoleKey.Escape:
                    {
                        m_GameOver = true;
                        break;
                    }
            }
        }


    }
}

