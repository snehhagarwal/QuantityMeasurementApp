using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementApp.Interface;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Service;
using QuantityMeasurementBusinessLayer.Exception;
using QuantityMeasurementRepository.Repository;

namespace QuantityMeasurementApp.Controller
{
    /// <summary>
    /// UC15: Controller layer — entry point for the QuantityMeasurementApp.
    ///
    /// Responsibilities:
    ///   - Handles user interaction via console.
    ///   - Accepts input as QuantityDTO objects.
    ///   - Delegates ALL business logic to IQuantityMeasurementService (no logic here).
    ///   - Formats and presents results to the user.
    ///   - Handles and displays QuantityMeasurementException errors.
    ///
    /// REST API ready: performXXX methods map directly to POST endpoints:
    ///   POST /api/quantity/compare
    ///   POST /api/quantity/convert
    ///   POST /api/quantity/add
    ///   POST /api/quantity/subtract
    ///   POST /api/quantity/divide
    ///
    /// Dependency injection: IQuantityMeasurementService injected via constructor.
    /// Facade pattern: hides service complexity behind simple performXXX methods.
    /// </summary>
    public class QuantityMeasurementController : IQuantityMeasurementApp
    {
        private readonly IQuantityMeasurementService    _service;
        private readonly IQuantityMeasurementRepository _repository;

        /// <summary>Default constructor — prompts user to select repository, then wires dependencies.</summary>
        public QuantityMeasurementController()
        {
            _repository = SelectRepository();
            _service    = new QuantityMeasurementServiceImpl(_repository);
        }

        private static IQuantityMeasurementRepository SelectRepository()
        {
            while (true)
            {
                Console.WriteLine("SELECT REPOSITORY TYPE");
                Console.WriteLine("1. Cache Repository (In-Memory + JSON)");
                Console.WriteLine("2. Database Repository (SQL Server)");
                Console.Write("Choice: ");

                string? input = Console.ReadLine()?.Trim();

                if (input == "1")
                    return QuantityMeasurementCacheRepository.Instance;

                if (input == "2")
                {
                    var configuration = new ConfigurationBuilder()
                        .AddJsonFile(
                            System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"),
                            optional: false, reloadOnChange: false)
                        .Build();

                    return new QuantityMeasurementDatabaseRepository(configuration);
                }

                Console.WriteLine("Invalid choice. Enter 1 or 2.");
            }
        }

                /// <summary>Constructor injection — receives service and repository.</summary>
        public QuantityMeasurementController(IQuantityMeasurementService    service,
                                              IQuantityMeasurementRepository repository)
        {
            _service    = service    ?? throw new ArgumentNullException(nameof(service));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // MAIN MENU

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\nQuantity Measurement Application");
                Console.WriteLine("1. Length Operations");
                Console.WriteLine("2. Weight Operations");
                Console.WriteLine("3. Volume Operations");
                Console.WriteLine("4. Temperature Operations");
                Console.WriteLine("5. Operation History");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice))
                { Console.WriteLine("Invalid input."); continue; }

                switch (choice)
                {
                    case 1: RunCategoryMenu("LENGTH");      break;
                    case 2: RunCategoryMenu("WEIGHT");      break;
                    case 3: RunCategoryMenu("VOLUME");      break;
                    case 4: RunCategoryMenu("TEMPERATURE"); break;
                    case 5: ShowHistory();                  break;
                    case 0: Console.WriteLine("Thank You"); return;
                    default: Console.WriteLine("Invalid choice. Enter 0–5."); break;
                }
            }
        }

        // CATEGORY SUB-MENU

        private void RunCategoryMenu(string category)
        {
            Console.WriteLine($"\n{category} Operations");
            Console.WriteLine("1. Compare");
            Console.WriteLine("2. Convert");
            Console.WriteLine("3. Add");
            Console.WriteLine("4. Subtract");
            Console.WriteLine("5. Divide");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int c))
            { Console.WriteLine("Invalid."); return; }

            switch (c)
            {
                case 1: PerformCompare(category);  break;
                case 2: PerformConvert(category);  break;
                case 3: PerformAdd(category);      break;
                case 4: PerformSubtract(category); break;
                case 5: PerformDivide(category);   break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        // PERFORM METHODS  (REST API — POST endpoints)

        /// <summary>POST /api/quantity/compare</summary>
        public void PerformCompare(string category)
        {
            try
            {
                Console.WriteLine($"\n--- {category} Comparison ---");
                var (first, second) = ReadTwoQuantities(category);
                QuantityDTO result = _service.Compare(first, second);
                Console.WriteLine($"This Quantity : {first}");
                Console.WriteLine($"That Quantity : {second}");
                Console.WriteLine($"Comparison Result: {result.MeasurementType}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>POST /api/quantity/convert</summary>
        public void PerformConvert(string category)
        {
            try
            {
                Console.WriteLine($"\n--- {category} Conversion ---");
                double val    = ReadDouble("Enter value: ");
                string srcUnit = ReadUnit(category, "Enter source unit");
                string tgtUnit = ReadUnit(category, "Enter target unit");

                var input      = new QuantityDTO(val, srcUnit, category);
                var targetDto  = new QuantityDTO(0,   tgtUnit, category);

                QuantityDTO result = _service.Convert(input, targetDto);
                Console.WriteLine($"This Quantity : {input}");
                Console.WriteLine($"Conversion Result: {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>POST /api/quantity/add</summary>
        public void PerformAdd(string category)
        {
            try
            {
                Console.WriteLine($"\n--- {category} Addition ---");
                var (first, second) = ReadTwoQuantities(category);
                QuantityDTO result  = _service.Add(first, second);
                Console.WriteLine($"This Quantity : {first}");
                Console.WriteLine($"That Quantity : {second}");
                Console.WriteLine($"Addition Result: {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>POST /api/quantity/subtract</summary>
        public void PerformSubtract(string category)
        {
            try
            {
                Console.WriteLine($"\n--- {category} Subtraction ---");
                var (first, second) = ReadTwoQuantities(category);
                QuantityDTO result  = _service.Subtract(first, second);
                Console.WriteLine($"This Quantity : {first}");
                Console.WriteLine($"That Quantity : {second}");
                Console.WriteLine($"Subtraction Result: {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>POST /api/quantity/divide</summary>
        public void PerformDivide(string category)
        {
            try
            {
                Console.WriteLine($"\n--- {category} Division ---");
                var (first, second) = ReadTwoQuantities(category);
                QuantityDTO result  = _service.Divide(first, second);
                Console.WriteLine($"This Quantity : {first}");
                Console.WriteLine($"That Quantity : {second}");
                Console.WriteLine($"Division Result: {result.Value:G} (dimensionless ratio)");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // HISTORY

        private void ShowHistory()
        {
            while (true)
            {
                Console.WriteLine("\n--- Operation History ---");
                Console.WriteLine("1.View All History");
                Console.WriteLine("2.View By Operation Type");
                Console.WriteLine("3.View By Measurement Type");
                Console.WriteLine("4.View Statistics");
                Console.WriteLine("0.  Back to Main Menu");
                Console.Write("Select Option: ");

                string? input = Console.ReadLine()?.Trim();

                switch (input)
                {
                    case "1":  ShowAllHistory();        break;
                    case "2":  ShowByOperationType();   break;
                    case "3":  ShowByMeasurementType(); break;
                    case "4": ShowStatistics();        break;
                    case "0":  return;
                    default:   Console.WriteLine("Invalid option. Enter 7-10 or 0 to go back."); break;
                }
            }
        }

        private void ShowAllHistory()
        {
            var history = _repository.GetAllMeasurements();
            Console.WriteLine("\n--- All History ---");
            if (history.Count == 0) { Console.WriteLine("No operations yet."); return; }
            for (int i = 0; i < history.Count; i++)
                Console.WriteLine($"{i + 1}. {history[i]}");
        }

        private void ShowByOperationType()
        {
            Console.Write("Enter operation type (COMPARE / CONVERT / ADD / SUBTRACT / DIVIDE): ");
            string? opType = Console.ReadLine()?.Trim().ToUpper();
            if (string.IsNullOrEmpty(opType)) { Console.WriteLine("Invalid."); return; }

            var results = _repository.GetMeasurementsByOperationType(opType);
            Console.WriteLine($"\n--- {opType} Operations ---");
            if (results.Count == 0) { Console.WriteLine("No records found."); return; }
            for (int i = 0; i < results.Count; i++)
                Console.WriteLine($"{i + 1}. {results[i]}");
        }

        private void ShowByMeasurementType()
        {
            Console.Write("Enter measurement type (LENGTH / WEIGHT / VOLUME / TEMPERATURE): ");
            string? measType = Console.ReadLine()?.Trim().ToUpper();
            if (string.IsNullOrEmpty(measType)) { Console.WriteLine("Invalid."); return; }

            // Map category to unit keywords — FirstOperand stores "67 FEET" not "LENGTH"
            string[] keywords = measType switch
            {
                "LENGTH"      => new[] { "FEET", "INCHES", "YARDS", "CENTIMETERS" },
                "WEIGHT"      => new[] { "KILOGRAM", "GRAM", "POUND" },
                "VOLUME"      => new[] { "LITRE", "MILLILITRE", "GALLON" },
                "TEMPERATURE" => new[] { "CELSIUS", "FAHRENHEIT", "KELVIN" },
                _             => new[] { measType }
            };

            var seen    = new HashSet<string>();
            var results = keywords
                .SelectMany(k => _repository.GetMeasurementsByMeasurementType(k))
                .Where(e => seen.Add(e.Timestamp.ToString("o") + e.OperationType))
                .ToList();

            Console.WriteLine($"\n--- {measType} Operations ---");
            if (results.Count == 0) { Console.WriteLine("No records found."); return; }
            for (int i = 0; i < results.Count; i++)
                Console.WriteLine($"{i + 1}. {results[i]}");
        }

        private void ShowStatistics()
        {
            int total = _repository.GetTotalCount();
            var all   = _repository.GetAllMeasurements();

            int compare  = _repository.GetMeasurementsByOperationType("COMPARE").Count;
            int convert  = _repository.GetMeasurementsByOperationType("CONVERT").Count;
            int add      = _repository.GetMeasurementsByOperationType("ADD").Count;
            int subtract = _repository.GetMeasurementsByOperationType("SUBTRACT").Count;
            int divide   = _repository.GetMeasurementsByOperationType("DIVIDE").Count;
            int errors   = all.Count(e => e.IsError);

            Console.WriteLine("\n--- Statistics ---");
            Console.WriteLine($"Total Records  : {total}");
            Console.WriteLine($"Successful     : {total - errors}");
            Console.WriteLine($"Failed         : {errors}");
            Console.WriteLine($"  COMPARE      : {compare}");
            Console.WriteLine($"  CONVERT      : {convert}");
            Console.WriteLine($"  ADD          : {add}");
            Console.WriteLine($"  SUBTRACT     : {subtract}");
            Console.WriteLine($"  DIVIDE       : {divide}");
        }

        // INPUT HELPERS

        private static (QuantityDTO, QuantityDTO) ReadTwoQuantities(string category)
        {
            double v1 = ReadDouble("Enter first value: ");
            string u1 = ReadUnit(category, "Enter first unit");
            double v2 = ReadDouble("Enter second value: ");
            string u2 = ReadUnit(category, "Enter second unit");
            return (new QuantityDTO(v1, u1, category),
                    new QuantityDTO(v2, u2, category));
        }

        private static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? raw = Console.ReadLine()?.Trim();
                if (!double.TryParse(raw, out double v))
                { Console.WriteLine("Invalid. Enter a numeric value."); continue; }
                if (v < 0)
                { Console.WriteLine("Invalid. Value cannot be negative."); continue; }
                if (v > 1_000_000)
                { Console.WriteLine("Invalid. Value cannot be greater than 1000000."); continue; }
                return v;
            }
        }

        private static string ReadUnit(string category, string prompt)
        {
            string hint = category switch
            {
                "LENGTH"      => "FEET/ft, INCHES/in, YARDS/yd, CENTIMETERS/cm",
                "WEIGHT"      => "KILOGRAM/kg, GRAM/g, POUND/lb",
                "VOLUME"      => "LITRE/l, MILLILITRE/ml, GALLON/gal",
                "TEMPERATURE" => "CELSIUS/c, FAHRENHEIT/f, KELVIN/k",
                _             => "unit name"
            };

            while (true)
            {
                Console.Write($"{prompt} ({hint}): ");
                string? input = Console.ReadLine()?.Trim().ToUpper();
                if (string.IsNullOrEmpty(input)) { Console.WriteLine("Unit cannot be empty."); continue; }
                return NormalizeUnit(input);
            }
        }

        /// <summary>
        /// Maps short forms and common aliases to the canonical unit names
        /// expected by ResolveUnit in the service layer.
        /// </summary>
        private static string NormalizeUnit(string input) => input switch
        {
            // LENGTH
            "FT"          => "FEET",
            "FOOT"        => "FEET",
            "IN"          => "INCHES",
            "INCH"        => "INCHES",
            "YD"          => "YARDS",
            "YARD"        => "YARDS",
            "CM"          => "CENTIMETERS",
            "CENTIMETER"  => "CENTIMETERS",

            // WEIGHT
            "KG"          => "KILOGRAM",
            "KILOGRAMS"   => "KILOGRAM",
            "G"           => "GRAM",
            "GRAMS"       => "GRAM",
            "LB"          => "POUND",
            "LBS"         => "POUND",
            "POUNDS"      => "POUND",

            // VOLUME
            "L"           => "LITRE",
            "LITRES"      => "LITRE",
            "LITER"       => "LITRE",
            "LITERS"      => "LITRE",
            "ML"          => "MILLILITRE",
            "MILLILITRES" => "MILLILITRE",
            "MILLILITER"  => "MILLILITRE",
            "MILLILITERS" => "MILLILITRE",
            "GAL"         => "GALLON",
            "GALLONS"     => "GALLON",

            // TEMPERATURE
            "C"           => "CELSIUS",
            "F"           => "FAHRENHEIT",
            "K"           => "KELVIN",

            // Already canonical — pass through unchanged
            _             => input
        };
    }
}