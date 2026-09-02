using System;
using System.Globalization;
using UnityEngine;

public readonly struct Vector2Parser : ICustomParser
{
    public object Parse(string value)
    {
        return ParseValue(value);
    }

    public static Vector2 ParseValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FormatException("Vector2 value is empty.");

        var parts = value.Split(',');
        if (parts.Length > 2)
            throw new FormatException($"Invalid Vector2 format: {value}");

        if (parts.Length == 1)
        {
            return new Vector2(ParseFloat(parts[0]), 0f);
        }

        return new Vector2(ParseFloat(parts[0]), ParseFloat(parts[1]));
    }

    private static float ParseFloat(string value) =>
        float.Parse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
}


public readonly struct Vector3Parser : ICustomParser
{
    public object Parse(string value)
    {
        return ParseValue(value);
    }

    public static Vector3 ParseValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FormatException("Vector3 value is empty.");

        var parts = value.Split(',');
        if (parts.Length > 3)
            throw new FormatException($"Invalid Vector3 format: {value}");

        if (parts.Length == 1)
        {
            return new Vector3(ParseFloat(parts[0]), 0f, 0f);
        }

        if (parts.Length == 2)
        {
            return new Vector3(ParseFloat(parts[0]), ParseFloat(parts[1]), 0f);
        }

        return new Vector3(
            ParseFloat(parts[0]),
            ParseFloat(parts[1]),
            ParseFloat(parts[2]));
    }

    private static float ParseFloat(string value) =>
        float.Parse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture);
}
