using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC12: Demonstrates Subtraction and Division using the generic Quantity&lt;TUnit&gt; class.
    /// Shows that the same Subtract and Divide methods work across Length, Weight, and Volume
    /// without any changes to the generic infrastructure — proving UC10 scalability.
    ///
    /// Subtraction — returns a new Quantity in the first operand's unit (or an explicit target unit).
    /// Division    — returns a dimensionless double ratio.
    /// </summary>
    public class SubtractionAndDivisionPresentation
    {
        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC12: Subtraction and Division\n");
                Console.WriteLine("1. Length  (FEET / INCHES / YARDS / CENTIMETERS)");
                Console.WriteLine("2. Weight  (KG / GRAM / POUND)");
                Console.WriteLine("3. Volume  (LITRE / MILLILITRE / GALLON)");
                Console.Write("\nEnter category: ");
                int category = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("\n1. Subtraction");
                Console.WriteLine("2. Division");
                Console.Write("Enter operation: ");
                int operation = Convert.ToInt32(Console.ReadLine());

                switch (category)
                {
                    case 1: RunLength(operation); break;
                    case 2: RunWeight(operation); break;
                    case 3: RunVolume(operation); break;
                    default: Console.WriteLine("Invalid category"); break;
                }
            }
            catch (FormatException)        { Console.WriteLine("Invalid input. Please enter a numeric value."); }
            catch (ArgumentException ex)   { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (ArithmeticException ex) { Console.WriteLine("Arithmetic Error: " + ex.Message); }
            catch (Exception ex)           { Console.WriteLine("Error: " + ex.Message); }
        }

        // ── Length ────────────────────────────────────────────────────────────

        private void RunLength(int operation)
        {
            Console.Write("First value: ");
            double v1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("First unit (FEET/INCHES/YARDS/CENTIMETERS): ");
            var q1 = new Quantity<LengthUnitMeasurable>(v1, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));

            Console.Write("Second value: ");
            double v2 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
            var q2 = new Quantity<LengthUnitMeasurable>(v2, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));

            if (operation == 1)
                RunSubtraction(q1, q2, "FEET/INCHES/YARDS/CENTIMETERS", ParseLengthUnitMeasurable);
            else if (operation == 2)
                RunDivision(q1, q2);
            else
                Console.WriteLine("Invalid operation");
        }

        // ── Weight ────────────────────────────────────────────────────────────

        private void RunWeight(int operation)
        {
            Console.Write("First value: ");
            double v1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("First unit (KG/GRAM/POUND): ");
            var q1 = new Quantity<WeightUnitMeasurable>(v1, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));

            Console.Write("Second value: ");
            double v2 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Second unit (KG/GRAM/POUND): ");
            var q2 = new Quantity<WeightUnitMeasurable>(v2, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));

            if (operation == 1)
                RunSubtraction(q1, q2, "KG/GRAM/POUND", ParseWeightUnitMeasurable);
            else if (operation == 2)
                RunDivision(q1, q2);
            else
                Console.WriteLine("Invalid operation");
        }

        // ── Volume ────────────────────────────────────────────────────────────

        private void RunVolume(int operation)
        {
            Console.Write("First value: ");
            double v1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("First unit (LITRE/MILLILITRE/GALLON): ");
            var q1 = new Quantity<VolumeUnitMeasurable>(v1, new VolumeUnitMeasurable(ParseVolumeUnit(Console.ReadLine())));

            Console.Write("Second value: ");
            double v2 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Second unit (LITRE/MILLILITRE/GALLON): ");
            var q2 = new Quantity<VolumeUnitMeasurable>(v2, new VolumeUnitMeasurable(ParseVolumeUnit(Console.ReadLine())));

            if (operation == 1)
                RunSubtraction(q1, q2, "LITRE/MILLILITRE/GALLON", ParseVolumeUnitMeasurable);
            else if (operation == 2)
                RunDivision(q1, q2);
            else
                Console.WriteLine("Invalid operation");
        }

        // ── Single generic methods — one method handles ALL categories ─────────

        /// <summary>Subtraction demo — same method works for Length, Weight, and Volume.</summary>
        private void RunSubtraction<TUnit>(Quantity<TUnit> first, Quantity<TUnit> second,
                                           string unitPrompt, Func<string?, TUnit> parser)
            where TUnit : IMeasurable
        {
            // Step 1 — implicit target: difference in first operand's unit
            var diffInFirstUnit = DemonstrateSubtraction(first, second, first.Unit);
            Console.WriteLine($"\nDifference in first unit: {first} - {second} = {diffInFirstUnit}");

            // Step 2 — optional explicit target unit
            Console.Write($"\nEnter target unit ({unitPrompt}), or press ENTER to skip: ");
            string? targetRaw = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(targetRaw))
            {
                var diffInTarget = DemonstrateSubtraction(first, second, parser(targetRaw));
                Console.WriteLine($"Difference in target unit: {diffInTarget}");
            }
        }

        /// <summary>Division demo — same method works for Length, Weight, and Volume.</summary>
        private void RunDivision<TUnit>(Quantity<TUnit> first, Quantity<TUnit> second)
            where TUnit : IMeasurable
        {
            double ratio = DemonstrateDivision(first, second);
            Console.WriteLine($"\nResult: {first} ÷ {second} = {ratio:F4}");
        }

        private Quantity<TUnit> DemonstrateSubtraction<TUnit>(Quantity<TUnit> q1, Quantity<TUnit> q2, TUnit target)
            where TUnit : IMeasurable => q1.Subtract(q2, target);

        private double DemonstrateDivision<TUnit>(Quantity<TUnit> q1, Quantity<TUnit> q2)
            where TUnit : IMeasurable => q1.Divide(q2);

        // ── Parsers ───────────────────────────────────────────────────────────

        private LengthUnit ParseLengthUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "FEET"        || text == "FOOT" || text == "FT") return LengthUnit.FEET;
            if (text == "INCHES"      || text == "INCH" || text == "IN") return LengthUnit.INCHES;
            if (text == "YARDS"       || text == "YARD" || text == "YD") return LengthUnit.YARDS;
            if (text == "CENTIMETERS" || text == "CM")                   return LengthUnit.CENTIMETERS;
            throw new ArgumentException($"Invalid unit '{raw}'. Use FEET, INCHES, YARDS, or CENTIMETERS.");
        }

        private LengthUnitMeasurable ParseLengthUnitMeasurable(string? raw)
            => new LengthUnitMeasurable(ParseLengthUnit(raw));

        private WeightUnit ParseWeightUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "KG"  || text == "KILOGRAM" || text == "KILOGRAMS") return WeightUnit.KILOGRAM;
            if (text == "G"   || text == "GRAM"     || text == "GRAMS")     return WeightUnit.GRAM;
            if (text == "LB"  || text == "LBS"      || text == "POUND" || text == "POUNDS") return WeightUnit.POUND;
            throw new ArgumentException($"Invalid unit '{raw}'. Use KG, GRAM, or POUND.");
        }

        private WeightUnitMeasurable ParseWeightUnitMeasurable(string? raw)
            => new WeightUnitMeasurable(ParseWeightUnit(raw));

        private VolumeUnit ParseVolumeUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "LITRE"      || text == "LITER"      || text == "L")   return VolumeUnit.LITRE;
            if (text == "MILLILITRE" || text == "MILLILITER" || text == "ML")  return VolumeUnit.MILLILITRE;
            if (text == "GALLON"     || text == "GALLONS"    || text == "GAL") return VolumeUnit.GALLON;
            throw new ArgumentException($"Invalid unit '{raw}'. Use LITRE, MILLILITRE, or GALLON.");
        }

        private VolumeUnitMeasurable ParseVolumeUnitMeasurable(string? raw)
            => new VolumeUnitMeasurable(ParseVolumeUnit(raw));
    }
}