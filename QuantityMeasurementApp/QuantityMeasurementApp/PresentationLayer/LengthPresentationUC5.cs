using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC5 - QuantityMeasurementApp - UC5 - Extended Unit Support with Conversion
    /// 
    /// <p>Extends UC4 to provide unit-to-unit conversion for values within the same measurement category
    /// (for example, length-to-length conversions such as feet-to-inches or yards-to-inches).
    /// It builds on validation and equality checks from UC4 and adds explicit conversion logic 
    /// so callers can obtain a numeric result in the requested target unit.</p>
    /// 
    /// <p>Responsibilities:</p>
    /// <ul>
    /// <li>Validate that source and target units belong to the same measurement category (e.g., both are length units).</li>
    /// <li>Perform conversion by normalizing the source value to a canonical/base unit and then converting to the target unit.</li>
    /// <li>Return a numeric result with a defined precision (implementation-defined, typically a fixed decimal/rounded value).</li>
    /// <li>Throw an ArgumentException for null inputs or incompatible unit pairs.</li>
    /// </ul>
    /// 
    /// <p>Examples:</p>
    /// <pre>
    /// // Convert 3 feet to inches => 36.0
    /// // Convert 2 yards to inches => 72.0
    /// </pre>
    /// 
    /// <p>Notes:</p>
    /// <ul>
    /// <li>Only conversions within the same category are allowed; cross-category conversion (e.g., length-to-weight or length-to-time) is not supported.</li>
    /// <li>Supported units include (but are not limited to) feet, inches, yards and other length units provided by UC4.</li>
    /// </ul>
    /// 
    /// @since 1.0
    /// @version 1.0
    /// @author Development Team
    /// </summary>
    public class LengthPresentationUC5
    {
        /// <summary>
        /// Demonstrate length equality between two Length instances.
        /// </summary>
        /// <param name="firstLength">the first Length instance</param>
        /// <param name="secondLength">the second Length instance</param>
        /// <returns>true if the two lengths are equal, false otherwise</returns>
        public static bool DemonstrateLengthEquality(Length firstLength, Length secondLength)
        {
            if (firstLength == null || secondLength == null)
            {
                Console.WriteLine("Error: Length instances cannot be null");
                return false;
            }

            bool result = firstLength.Equals(secondLength);
            Console.WriteLine($"Length Equality: {firstLength} == {secondLength} => {result}");
            return result;
        }

        /// <summary>
        /// Demonstrate length comparison between two Length instances.
        /// </summary>
        /// <param name="firstLength">the first Length instance</param>
        /// <param name="secondLength">the second Length instance</param>
        /// <returns>true if the two lengths are equal, false otherwise</returns>
        public static bool DemonstrateLengthComparison(Length firstLength, Length secondLength)
        {
            if (firstLength == null || secondLength == null)
            {
                Console.WriteLine("Error: Length instances cannot be null");
                return false;
            }

            return DemonstrateLengthEquality(firstLength, secondLength);
        }

        /// <summary>
        /// Demonstrate length conversion from one unit to another.
        /// Method overload 1: Takes a numeric value and two units (from and to).
        /// Used when you have raw values to convert.
        /// </summary>
        /// <param name="value">the length value to convert</param>
        /// <param name="fromUnit">the unit of the length value</param>
        /// <param name="toUnit">the target unit to convert to</param>
        /// <returns>a new Length instance representing the converted length</returns>
        public static Length DemonstrateLengthConversion(double value, LengthUnit fromUnit, LengthUnit toUnit)
        {
            try
            {
                // Validate inputs
                if (!double.IsFinite(value))
                {
                    throw new ArgumentException("Value must be a finite number");
                }

                if (!Enum.IsDefined(typeof(LengthUnit), fromUnit) || fromUnit == LengthUnit.UNKNOWN)
                {
                    throw new ArgumentException("Invalid source unit");
                }

                if (!Enum.IsDefined(typeof(LengthUnit), toUnit) || toUnit == LengthUnit.UNKNOWN)
                {
                    throw new ArgumentException("Invalid target unit");
                }

                // Perform conversion using static method
                double convertedValue = Length.Convert(value, fromUnit, toUnit);
                Length convertedLength = new Length(convertedValue, toUnit);

                Console.WriteLine($"Conversion: {value} {fromUnit} => {convertedValue:F2} {toUnit}");
                return convertedLength;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Conversion Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Demonstrate length conversion from one Length instance to another unit.
        /// Method overload 2: Takes an existing Length object and target unit.
        /// Used when you already have a Length instance.
        /// Method Overloading means having multiple methods
        /// with the same name but different parameter lists within the same class.
        /// </summary>
        /// <param name="length">the Length instance to convert</param>
        /// <param name="toUnit">the target unit to convert to</param>
        /// <returns>a new Length instance representing the converted length</returns>
        public static Length DemonstrateLengthConversion(Length length, LengthUnit toUnit)
        {
            if (length == null)
            {
                throw new ArgumentNullException(nameof(length), "Length instance cannot be null");
            }

            if (!Enum.IsDefined(typeof(LengthUnit), toUnit) || toUnit == LengthUnit.UNKNOWN)
            {
                throw new ArgumentException("Invalid target unit");
            }

            try
            {
                // Use instance method for conversion
                Length convertedLength = length.ConvertTo(toUnit);
                Console.WriteLine($"Conversion: {length} => {convertedLength}");
                return convertedLength;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Conversion Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Standalone test method for demonstration of UC5 functionality.
        /// This allows quick verification of functionality without needing a separate test framework.
        /// Can be called independently for testing purposes.
        /// </summary>
        public static void RunStandaloneTest()
        {
            Console.WriteLine("\n=== UC5: Unit-to-Unit Conversion (Same Measurement Type) ===\n");

            try
            {
                // Example 1: Convert 3 feet to inches
                Console.WriteLine("Example 1: Convert 3 feet to inches");
                Length feetLength = new Length(3.0, LengthUnit.FEET);
                Length inchesLength = DemonstrateLengthConversion(feetLength, LengthUnit.INCHES);
                Console.WriteLine($"Result: {inchesLength}\n");

                // Example 2: Convert 2 yards to inches using static method
                Console.WriteLine("Example 2: Convert 2 yards to inches");
                double yardsToInches = Length.Convert(2.0, LengthUnit.YARDS, LengthUnit.INCHES);
                Console.WriteLine($"Result: {yardsToInches:F2} {LengthUnit.INCHES}\n");

                // Example 3: Convert 100 centimeters to feet
                Console.WriteLine("Example 3: Convert 100 centimeters to feet");
                Length cmLength = new Length(100.0, LengthUnit.CENTIMETERS);
                Length feetFromCm = DemonstrateLengthConversion(cmLength, LengthUnit.FEET);
                Console.WriteLine($"Result: {feetFromCm}\n");

                // Example 4: Demonstrate equality check
                Console.WriteLine("Example 4: Equality check");
                Length lengthInInches = new Length(36.0, LengthUnit.INCHES);
                Length lengthInFeet = new Length(3.0, LengthUnit.FEET);
                DemonstrateLengthEquality(lengthInInches, lengthInFeet);
                Console.WriteLine();

                // Example 5: Convert using overloaded method with raw values
                Console.WriteLine("Example 5: Convert 5 feet to yards using overloaded method");
                Length converted = DemonstrateLengthConversion(5.0, LengthUnit.FEET, LengthUnit.YARDS);
                Console.WriteLine($"Result: {converted}\n");

                Console.WriteLine("=== UC5 Demonstration Complete ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during demonstration: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        private readonly LengthService service;

        public LengthPresentationUC5()
        {
            service = new LengthService();
        }

        /// <summary>
        /// UC5: Interactive method to run conversion functionality.
        /// Flow:
        /// 1. Enter value to convert
        /// 2. Enter source unit (feet, inches, yards, centimeter)
        /// 3. Enter target unit (feet, inches, yards, centimeter)
        /// 4. Display conversion result
        /// 5. Display equality check (with and without tolerance)
        /// </summary>
        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC5: Unit-to-Unit Conversion\n");

                // Step 1: Enter value to convert
                Console.Write("Enter value to convert: ");
                double value = Convert.ToDouble(Console.ReadLine());

                // Step 2: Enter source unit
                Console.Write("Enter source unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                string sourceUnitStr = Console.ReadLine()!.Trim().ToUpper();
                LengthUnit sourceUnit = ParseUnit(sourceUnitStr);

                // Step 3: Enter target unit
                Console.Write("Enter target unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                string targetUnitStr = Console.ReadLine()!.Trim().ToUpper();
                LengthUnit targetUnit = ParseUnit(targetUnitStr);

                // Perform conversion
                Console.WriteLine("\n--- Conversion Result ---");
                Length originalLength = new Length(value, sourceUnit);
                Length convertedLength = originalLength.ConvertTo(targetUnit);
                
                Console.WriteLine($"Original: {originalLength}");
                Console.WriteLine($"Converted: {convertedLength}");
                Console.WriteLine($"Conversion: {value} {sourceUnit} = {convertedLength.Value:F2} {targetUnit}");

                // Step 4 & 5: Display equality check (with and without tolerance)
                Console.WriteLine("\n--- Equality Check ---");
                
                // Exact equality check
                bool exactEqual = service.AreEqual(originalLength, convertedLength);
                Console.WriteLine($"Exact Equality: {exactEqual}");

                // Tolerance equality check
                Console.Write("Enter tolerance (in inches, default 0.01): ");
                string toleranceInput = Console.ReadLine()!.Trim();
                double tolerance = string.IsNullOrEmpty(toleranceInput) ? 0.01 : Convert.ToDouble(toleranceInput);

                bool toleranceEqual = service.AreEqualWithTolerance(originalLength, convertedLength, tolerance);
                Console.WriteLine($"Tolerance Equality (tolerance: {tolerance} inches): {toleranceEqual}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Invalid input format. Please enter a valid number.");
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
        /// Helper method to parse unit string input.
        /// Supports case-insensitive input and common variations.
        /// </summary>
        private LengthUnit ParseUnit(string unitStr)
        {
            // Normalize input
            unitStr = unitStr.Trim().ToUpper();

            // Handle common variations
            if (unitStr == "FEET" || unitStr == "FOOT" || unitStr == "FT")
                return LengthUnit.FEET;
            if (unitStr == "INCHES" || unitStr == "INCH" || unitStr == "IN")
                return LengthUnit.INCHES;
            if (unitStr == "YARDS" || unitStr == "YARD" || unitStr == "YD")
                return LengthUnit.YARDS;
            if (unitStr == "CENTIMETERS" || unitStr == "CENTIMETER" || unitStr == "CM")
                return LengthUnit.CENTIMETERS;

            // Try direct enum parse
            if (Enum.TryParse<LengthUnit>(unitStr, true, out LengthUnit unit))
            {
                if (unit != LengthUnit.UNKNOWN)
                    return unit;
            }

            throw new ArgumentException($"Invalid unit: {unitStr}. Please use FEET, INCHES, YARDS, or CENTIMETERS.");
        }
    }
}

