using System;
using System.Collections.Generic;

namespace SnakeGameUsingNeuralNetwork
{
    public class Snake
    {
        public enum eSnakeShapes { SnakeBody = 'O', SnakeHead = '#' };
        public enum eSnakeDirections { Right = ConsoleKey.RightArrow, Left = ConsoleKey.LeftArrow, Down = ConsoleKey.DownArrow, Up = ConsoleKey.UpArrow };
        private List<Point> m_Snake = new List<Point>(1);
        private int DirX;
        private int DirY;
        public int SnakeSize
        {
            get
            {
                return m_Snake.Count;
            }
        }
        public Point Head
        {
            get
            {
                return m_Snake[SnakeSize - 1];
            }
        }
        public eSnakeDirections Dir
        {
            set
            {
                switch (value)
                {
                    case eSnakeDirections.Down:
                        {
                            DirX = 0;
                            DirY = 1;
                            break;
                        }
                    case eSnakeDirections.Up:
                        {
                            DirX = 0;
                            DirY = -1;
                            break;
                        }
                    case eSnakeDirections.Right:
                        {
                            DirX = 1;
                            DirY = 0;
                            break;
                        }
                    case eSnakeDirections.Left:
                        {
                            DirX = -1;
                            DirY = 0;
                            break;
                        }
                }
            }
            get
            {
                if (DirX == 0)
                {
                    if (DirY == 1)
                    {
                        return eSnakeDirections.Down;
                    }
                    else
                    {
                        return eSnakeDirections.Up;
                    }
                }
                else
                {
                    if (DirX == 1)
                    {
                        return eSnakeDirections.Right;
                    }
                    else
                    {
                        return eSnakeDirections.Left;
                    }
                }
            }
        }


        public Snake()
        {
            m_Snake.Insert(0, new Point(10, 10, (char)eSnakeShapes.SnakeHead));
            DirX = 1;
            DirY = 0;
        }

        public void Grow()
        {
            Point newTail = new Point(m_Snake[0], (char)eSnakeShapes.SnakeBody);
            m_Snake.Insert(0, newTail);
            DrawSnake();
        }

        private void DrawSnake()
        {
            foreach (Point point in m_Snake)
            {
                point.Draw();
            }
        }

        public void Move()
        {
            Console.SetCursorPosition(m_Snake[0].X, m_Snake[0].Y);
            Console.Write(' ');

            for (int i = 0; i < SnakeSize - 1; i++)
            {
                m_Snake[i].X = m_Snake[i + 1].X;
                m_Snake[i].Y = m_Snake[i + 1].Y;
            }

            Head.MovePoint(DirX, DirY);
            DrawSnake();
        }

        public bool isEatingApple(Apple i_MyApple)
        {
            return (Head.X == i_MyApple.X) && (Head.Y == i_MyApple.Y);
        }

        public static int DistanceFromApple(Point i_SnakeHead, Apple i_Apple)
        {
            int x_Distance = Math.Abs(i_SnakeHead.X - i_Apple.X);
            int y_Distance = Math.Abs(i_SnakeHead.Y - i_Apple.Y);

            return x_Distance + y_Distance;


        }
    }
}
