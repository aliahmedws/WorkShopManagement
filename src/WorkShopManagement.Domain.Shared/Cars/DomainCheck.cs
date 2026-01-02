using Volo.Abp;

namespace WorkShopManagement.Cars;

public static class DomainCheck
{
    public static string? TrimOptional(string? value, string parameterName, int maxLength)
    {
        if (value is null)
        {
            return null;
        }

        value = value.Trim();
        if (value.Length == 0)
        {
            return null;
        }

        return Check.Length(value, parameterName, maxLength);
    }
}
