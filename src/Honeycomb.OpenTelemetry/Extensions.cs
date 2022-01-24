using System;

namespace Honeycomb.OpenTelemetry
{
    internal static class StringExtensions
    {
        internal static object ToValueAsObject(this string str)
        {
            if (int.TryParse(str, out int intVal))
            {
                return intVal;
            }
            else if (double.TryParse(str, out double doubleVal))
            {
                return doubleVal;
            }
            else if (bool.TryParse(str, out bool boolVal))
            {
                return boolVal;
            }
            else if (DateTimeOffset.TryParse(str, out DateTimeOffset dateTimeVal))
            {
                return dateTimeVal;
            }
            else
            {
                return str;
            }
        }
    }
}