using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC7: Addition with target unit specification.
    /// Lets you choose the unit for the result.
    /// </summary>
    public class LengthPresentationUC7
    {
        private readonly LengthService service;

        public LengthPresentationUC7()
        {
            service = new LengthService();
        }

        /// <summary>
        /// Interactive UC7 demo.
        /// 1. Read first value and unit.
        /// 2. Read second value and unit.
        /// 3. Read target unit for the result.
        /// 4. Add and show result in the target unit.
        /// 5. Check commutativity (A+B vs B+A) with tolerance.
        /// </summary>
        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC7: Add two lengths with target unit\n");

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

                // Target unit
                Console.Write("Enter target unit for result (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit targetUnit = ParseUnit(Console.ReadLine());

                Length firstLength = new Length(firstValue, firstUnit);
                Length secondLength = new Length(secondValue, secondUnit);

                // A + B in target unit
                Length sumFirstThenSecond = Length.Add(firstLength, secondLength, targetUnit);

                Console.WriteLine("\n--- Addition Result ---");
                Console.WriteLine($"{firstLength} + {secondLength} = {sumFirstThenSecond} (target unit: {targetUnit})");

                // B + A in same target unit for commutativity check
                Length sumSecondThenFirst = Length.Add(secondLength, firstLength, targetUnit);

                // Tolerance feature: are the two sums equal within user tolerance?
                Console.Write("\nEnter tolerance: ");
                string toleranceInput = Console.ReadLine()!.Trim();
                double tolerance = string.IsNullOrEmpty(toleranceInput) ? 0.01 : Convert.ToDouble(toleranceInput);

                bool equalWithTolerance = service.AreEqualWithTolerance(sumFirstThenSecond, sumSecondThenFirst, tolerance);
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
        /// Parses a unit string into LengthUnit.
        /// Accepts full names and short forms.
        /// </summary>
        private LengthUnit ParseUnit(string? rawInput)
        {
            if (rawInput == null)
                throw new ArgumentException("Unit cannot be empty");

            string unitText = rawInput.Trim().ToUpper();

            if (unitText == "FEET" || unitText == "FOOT" || unitText == "FT")
                return LengthUnit.FEET;

            if (unitText == "INCHES" || unitText == "INCH" || unitText == "IN")
                return LengthUnit.INCHES;

            if (unitText == "YARDS" || unitText == "YARD" || unitText == "YD")
                return LengthUnit.YARDS;

            if (unitText == "CENTIMETERS" || unitText == "CENTIMETER" || unitText == "CM")
                return LengthUnit.CENTIMETERS;

            throw new ArgumentException("Unit is not valid. Use FEET, INCHES, YARDS or CENTIMETERS.");
        }
    }
}

