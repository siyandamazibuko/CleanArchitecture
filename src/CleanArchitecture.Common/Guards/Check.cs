using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CleanArchitecture.Common.Guards
{
  public static class Check
  {
    public static T NotNull<T>(T value, string parameterName)
    {
      if ((object) value == null)
      {
        Check.NotEmpty(parameterName, nameof (parameterName));
        throw new ArgumentNullException(parameterName);
      }
      return value;
    }

    public static T NotNull<T>(T value, string parameterName, string propertyName)
    {
      if ((object) value == null)
      {
        Check.NotEmpty(parameterName, nameof (parameterName));
        Check.NotEmpty(propertyName, nameof (propertyName));
        throw new ArgumentException("The property '" + propertyName + "' of the argument '" + parameterName + "' cannot be null.");
      }
      return value;
    }

    public static IReadOnlyList<T> NotEmpty<T>(
      IReadOnlyList<T> value,
      string parameterName)
    {
      Check.NotNull<IReadOnlyList<T>>(value, parameterName);
      if (value.Count == 0)
      {
        Check.NotEmpty(parameterName, nameof (parameterName));
        throw new ArgumentException("The collection argument '" + parameterName + "' must contain at least one element.");
      }
      return value;
    }

    public static string NotEmpty(string value, string parameterName)
    {
      Exception exception = (Exception) null;
      if (value == null)
        exception = (Exception) new ArgumentNullException(parameterName);
      else if (value.Trim().Length == 0)
        exception = (Exception) new ArgumentException("The string argument '" + parameterName + "' cannot be empty.");
      if (exception != null)
      {
        Check.NotEmpty(parameterName, nameof (parameterName));
        throw exception;
      }
      return value;
    }

    public static string NullButNotEmpty(string value, string parameterName)
    {
      switch (value)
      {
        case "":
          Check.NotEmpty(parameterName, nameof (parameterName));
          throw new ArgumentException("The string argument '" + parameterName + "' cannot be empty.");
        default:
          return value;
      }
    }

    public static int NotZero(int value, string parameterName)
    {
      var exception = (Exception) null;
      if (value <= 0)
        exception = new ArgumentException("The integer argument '" + parameterName + "' cannot be zero or less.");
      if (exception != null)
      {
        Check.NotEmpty(parameterName, nameof (parameterName));
        throw exception;
      }
      return value;
    }

    public static IReadOnlyList<T> HasNoNulls<T>(
      IReadOnlyList<T> value,
      string parameterName)
      where T : class
    {
      Check.NotNull<IReadOnlyList<T>>(value, parameterName);
      if (value.Any<T>((Func<T, bool>) (e => (object) e == null)))
      {
        Check.NotEmpty(parameterName, nameof (parameterName));
        throw new ArgumentException(parameterName);
      }
      return value;
    }

    public static T IsDefined<T>(T value, string parameterName) where T : struct
    {
      if (!Enum.IsDefined(typeof (T), (object) value))
      {
        Check.NotEmpty(parameterName, nameof (parameterName));
        throw new ArgumentException(string.Format("The value provided for argument '{0}' must be a valid value of enum type '{1}'.", (object) parameterName, (object) typeof (T)));
      }
      return value;
    }

    public static Type ValidEntityType(Type value, string parameterName)
    {
      if (!value.GetTypeInfo().IsClass)
      {
        Check.NotEmpty(parameterName, nameof (parameterName));
        throw new ArgumentException(string.Format("The entity type '{0}' provided for the argument '{1}' must be a reference type.", (object) parameterName, (object) value));
      }
      return value;
    }

    public static double ConditionalAssign(
      this double? input,
      double? fallbackValue,
      double fallbackDefault = 0.0)
    {
      return input.HasValue ? input.Value : fallbackValue.GetValueOrDefault(fallbackDefault);
    }

    public static string ConditionalAssign(
      this string input,
      string fallbackValue,
      string fallbackDefault = "")
    {
      if (input != null)
        return input;
      return fallbackValue != null ? fallbackValue : fallbackDefault;
    }
  }
}
