using MonoGame.Training.Repositories;
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

            while (Run()) { }
        }

        private static bool Run()
        {
            using (var game = new Game1())
            {
                game.Run();

                return game.RequireRestart;
            }
        }
    }
}
