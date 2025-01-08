

using System.Reflection;

namespace SIFO.Utility.Implementations
{
    public class HelperService 
    {
        public static async Task<T> CleanModelAsync<T>(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            { 
                if (property.PropertyType == typeof(string))
                {
                    var currentValue = property.GetValue(model) as string;

                    if (currentValue != null)
                    {
                        await Task.Yield();

                        property.SetValue(model, currentValue.Trim());
                    }
                }
            }

            return model;
        }

        public static async Task<List<string>> ValidateGet(long pageNo, long pageSize, string? filter, string? sortColumn, string? sortDirection)
        {
            var errors = new List<string>();

            if (pageNo <= 0)
                errors.Add("Page number must be greater than 0");

            if (pageSize <= 0)
                errors.Add("Page size must be greater than 0");

            if (!string.IsNullOrEmpty(filter) && filter.Length > 255)
                errors.Add("Filter length cannot exceed 255 characters");

            if (string.IsNullOrEmpty(sortColumn))
                errors.Add("Invalid sort column");

            if (!string.IsNullOrEmpty(sortDirection) && !new[] { "asc", "desc" }.Contains(sortDirection.ToLower()))
                errors.Add("Invalid sort direction. It should be 'asc' or 'desc'");

            return errors;
        }
    }
}
