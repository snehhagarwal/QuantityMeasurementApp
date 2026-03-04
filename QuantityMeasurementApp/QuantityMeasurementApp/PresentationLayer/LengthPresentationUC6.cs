using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC6: Presentation layer for Length addition (result in first operand's unit).
    /// Depends on ILengthService — ready for ASP.NET Controller migration.
    ///
    /// The static DemonstrateLengthAddition helpers that were in this class have been
    /// removed. All addition logic now flows through the service layer.
    /// </summary>
    public class LengthPresentationUC6
    {
        private readonly ILengthService _service;

        public LengthPresentationUC6()
        {
            _service = new LengthService();
        }

        /// <summary>ASP.NET-ready constructor — accepts service via dependency injection.</summary>
        public LengthPresentationUC6(ILengthService service)
        {
            _service = service;
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC6: Addition of Lengths");

                Console.Write("Enter first value: ");
                double firstValue = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter first unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit firstUnit = ParseUnit(Console.ReadLine());

                Console.Write("Enter second value: ");
                double secondValue = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit secondUnit = ParseUnit(Console.ReadLine());

                Length first  = new Length(firstValue,  firstUnit);
                Length second = new Length(secondValue, secondUnit);

                // Result in first operand's unit (UC6 requirement)
                Length result = _service.Add(first, second);
                Console.WriteLine($"\nResult: {firstValue} {firstUnit} + {secondValue} {secondUnit} = {result}");

                Console.Write("\nEnter tolerance (in inches): ");
                string? toleranceInput = Console.ReadLine();
                double tolerance = string.IsNullOrWhiteSpace(toleranceInput) ? 0.01 : Convert.ToDouble(toleranceInput);
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