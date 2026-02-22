using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// Presentation Layer.
    /// Handles user input and output.
    /// Shows menu and handles exceptions.
    /// </summary>
    public class FeetPresentation
    {
        private FeetService service;
        public void Run()
        {
          service = new FeetService();

            while (true)
            {
                Console.WriteLine("\nQuantity Measurement Application");
                Console.WriteLine("1. UC1: Feet Equality (Includes Tolerance Feature)");
                Console.WriteLine("0. Exit");
                Console.Write("\nEnter choice: ");

                try
                {
                    int choice = Convert.ToInt32(Console.ReadLine());

                    switch (choice)
                    {
                        case 1:

                            Console.Write("Enter first value: ");
                            double firstValue = Convert.ToDouble(Console.ReadLine());
                            Feet firstMeasurement = new Feet(firstValue);
                            Console.Write("Enter second value: ");
                            double secondValue = Convert.ToDouble(Console.ReadLine());
                            Feet secondMeasurement = new Feet(secondValue);
                            bool result = service.AreEqual(firstMeasurement, secondMeasurement);
                            Console.WriteLine("Exact Equality: " + result);
                            Console.Write("Enter tolerance: ");
                            double tolerance = Convert.ToDouble(Console.ReadLine());
                            bool toleranceResult =service.AreEqualWithTolerance(firstMeasurement, secondMeasurement, tolerance);
                            Console.WriteLine("Tolerance Equality: " + toleranceResult);
                            break;
                        case 0:
                            Console.WriteLine("Thank You");
                            return;
                        default:
                            Console.WriteLine("Invalid choice");
                            break;
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("Validation Error: " + ex.Message);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter numeric value.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
}