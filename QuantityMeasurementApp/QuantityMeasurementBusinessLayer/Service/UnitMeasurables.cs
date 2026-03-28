using System;
using QuantityMeasurementModel.Enums;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Unit;

namespace QuantityMeasurementBusinessLayer.Service
{
    /// <summary>
    /// UC15: Concrete IMeasurable implementations for each measurement category.
    /// Delegates conversion logic to the *UnitExtensions classes.
    /// </summary>

    public readonly struct LengthUnitMeasurable : IMeasurable
    {
        public LengthUnit Unit { get; }
        public LengthUnitMeasurable(LengthUnit unit) { Unit = unit; }

        public double GetConversionFactor()           => new LengthUnitExtensions(Unit).GetConversionFactor();
        public double ConvertToBaseUnit(double value) => new LengthUnitExtensions(Unit).ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double v)   => new LengthUnitExtensions(Unit).ConvertFromBaseUnit(v);
        public string GetUnitName()                   => Unit.ToString();
        public string GetMeasurementType()            => "LENGTH";

        public override bool Equals(object? obj) => obj is LengthUnitMeasurable o && Unit == o.Unit;
        public override int GetHashCode()         => Unit.GetHashCode();
        public override string ToString()         => Unit.ToString();
    }

    public readonly struct WeightUnitMeasurable : IMeasurable
    {
        public WeightUnit Unit { get; }
        public WeightUnitMeasurable(WeightUnit unit) { Unit = unit; }

        public double GetConversionFactor()           => new WeightUnitExtensions(Unit).GetConversionFactor();
        public double ConvertToBaseUnit(double value) => new WeightUnitExtensions(Unit).ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double v)   => new WeightUnitExtensions(Unit).ConvertFromBaseUnit(v);
        public string GetUnitName()                   => Unit.ToString();
        public string GetMeasurementType()            => "WEIGHT";

        public override bool Equals(object? obj) => obj is WeightUnitMeasurable o && Unit == o.Unit;
        public override int GetHashCode()         => Unit.GetHashCode();
        public override string ToString()         => Unit.ToString();
    }

    public readonly struct VolumeUnitMeasurable : IMeasurable
    {
        public VolumeUnit Unit { get; }
        public VolumeUnitMeasurable(VolumeUnit unit) { Unit = unit; }

        public double GetConversionFactor()           => new VolumeUnitExtensions(Unit).GetConversionFactor();
        public double ConvertToBaseUnit(double value) => new VolumeUnitExtensions(Unit).ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double v)   => new VolumeUnitExtensions(Unit).ConvertFromBaseUnit(v);
        public string GetUnitName()                   => Unit.ToString();
        public string GetMeasurementType()            => "VOLUME";

        public override bool Equals(object? obj) => obj is VolumeUnitMeasurable o && Unit == o.Unit;
        public override int GetHashCode()         => Unit.GetHashCode();
        public override string ToString()         => Unit.ToString();
    }

    public readonly struct TemperatureUnitMeasurable : IMeasurable
    {
        public TemperatureUnit Unit { get; }
        private static readonly IMeasurable.SupportsArithmetic supportsArithmetic = () => false;

        public TemperatureUnitMeasurable(TemperatureUnit unit) { Unit = unit; }

        public double GetConversionFactor()           => new TemperatureUnitExtensions(Unit).GetConversionFactor();
        public double ConvertToBaseUnit(double value) => new TemperatureUnitExtensions(Unit).ConvertToBaseUnit(value);
        public double ConvertFromBaseUnit(double v)   => new TemperatureUnitExtensions(Unit).ConvertFromBaseUnit(v);
        public string GetUnitName()                   => Unit.ToString();
        public string GetMeasurementType()            => "TEMPERATURE";

        public bool SupportsArithmeticOps() => supportsArithmetic();
        public void ValidateOperationSupport(string operation)
            => throw new NotSupportedException($"Temperature does not support {operation}.");

        public override bool Equals(object? obj) => obj is TemperatureUnitMeasurable o && Unit == o.Unit;
        public override int GetHashCode()         => Unit.GetHashCode();
        public override string ToString()         => Unit.ToString();
    }
}
