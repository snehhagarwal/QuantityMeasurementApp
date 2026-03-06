using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC9: Presentation layer for Weight measurement (KILOGRAM, GRAM, POUND).
    /// Depends on IWeightService — ready for ASP.NET Controller migration.
    ///
    /// Addition follows the two-step pattern:
    ///   Step 1 — shows sum in first operand's unit (implicit target).
    ///   Step 2 — optionally asks for explicit target unit and shows again.
    /// </summary>
    public class WeightPresentationUC9
    {
        private readonly IWeightService _service;

        public WeightPresentationUC9()
        {
            _service = new WeightService();
        }

        /// <summary>ASP.NET-ready constructor — accepts service via dependency injection.</summary>
        public WeightPresentationUC9(IWeightService service)
        {
            _service = service;
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC9: Weight Measurement (Kilogram, Gram, Pound)\n");
                Console.WriteLine("1. Equality Comparison");
                Console.WriteLine("2. Unit Conversion");
                Console.WriteLine("3. Addition");
                Console.Write("\nEnter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1: RunEquality();   break;
                    case 2: RunConversion(); break;
                    case 3: RunAddition();   break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
            catch (FormatException)      { Console.WriteLine("Invalid input. Please enter a numeric value."); }
            catch (ArgumentException ex) { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("Error: " + ex.Message); }
        }

        private void RunEquality()
        {
            Console.WriteLine("\nEquality Comparison");

            Console.Write("Enter first weight value: ");
            double firstValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (KG/GRAM/POUND): ");
            WeightUnit firstUnit = ParseUnit(Console.ReadLine());

            Console.Write("Enter second weight value: ");
            double secondValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (KG/GRAM/POUND): ");
            WeightUnit secondUnit = ParseUnit(Console.ReadLine());

            Weight first  = new Weight(firstValue,  firstUnit);
            Weight second = new Weight(secondValue, secondUnit);

            Console.WriteLine($"\nResult: {first} == {second} -> {_service.AreEqual(first, second)}");
        }

        private void RunConversion()
        {
            Console.WriteLine("\nUnit Conversion");

            Console.Write("Enter weight value: ");
            double value = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter source unit (KG/GRAM/POUND): ");
            WeightUnit sourceUnit = ParseUnit(Console.ReadLine());
            Console.Write("Enter target unit (KG/GRAM/POUND): ");
            WeightUnit targetUnit = ParseUnit(Console.ReadLine());

            Weight original  = new Weight(value, sourceUnit);
            Weight converted = _service.ConvertTo(original, targetUnit);

            Console.WriteLine($"\nResult: {original} -> {converted}");
        }

        private void RunAddition()
        {
            Console.WriteLine("\nAddition");

            Console.Write("Enter first weight value: ");
            double firstValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (KG/GRAM/POUND): ");
            WeightUnit firstUnit = ParseUnit(Console.ReadLine());

            Console.Write("Enter second weight value: ");
            double secondValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (KG/GRAM/POUND): ");
            WeightUnit secondUnit = ParseUnit(Console.ReadLine());

            Weight first  = new Weight(firstValue,  firstUnit);
            Weight second = new Weight(secondValue, secondUnit);

            // Step 1 — implicit target: sum in first operand's unit
            Weight sumInFirstUnit = _service.Add(first, second);
            Console.WriteLine($"\nSum in first unit ({firstUnit}): {first} + {second} = {sumInFirstUnit}");

            // Step 2 — optional explicit target unit
            Console.Write("\nEnter target unit (KG/GRAM/POUND), or press ENTER to skip: ");
            string? targetRaw = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(targetRaw))
            {
                WeightUnit targetUnit  = ParseUnit(targetRaw);
                Weight     sumInTarget = _service.Add(first, second, targetUnit);
                Console.WriteLine($"Sum in target unit ({targetUnit}): {sumInTarget}");
            }
        }

        private WeightUnit ParseUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "KG"    || text == "KILOGRAM"  || text == "KILOGRAMS") return WeightUnit.KILOGRAM;
            if (text == "G"     || text == "GRAM"      || text == "GRAMS")     return WeightUnit.GRAM;
            if (text == "LB"    || text == "LBS"       || text == "POUND" || text == "POUNDS") return WeightUnit.POUND;
            throw new ArgumentException($"Invalid unit '{raw}'. Use KG, GRAM, or POUND.");
        }
    }
}