using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// Interface defining Inches business methods.
    /// </summary>
    public interface IInchesService
    {
        bool AreEqual(Inches firstMeasurement,Inches secondMeasurement);
        bool AreEqualWithTolerance(Inches firstMeasurement,Inches secondMeasurement,double tolerance);
    }
}