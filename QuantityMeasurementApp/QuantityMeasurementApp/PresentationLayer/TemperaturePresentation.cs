using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC14: Presentation layer for Temperature measurement (CELSIUS, FAHRENHEIT, KELVIN).
    /// Output format mirrors existing Weight/Volume/Length presentations:
    ///   Equality:   Result: {first} == {second} -> {bool}
    ///   Conversion: Result: {original} -> {converted}
    ///   Arithmetic: Attempting: {t1} + {t2}  =>  NotSupportedException: ...
    ///   Cross-cat:  Result: {temp} == {other} -> False (different categories)
    /// </summary>
    public class TemperaturePresentation
    {
        private readonly ITemperatureService _service;

        public TemperaturePresentation()
        {
            _service = new TemperatureService();
        }

        /// <summary>ASP.NET-ready constructor — accepts service via dependency injection.</summary>
        public TemperaturePresentation(ITemperatureService service)
        {
            _service = service;
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC14: Temperature Measurement (Celsius, Fahrenheit, Kelvin)\n");
                Console.WriteLine("1. Equality Comparison");
                Console.WriteLine("2. Unit Conversion");
                Console.WriteLine("3. Unsupported Arithmetic (Add / Subtract / Divide)");
                Console.WriteLine("4. Cross-Category Prevention");
                Console.Write("\nEnter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1: RunEquality();              break;
                    case 2: RunConversion();            break;
                    case 3: RunUnsupportedArithmetic(); break;
                    case 4: RunCrossCategory();         break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
            catch (FormatException)          { Console.WriteLine("Invalid input. Please enter a numeric value."); }
            catch (ArgumentException ex)     { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (NotSupportedException ex) { Console.WriteLine("Unsupported Operation: " + ex.Message); }
            catch (Exception ex)             { Console.WriteLine("Error: " + ex.Message); }
        }

        // ── Option 1: Equality ────────────────────────────────────────────────

        private void RunEquality()
        {
            Console.WriteLine("\nEquality Comparison");

            Console.Write("Enter first temperature value: ");
            double firstValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (CELSIUS/FAHRENHEIT/KELVIN): ");
            TemperatureUnit firstUnit = ParseTempUnit(Console.ReadLine());

            Console.Write("Enter second temperature value: ");
            double secondValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (CELSIUS/FAHRENHEIT/KELVIN): ");
            TemperatureUnit secondUnit = ParseTempUnit(Console.ReadLine());

            var first  = new Quantity<TemperatureUnitMeasurable>(firstValue,  new TemperatureUnitMeasurable(firstUnit));
            var second = new Quantity<TemperatureUnitMeasurable>(secondValue, new TemperatureUnitMeasurable(secondUnit));

            Console.WriteLine($"\nResult: {first} == {second} -> {_service.AreEqual(first, second)}");
        }

        // ── Option 2: Conversion ──────────────────────────────────────────────

        private void RunConversion()
        {
            Console.WriteLine("\nUnit Conversion");

            Console.Write("Enter temperature value: ");
            double value = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter source unit (CELSIUS/FAHRENHEIT/KELVIN): ");
            TemperatureUnit sourceUnit = ParseTempUnit(Console.ReadLine());
            Console.Write("Enter target unit (CELSIUS/FAHRENHEIT/KELVIN): ");
            TemperatureUnit targetUnit = ParseTempUnit(Console.ReadLine());

            var original  = new Quantity<TemperatureUnitMeasurable>(value, new TemperatureUnitMeasurable(sourceUnit));
            var converted = _service.ConvertTo(original, new TemperatureUnitMeasurable(targetUnit));

            Console.WriteLine($"\nResult: {original} -> {converted}");
        }

        // ── Option 3: Unsupported Arithmetic ─────────────────────────────────

        private void RunUnsupportedArithmetic()
        {
            Console.WriteLine("\nUnsupported Arithmetic Operations on Temperature");
            Console.WriteLine("Enter two temperature values — Add, Subtract, and Divide will all be attempted.\n");

            Console.Write("Enter first temperature value: ");
            double firstValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (CELSIUS/FAHRENHEIT/KELVIN): ");
            TemperatureUnit firstUnit = ParseTempUnit(Console.ReadLine());

            Console.Write("Enter second temperature value: ");
            double secondValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (CELSIUS/FAHRENHEIT/KELVIN): ");
            TemperatureUnit secondUnit = ParseTempUnit(Console.ReadLine());

            var t1 = new Quantity<TemperatureUnitMeasurable>(firstValue,  new TemperatureUnitMeasurable(firstUnit));
            var t2 = new Quantity<TemperatureUnitMeasurable>(secondValue, new TemperatureUnitMeasurable(secondUnit));

            Console.WriteLine();

            // Add
            Console.Write($"Attempting: {t1} + {t2}  =>  ");
            try   { t1.Add(t2); Console.WriteLine("(no exception — unexpected)"); }
            catch (NotSupportedException ex) { Console.WriteLine($"NotSupportedException: {ex.Message}"); }

            // Subtract
            Console.Write($"Attempting: {t1} - {t2}  =>  ");
            try   { t1.Subtract(t2); Console.WriteLine("(no exception — unexpected)"); }
            catch (NotSupportedException ex) { Console.WriteLine($"NotSupportedException: {ex.Message}"); }

            // Divide
            Console.Write($"Attempting: {t1} ÷ {t2}  =>  ");
            try   { t1.Divide(t2); Console.WriteLine("(no exception — unexpected)"); }
            catch (NotSupportedException ex) { Console.WriteLine($"NotSupportedException: {ex.Message}"); }
        }

        // ── Option 4: Cross-Category Prevention ──────────────────────────────

        private void RunCrossCategory()
        {
            Console.WriteLine("\nCross-Category Prevention");
            Console.WriteLine("Compare a temperature quantity against a quantity from another category.\n");

            Console.Write("Enter temperature value: ");
            double tempValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter temperature unit (CELSIUS/FAHRENHEIT/KELVIN): ");
            TemperatureUnit tempUnit = ParseTempUnit(Console.ReadLine());

            Console.Write("Enter the other quantity value: ");
            double otherValue = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("\nWhich category do you want to compare against?");
            Console.WriteLine("1. Length  (FEET/INCHES/YARDS/CENTIMETERS)");
            Console.WriteLine("2. Weight  (KILOGRAM/GRAM/POUND)");
            Console.WriteLine("3. Volume  (LITRE/MILLILITRE/GALLON)");
            Console.Write("Enter choice: ");
            int categoryChoice = Convert.ToInt32(Console.ReadLine());

            var tempQty = new Quantity<TemperatureUnitMeasurable>(tempValue, new TemperatureUnitMeasurable(tempUnit));
            bool result;
            string otherDisplay;

            switch (categoryChoice)
            {
                case 1:
                    Console.Write("Enter length unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                    LengthUnit lengthUnit = ParseLengthUnit(Console.ReadLine());
                    var lengthQty = new Quantity<LengthUnitMeasurable>(otherValue, new LengthUnitMeasurable(lengthUnit));
                    result       = tempQty.Equals(lengthQty);
                    otherDisplay = lengthQty.ToString();
                    break;

                case 2:
                    Console.Write("Enter weight unit (KILOGRAM/GRAM/POUND): ");
                    WeightUnit weightUnit = ParseWeightUnit(Console.ReadLine());
                    var weightQty = new Quantity<WeightUnitMeasurable>(otherValue, new WeightUnitMeasurable(weightUnit));
                    result       = tempQty.Equals(weightQty);
                    otherDisplay = weightQty.ToString();
                    break;

                case 3:
                    Console.Write("Enter volume unit (LITRE/MILLILITRE/GALLON): ");
                    VolumeUnit volumeUnit = ParseVolumeUnit(Console.ReadLine());
                    var volumeQty = new Quantity<VolumeUnitMeasurable>(otherValue, new VolumeUnitMeasurable(volumeUnit));
                    result       = tempQty.Equals(volumeQty);
                    otherDisplay = volumeQty.ToString();
                    break;

                default:
                    Console.WriteLine("Invalid category choice.");
                    return;
            }

            Console.WriteLine($"\nResult: {tempQty} == {otherDisplay} -> {result}");
        }

        // ── Unit parsers ──────────────────────────────────────────────────────

        private TemperatureUnit ParseTempUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "CELSIUS"    || text == "C") return TemperatureUnit.CELSIUS;
            if (text == "FAHRENHEIT" || text == "F") return TemperatureUnit.FAHRENHEIT;
            if (text == "KELVIN"     || text == "K") return TemperatureUnit.KELVIN;
            throw new ArgumentException($"Invalid unit '{raw}'. Use CELSIUS, FAHRENHEIT, or KELVIN.");
        }

        private LengthUnit ParseLengthUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "FEET"        || text == "FT") return LengthUnit.FEET;
            if (text == "INCHES"      || text == "IN") return LengthUnit.INCHES;
            if (text == "YARDS"       || text == "YD") return LengthUnit.YARDS;
            if (text == "CENTIMETERS" || text == "CM") return LengthUnit.CENTIMETERS;
            throw new ArgumentException($"Invalid unit '{raw}'. Use FEET, INCHES, YARDS, or CENTIMETERS.");
        }

        private WeightUnit ParseWeightUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "KILOGRAM" || text == "KG") return WeightUnit.KILOGRAM;
            if (text == "GRAM"     || text == "G")  return WeightUnit.GRAM;
            if (text == "POUND"    || text == "LB") return WeightUnit.POUND;
            throw new ArgumentException($"Invalid unit '{raw}'. Use KILOGRAM, GRAM, or POUND.");
        }

        private VolumeUnit ParseVolumeUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "LITRE"      || text == "L")   return VolumeUnit.LITRE;
            if (text == "MILLILITRE" || text == "ML")  return VolumeUnit.MILLILITRE;
            if (text == "GALLON"     || text == "GAL") return VolumeUnit.GALLON;
            throw new ArgumentException($"Invalid unit '{raw}'. Use LITRE, MILLILITRE, or GALLON.");
        }
    }
}