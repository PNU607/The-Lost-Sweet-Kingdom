using System;
using System.Globalization;

#region Custom Parser Interfaces and Implementations


public readonly struct SetParser : IMultiColumnParser
{
    // values[0] -> setId, values[1] -> setLevel
    public object Parse(params string[] values)
    {
        if (values.Length < 2)
            throw new Exception($"Not enough columns to parse Set object. Got {values.Length} columns.");

        return new Set
        {
            Name = values[0],
            Value = values[1],
        };
    }
}

public class Set
{
    public string Name;
    public string Value;
}


public readonly struct WeightedValue<T> : ICustomParser
{
    public readonly T value;
    public readonly float weight;

    public WeightedValue(T value, float weight)
    {
        this.value = value;
        this.weight = weight;
    }

    public override string ToString() => $"{value} ({weight})";

    public object Parse(string value)
    {
        return ParseValue(value);
    }

    // 예: "Apple:0.75" → value="Apple", weight=0.75

    public static WeightedValue<T> ParseValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FormatException("WeightedValue is empty.");

        int separatorIndex = value.LastIndexOf(':');
        if (separatorIndex <= 0 || separatorIndex == value.Length - 1)
            throw new FormatException(
                "Invalid format for WeightedValue. Expected: value:weight");

        string rawValue = value[..separatorIndex].Trim();
        string rawWeight = value[(separatorIndex + 1)..].Trim();
        T val = (T)Convert.ChangeType(
            rawValue,
            typeof(T),
            CultureInfo.InvariantCulture);
        float w = float.Parse(
            rawWeight,
            NumberStyles.Float,
            CultureInfo.InvariantCulture);
        return new WeightedValue<T>(val, w);
    }
}

public readonly struct UnitCount : ICustomParser
{
    public readonly string UnitId;
    public readonly int Count;

    public UnitCount(string unitId, int count)
    {
        UnitId = unitId;
        Count = count;
    }

    public object Parse(string value)
    {
        return ParseValue(value);
    }

    // 예: "abcd*2" → UnitId="abcd", count =2
    // 예: "abcd" → UnitId="abcd", count =1

    public static UnitCount ParseValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FormatException("UnitCount is empty.");

        var parts = value.Split('*');
        if (parts.Length > 2)
            throw new FormatException(
                $"Invalid UnitCount format: {value}");

        //기본적인 카운트는 1개다
        string id = parts[0].Trim();
        if (id.Length == 0)
            throw new FormatException("UnitCount unit ID is empty.");

        int count = 1;
        if (parts.Length >= 2)
        {
            count = int.Parse(
                parts[1].Trim(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture);
        }

        return new UnitCount(id, count);
    }
}

#endregion
