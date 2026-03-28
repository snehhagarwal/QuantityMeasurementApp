using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementBusinessLayer.Services.Interface;

namespace QuantityMeasurementApi.Tests.Controllers
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "testuser"), new Claim(ClaimTypes.Role, "User") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    [TestClass]
    public class QuantityMeasurementControllerTest
    {
        private static Mock<IQuantityMeasurementService> _mockService = null!;
        private static HttpClient _client = null!;
        private static WebApplicationFactory<Program> _factory = null!;

        [ClassInitialize]
        public static void Init(TestContext _)
        {
            _mockService = new Mock<IQuantityMeasurementService>();
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(b =>
                {
                    b.ConfigureAppConfiguration((_, config) =>
                    {
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["UseDatabase"] = "InMemory",
                            ["InMemoryDatabaseName"] = "ControllerTestDb",
                            ["Redis:Enabled"] = "false",
                            ["DisableJwtAuth"] = "true"
                        });
                    });
                    b.ConfigureServices(services =>
                    {
                        var svc = services.SingleOrDefault(
                            d => d.ServiceType == typeof(IQuantityMeasurementService));
                        if (svc != null) services.Remove(svc);
                        services.AddScoped<IQuantityMeasurementService>(_ => _mockService.Object);

                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                    });
                });
            _client = _factory.CreateClient();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        private static QuantityInputDto In(
            double v1, string u1, string t1,
            double v2, string u2, string t2) => new()
        {
            ThisQuantityDTO = new QuantityOperandDto { Value = v1, Unit = u1, MeasurementType = t1 },
            ThatQuantityDTO = new QuantityOperandDto { Value = v2, Unit = u2, MeasurementType = t2 }
        };

        private static QuantityMeasurementDto Dto(string op, string result, bool error = false) =>
            new() { Operation = op, ResultString = result, IsError = error };

        [TestMethod]
        public async Task PerformCompare_EqualQuantities_Returns200True()
        {
            _mockService.Setup(s => s.CompareAsync(It.IsAny<QuantityOperandDto>(),
                    It.IsAny<QuantityOperandDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Dto("COMPARE", "true"));

            var r = await _client.PostAsJsonAsync("/api/v1/quantities/compare",
                In(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH"));

            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            var body = await r.Content.ReadFromJsonAsync<QuantityMeasurementDto>();
            Assert.AreEqual("true", body!.ResultString);
        }

        [TestMethod]
        public async Task PerformCompare_InvalidUnit_Returns400()
        {
            var r = await _client.PostAsJsonAsync("/api/v1/quantities/compare",
                In(1, "FOOT", "LENGTH", 12, "INCHE", "LENGTH"));
            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
        }

        [TestMethod]
        public async Task PerformCompare_ServiceThrows_Returns400()
        {
            _mockService.Setup(s => s.CompareAsync(It.IsAny<QuantityOperandDto>(),
                    It.IsAny<QuantityOperandDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new QuantityMeasurementBusinessLayer.Exceptions
                    .QuantityMeasurementException("Cross-category not allowed."));

            var r = await _client.PostAsJsonAsync("/api/v1/quantities/compare",
                In(1, "FEET", "LENGTH", 1, "KILOGRAM", "WEIGHT"));

            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
        }

        [TestMethod]
        public async Task PerformConvert_FeetToInches_Returns200()
        {
            _mockService.Setup(s => s.ConvertAsync(It.IsAny<QuantityOperandDto>(),
                    It.IsAny<QuantityOperandDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Dto("CONVERT", "12 INCHES"));

            var r = await _client.PostAsJsonAsync("/api/v1/quantities/convert",
                In(1, "FEET", "LENGTH", 0, "INCHES", "LENGTH"));

            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            Assert.AreEqual("12 INCHES",
                (await r.Content.ReadFromJsonAsync<QuantityMeasurementDto>())!.ResultString);
        }

        [TestMethod]
        public async Task PerformAdd_FeetAndInches_Returns200WithSum()
        {
            _mockService.Setup(s => s.AddAsync(It.IsAny<QuantityOperandDto>(),
                    It.IsAny<QuantityOperandDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Dto("ADD", "2 FEET"));

            var r = await _client.PostAsJsonAsync("/api/v1/quantities/add",
                In(1, "FEET", "LENGTH", 12, "INCHES", "LENGTH"));

            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            Assert.AreEqual("2 FEET",
                (await r.Content.ReadFromJsonAsync<QuantityMeasurementDto>())!.ResultString);
        }

        [TestMethod]
        public async Task PerformAdd_ServiceThrowsQMException_Returns400()
        {
            _mockService.Setup(s => s.AddAsync(It.IsAny<QuantityOperandDto>(),
                    It.IsAny<QuantityOperandDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new QuantityMeasurementBusinessLayer.Exceptions
                    .QuantityMeasurementException("Temperature does not support Add."));

            var r = await _client.PostAsJsonAsync("/api/v1/quantities/add",
                In(100, "CELSIUS", "TEMPERATURE", 50, "CELSIUS", "TEMPERATURE"));

            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
        }

        [TestMethod]
        public async Task PerformSubtract_Returns200()
        {
            _mockService.Setup(s => s.SubtractAsync(It.IsAny<QuantityOperandDto>(),
                    It.IsAny<QuantityOperandDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Dto("SUBTRACT", "1 FEET"));

            var r = await _client.PostAsJsonAsync("/api/v1/quantities/subtract",
                In(2, "FEET", "LENGTH", 12, "INCHES", "LENGTH"));

            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        [TestMethod]
        public async Task PerformDivide_Returns200()
        {
            _mockService.Setup(s => s.DivideAsync(It.IsAny<QuantityOperandDto>(),
                    It.IsAny<QuantityOperandDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Dto("DIVIDE", "5"));

            var r = await _client.PostAsJsonAsync("/api/v1/quantities/divide",
                In(10, "KILOGRAM", "WEIGHT", 2, "KILOGRAM", "WEIGHT"));

            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        [TestMethod]
        public async Task PerformDivide_ByZeroThrowsArithmetic_Returns500()
        {
            _mockService.Setup(s => s.DivideAsync(It.IsAny<QuantityOperandDto>(),
                    It.IsAny<QuantityOperandDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArithmeticException("Divide by zero"));

            var r = await _client.PostAsJsonAsync("/api/v1/quantities/divide",
                In(1, "FEET", "LENGTH", 0, "FEET", "LENGTH"));

            Assert.AreEqual(HttpStatusCode.InternalServerError, r.StatusCode);
        }

        [TestMethod]
        public async Task GetHistory_Returns200WithList()
        {
            _mockService.Setup(s => s.GetAllHistoryAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<QuantityMeasurementDto>
                {
                    Dto("COMPARE", "true"),
                    Dto("ADD", "2 FEET")
                }.AsReadOnly());

            var r = await _client.GetAsync("/api/v1/quantities/history");
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            var list = await r.Content.ReadFromJsonAsync<List<QuantityMeasurementDto>>();
            Assert.AreEqual(2, list!.Count);
        }

        [TestMethod]
        public async Task GetHistoryByOperationType_Returns200()
        {
            _mockService.Setup(s => s.GetHistoryByOperationTypeAsync("COMPARE", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<QuantityMeasurementDto>
                    { Dto("COMPARE", "true") }.AsReadOnly());

            var r = await _client.GetAsync("/api/v1/quantities/history/operation/COMPARE");
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        [TestMethod]
        public async Task GetHistoryByMeasurementType_Returns200()
        {
            _mockService.Setup(s => s.GetHistoryByMeasurementTypeAsync("LENGTH", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<QuantityMeasurementDto>().AsReadOnly());

            var r = await _client.GetAsync("/api/v1/quantities/history/type/LENGTH");
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        [TestMethod]
        public async Task GetErrorHistory_Returns200()
        {
            _mockService.Setup(s => s.GetErrorHistoryAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<QuantityMeasurementDto>
                    { Dto("ADD", null!, true) }.AsReadOnly());

            var r = await _client.GetAsync("/api/v1/quantities/history/errored");
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            var list = await r.Content.ReadFromJsonAsync<List<QuantityMeasurementDto>>();
            Assert.IsTrue(list![0].IsError);
        }

        [TestMethod]
        public async Task GetCount_Returns200WithLong()
        {
            _mockService.Setup(s => s.CountByOperationTypeAsync("ADD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(3L);

            var r = await _client.GetAsync("/api/v1/quantities/count/ADD");
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            Assert.AreEqual(3L, await r.Content.ReadFromJsonAsync<long>());
        }
    }
}
