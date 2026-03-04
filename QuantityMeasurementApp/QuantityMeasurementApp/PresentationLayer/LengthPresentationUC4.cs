using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC4: Presentation layer for extended unit equality (FEET, INCHES, YARDS, CENTIMETERS).
    /// Depends on ILengthService — ready for ASP.NET Controller migration.
    /// </summary>
    public class LengthPresentationUC4
    {
        private readonly ILengthService _service;

        public LengthPresentationUC4()
        {
            _service = new LengthService();
        }

        /// <summary>ASP.NET-ready constructor — accepts service via dependency injection.</summary>
        public LengthPresentationUC4(ILengthService service)
        {
            _service = service;
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC4: Extended Unit Support");

                Console.Write("Enter first value: ");
                double value1 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter first unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit unit1 = ParseUnit(Console.ReadLine());

                Console.Write("Enter second value: ");
                double value2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
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
            if (text == "FEET"        || text == "FOOT" || text == "FT") return LengthUnit.FEET;
            if (text == "INCHES"      || text == "INCH" || text == "IN") return LengthUnit.INCHES;
            if (text == "YARDS"       || text == "YARD" || text == "YD") return LengthUnit.YARDS;
            if (text == "CENTIMETERS" || text == "CENTIMETER" || text == "CM") return LengthUnit.CENTIMETERS;
            throw new ArgumentException($"Invalid unit '{raw}'. Use FEET, INCHES, YARDS, or CENTIMETERS.");
        }
    }
}