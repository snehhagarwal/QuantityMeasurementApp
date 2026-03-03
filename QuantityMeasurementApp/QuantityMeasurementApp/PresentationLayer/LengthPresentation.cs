using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    public class LengthPresentation
    {
        private readonly LengthService service;

        public LengthPresentation()
        {
            service = new LengthService();
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC3: Generic Length Equality");

                Console.Write("Enter first value: ");
                double value1 = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter unit (FEET / INCHES): ");
                LengthUnit unit1 = ReadUnitUC3();

                Console.Write("Enter second value: ");
                double value2 = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter unit (FEET / INCHES): ");
                LengthUnit unit2 = ReadUnitUC3();

                Length first = new Length(value1, unit1);
                Length second = new Length(value2, unit2);

                bool result = service.AreEqual(first, second);

                Console.WriteLine("Exact Equality: " + result);

                Console.Write("Enter tolerance (in inches): ");

                double tolerance =
                Convert.ToDouble(Console.ReadLine());

                bool toleranceResult =service.AreEqualWithTolerance(first,second,tolerance);

                Console.WriteLine("Tolerance Equality: " + toleranceResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private LengthUnit ReadUnitUC3()
        {
            while (true)
            {
                string input =
                Console.ReadLine()!.ToUpper();

                if (input == "FEET")
                    return LengthUnit.FEET;

                if (input == "INCHES")
                    return LengthUnit.INCHES;

                Console.Write("Invalid. Enter only FEET / INCHES: ");
            }
        }
    }
}