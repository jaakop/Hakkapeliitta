using System;

namespace ReeGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// Please do not touch c:
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}
