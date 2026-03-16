namespace QuantityMeasurementApp.Interface
{
    /// <summary>
    /// Contract for the application entry-point.
    /// Program.cs upcasts the concrete controller to this interface
    /// before invoking Run(), avoiding a direct dependency on the implementation.
    /// </summary>
    public interface IQuantityMeasurementApp
    {
        void Run();
    }
}