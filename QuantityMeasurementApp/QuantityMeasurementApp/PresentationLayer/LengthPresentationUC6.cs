using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC6: Addition of two length measurements.
    /// This class shows how to add two Length values and get the result
    /// in the unit of the first value.
    /// </summary>
    public class LengthPresentationUC6
    {
        /// <summary>
        /// Service used for equality and tolerance checks.
        /// </summary>
        private readonly LengthService service;

        public LengthPresentationUC6()
        {
            service = new LengthService();
        }

        /// <summary>
        /// Simple helper that adds two Length objects.
        /// The result unit is the unit of the first length.
        /// </summary>
        public static Length DemonstrateLengthAddition(Length first, Length second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first), "First length cannot be null");

            if (second == null)
                throw new ArgumentNullException(nameof(second), "Second length cannot be null");

            Length result = first.Add(second);
            Console.WriteLine($"Addition: {first} + {second} = {result}");
            return result;
        }

        /// <summary>
        /// Helper that adds two raw values with units.
        /// Result is returned in the unit of the first value.
        /// </summary>
        public static Length DemonstrateLengthAddition(double firstValue, LengthUnit firstUnit,
                                                       double secondValue, LengthUnit secondUnit)
        {
            Length result = Length.Add(firstValue, firstUnit, secondValue, secondUnit, firstUnit);
            Console.WriteLine($"Addition: {firstValue} {firstUnit} + {secondValue} {secondUnit} = {result}");
            return result;
        }

        /// <summary>
        /// Interactive UC6 demo.
        /// Steps:
        /// 1. Read first value and unit.
        /// 2. Read second value and unit.
        /// 3. Add them and show the sum in the unit of the first value.
        /// </summary>
        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC6: Add two length values\n");

                // First length
                Console.Write("Enter first value: ");
                double firstValue = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter first unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit firstUnit = ParseUnit(Console.ReadLine());

                // Second length
                Console.Write("Enter second value: ");
                double secondValue = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit secondUnit = ParseUnit(Console.ReadLine());

                // Build Length objects
                Length first = new Length(firstValue, firstUnit);
                Length second = new Length(secondValue, secondUnit);

                // Add and show result (result unit = first unit)
                Length result = first.Add(second);

                Console.WriteLine("\n--- Addition Result ---");
                Console.WriteLine($"{firstValue} {firstUnit} + {secondValue} {secondUnit} = {result}");

                // Ask for tolerance and tell if the two inputs are equal within that tolerance
                Console.Write("\nEnter tolerance: ");
                string toleranceInput = Console.ReadLine()!.Trim();
                double tolerance = string.IsNullOrEmpty(toleranceInput) ? 0.01 : Convert.ToDouble(toleranceInput);

                bool equalWithTolerance = service.AreEqualWithTolerance(first, second, tolerance);
                Console.WriteLine($"Tolerance Equality: {equalWithTolerance}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Please enter a valid number.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Reads and parses a length unit from user input.
        /// Accepts simple names and short forms.
        /// </summary>
        private LengthUnit ParseUnit(string? raw)
        {
            if (raw == null)
                throw new ArgumentException("Unit cannot be empty");

            string unitStr = raw.Trim().ToUpper();

            if (unitStr == "FEET" || unitStr == "FOOT" || unitStr == "FT")
                return LengthUnit.FEET;

            if (unitStr == "INCHES" || unitStr == "INCH" || unitStr == "IN")
                return LengthUnit.INCHES;

            if (unitStr == "YARDS" || unitStr == "YARD" || unitStr == "YD")
                return LengthUnit.YARDS;

            if (unitStr == "CENTIMETERS" || unitStr == "CENTIMETER" || unitStr == "CM")
                return LengthUnit.CENTIMETERS;

            throw new ArgumentException("Unit is not valid. Use FEET, INCHES, YARDS or CENTIMETERS.");
        }
    }
}

