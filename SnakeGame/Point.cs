using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGameUsingNeuralNetwork
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public char Char {get; set;}

        public Point(int _x, int _y,char _pChar)
        {
            X = _x;
            Y = _y;
            Char = _pChar;
        }

        public Point(Point point, char v)
        {
            X = point.X;
            Y = point.Y;
            Char = v;
        }

        public void Draw()
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(Char);
        }

        public void MovePoint(int dirX,int dirY)
        {
            // If we cross the board boundaries so we continue the game.
            if (X <= 0)
            {
                X = (int)SnakeGame.eBoardSize.Width;
            }

            if (Y <=0)
            {
                Y = (int)SnakeGame.eBoardSize.Height;
            }

            // Move at DirX,DirY.
            X = (X + dirX) % (int)(SnakeGame.eBoardSize.Width);
            Y = (Y + dirY) % (int)(SnakeGame.eBoardSize.Height);

        }
        public bool Equal(Point p)
        {
            return (X == p.X && Y == p.Y);
        }

    }
}
