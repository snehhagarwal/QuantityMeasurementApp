using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interface;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC11: Presentation layer for Volume measurement (LITRE, MILLILITRE, GALLON).
    /// Depends on IVolumeService — ready for ASP.NET Controller migration.
    ///
    /// Addition follows the two-step pattern:
    ///   Step 1 — shows sum in first operand's unit (implicit target).
    ///   Step 2 — optionally asks for explicit target unit and shows again.
    /// </summary>
    public class VolumePresentationUC11
    {
        private readonly IVolumeService _service;

        public VolumePresentationUC11()
        {
            _service = new VolumeService();
        }

        /// <summary>ASP.NET-ready constructor — accepts service via dependency injection.</summary>
        public VolumePresentationUC11(IVolumeService service)
        {
            _service = service;
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC11: Volume Measurement (Litre, Millilitre, Gallon)\n");
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

            Console.Write("Enter first volume value: ");
            double firstValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (LITRE/MILLILITRE/GALLON): ");
            VolumeUnit firstUnit = ParseUnit(Console.ReadLine());

            Console.Write("Enter second volume value: ");
            double secondValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (LITRE/MILLILITRE/GALLON): ");
            VolumeUnit secondUnit = ParseUnit(Console.ReadLine());

            var first  = new Quantity<VolumeUnitMeasurable>(firstValue,  new VolumeUnitMeasurable(firstUnit));
            var second = new Quantity<VolumeUnitMeasurable>(secondValue, new VolumeUnitMeasurable(secondUnit));

            Console.WriteLine($"\nResult: {first} == {second} -> {_service.AreEqual(first, second)}");
        }

        private void RunConversion()
        {
            Console.WriteLine("\nUnit Conversion");

            Console.Write("Enter volume value: ");
            double value = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter source unit (LITRE/MILLILITRE/GALLON): ");
            VolumeUnit sourceUnit = ParseUnit(Console.ReadLine());
            Console.Write("Enter target unit (LITRE/MILLILITRE/GALLON): ");
            VolumeUnit targetUnit = ParseUnit(Console.ReadLine());

            var original  = new Quantity<VolumeUnitMeasurable>(value, new VolumeUnitMeasurable(sourceUnit));
            var converted = _service.ConvertTo(original, new VolumeUnitMeasurable(targetUnit));

            Console.WriteLine($"\nResult: {original} -> {converted}");
        }

        private void RunAddition()
        {
            Console.WriteLine("\nAddition");

            Console.Write("Enter first volume value: ");
            double firstValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter first unit (LITRE/MILLILITRE/GALLON): ");
            VolumeUnit firstUnit = ParseUnit(Console.ReadLine());

            Console.Write("Enter second volume value: ");
            double secondValue = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter second unit (LITRE/MILLILITRE/GALLON): ");
            VolumeUnit secondUnit = ParseUnit(Console.ReadLine());

            var first  = new Quantity<VolumeUnitMeasurable>(firstValue,  new VolumeUnitMeasurable(firstUnit));
            var second = new Quantity<VolumeUnitMeasurable>(secondValue, new VolumeUnitMeasurable(secondUnit));

            // Step 1 — implicit target: sum in first operand's unit
            var sumInFirstUnit = _service.Add(first, second);
            Console.WriteLine($"\nSum in first unit ({firstUnit}): {first} + {second} = {sumInFirstUnit}");

            // Step 2 — optional explicit target unit
            Console.Write("\nEnter target unit (LITRE/MILLILITRE/GALLON), or press ENTER to skip: ");
            string? targetRaw = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(targetRaw))
            {
                VolumeUnit targetUnit  = ParseUnit(targetRaw);
                var        sumInTarget = _service.Add(first, second, new VolumeUnitMeasurable(targetUnit));
                Console.WriteLine($"Sum in target unit ({targetUnit}): {sumInTarget}");
            }
        }

        private VolumeUnit ParseUnit(string? raw)
        {
            string text = raw?.Trim().ToUpper() ?? "";
            if (text == "LITRE"      || text == "LITER"      || text == "L")   return VolumeUnit.LITRE;
            if (text == "MILLILITRE" || text == "MILLILITER" || text == "ML")  return VolumeUnit.MILLILITRE;
            if (text == "GALLON"     || text == "GALLONS"    || text == "GAL") return VolumeUnit.GALLON;
            throw new ArgumentException($"Invalid unit '{raw}'. Use LITRE, MILLILITRE, or GALLON.");
        }
    }
}