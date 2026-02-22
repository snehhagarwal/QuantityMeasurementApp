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
            FeetPresentation feetUI = new FeetPresentation();
            InchesPresentation inchesUI = new InchesPresentation();
            LengthPresentation lengthUI = new LengthPresentation();
            LengthPresentationUC4 uc4UI =new LengthPresentationUC4();
            while (true)
            {
                Console.WriteLine("\nQuantity Measurement Application");
                Console.WriteLine("1. Feet Equality");
                Console.WriteLine("2. Feet and Inches Equality");
                Console.WriteLine("3. Generic Length Equality");
                Console.WriteLine("4. Extended Units Equality");
                Console.WriteLine("0. Exit");
                Console.Write("\nEnter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        feetUI.Run();
                        break;
                    case 2:
                        inchesUI.Run();
                        break;
                    case 3:
                        lengthUI.Run();
                        break;
                    case 4:
                        uc4UI.Run();
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