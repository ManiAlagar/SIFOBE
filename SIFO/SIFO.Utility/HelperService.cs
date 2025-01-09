using SIFO.Model.Constant;
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
                errors.Add(Constants.INVALID_PAGE_NUMBER);

            if (pageSize <= 0)
                errors.Add(Constants.INVALID_PAGE_SIZE);

            if (!string.IsNullOrEmpty(filter) && filter.Length > 255)
                errors.Add(Constants.INVALID_FILTER_LENGTH);

            if (string.IsNullOrEmpty(sortColumn))
                errors.Add(Constants.INVALID_SORT_COLUMN);

            if (!string.IsNullOrEmpty(sortDirection) && !new[] { "asc", "desc" }.Contains(sortDirection.ToLower()))
                errors.Add(Constants.INVALID_SORT_DIRECTION);

            return errors;
        }
    }
}
