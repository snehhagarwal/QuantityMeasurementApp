using System;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// Generic Quantity Length class.
    /// Lives in QuantityMeasurementModel.Entities: entity with self-contained conversion constants.
    /// Conversion logic duplicates LengthUnitExtensions constants to avoid circular dependency.
    /// </summary>
    public class Length
    {
        private const double InchesPerFoot = 12.0;
        private const double FeetPerYard = 3.0;
        private const double InchesPerCentimeter = 0.393701;

        public double Value { get; }
        public LengthUnit Unit { get; }

        public Length(double value, LengthUnit unit)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Value cannot be NaN");
            if (double.IsInfinity(value))
                throw new ArgumentException("Value cannot be Infinity");
            if (Math.Abs(value) > 10000000)
                throw new ArgumentException("Length value too large. Invalid measurement.");
            if (!Enum.IsDefined(typeof(LengthUnit), unit) || unit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Invalid Length Unit");

            Value = value;
            Unit  = unit;
        }

        public double ToBaseUnit() => ConvertToBaseUnit(Unit, Value);

        public Length ConvertTo(LengthUnit targetUnit, int decimalPlaces = 2)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit) || targetUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Target unit cannot be null or UNKNOWN");
            double rounded = Math.Round(ConvertFromBaseUnit(targetUnit, ToBaseUnit()), decimalPlaces);
            return new Length(rounded, targetUnit);
        }

        public static double Convert(double value, LengthUnit sourceUnit, LengthUnit targetUnit, int decimalPlaces = 2)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be a finite number");
            if (!Enum.IsDefined(typeof(LengthUnit), sourceUnit) || sourceUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Source unit is not valid");
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit) || targetUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Target unit is not valid");
            return Math.Round(ConvertFromBaseUnit(targetUnit, ConvertToBaseUnit(sourceUnit, value)), decimalPlaces);
        }

        public Length Add(Length other, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Second length cannot be null");
            return Add(this, other, this.Unit, decimalPlaces);
        }

        public Length Add(Length other, LengthUnit resultUnit, int decimalPlaces = 2)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Second length cannot be null");
            return Add(this, other, resultUnit, decimalPlaces);
        }

        public static Length Add(Length first, Length second, LengthUnit resultUnit, int decimalPlaces = 2)
        {
            if (first == null)  throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (!Enum.IsDefined(typeof(LengthUnit), resultUnit) || resultUnit == LengthUnit.UNKNOWN)
                throw new ArgumentException("Result unit is not valid");
            double sumInBase       = first.ToBaseUnit() + second.ToBaseUnit();
            double sumInResultUnit = ConvertFromBaseUnit(resultUnit, sumInBase);
            return new Length(Math.Round(sumInResultUnit, decimalPlaces), resultUnit);
        }

        public static Length Add(double firstValue, LengthUnit firstUnit,
                                 double secondValue, LengthUnit secondUnit,
                                 LengthUnit resultUnit, int decimalPlaces = 2)
        {
            if (!double.IsFinite(firstValue) || !double.IsFinite(secondValue))
                throw new ArgumentException("Values must be finite numbers");
            return Add(new Length(firstValue, firstUnit), new Length(secondValue, secondUnit),
                       resultUnit, decimalPlaces);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Length)) return false;
            return this.ToBaseUnit() == ((Length)obj).ToBaseUnit();
        }

        public override int GetHashCode() => ToBaseUnit().GetHashCode();
        public override string ToString() => $"{Value:F2} {Unit}";

        private static double ConvertToBaseUnit(LengthUnit unit, double value) =>
            unit switch
            {
                LengthUnit.FEET        => value,
                LengthUnit.INCHES      => value / InchesPerFoot,
                LengthUnit.YARDS       => value * FeetPerYard,
                LengthUnit.CENTIMETERS => (value * InchesPerCentimeter) / InchesPerFoot,
                _ => throw new ArgumentException($"Invalid LengthUnit: {unit}")
            };

        private static double ConvertFromBaseUnit(LengthUnit unit, double baseValueInFeet) =>
            unit switch
            {
                LengthUnit.FEET        => baseValueInFeet,
                LengthUnit.INCHES      => baseValueInFeet * InchesPerFoot,
                LengthUnit.YARDS       => baseValueInFeet / FeetPerYard,
                LengthUnit.CENTIMETERS => (baseValueInFeet * InchesPerFoot) / InchesPerCentimeter,
                _ => throw new ArgumentException($"Invalid LengthUnit: {unit}")
            };
    }
}
