namespace AdventureArray.Application.UI.Client.Shared.Utilities;

using System;
using System.Globalization;
using System.Text.RegularExpressions;

public static class DutchDecimalHelper
{
	private static readonly CultureInfo DutchCulture = new("nl-NL");

	public static string ToDutchDecimalString(long waarde, int schaal, bool grouping = true)
	{
		var value = waarde / Math.Pow(10, schaal);

		var numberFormat = (NumberFormatInfo)DutchCulture.NumberFormat.Clone();
		numberFormat.NumberGroupSeparator = grouping ? "." : string.Empty;
		numberFormat.NumberGroupSizes = [3];
		numberFormat.NumberDecimalDigits = schaal;
		numberFormat.NumberDecimalSeparator = ",";

		return value.ToString("N", numberFormat);
	}

	public static bool TryParseDutchDecimalString(string? value, out long waarde, out int schaal)
	{
		waarde = 0;
		schaal = 0;

		if (string.IsNullOrEmpty(value))
		{
			return false;
		}

		if (!IsDutchDecimal(value))
		{
			return false;
		}

		var result = FromDutchDecimalString(value);
		waarde = result?.waarde ?? 0;
		schaal = result?.schaal ?? 0;
		return true;
	}

	public static (int waarde, int schaal)? FromDutchDecimalString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}

		if (!IsDutchDecimal(value))
		{
			return null;
		}

		// Verwijder de groep-separators
		var replacedValue = value.Replace(".", string.Empty);

		var parts = replacedValue.Split(',');

		int schaal = parts.Length > 1 ? parts[1].Length : 0;

		int waarde = int.Parse(replacedValue.Replace(",", string.Empty), DutchCulture);

		return (waarde, schaal);
	}

	public static bool IsDutchDecimal(string value)
	{
		if (string.IsNullOrEmpty(value)) return false;

		var regex = new Regex(@"^[-+]?\d{1,3}((\.\d{3})*|\d*)(,\d+)?$");
		return regex.IsMatch(value);
	}
}
