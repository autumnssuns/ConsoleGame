using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ConsoleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;
            //Console.WriteLine(testInts?[0] == 1);
            Console.WriteLine("Press [Spacebar] to shoot.");
            Console.WriteLine("Press any key to start the game.");
            Console.ReadKey();
            Game game = new Game();
            game.Start();
        }
    }
}
