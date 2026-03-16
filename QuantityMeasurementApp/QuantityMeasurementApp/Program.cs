using QuantityMeasurementApp.Controller;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp
{
    public class Program
    {
        public static void Main()
        {
            IQuantityMeasurementApp app = new QuantityMeasurementController();
            app.Run();
        }
    }
}
 