using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC10: Generic presentation class.
    /// Takes user input and demonstrates Quantity<TUnit> for both Length and Weight.
    /// Error handling matches the pattern established in UC7 and UC9.
    /// </summary>
    public class QuantityPresentation
    {
        public void Run()
        {
            try
            {
                Console.WriteLine("\nGeneric Quantity Measurement \n");
                Console.WriteLine("1. Length Operations");
                Console.WriteLine("2. Weight Operations");
                Console.WriteLine("3. Cross-Category Prevention");
                Console.WriteLine("4. Generic Demonstration Methods");
                Console.Write("\nEnter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        RunLengthOperations();
                        break;
                    case 2:
                        RunWeightOperations();
                        break;
                    case 3:
                        RunCrossCategoryCheck();
                        break;
                    case 4:
                        RunGenericDemonstration();
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

        // ---- Cross-Category Prevention ----

        /// <summary>
        /// Takes a Length and a Weight quantity from user input and calls equals().
        /// Always returns false — proves runtime cross-category safety.
        /// </summary>
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
       
               bool result = lq.Equals(wq);
               Console.WriteLine($"\nResult: {lq} == {wq} -> {result}");
            }
            catch (FormatException) { Console.WriteLine("Error: Please enter a valid numeric value."); }
            catch (ArgumentException ex) { Console.WriteLine("Validation Error: " + ex.Message); }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }

    // ---- Generic Demonstration Methods ----
    
    /// <summary>
    /// Lets user pick Length or Weight, then runs equality/conversion/addition
    /// through a single generic method — proving unified handling across categories.
    /// </summary>
    private void RunGenericDemonstration()
    {
        try
        {
            Console.WriteLine("\nGeneric Demonstration Methods\n");
            Console.WriteLine("1. Equality");
            Console.WriteLine("2. Conversion");
            Console.WriteLine("3. Addition");
            Console.Write("\nEnter choice: ");
            int op = Convert.ToInt32(Console.ReadLine());
    
            Console.WriteLine("\n1. Length   2. Weight");
            Console.Write("Category: ");
            int cat = Convert.ToInt32(Console.ReadLine());
    
            if (cat == 1)
            {
                Console.Write("First value: ");  double v1 = Convert.ToDouble(Console.ReadLine());
                Console.Write("First unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                var u1 = new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine()));
                var q1 = new Quantity<LengthUnitMeasurable>(v1, u1);
    
                if (op == 1)
                {
                    Console.Write("Second value: "); double v2 = Convert.ToDouble(Console.ReadLine());
                    Console.Write("Second unit(FEET/INCHES/YARDS/CENTIMETERS): ");
                    var q2 = new Quantity<LengthUnitMeasurable>(v2, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));
                    Console.WriteLine($"\nResult: {q1} == {q2} -> {DemonstrateEquality(q1, q2)}");
                }
                else if (op == 2)
                {
                    Console.Write("Target unit: ");
                    var result = DemonstrateConversion(q1, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));
                    Console.WriteLine($"\nResult: {q1} -> {result}");
                }
                else if (op == 3)
                {
                    Console.Write("Second value: "); double v2 = Convert.ToDouble(Console.ReadLine());
                    Console.Write("Second unit: ");
                    var q2 = new Quantity<LengthUnitMeasurable>(v2, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));
                    Console.Write("Result unit: ");
                    var sum = DemonstrateAddition(q1, q2, new LengthUnitMeasurable(ParseLengthUnit(Console.ReadLine())));
                    Console.WriteLine($"\nResult: {q1} + {q2} = {sum}");
                }
            }
            else if (cat == 2)
            {
                Console.Write("First value: "); double v1 = Convert.ToDouble(Console.ReadLine());
                Console.Write("First unit (KG/GRAM/POUND): ");
                var u1 = new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine()));
                var q1 = new Quantity<WeightUnitMeasurable>(v1, u1);
    
                if (op == 1)
                {
                    Console.Write("Second value: "); double v2 = Convert.ToDouble(Console.ReadLine());
                    Console.Write("Second unit (KG/GRAM/POUND): ");
                    var q2 = new Quantity<WeightUnitMeasurable>(v2, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));
                    Console.WriteLine($"\nResult: {q1} == {q2} -> {DemonstrateEquality(q1, q2)}");
                }
                else if (op == 2)
                {
                    Console.Write("Target unit: ");
                    var result = DemonstrateConversion(q1, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));
                    Console.WriteLine($"\nResult: {q1} -> {result}");
                }
                else if (op == 3)
                {
                    Console.Write("Second value: "); double v2 = Convert.ToDouble(Console.ReadLine());
                    Console.Write("Second unit: ");
                    var q2 = new Quantity<WeightUnitMeasurable>(v2, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));
                    Console.Write("Result unit: ");
                    var sum = DemonstrateAddition(q1, q2, new WeightUnitMeasurable(ParseWeightUnit(Console.ReadLine())));
                    Console.WriteLine($"\nResult: {q1} + {q2} = {sum}");
                }
            }
        }
        catch (FormatException) { Console.WriteLine("Error: Please enter a valid numeric value."); }
        catch (ArgumentException ex) { Console.WriteLine("Validation Error: " + ex.Message); }
        catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
    }

    /// <summary>Single generic equality — replaces demonstrateLengthEquality() + demonstrateWeightEquality().</summary>
    private bool DemonstrateEquality<TUnit>(Quantity<TUnit> q1, Quantity<TUnit> q2)
    where TUnit : IMeasurable => q1.Equals(q2);

    /// <summary>Single generic conversion — handles ALL measurement categories.</summary>
    private Quantity<TUnit> DemonstrateConversion<TUnit>(Quantity<TUnit> q, TUnit target)
    where TUnit : IMeasurable => q.ConvertTo(target);

    /// <summary>Single generic addition — handles ALL measurement categories.</summary>
    private Quantity<TUnit> DemonstrateAddition<TUnit>(Quantity<TUnit> q1, Quantity<TUnit> q2, TUnit target)
    where TUnit : IMeasurable => q1.Add(q2, target);
    
        // ---- Length ----

        private void RunLengthOperations()
        {
            try
            {
                Console.WriteLine("\nLength Operations\n");
                Console.WriteLine("1. Equality");
                Console.WriteLine("2. Conversion");
                Console.WriteLine("3. Addition");
                Console.Write("\nEnter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1: RunLengthEquality();   break;
                    case 2: RunLengthConversion(); break;
                    case 3: RunLengthAddition();   break;
                    default: Console.WriteLine("Invalid choice"); break;
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

        private void RunLengthEquality()
        {
            try
            {
                Console.WriteLine("\nEquality Comparison");

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

        private void RunLengthConversion()
        {
            try
            {
                Console.WriteLine("\nUnit Conversion");

                Console.Write("Enter length value: ");
                double val = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter source unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit sourceUnit = ParseLengthUnit(Console.ReadLine());
                Console.Write("Enter target unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit targetUnit = ParseLengthUnit(Console.ReadLine());

                var q = new Quantity<LengthUnitMeasurable>(val, new LengthUnitMeasurable(sourceUnit));
                var result = q.ConvertTo(new LengthUnitMeasurable(targetUnit));

                Console.WriteLine($"\nResult: {q} -> {result}");
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

        private void RunLengthAddition()
        {
            try
            {
                Console.WriteLine("\nAddition");

                Console.Write("Enter first length value: ");
                double val1 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter first unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit unit1 = ParseLengthUnit(Console.ReadLine());

                Console.Write("Enter second length value: ");
                double val2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter second unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit unit2 = ParseLengthUnit(Console.ReadLine());

                Console.Write("Enter result unit (FEET/INCHES/YARDS/CENTIMETERS): ");
                LengthUnit resultUnit = ParseLengthUnit(Console.ReadLine());

                var q1 = new Quantity<LengthUnitMeasurable>(val1, new LengthUnitMeasurable(unit1));
                var q2 = new Quantity<LengthUnitMeasurable>(val2, new LengthUnitMeasurable(unit2));
                var sum = q1.Add(q2, new LengthUnitMeasurable(resultUnit));

                Console.WriteLine($"\nResult: {q1} + {q2} = {sum}");
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

        // ---- Weight ----

        private void RunWeightOperations()
        {
            try
            {
                Console.WriteLine("\nWeight Operations\n");
                Console.WriteLine("1. Equality");
                Console.WriteLine("2. Conversion");
                Console.WriteLine("3. Addition");
                Console.Write("\nEnter choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1: RunWeightEquality();   break;
                    case 2: RunWeightConversion(); break;
                    case 3: RunWeightAddition();   break;
                    default: Console.WriteLine("Invalid choice"); break;
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

        private void RunWeightEquality()
        {
            try
            {
                Console.WriteLine("\nEquality Comparison");

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

        private void RunWeightConversion()
        {
            try
            {
                Console.WriteLine("\nUnit Conversion");

                Console.Write("Enter weight value: ");
                double val = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter source unit (KG/GRAM/POUND): ");
                WeightUnit sourceUnit = ParseWeightUnit(Console.ReadLine());
                Console.Write("Enter target unit (KG/GRAM/POUND): ");
                WeightUnit targetUnit = ParseWeightUnit(Console.ReadLine());

                var q = new Quantity<WeightUnitMeasurable>(val, new WeightUnitMeasurable(sourceUnit));
                var result = q.ConvertTo(new WeightUnitMeasurable(targetUnit));

                Console.WriteLine($"\nResult: {q} -> {result}");
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

        private void RunWeightAddition()
        {
            try
            {
                Console.WriteLine("\nAddition");

                Console.Write("Enter first weight value: ");
                double val1 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter first unit (KG/GRAM/POUND): ");
                WeightUnit unit1 = ParseWeightUnit(Console.ReadLine());

                Console.Write("Enter second weight value: ");
                double val2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter second unit (KG/GRAM/POUND): ");
                WeightUnit unit2 = ParseWeightUnit(Console.ReadLine());

                Console.Write("Enter result unit (KG/GRAM/POUND): ");
                WeightUnit resultUnit = ParseWeightUnit(Console.ReadLine());

                var q1 = new Quantity<WeightUnitMeasurable>(val1, new WeightUnitMeasurable(unit1));
                var q2 = new Quantity<WeightUnitMeasurable>(val2, new WeightUnitMeasurable(unit2));
                var sum = q1.Add(q2, new WeightUnitMeasurable(resultUnit));

                Console.WriteLine($"\nResult: {q1} + {q2} = {sum}");
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

        // ---- Parsers ----

        private LengthUnit ParseLengthUnit(string? raw)
        {
            if (raw == null)
                throw new ArgumentException("Unit cannot be empty");

            string text = raw.Trim().ToUpper();

            if (text == "FEET" || text == "FOOT" || text == "FT")
                return LengthUnit.FEET;

            if (text == "INCHES" || text == "INCH" || text == "IN")
                return LengthUnit.INCHES;

            if (text == "YARDS" || text == "YARD" || text == "YD")
                return LengthUnit.YARDS;

            if (text == "CENTIMETERS" || text == "CENTIMETER" || text == "CM")
                return LengthUnit.CENTIMETERS;

            throw new ArgumentException("Unit is not valid. Use FEET, INCHES, YARDS or CENTIMETERS.");
        }

        private WeightUnit ParseWeightUnit(string? raw)
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