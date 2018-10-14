using SnakeGameUsingNeuralNetwork;

namespace SnakeGameUsingNeuralNetwork
{
    public class Apple
    {
        public enum eSnakeShapes { Apple = '@'};
        static System.Random rand = new System.Random();

        Point m_Apple = new Point(0, 0, (char)eSnakeShapes.Apple);
        public int X { get { return m_Apple.X; } }
        public int Y { get { return m_Apple.Y; } }

        public Apple() { }



    public void GenerateNewRandomApple(int i_Width,int i_Height)
        {
            m_Apple.X = (rand.Next(i_Width));
            m_Apple.Y = (rand.Next(i_Height));

            // Apple color is red.
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            m_Apple.Draw();
            System.Console.ForegroundColor = System.ConsoleColor.White;
        }
    }


}
