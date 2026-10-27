using System;
using SimMarket;

namespace SimMarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nPresiona ENTER para empezar...");
            Console.ReadLine();

            Simulation sim = new Simulation();
            sim.Run();

            Console.WriteLine("\nPresiona ENTER para salir...");
            Console.ReadLine();
        }
    }
}