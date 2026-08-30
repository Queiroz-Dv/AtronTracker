using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Shared.Extensions;

/// <summary>
/// Persiste enums como o valor retornado por <see cref="EnumExtensions.GetDescription"/>.
/// A conversão é exata e não aplica normalização.
/// </summary>
public static class EnumStringConverter
{
    public static ValueConverter<TEnum, string> Create<TEnum>()
        where TEnum : struct, Enum
    {
        return new ValueConverter<TEnum, string>(
            value => value.GetDescription(),
            value => EnumExtensions.GetEnumFromDescriptionStrict<TEnum>(value));
    }
}
