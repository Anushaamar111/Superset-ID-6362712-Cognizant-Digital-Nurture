using System;
using CalcLibrary;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Simple Calculator Demo ===");
            Console.WriteLine();

            // Create an instance of the calculator
            SimpleCalculator calc = new SimpleCalculator();

            try
            {
                // Test Addition
                double result1 = calc.Addition(10, 5);
                Console.WriteLine($"Addition: 10 + 5 = {result1}");
                Console.WriteLine($"Current Result: {calc.GetResult}");
                Console.WriteLine();

                // Test Subtraction
                double result2 = calc.Subtraction(20, 8);
                Console.WriteLine($"Subtraction: 20 - 8 = {result2}");
                Console.WriteLine($"Current Result: {calc.GetResult}");
                Console.WriteLine();

                // Test Multiplication
                double result3 = calc.Multiplication(6, 7);
                Console.WriteLine($"Multiplication: 6 * 7 = {result3}");
                Console.WriteLine($"Current Result: {calc.GetResult}");
                Console.WriteLine();

                // Test Division
                double result4 = calc.Division(15, 3);
                Console.WriteLine($"Division: 15 / 3 = {result4}");
                Console.WriteLine($"Current Result: {calc.GetResult}");
                Console.WriteLine();

                // Test AllClear
                calc.AllClear();
                Console.WriteLine("Called AllClear()");
                Console.WriteLine($"Current Result after AllClear: {calc.GetResult}");
                Console.WriteLine();

                // Test Division by Zero (should throw exception)
                Console.WriteLine("Testing division by zero...");
                double result5 = calc.Division(10, 0);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Caught expected exception: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("=== Demo Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
