using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC5: Presentation layer for unit-to-unit Length conversion.
    /// Depends on ILengthService — ready for ASP.NET Controller migration.
    ///
    /// The static demonstration helpers that were previously in this class
    /// have been removed. All conversion logic is now routed through the
    /// service layer (ILengthService → LengthService → LengthRepository → Length entity)
    /// to keep every layer's responsibility clean.
    /// </summary>
    public class LengthPresentationUC5
    {
        private readonly ILengthService _service;

        public LengthPresentationUC5()
        {
            _service = new LengthService();
        }

        /// <summary>ASP.NET-ready constructor — accepts service via dependency injection.</summary>
        public LengthPresentationUC5(ILengthService service)
        {
            _service = service;
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC5: Unit-to-Unit Conversion");

                Console.Write("Enter value to convert: ");
                double value = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter source unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit sourceUnit = ParseUnit(Console.ReadLine());
                Console.Write("Enter target unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit targetUnit = ParseUnit(Console.ReadLine());

                Length original  = new Length(value, sourceUnit);
                Length converted = _service.ConvertTo(original, targetUnit);

                Console.WriteLine($"\nConversion: {original} -> {converted}");

                // Equality check — original and converted represent the same physical length
                Console.WriteLine($"Are they equal? {_service.AreEqual(original, converted)}");

                Console.Write("Enter tolerance (in inches): ");
                double tolerance = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Tolerance Equality: " +
                    _service.AreEqualWithTolerance(original, converted, tolerance));
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