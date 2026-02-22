using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// Handles Feet UI.
    /// Takes input and shows equality result.
    /// </summary>
    public class FeetPresentation
    {
        private readonly FeetService service;

        public FeetPresentation()
        {
            service = new FeetService();
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nFeet Equality");
                Console.Write("Enter first value: ");
                double firstValue =Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter second value: ");
                double secondValue =Convert.ToDouble(Console.ReadLine());
                Feet firstMeasurement =new Feet(firstValue);
                Feet secondMeasurement =new Feet(secondValue);
                bool result =service.AreEqual(firstMeasurement,secondMeasurement);
                Console.WriteLine("Exact Equality: " + result);
                Console.Write("Enter tolerance: ");
                double tolerance =Convert.ToDouble(Console.ReadLine());
                bool toleranceResult =service.AreEqualWithTolerance(firstMeasurement,secondMeasurement,tolerance);
                Console.WriteLine("Tolerance Equality: " + toleranceResult);
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