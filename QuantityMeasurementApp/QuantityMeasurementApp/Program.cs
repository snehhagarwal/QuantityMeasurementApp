using QuantityMeasurementApp.PresentationLayer;

namespace QuantityMeasurementApp
{
    
    // Program.cs
    // Entry point of the application
    // It starts the program by calling Presentation Layer
    class Program
    {
        static void Main()
        {
            // Create object of Presentation Layer class
            FeetPresentation presentation = new FeetPresentation();
            // Call Run method to start application
            presentation.Run();
        }
    }
}
