using System;
using QuantityMeasurementApp.PresentationLayer;

namespace QuantityMeasurementApp
{
    /// <summary>
    /// Main Entry Point.
    /// Handles application menu.
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            FeetPresentation feetUI =new FeetPresentation();
            InchesPresentation inchesUI =new InchesPresentation();
            while (true)
            {
                Console.WriteLine("\nQuantity Measurement Application");
                Console.WriteLine("1. Feet Equality");
                Console.WriteLine("2. Feet and Inches Equality");
                Console.WriteLine("0. Exit");
                Console.Write("\nEnter choice: ");
                int choice =Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        feetUI.Run();
                        break;
                    case 2:
                        inchesUI.Run();
                        break;
                    case 0:
                        Console.WriteLine("Thank You");
                        return;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
        }
    }
}