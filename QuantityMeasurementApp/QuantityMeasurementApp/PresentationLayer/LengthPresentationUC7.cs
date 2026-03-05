using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC7: Presentation layer for Length addition with explicit target unit.
    /// The user specifies both operands and a target unit.
    /// The result is returned directly in the requested target unit.
    /// </summary>
    public class LengthPresentationUC7
    {
        private readonly ILengthService _service;

        public LengthPresentationUC7()
        {
            _service = new LengthService();
        }

        /// <summary>ASP.NET-ready constructor — accepts service via dependency injection.</summary>
        public LengthPresentationUC7(ILengthService service)
        {
            _service = service;
        }

        public void Run()
       {
           try
           {
               Console.WriteLine("\nAddition with Target Unit");

               Console.Write("Enter first value: ");
               double firstValue = Convert.ToDouble(Console.ReadLine());
               Console.Write("Enter first unit (FEET/INCHES/YARDS/CENTIMETERS): ");
               LengthUnit firstUnit = ParseUnit(Console.ReadLine());

               Console.Write("Enter second value: ");
               double secondValue = Convert.ToDouble(Console.ReadLine());
               Console.Write("Enter second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
               LengthUnit secondUnit = ParseUnit(Console.ReadLine());

               Console.Write("Enter target unit for result (FEET/INCHES/YARDS/CENTIMETERS): ");
               LengthUnit targetUnit = ParseUnit(Console.ReadLine());

               Length first  = new Length(firstValue,  firstUnit);
               Length second = new Length(secondValue, secondUnit);

               Length result = _service.Add(first, second, targetUnit);
               Console.WriteLine($"\nResult: {first} + {second} = {result} ({targetUnit})");
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