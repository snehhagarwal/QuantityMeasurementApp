using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC4 Presentation Layer
    /// Supports Feet, Inches, Yards, Centimeters
    /// </summary>
    public class LengthPresentationUC4
    {
        private readonly LengthService service;

        public LengthPresentationUC4()
        {
            service = new LengthService();
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("\nUC4: Extended Unit Support");
                Console.Write("Enter first value: ");
                double value1 =
                Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter unit (FEET / INCHES / YARDS / CENTIMETERS): ");
                LengthUnit unit1 =(LengthUnit)Enum.Parse(typeof(LengthUnit),Console.ReadLine()!.ToUpper());
                Console.Write("Enter second value: ");
                double value2 =Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter unit (FEET / INCHES / YARDS / CENTIMETERS): ");
                LengthUnit unit2 =(LengthUnit)Enum.Parse(typeof(LengthUnit),Console.ReadLine()!.ToUpper());
                Length first =new Length(value1, unit1);
                Length second =new Length(value2, unit2);
                bool result =service.AreEqual(first, second);
                Console.WriteLine("Exact Equality: " + result);
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