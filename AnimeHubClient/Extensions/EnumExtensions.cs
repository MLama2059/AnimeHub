using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AnimeHubClient.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            if (enumValue == null)
            {
                return string.Empty;
            }

            // Use GetCustomAttribute<T> for cleaner code (requires System.Reflection)
            var displayAttribute = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>();

            // Return the Display Name, or fall back to the string representation
            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}
