using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementApi.Tests.Integration
{
    /// <summary>
    /// Bypasses JWT for integration tests — auto-authenticates every request.
    /// </summary>
    public class IntegrationTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public IntegrationTestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims    = new[] { new Claim(ClaimTypes.Name, "testuser"), new Claim(ClaimTypes.Role, "User") };
            var identity  = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket    = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    /// <summary>
    /// Full-stack integration tests — real service, real EF Core in-memory DB,
    /// real HTTP round-trips. No mocking.
    ///
    /// Equivalent to Spring @SpringBootTest(webEnvironment=RANDOM_PORT) + TestRestTemplate.
    /// Each test class gets a unique in-memory database to prevent state pollution.
    /// </summary>
    [TestClass]
    public class QuantityMeasurementApplicationTests
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private static HttpClient _client = null!;

        [ClassInitialize]
        public static void Init(TestContext _)
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(b =>
                {
                    b.ConfigureAppConfiguration((_, config) =>
                    {
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["UseDatabase"] = "InMemory",
                            ["InMemoryDatabaseName"] = "IntegrationTestDb_" + Guid.NewGuid(),
                            ["Redis:Enabled"] = "false",
                            ["DisableJwtAuth"] = "true"
                        });
                    });
                    b.ConfigureServices(services =>
                    {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, IntegrationTestAuthHandler>("Test", _ => { });
                });
                });

            _client = _factory.CreateClient();
            // InMemory DB needs no schema creation — EnsureCreated() removed
        }

        [ClassCleanup]
        public static void Cleanup() { _client.Dispose(); _factory.Dispose(); }

        // ── Helper ────────────────────────────────────────────────────────────

        private static QuantityInputDto In(
            double v1, string u1, string t1,
            double v2, string u2, string t2) => new()
        {
            ThisQuantityDTO = new QuantityOperandDto
                { Value = v1, Unit = u1, MeasurementType = t1 },
            ThatQuantityDTO = new QuantityOperandDto
                { Value = v2, Unit = u2, MeasurementType = t2 }
        };

        private static async Task<QuantityMeasurementDto> Post(string url, QuantityInputDto body)
        {
            var r = await _client.PostAsJsonAsync(url, body);
            r.EnsureSuccessStatusCode();
            return (await r.Content.ReadFromJsonAsync<QuantityMeasurementDto>())!;
        }

        // ═════════════════════════════════════════════════════════════════════
        // COMPARE — Length
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public async Task Compare_1Foot_12Inches_ReturnsTrue()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH"));
            Assert.AreEqual("true", dto.ResultString?.ToLower());
            Assert.IsFalse(dto.IsError);
            Assert.AreEqual("COMPARE", dto.Operation);
        }

        [TestMethod]
        public async Task Compare_34Feet_36Feet_ReturnsFalse()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(34, "FEET", "LENGTH", 36, "FEET", "LENGTH"));
            Assert.AreEqual("false", dto.ResultString?.ToLower());
        }

        [TestMethod]
        public async Task Compare_1Yard_3Feet_ReturnsTrue()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(1, "YARDS", "LENGTH", 3, "FEET", "LENGTH"));
            Assert.AreEqual("true", dto.ResultString?.ToLower());
        }

        [TestMethod]
        public async Task Compare_1Foot_1Foot_ReturnsTrue()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(1, "FEET", "LENGTH", 1, "FEET", "LENGTH"));
            Assert.AreEqual("true", dto.ResultString?.ToLower());
        }

        // ── COMPARE — Weight ──────────────────────────────────────────────────

        [TestMethod]
        public async Task Compare_1000Gram_1Kilogram_ReturnsTrue()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(1000, "GRAM", "WEIGHT", 1, "KILOGRAM", "WEIGHT"));
            Assert.AreEqual("true", dto.ResultString?.ToLower());
        }

        [TestMethod]
        public async Task Compare_1Pound_1Kilogram_ReturnsFalse()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(1, "POUND", "WEIGHT", 1, "KILOGRAM", "WEIGHT"));
            Assert.AreEqual("false", dto.ResultString?.ToLower());
        }

        // ── COMPARE — Volume ──────────────────────────────────────────────────

        [TestMethod]
        public async Task Compare_1000Millilitre_1Litre_ReturnsTrue()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(1000, "MILLILITRE", "VOLUME", 1, "LITRE", "VOLUME"));
            Assert.AreEqual("true", dto.ResultString?.ToLower());
        }

        // ── COMPARE — Temperature ─────────────────────────────────────────────

        [TestMethod]
        public async Task Compare_0Celsius_32Fahrenheit_ReturnsTrue()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(0, "CELSIUS", "TEMPERATURE", 32, "FAHRENHEIT", "TEMPERATURE"));
            Assert.AreEqual("true", dto.ResultString?.ToLower());
        }

        [TestMethod]
        public async Task Compare_100Celsius_212Fahrenheit_ReturnsTrue()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(100, "CELSIUS", "TEMPERATURE", 212, "FAHRENHEIT", "TEMPERATURE"));
            Assert.AreEqual("true", dto.ResultString?.ToLower());
        }

        [TestMethod]
        public async Task Compare_0Celsius_273Point15Kelvin_ReturnsTrue()
        {
            var dto = await Post("/api/v1/quantities/compare",
                In(0, "CELSIUS", "TEMPERATURE", 273.15, "KELVIN", "TEMPERATURE"));
            Assert.AreEqual("true", dto.ResultString?.ToLower());
        }

        // ═════════════════════════════════════════════════════════════════════
        // CONVERT
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public async Task Convert_1Foot_To_12Inches()
        {
            var dto = await Post("/api/v1/quantities/convert",
                In(1, "FEET", "LENGTH", 0, "INCHES", "LENGTH"));
            Assert.IsTrue(dto.ResultString!.StartsWith("12"),
                $"Expected 12 INCHES, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Convert_12Inches_To_1Foot()
        {
            var dto = await Post("/api/v1/quantities/convert",
                In(12, "INCHES", "LENGTH", 0, "FEET", "LENGTH"));
            Assert.IsTrue(dto.ResultString!.StartsWith("1"),
                $"Expected 1 FEET, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Convert_1Yard_To_3Feet()
        {
            var dto = await Post("/api/v1/quantities/convert",
                In(1, "YARDS", "LENGTH", 0, "FEET", "LENGTH"));
            Assert.IsTrue(dto.ResultString!.StartsWith("3"),
                $"Expected 3 FEET, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Convert_1Kilogram_To_1000Gram()
        {
            var dto = await Post("/api/v1/quantities/convert",
                In(1, "KILOGRAM", "WEIGHT", 0, "GRAM", "WEIGHT"));
            Assert.IsTrue(dto.ResultString!.StartsWith("1000"),
                $"Expected 1000 GRAM, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Convert_56Kilogram_To_Pounds()
        {
            var dto = await Post("/api/v1/quantities/convert",
                In(56, "KILOGRAM", "WEIGHT", 0, "POUND", "WEIGHT"));
            Assert.IsTrue(dto.ResultString!.Contains("123"),
                $"Expected ~123.459 POUND, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Convert_1Litre_To_1000Millilitre()
        {
            var dto = await Post("/api/v1/quantities/convert",
                In(1, "LITRE", "VOLUME", 0, "MILLILITRE", "VOLUME"));
            Assert.IsTrue(dto.ResultString!.StartsWith("1000"),
                $"Expected 1000 MILLILITRE, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Convert_0Celsius_To_32Fahrenheit()
        {
            var dto = await Post("/api/v1/quantities/convert",
                In(0, "CELSIUS", "TEMPERATURE", 0, "FAHRENHEIT", "TEMPERATURE"));
            Assert.IsTrue(dto.ResultString!.StartsWith("32"),
                $"Expected 32 FAHRENHEIT, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Convert_100Celsius_To_212Fahrenheit()
        {
            var dto = await Post("/api/v1/quantities/convert",
                In(100, "CELSIUS", "TEMPERATURE", 0, "FAHRENHEIT", "TEMPERATURE"));
            Assert.IsTrue(dto.ResultString!.StartsWith("212"),
                $"Expected 212 FAHRENHEIT, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Convert_0Celsius_To_273Point15Kelvin()
        {
            var dto = await Post("/api/v1/quantities/convert",
                In(0, "CELSIUS", "TEMPERATURE", 0, "KELVIN", "TEMPERATURE"));
            Assert.IsTrue(dto.ResultString!.Contains("273"),
                $"Expected ~273.15 KELVIN, got: {dto.ResultString}");
        }

        // ═════════════════════════════════════════════════════════════════════
        // ADD
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public async Task Add_1Foot_12Inches_Returns2Feet()
        {
            var dto = await Post("/api/v1/quantities/add",
                In(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH"));
            Assert.IsTrue(dto.ResultString!.StartsWith("2"),
                $"Expected 2 FEET, got: {dto.ResultString}");
            Assert.AreEqual("LENGTH", dto.MeasurementType);
        }

        [TestMethod]
        public async Task Add_22Litre_45Millilitre_Returns22Point045Litre()
        {
            var dto = await Post("/api/v1/quantities/add",
                In(22, "LITRE", "VOLUME", 45, "MILLILITRE", "VOLUME"));
            Assert.IsTrue(
                dto.ResultString!.Contains("22.045") || dto.ResultString.Contains("22,045"),
                $"Expected 22.045 LITRE, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Add_1Kilogram_1000Gram_Returns2Kilogram()
        {
            var dto = await Post("/api/v1/quantities/add",
                In(1, "KILOGRAM", "WEIGHT", 1000, "GRAM", "WEIGHT"));
            Assert.IsTrue(dto.ResultString!.StartsWith("2"),
                $"Expected 2 KILOGRAM, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Add_Temperature_Returns400()
        {
            var r = await _client.PostAsJsonAsync("/api/v1/quantities/add",
                In(100, "CELSIUS", "TEMPERATURE", 50, "CELSIUS", "TEMPERATURE"));
            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
        }

        // ═════════════════════════════════════════════════════════════════════
        // SUBTRACT
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public async Task Subtract_2Feet_12Inches_Returns1Foot()
        {
            var dto = await Post("/api/v1/quantities/subtract",
                In(2, "FEET", "LENGTH", 12, "INCHES", "LENGTH"));
            Assert.IsTrue(dto.ResultString!.StartsWith("1"),
                $"Expected 1 FEET, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Subtract_5Litre_500Millilitre_Returns4Point5Litre()
        {
            var dto = await Post("/api/v1/quantities/subtract",
                In(5, "LITRE", "VOLUME", 500, "MILLILITRE", "VOLUME"));
            Assert.IsTrue(
                dto.ResultString!.Contains("4.5") || dto.ResultString.Contains("4,5"),
                $"Expected 4.5 LITRE, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Subtract_Temperature_Returns400()
        {
            var r = await _client.PostAsJsonAsync("/api/v1/quantities/subtract",
                In(100, "CELSIUS", "TEMPERATURE", 50, "CELSIUS", "TEMPERATURE"));
            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
        }

        // ═════════════════════════════════════════════════════════════════════
        // DIVIDE
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public async Task Divide_10Kg_By_2Kg_Returns5()
        {
            var dto = await Post("/api/v1/quantities/divide",
                In(10, "KILOGRAM", "WEIGHT", 2, "KILOGRAM", "WEIGHT"));
            Assert.IsTrue(dto.ResultString!.StartsWith("5"),
                $"Expected 5, got: {dto.ResultString}");
        }

        [TestMethod]
        public async Task Divide_ByZero_Returns500()
        {
            var r = await _client.PostAsJsonAsync("/api/v1/quantities/divide",
                In(1, "FEET", "LENGTH", 0, "INCHES", "LENGTH"));
            Assert.AreEqual(HttpStatusCode.InternalServerError, r.StatusCode);
            var body = await r.Content.ReadAsStringAsync();
            Assert.IsTrue(body.Contains("500") || body.Contains("error") ||
                          body.Contains("zero"), $"Got: {body}");
        }

        [TestMethod]
        public async Task Divide_Temperature_Returns400()
        {
            var r = await _client.PostAsJsonAsync("/api/v1/quantities/divide",
                In(100, "CELSIUS", "TEMPERATURE", 50, "CELSIUS", "TEMPERATURE"));
            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
        }

        // ═════════════════════════════════════════════════════════════════════
        // ERROR HANDLING
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public async Task Add_CrossCategory_Returns400WithErrorBody()
        {
            var r = await _client.PostAsJsonAsync("/api/v1/quantities/add",
                In(1, "FEET", "LENGTH", 1, "KILOGRAM", "WEIGHT"));
            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
            var body = await r.Content.ReadAsStringAsync();
            Assert.IsTrue(body.Contains("error") || body.Contains("Error") ||
                          body.Contains("400"), $"Got: {body}");
        }

        [TestMethod]
        public async Task InvalidUnit_Returns400FromValidation()
        {
            var r = await _client.PostAsJsonAsync("/api/v1/quantities/compare",
                In(1, "FOOT", "LENGTH", 12, "INCHE", "LENGTH"));
            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
        }

        [TestMethod]
        public async Task InvalidMeasurementType_Returns400FromValidation()
        {
            var r = await _client.PostAsJsonAsync("/api/v1/quantities/add",
                In(1, "FEET", "DISTANCE", 12, "INCHES", "DISTANCE"));
            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
        }

        // ═════════════════════════════════════════════════════════════════════
        // HISTORY & REPORTING
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public async Task GetHistory_AfterOperations_ReturnsRecords()
        {
            await Post("/api/v1/quantities/compare",
                In(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH"));

            var r = await _client.GetAsync("/api/v1/quantities/history");
            r.EnsureSuccessStatusCode();
            var list = await r.Content.ReadFromJsonAsync<List<QuantityMeasurementDto>>();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0, "Expected at least one history record.");
        }

        [TestMethod]
        public async Task GetHistoryByOperationType_ReturnsOnlyMatchingRecords()
        {
            await Post("/api/v1/quantities/compare",
                In(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH"));

            var r = await _client.GetAsync("/api/v1/quantities/history/operation/COMPARE");
            r.EnsureSuccessStatusCode();
            var list = await r.Content.ReadFromJsonAsync<List<QuantityMeasurementDto>>();
            Assert.IsTrue(list!.Count > 0);
            Assert.IsTrue(list.TrueForAll(d =>
                string.Equals(d.Operation, "COMPARE",
                    StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task GetHistoryByMeasurementType_ReturnsOnlyMatchingRecords()
        {
            await Post("/api/v1/quantities/add",
                In(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH"));

            var r = await _client.GetAsync("/api/v1/quantities/history/type/LENGTH");
            r.EnsureSuccessStatusCode();
            var list = await r.Content.ReadFromJsonAsync<List<QuantityMeasurementDto>>();
            Assert.IsTrue(list!.Count > 0);
            Assert.IsTrue(list.TrueForAll(d =>
                string.Equals(d.ThisMeasurementType, "LENGTH",
                    StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public async Task GetErrorHistory_AfterBadOperation_ReturnsErrorRecords()
        {
            // Trigger a cross-category error to seed an error record
            await _client.PostAsJsonAsync("/api/v1/quantities/add",
                In(1, "FEET", "LENGTH", 1, "KILOGRAM", "WEIGHT"));

            var r = await _client.GetAsync("/api/v1/quantities/history/errored");
            r.EnsureSuccessStatusCode();
            var list = await r.Content.ReadFromJsonAsync<List<QuantityMeasurementDto>>();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0,
                "Expected at least one error record after cross-category add.");
            Assert.IsTrue(list.TrueForAll(d => d.IsError));
        }

        [TestMethod]
        public async Task GetCount_ReturnsNonNegativeLong()
        {
            var r = await _client.GetAsync("/api/v1/quantities/count/COMPARE");
            r.EnsureSuccessStatusCode();
            var count = await r.Content.ReadFromJsonAsync<long>();
            Assert.IsTrue(count >= 0, $"Expected count >= 0 but got {count}.");
        }

        // ═════════════════════════════════════════════════════════════════════
        // BACKWARD COMPATIBILITY — all UC1–UC16 scenarios via REST
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public async Task BackwardCompatibility_AllUC1ToUC16Scenarios()
        {
            // UC1: same feet
            Assert.AreEqual("true",
                (await Post("/api/v1/quantities/compare",
                    In(10, "FEET", "LENGTH", 10, "FEET", "LENGTH")))
                .ResultString?.ToLower());

            // UC3: feet vs inches
            Assert.AreEqual("true",
                (await Post("/api/v1/quantities/compare",
                    In(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH")))
                .ResultString?.ToLower());

            // UC4: yards vs feet
            Assert.AreEqual("true",
                (await Post("/api/v1/quantities/compare",
                    In(1, "YARDS", "LENGTH", 3, "FEET", "LENGTH")))
                .ResultString?.ToLower());

            // UC5: convert feet → inches
            Assert.IsTrue(
                (await Post("/api/v1/quantities/convert",
                    In(1, "FEET", "LENGTH", 0, "INCHES", "LENGTH")))
                .ResultString!.StartsWith("12"));

            // UC6: add feet + inches
            Assert.IsTrue(
                (await Post("/api/v1/quantities/add",
                    In(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH")))
                .ResultString!.StartsWith("2"));

            // UC9: weight comparison
            Assert.AreEqual("true",
                (await Post("/api/v1/quantities/compare",
                    In(1000, "GRAM", "WEIGHT", 1, "KILOGRAM", "WEIGHT")))
                .ResultString?.ToLower());

            // UC11: volume conversion
            Assert.IsTrue(
                (await Post("/api/v1/quantities/convert",
                    In(1, "LITRE", "VOLUME", 0, "MILLILITRE", "VOLUME")))
                .ResultString!.StartsWith("1000"));

            // UC14: temperature conversion
            Assert.IsTrue(
                (await Post("/api/v1/quantities/convert",
                    In(0, "CELSIUS", "TEMPERATURE", 0, "FAHRENHEIT", "TEMPERATURE")))
                .ResultString!.StartsWith("32"));
        }

        // ═════════════════════════════════════════════════════════════════════
        // SWAGGER
        // ═════════════════════════════════════════════════════════════════════

        [TestMethod]
        public async Task SwaggerJson_IsAccessibleInDevelopment()
        {
            var r = await _client.GetAsync("/swagger/v1/swagger.json");
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode,
                "Swagger JSON should return 200.");
        }
    }
}
