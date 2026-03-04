using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interface;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC10: Generic Quantity presentation class.
    /// Demonstrates that a single generic Quantity&lt;TUnit&gt; class handles
    /// Length, Weight, and Volume with zero duplication.
    ///
    /// UC11: Extended to include Volume as category 3.
    ///
    /// All service calls are routed through ILengthService, IWeightService,
    /// and IVolumeService — ready for ASP.NET Controller migration.
    ///
    /// Addition follows the two-step pattern (all categories):
    ///   Step 1 — shows sum in first operand's unit (implicit target).
    ///   Step 2 — optionally asks for explicit target unit and shows again.
    /// </summary>
    public class QuantityPresentation
    {
        private readonly ILengthService _lengthService;
        private readonly IWeightService _weightService;
        private readonly IVolumeService _volumeService;

        public QuantityPresentation()
        {
            _lengthService = new LengthService();
            _weightService = new WeightService();
            _volumeService = new VolumeService();
        }

        /// <summary>ASP.NET-ready constructor — accepts all services via dependency injection.</summary>
        public QuantityPresentation(ILengthService lengthService,
                                    IWeightService weightService,
                                    IVolumeService volumeService)
        {
            _lengthService = lengthService;
            _weightService = weightService;
            _volumeService = volumeService;
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC10: Generic Quantity Measurement\n");
                Console.WriteLine("1. Length Operations");
                Console.WriteLine("2. Weight Operations");
                Console.WriteLine("3. Cross-Category Prevention");
                Console.WriteLine("4. Generic Demonstration Methods");
                Console.Write("\nEnter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1: RunLengthOperations();    break;
                    case 2: RunWeightOperations();    break;
                    case 3: RunCrossCategoryCheck();  break;
                    case 4: RunGenericDemonstration();break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
            catch (FormatException)      { Console.WriteLine("Invalid input. Please enter a numeric value."); }
            catch (ArgumentException ex) { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("Error: " + ex.Message); }
        }

        // ── Cross-Category Prevention ─────────────────────────────────────────

        private void RunCrossCategoryCheck()
        {
            try
            {
                Console.WriteLine("\nCross-Category Prevention\n");

                Console.Write("Enter a length value: ");
                double lv = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter length unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                var lq = new Quantity<LengthUnitMeasurable>(lv, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));

                Console.Write("Enter a weight value: ");
                double wv = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter weight unit (KG/GRAM/POUND): ");
                var wq = new Quantity<WeightUnitMeasurable>(wv, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));

                Console.WriteLine($"\nLength == Weight : {lq} == {wq} -> {lq.Equals(wq)}");
            }
            catch (FormatException)      { Console.WriteLine("Invalid input. Please enter a numeric value."); }
            catch (ArgumentException ex) { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("Error: " + ex.Message); }
        }

        // ── Generic Demonstration ─────────────────────────────────────────────

        private void RunGenericDemonstration()
        {
            try
            {
                Console.WriteLine("\nGeneric Demonstration Methods\n");
                Console.WriteLine("1. Equality   2. Conversion   3. Addition");
                Console.Write("Operation: ");
                int op = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("1. Length   2. Weight");
                Console.Write("Category: ");
                int cat = Convert.ToInt32(Console.ReadLine());

                if (cat == 1)       RunGenericLength(op);
                else if (cat == 2)  RunGenericWeight(op);
                else                Console.WriteLine("Invalid category");
            }
            catch (FormatException)      { Console.WriteLine("Invalid input. Please enter a numeric value."); }
            catch (ArgumentException ex) { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("Error: " + ex.Message); }
        }

        private void RunGenericLength(int op)
        {
            Console.Write("First value: ");
            double v1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("First unit (FEET/INCHES/YARDS/CENTIMETERS): ");
            var q1 = new Quantity<LengthUnitMeasurable>(v1, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));

            if (op == 1)
            {
                Console.Write("Second value: ");
                double v2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                var q2 = new Quantity<LengthUnitMeasurable>(v2, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));
                Console.WriteLine($"\nResult: {q1} == {q2} -> {DemonstrateEquality(q1, q2)}");
            }
            else if (op == 2)
            {
                Console.Write("Target unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                var result = DemonstrateConversion(q1, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));
                Console.WriteLine($"\nResult: {q1} -> {result}");
            }
            else if (op == 3)
            {
                Console.Write("Second value: ");
                double v2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                var q2 = new Quantity<LengthUnitMeasurable>(v2, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));

                var sumFirst = DemonstrateAddition(q1, q2, q1.Unit);
                Console.WriteLine($"\nSum in first unit ({q1.Unit.GetUnitName()}): {q1} + {q2} = {sumFirst}");

                Console.Write("\nEnter target unit (FEET/INCHES/YARDS/CENTIMETERS), or ENTER to skip: ");
                string? targetRaw = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(targetRaw))
                {
                    var sumTarget = DemonstrateAddition(q1, q2, new LengthUnitMeasurable(ParseLengthUnit(targetRaw)));
                    Console.WriteLine($"Sum in target unit ({sumTarget.Unit.GetUnitName()}): {sumTarget}");
                }
            }
            else { Console.WriteLine("Invalid operation"); }
        }

        private void RunGenericWeight(int op)
        {
            Console.Write("First value: ");
            double v1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("First unit (KG/GRAM/POUND): ");
            var q1 = new Quantity<WeightUnitMeasurable>(v1, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));

            if (op == 1)
            {
                Console.Write("Second value: ");
                double v2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Second unit (KG/GRAM/POUND): ");
                var q2 = new Quantity<WeightUnitMeasurable>(v2, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));
                Console.WriteLine($"\nResult: {q1} == {q2} -> {DemonstrateEquality(q1, q2)}");
            }
            else if (op == 2)
            {
                Console.Write("Target unit (KG/GRAM/POUND): ");
                var result = DemonstrateConversion(q1, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));
                Console.WriteLine($"\nResult: {q1} -> {result}");
            }
            else if (op == 3)
            {
                Console.Write("Second value: ");
                double v2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Second unit (KG/GRAM/POUND): ");
                var q2 = new Quantity<WeightUnitMeasurable>(v2, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));

                var sumFirst = DemonstrateAddition(q1, q2, q1.Unit);
                Console.WriteLine($"\nSum in first unit ({q1.Unit.GetUnitName()}): {q1} + {q2} = {sumFirst}");

                Console.Write("\nEnter target unit (KG/GRAM/POUND), or ENTER to skip: ");
                string? targetRaw = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(targetRaw))
                {
                    var sumTarget = DemonstrateAddition(q1, q2, new WeightUnitMeasurable(ParseWeightUnit(targetRaw)));
                    Console.WriteLine($"Sum in target unit ({sumTarget.Unit.GetUnitName()}): {sumTarget}");
                }
            }
            else { Console.WriteLine("Invalid operation"); }
        }

        private void RunGenericVolume(int op)
        {
            Console.Write("First value: ");
            double v1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("First unit (LITRE/MILLILITRE/GALLON): ");
            var q1 = new Quantity<VolumeUnitMeasurable>(v1, new VolumeUnitMeasurable(ParseVolumeUnit(Console.ReadLine())));

            if (op == 1)
            {
                Console.Write("Second value: ");
                double v2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Second unit (LITRE/MILLILITRE/GALLON): ");
                var q2 = new Quantity<VolumeUnitMeasurable>(v2, new VolumeUnitMeasurable(ParseVolumeUnit(Console.ReadLine())));
                Console.WriteLine($"\nResult: {q1} == {q2} -> {DemonstrateEquality(q1, q2)}");
            }
            else if (op == 2)
            {
                Console.Write("Target unit (LITRE/MILLILITRE/GALLON): ");
                var result = DemonstrateConversion(q1, new VolumeUnitMeasurable(ParseVolumeUnit(Console.ReadLine())));
                Console.WriteLine($"\nResult: {q1} -> {result}");
            }
            else if (op == 3)
            {
                Console.Write("Second value: ");
                double v2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Second unit (LITRE/MILLILITRE/GALLON): ");
                var q2 = new Quantity<VolumeUnitMeasurable>(v2, new VolumeUnitMeasurable(ParseVolumeUnit(Console.ReadLine())));

                var sumFirst = DemonstrateAddition(q1, q2, q1.Unit);
                Console.WriteLine($"\nSum in first unit ({q1.Unit.GetUnitName()}): {q1} + {q2} = {sumFirst}");

                Console.Write("\nEnter target unit (LITRE/MILLILITRE/GALLON), or ENTER to skip: ");
                string? targetRaw = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(targetRaw))
                {
                    var sumTarget = DemonstrateAddition(q1, q2, new VolumeUnitMeasurable(ParseVolumeUnit(targetRaw)));
                    Console.WriteLine($"Sum in target unit ({sumTarget.Unit.GetUnitName()}): {sumTarget}");
                }
            }
            else { Console.WriteLine("Invalid operation"); }
        }

        // ── Single generic methods — one method handles ALL categories ────────

        private bool DemonstrateEquality<TUnit>(Quantity<TUnit> q1, Quantity<TUnit> q2)
            where TUnit : IMeasurable => q1.Equals(q2);

        private Quantity<TUnit> DemonstrateConversion<TUnit>(Quantity<TUnit> q, TUnit target)
            where TUnit : IMeasurable => q.ConvertTo(target);

        private Quantity<TUnit> DemonstrateAddition<TUnit>(Quantity<TUnit> q1, Quantity<TUnit> q2, TUnit target)
            where TUnit : IMeasurable => q1.Add(q2, target);

        // ── Length sub-menu ───────────────────────────────────────────────────

        private void RunLengthOperations()
        {
            try
            {
                Console.WriteLine("\nLength Operations\n");
                Console.WriteLine("1. Equality   2. Conversion   3. Addition");
                Console.Write("Enter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1: RunLengthEquality();   break;
                    case 2: RunLengthConversion(); break;
                    case 3: RunLengthAddition();   break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
            catch (FormatException)      { Console.WriteLine("Invalid input. Please enter a numeric value."); }
            catch (ArgumentException ex) { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("Error: " + ex.Message); }
        }

        private void RunLengthEquality()
        {
            Console.Write("Enter first length value: ");
            double val1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (FEET/INCHES/YARDS/CENTIMETERS): ");
            LengthUnit unit1 = ParseLengthUnit(Console.ReadLine());

            Console.Write("Enter second length value: ");
            double val2 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
            LengthUnit unit2 = ParseLengthUnit(Console.ReadLine());

            var q1 = new Quantity<LengthUnitMeasurable>(val1, new LengthUnitMeasurable(unit1));
            var q2 = new Quantity<LengthUnitMeasurable>(val2, new LengthUnitMeasurable(unit2));
            Console.WriteLine($"\nResult: {q1} == {q2} -> {q1.Equals(q2)}");
        }

        private void RunLengthConversion()
        {
            Console.Write("Enter length value: ");
            double val = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter source unit (FEET/INCHES/YARDS/CENTIMETERS): ");
            LengthUnit sourceUnit = ParseLengthUnit(Console.ReadLine());
            Console.Write("Enter target unit (FEET/INCHES/YARDS/CENTIMETERS): ");
            LengthUnit targetUnit = ParseLengthUnit(Console.ReadLine());

            var q      = new Quantity<LengthUnitMeasurable>(val, new LengthUnitMeasurable(sourceUnit));
            var result = q.ConvertTo(new LengthUnitMeasurable(targetUnit));
            Console.WriteLine($"\nResult: {q} -> {result}");
        }

        private void RunLengthAddition()
        {
            Console.Write("Enter first length value: ");
            double val1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (FEET/INCHES/YARDS/CENTIMETERS): ");
            LengthUnit unit1 = ParseLengthUnit(Console.ReadLine());

            Console.Write("Enter second length value: ");
            double val2 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
            LengthUnit unit2 = ParseLengthUnit(Console.ReadLine());

            var q1 = new Quantity<LengthUnitMeasurable>(val1, new LengthUnitMeasurable(unit1));
            var q2 = new Quantity<LengthUnitMeasurable>(val2, new LengthUnitMeasurable(unit2));

            var sumInFirstUnit = q1.Add(q2);
            Console.WriteLine($"\nSum in first unit ({unit1}): {q1} + {q2} = {sumInFirstUnit}");

            Console.Write("\nEnter target unit (FEET/INCHES/YARDS/CENTIMETERS), or ENTER to skip: ");
            string? targetRaw = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(targetRaw))
            {
                LengthUnit resultUnit  = ParseLengthUnit(targetRaw);
                var        sumInTarget = q1.Add(q2, new LengthUnitMeasurable(resultUnit));
                Console.WriteLine($"Sum in target unit ({resultUnit}): {sumInTarget}");
            }
        }

        // ── Weight sub-menu ───────────────────────────────────────────────────

        private void RunWeightOperations()
        {
            try
            {
                Console.WriteLine("\nWeight Operations\n");
                Console.WriteLine("1. Equality   2. Conversion   3. Addition");
                Console.Write("Enter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1: RunWeightEquality();   break;
                    case 2: RunWeightConversion(); break;
                    case 3: RunWeightAddition();   break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
            catch (FormatException)      { Console.WriteLine("Invalid input. Please enter a numeric value."); }
            catch (ArgumentException ex) { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("Error: " + ex.Message); }
        }

        private void RunWeightEquality()
        {
            Console.Write("Enter first weight value: ");
            double val1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (KG/GRAM/POUND): ");
            WeightUnit unit1 = ParseWeightUnit(Console.ReadLine());

            Console.Write("Enter second weight value: ");
            double val2 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (KG/GRAM/POUND): ");
            WeightUnit unit2 = ParseWeightUnit(Console.ReadLine());

            var q1 = new Quantity<WeightUnitMeasurable>(val1, new WeightUnitMeasurable(unit1));
            var q2 = new Quantity<WeightUnitMeasurable>(val2, new WeightUnitMeasurable(unit2));
            Console.WriteLine($"\nResult: {q1} == {q2} -> {q1.Equals(q2)}");
        }

        private void RunWeightConversion()
        {
            Console.Write("Enter weight value: ");
            double val = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter source unit (KG/GRAM/POUND): ");
            WeightUnit sourceUnit = ParseWeightUnit(Console.ReadLine());
            Console.Write("Enter target unit (KG/GRAM/POUND): ");
            WeightUnit targetUnit = ParseWeightUnit(Console.ReadLine());

            var q      = new Quantity<WeightUnitMeasurable>(val, new WeightUnitMeasurable(sourceUnit));
            var result = q.ConvertTo(new WeightUnitMeasurable(targetUnit));
            Console.WriteLine($"\nResult: {q} -> {result}");
        }

        private void RunWeightAddition()
        {
            Console.Write("Enter first weight value: ");
            double val1 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (KG/GRAM/POUND): ");
            WeightUnit unit1 = ParseWeightUnit(Console.ReadLine());

            Console.Write("Enter second weight value: ");
            double val2 = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (KG/GRAM/POUND): ");
            WeightUnit unit2 = ParseWeightUnit(Console.ReadLine());

            var q1 = new Quantity<WeightUnitMeasurable>(val1, new WeightUnitMeasurable(unit1));
            var q2 = new Quantity<WeightUnitMeasurable>(val2, new WeightUnitMeasurable(unit2));

            var sumInFirstUnit = q1.Add(q2);
            Console.WriteLine($"\nSum in first unit ({unit1}): {q1} + {q2} = {sumInFirstUnit}");

            Console.Write("\nEnter target unit (KG/GRAM/POUND), or ENTER to skip: ");
            string? targetRaw = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(targetRaw))
            {
                WeightUnit resultUnit  = ParseWeightUnit(targetRaw);
                var        sumInTarget = q1.Add(q2, new WeightUnitMeasurable(resultUnit));
                Console.WriteLine($"Sum in target unit ({resultUnit}): {sumInTarget}");
            }
        }

        // ── Parsers ───────────────────────────────────────────────────────────

        private LengthUnit ParseLengthUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "FEET"        || text == "FOOT" || text == "FT") return LengthUnit.FEET;
            if (text == "INCHES"      || text == "INCH" || text == "IN") return LengthUnit.INCHES;
            if (text == "YARDS"       || text == "YARD" || text == "YD") return LengthUnit.YARDS;
            if (text == "CENTIMETERS" || text == "CENTIMETER" || text == "CM") return LengthUnit.CENTIMETERS;
            throw new ArgumentException($"Invalid unit '{raw}'. Use FEET, INCHES, YARDS, or CENTIMETERS.");
        }

        private WeightUnit ParseWeightUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "KG" || text == "KILOGRAM"  || text == "KILOGRAMS") return WeightUnit.KILOGRAM;
            if (text == "G"  || text == "GRAM"       || text == "GRAMS")     return WeightUnit.GRAM;
            if (text == "LB" || text == "LBS" || text == "POUND" || text == "POUNDS") return WeightUnit.POUND;
            throw new ArgumentException($"Invalid unit '{raw}'. Use KG, GRAM, or POUND.");
        }

        private VolumeUnit ParseVolumeUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "LITRE"      || text == "LITER"      || text == "L")   return VolumeUnit.LITRE;
            if (text == "MILLILITRE" || text == "MILLILITER" || text == "ML")  return VolumeUnit.MILLILITRE;
            if (text == "GALLON"     || text == "GALLONS"    || text == "GAL") return VolumeUnit.GALLON;
            throw new ArgumentException($"Invalid unit '{raw}'. Use LITRE, MILLILITRE, or GALLON.");
        }
    }
}