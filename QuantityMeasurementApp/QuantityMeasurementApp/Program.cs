using System;
using QuantityMeasurementApp.PresentationLayer;

namespace QuantityMeasurementApp
{
    /// <summary>
    /// Main entry point.
    /// Each menu option maps to exactly one Presentation class.
    /// All Presentation classes depend on service interfaces, so this file
    /// becomes a simple Controller registration list when migrating to ASP.NET.
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            // UC1
            var feetUI           = new FeetPresentation();
            // UC2
            var inchesUI         = new InchesPresentation();
            // UC3
            var lengthUI         = new LengthPresentation();
            // UC4
            var extendedUI       = new LengthPresentationUC4();
            // UC5
            var conversionUI     = new LengthPresentationUC5();
            // UC6
            var additionUI       = new LengthPresentationUC6();
            // UC7
            var targetAdditionUI = new LengthPresentationUC7();
            // UC9
            var weightUI         = new WeightPresentationUC9();
            // UC10
            var quantityUI       = new QuantityPresentation();
            // UC11
            var volumeUI         = new VolumePresentation();

            while (true)
            {
                Console.WriteLine("\nQuantity Measurement Application");
                Console.WriteLine("1.  Feet Equality");
                Console.WriteLine("2.  Feet and Inches Equality");
                Console.WriteLine("3.  Generic Length Equality");
                Console.WriteLine("4.  Extended Units Equality");
                Console.WriteLine("5.  Unit-to-Unit Conversion");
                Console.WriteLine("6.  Addition of Lengths");
                Console.WriteLine("7.  Addition with Target Unit");
                Console.WriteLine("8.  Weight Measurement");
                Console.WriteLine("9.  Generic Quantity Measurement");
                Console.WriteLine("10. Volume Measurement");
                Console.WriteLine("0.  Exit");
                Console.Write("\nEnter choice: ");

                string? input = Console.ReadLine();
                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:  feetUI.Run();           break;
                    case 2:  inchesUI.Run();         break;
                    case 3:  lengthUI.Run();         break;
                    case 4:  extendedUI.Run();       break;
                    case 5:  conversionUI.Run();     break;
                    case 6:  additionUI.Run();       break;
                    case 7:  targetAdditionUI.Run(); break;
                    case 8:  weightUI.Run();         break;
                    case 9:  quantityUI.Run();       break;
                    case 10: volumeUI.Run();         break;
                    case 0:
                        Console.WriteLine("Thank You");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 0–10.");
                        break;
                }
            }
        }
    }
}