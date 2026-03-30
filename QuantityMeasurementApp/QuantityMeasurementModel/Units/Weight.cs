using QuantityMeasurementModel.Enums;

namespace QuantityMeasurementModel.Units
{
    /// <summary>Generic weight quantity with unit conversions.</summary>
    public class Weight
    {
        private const double GramsPerKilogram = 1000.0;
        private const double KilogramsPerPound = 0.453592;

        public double Value { get; }
        public WeightUnit Unit { get; }

        public Weight(double value, WeightUnit unit)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Weight value cannot be NaN");
            if (double.IsInfinity(value))
                throw new ArgumentException("Weight value cannot be Infinity");
            if (Math.Abs(value) > 10000000)
                throw new ArgumentException("Weight value too large. Invalid measurement.");
            if (!Enum.IsDefined(typeof(WeightUnit), unit) || unit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Invalid Weight Unit");

            Value = value;
            Unit = unit;
        }

        public double ToBaseUnit() => ConvertToBaseUnit(Unit, Value);

        public Weight ConvertTo(WeightUnit targetUnit, int decimalPlaces = 2)
        {
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit) || targetUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Target unit cannot be UNKNOWN");
            double rounded = Math.Round(ConvertFromBaseUnit(targetUnit, ToBaseUnit()), decimalPlaces);
            return new Weight(rounded, targetUnit);
        }

        public static double Convert(double value, WeightUnit sourceUnit, WeightUnit targetUnit, int decimalPlaces = 2)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");
            if (!Enum.IsDefined(typeof(WeightUnit), sourceUnit) || sourceUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Source unit is not valid");
            if (!Enum.IsDefined(typeof(WeightUnit), targetUnit) || targetUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Target unit is not valid");
            return Math.Round(ConvertFromBaseUnit(targetUnit, ConvertToBaseUnit(sourceUnit, value)), decimalPlaces);
        }

        public Weight Add(Weight other, int decimalPlaces = 2)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other), "Second weight cannot be null");
            return Add(this, other, Unit, decimalPlaces);
        }

        public Weight Add(Weight other, WeightUnit resultUnit, int decimalPlaces = 2)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other), "Second weight cannot be null");
            return Add(this, other, resultUnit, decimalPlaces);
        }

        public static Weight Add(Weight first, Weight second, WeightUnit resultUnit, int decimalPlaces = 2)
        {
            if (first is null) throw new ArgumentNullException(nameof(first));
            if (second is null) throw new ArgumentNullException(nameof(second));
            if (!Enum.IsDefined(typeof(WeightUnit), resultUnit) || resultUnit == WeightUnit.UNKNOWN)
                throw new ArgumentException("Result unit is not valid");
            double sumBase = first.ToBaseUnit() + second.ToBaseUnit();
            double sumInResult = Math.Round(ConvertFromBaseUnit(resultUnit, sumBase), decimalPlaces);
            return new Weight(sumInResult, resultUnit);
        }

        public static Weight Add(double firstValue, WeightUnit firstUnit,
            double secondValue, WeightUnit secondUnit,
            WeightUnit resultUnit, int decimalPlaces = 2)
        {
            if (!double.IsFinite(firstValue) || !double.IsFinite(secondValue))
                throw new ArgumentException("Values must be finite numbers");
            return Add(new Weight(firstValue, firstUnit), new Weight(secondValue, secondUnit),
                resultUnit, decimalPlaces);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Weight w) return false;
            return ToBaseUnit().Equals(w.ToBaseUnit());
        }

        public override int GetHashCode() => ToBaseUnit().GetHashCode();
        public override string ToString() => $"{Value:F2} {Unit}";

        private static double ConvertToBaseUnit(WeightUnit unit, double value) =>
            unit switch
            {
                WeightUnit.KILOGRAM => value,
                WeightUnit.GRAM => value / GramsPerKilogram,
                WeightUnit.POUND => value * KilogramsPerPound,
                _ => throw new ArgumentException($"Invalid WeightUnit: {unit}")
            };

        private static double ConvertFromBaseUnit(WeightUnit unit, double baseKg) =>
            unit switch
            {
                WeightUnit.KILOGRAM => baseKg,
                WeightUnit.GRAM => baseKg * GramsPerKilogram,
                WeightUnit.POUND => baseKg / KilogramsPerPound,
                _ => throw new ArgumentException($"Invalid WeightUnit: {unit}")
            };
    }
}
