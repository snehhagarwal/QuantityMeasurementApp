using System;
using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC9: Presentation layer for weight measurements (KILOGRAM, GRAM, POUND).
    /// Allows the user to enter two weights and see equality and conversion results.
    /// </summary>
    public class WeightPresentationUC9
    {
        public void Run()
        {
            try
            {
                Console.WriteLine("\nWeight Measurement (Kilogram, Gram, Pound)\n");
                Console.WriteLine("1. Equality Comparison");
                Console.WriteLine("2. Unit Conversion");
                Console.WriteLine("3. Addition");
                Console.Write("\nEnter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        RunEquality();
                        break;
                    case 2:
                        RunConversion();
                        break;
                    case 3:
                        RunAddition();
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Please enter a valid numeric value.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Validation Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // Main Flow: Equality Comparison
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

            Weight first = new Weight(firstValue, firstUnit);
            Weight second = new Weight(secondValue, secondUnit);

            bool equal = first.Equals(second);
            Console.WriteLine($"\nResult: {first} == {second} -> {equal}");
        }

        // Main Flow: Unit Conversion
        private void RunConversion()
        {
            Console.WriteLine("\nUnit Conversion");

            Console.Write("Enter weight value: ");
            double value = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter source unit (KG/GRAM/POUND): ");
            WeightUnit sourceUnit = ParseUnit(Console.ReadLine());

            Console.Write("Enter target unit (KG/GRAM/POUND): ");
            WeightUnit targetUnit = ParseUnit(Console.ReadLine());

            Weight original = new Weight(value, sourceUnit);
            Weight converted = original.ConvertTo(targetUnit);

            Console.WriteLine($"\nResult: {original} -> {converted}");
        }

        // Main Flow: Addition Operations
        private void RunAddition()
        {
            Console.WriteLine("\nAddition");

            // First weight
            Console.Write("Enter first weight value: ");
            double firstValue = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter first unit (KG/GRAM/POUND): ");
            WeightUnit firstUnit = ParseUnit(Console.ReadLine());

            // Second weight
            Console.Write("Enter second weight value: ");
            double secondValue = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter second unit (KG/GRAM/POUND): ");
            WeightUnit secondUnit = ParseUnit(Console.ReadLine());

            Weight first = new Weight(firstValue, firstUnit);
            Weight second = new Weight(secondValue, secondUnit);

            // Sum in first operand's unit
            Weight sumInFirstUnit = first.Add(second);
            Console.WriteLine($"\nSum in first unit ({firstUnit}): {first} + {second} = {sumInFirstUnit}");

            // Optional explicit target unit
            Console.Write("\nEnter target unit for sum (KG/GRAM/POUND), or press ENTER to skip: ");
            string? targetRaw = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(targetRaw))
            {
                WeightUnit targetUnit = ParseUnit(targetRaw);
                Weight sumInTarget = first.Add(second, targetUnit);
                Console.WriteLine($"Sum in target unit ({targetUnit}): {sumInTarget}");
            }
        }

        private WeightUnit ParseUnit(string? raw)
        {
            if (raw == null)
                throw new ArgumentException("Unit cannot be empty");

            string text = raw.Trim().ToUpper();

            if (text == "KG" || text == "KILOGRAM" || text == "KILOGRAMS")
                return WeightUnit.KILOGRAM;

            if (text == "G" || text == "GRAM" || text == "GRAMS")
                return WeightUnit.GRAM;

            if (text == "LB" || text == "LBS" || text == "POUND" || text == "POUNDS")
                return WeightUnit.POUND;

            throw new ArgumentException("Unit is not valid. Use KG, GRAM or POUND.");
        }
    }
}

