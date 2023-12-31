using System;

namespace MonoGame.Training
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            // TODO : Load config
            // TODO : Add Logger
            using (var game = new Game1())
            {
                game.Run();
            }
        }
    }
}
