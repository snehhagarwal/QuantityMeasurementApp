using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC3: Presentation layer for generic Length equality (FEET and INCHES).
    /// Depends on ILengthService — ready for ASP.NET Controller migration.
    /// </summary>
    public class LengthPresentation
    {
        private readonly ILengthService _service;

        public LengthPresentation()
        {
            _service = new LengthService();
        }

        /// <summary>ASP.NET-ready constructor — accepts service via dependency injection.</summary>
        public LengthPresentation(ILengthService service)
        {
            _service = service;
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC3: Generic Length Equality");

                Console.Write("Enter first value: ");
                double value1 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter first unit (FEET/INCHES): ");
                LengthUnit unit1 = ParseUnit(Console.ReadLine());

                Console.Write("Enter second value: ");
                double value2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter second unit (FEET/INCHES): ");
                LengthUnit unit2 = ParseUnit(Console.ReadLine());

                Length first  = new Length(value1, unit1);
                Length second = new Length(value2, unit2);

                Console.WriteLine("Exact Equality: " + _service.AreEqual(first, second));

                Console.Write("Enter tolerance (in inches): ");
                double tolerance = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Tolerance Equality: " +
                    _service.AreEqualWithTolerance(first, second, tolerance));
            }
            catch (ArgumentException ex) { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (FormatException)      { Console.WriteLine("Invalid input. Please enter a numeric value."); }
            catch (Exception ex)         { Console.WriteLine("Error: " + ex.Message); }
        }

        private LengthUnit ParseUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "FEET" || text == "FT")   return LengthUnit.FEET;
            if (text == "INCHES" || text == "IN")  return LengthUnit.INCHES;
            throw new ArgumentException($"Invalid unit '{raw}'. Use FEET or INCHES.");
        }
    }
}