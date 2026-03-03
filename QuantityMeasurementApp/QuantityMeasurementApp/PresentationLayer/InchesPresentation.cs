using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC2 Presentation Layer.
    /// Compares Feet and Inches including tolerance.
    /// If Feet invalid, still continues with Inches.
    /// </summary>
    public class InchesPresentation
    {
        private readonly FeetService feetService;
        private readonly InchesService inchesService;

        public InchesPresentation()
        {
            feetService = new FeetService();
            inchesService = new InchesService();
        }

        public void Run()
        {
            Console.WriteLine("\nUC2: Feet and Inches Equality");
            //FEET
            try
            {
                Console.WriteLine("\n---- Feet Equality ----");
                Console.Write("Enter first feet value: ");
                double firstFeetValue =Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter second feet value: ");
                double secondFeetValue =Convert.ToDouble(Console.ReadLine());
                Feet firstFeet =new Feet(firstFeetValue);
                Feet secondFeet =new Feet(secondFeetValue);
                bool result =feetService.AreEqual(firstFeet,secondFeet);
                Console.WriteLine("Exact Equality: " + result);
                // TOLERANCE FEATURE
                Console.Write("Enter tolerance: ");
                double tolerance =Convert.ToDouble(Console.ReadLine());
                bool toleranceResult =feetService.AreEqualWithTolerance(firstFeet,secondFeet,tolerance);
                Console.WriteLine("Tolerance Equality: " + toleranceResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Feet invalid: "+ ex.Message);
                Console.WriteLine("Now continue with Inches...");
            }
            //INCHES 
            try
            {
                Console.WriteLine("\n---- Inches Equality ----");
                Console.Write("Enter first inches value: ");
                double firstInchesValue =Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter second inches value: ");
                double secondInchesValue =Convert.ToDouble(Console.ReadLine());
                Inches firstInches =new Inches(firstInchesValue);
                Inches secondInches =new Inches(secondInchesValue);
                bool result =inchesService.AreEqual(firstInches,secondInches);
                Console.WriteLine("Exact Equality: " + result);
                // TOLERANCE FEATURE
                Console.Write("Enter tolerance: ");
                double tolerance =Convert.ToDouble(Console.ReadLine());
                bool toleranceResult =inchesService.AreEqualWithTolerance(firstInches,secondInches,tolerance);
                Console.WriteLine("Tolerance Equality: "+ toleranceResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inches invalid: "+ ex.Message);
            }
        }
    }
}