using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.BusinessLogicLayer;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// UC2: Presentation layer for Feet and Inches equality.
    /// Depends on IFeetService and IInchesService — not concrete classes — so it is
    /// ready for ASP.NET Controller migration without any logic changes.
    /// If Feet input is invalid the flow continues to Inches.
    /// </summary>
    public class InchesPresentation
    {
        private readonly IFeetService   _feetService;
        private readonly IInchesService _inchesService;

        public InchesPresentation()
        {
            _feetService   = new FeetService();
            _inchesService = new InchesService();
        }

        /// <summary>ASP.NET-ready constructor — accepts services via dependency injection.</summary>
        public InchesPresentation(IFeetService feetService, IInchesService inchesService)
        {
            _feetService   = feetService;
            _inchesService = inchesService;
        }

        public void Run()
        {
            Console.WriteLine("\nUC2: Feet and Inches Equality");

            // ---- Feet section ----
            try
            {
                Console.WriteLine("\n---- Feet Equality ----");

                Console.Write("Enter first feet value: ");
                double firstFeetValue  = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter second feet value: ");
                double secondFeetValue = Convert.ToDouble(Console.ReadLine());

                Feet firstFeet  = new Feet(firstFeetValue);
                Feet secondFeet = new Feet(secondFeetValue);

                Console.WriteLine("Exact Equality: " + _feetService.AreEqual(firstFeet, secondFeet));

                Console.Write("Enter tolerance: ");
                double tolerance = Convert.ToDouble(Console.ReadLine());

                Console.WriteLine("Tolerance Equality: " +
                    _feetService.AreEqualWithTolerance(firstFeet, secondFeet, tolerance));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Feet invalid: " + ex.Message);
                Console.WriteLine("Continuing with Inches...");
            }

            // ---- Inches section ----
            try
            {
                Console.WriteLine("\n---- Inches Equality ----");

                Console.Write("Enter first inches value: ");
                double firstInchesValue  = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter second inches value: ");
                double secondInchesValue = Convert.ToDouble(Console.ReadLine());

                Inches firstInches  = new Inches(firstInchesValue);
                Inches secondInches = new Inches(secondInchesValue);

                Console.WriteLine("Exact Equality: " + _inchesService.AreEqual(firstInches, secondInches));

                Console.Write("Enter tolerance: ");
                double tolerance = Convert.ToDouble(Console.ReadLine());

                Console.WriteLine("Tolerance Equality: " +
                    _inchesService.AreEqualWithTolerance(firstInches, secondInches, tolerance));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inches invalid: " + ex.Message);
            }
        }
    }
}