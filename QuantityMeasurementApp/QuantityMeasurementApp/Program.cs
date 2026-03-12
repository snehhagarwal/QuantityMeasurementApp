using QuantityMeasurementApp.Controller;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp
{
    public class Program
    {
        private static readonly IQuantityMeasurementApp app = new QuantityMeasurementController();

        public static void Main()
        {
            app.Run();
        }
    }
}