using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC3 Presentation Layer.
    /// Demonstrates Generic Quantity Length comparison.
    /// </summary>
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
                double value1 =Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter unit (FEET / INCHES): ");
                LengthUnit unit1 =(LengthUnit)Enum.Parse(typeof(LengthUnit),Console.ReadLine()!.ToUpper());
                Console.Write("Enter second value: ");
                double value2 = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter unit (FEET / INCHES): ");
                LengthUnit unit2 =(LengthUnit)Enum.Parse(typeof(LengthUnit),Console.ReadLine()!.ToUpper());
                Length first =new Length(value1, unit1);
                Length second = new Length(value2, unit2);
                // EXACT
                bool result =service.AreEqual(first, second);
                Console.WriteLine("Exact Equality: " + result);
                // TOLERANCE FEATURE
                Console.Write("Enter tolerance (in inches): ");
                double tolerance =Convert.ToDouble(Console.ReadLine());
                bool toleranceResult =service.AreEqualWithTolerance(first,second,tolerance);
                Console.WriteLine("Tolerance Equality: "+ toleranceResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}