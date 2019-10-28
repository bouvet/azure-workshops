using System.Collections.Generic;

namespace AzureWorkshopApp.Models
{
    public class AzureStorageConfigValidationResult
    {
        public Dictionary<string, string> Errors { get; private set; } = new Dictionary<string, string>();

        public void AddError(string key, string value)
        {
            Errors.Add(key, value);
        }

        public bool IsValid()
        {
            return Errors.Count == 0;
        }

        public Dictionary<string, string> GetErrors()
        {
            return Errors;
        }
    }
}