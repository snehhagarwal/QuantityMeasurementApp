using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC1: Presentation layer for Feet equality.
    /// Depends on IFeetService — not the concrete class — so it is
    /// ready for ASP.NET Controller migration without any logic changes.
    /// </summary>
    public class FeetPresentation
    {
        private readonly IFeetService _service;

        public FeetPresentation()
        {
            _service = new FeetService();
        }

        /// <summary>ASP.NET-ready constructor — accepts service via dependency injection.</summary>
        public FeetPresentation(IFeetService service)
        {
            _service = service;
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC1: Feet Equality");

                Console.Write("Enter first value: ");
                double firstValue = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter second value: ");
                double secondValue = Convert.ToDouble(Console.ReadLine());

                Feet firstMeasurement  = new Feet(firstValue);
                Feet secondMeasurement = new Feet(secondValue);

                bool result = _service.AreEqual(firstMeasurement, secondMeasurement);
                Console.WriteLine("Exact Equality: " + result);

                Console.Write("Enter tolerance: ");
                double tolerance = Convert.ToDouble(Console.ReadLine());

                bool toleranceResult = _service.AreEqualWithTolerance(firstMeasurement, secondMeasurement, tolerance);
                Console.WriteLine("Tolerance Equality: " + toleranceResult);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Validation Error: " + ex.Message);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter a numeric value.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}