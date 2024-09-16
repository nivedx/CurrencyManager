using System.Reflection;

namespace CurrencyManager.Models.SystemModels
{
    public class ServiceConfig
    {
        private readonly IConfiguration _configuration;

        public ServiceConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConstructUri(string service, string endpoint, object model)
        {
            string key = $"ServiceConfig:Modules:{service}:EndPoints:{endpoint}:Path";
            string key2 = $"ServiceConfig:Modules:{service}:BaseUrl";
            string empty = string.Empty;
            string empty2 = string.Empty;
            empty = _configuration[key];
            empty2 = _configuration[key2];
            if (empty == null)
            {
                string errorMessage = $"Could not find end point configuration for module {service} / {endpoint}";
                throw new InvalidConfigurationException("InvalidConfiguration", errorMessage);
            }

            if (empty2 == null)
            {
                string errorMessage2 = $"Could not find base Url configuration for module {service} ";
                throw new InvalidConfigurationException("InvalidConfiguration", errorMessage2);
            }

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            IEnumerable<PropertyInfo> enumerable = model.GetType().GetProperties().AsEnumerable();
            foreach (PropertyInfo item in enumerable)
            {
                dictionary.Add(item.Name, item.GetValue(model));
            }

            string text = dictionary.Aggregate(empty, delegate (string current, KeyValuePair<string, object> parameter)
            {
                string oldValue = "{" + parameter.Key + "}";
                string newValue = ((parameter.Value == null) ? "" : parameter.Value.ToString());
                return current.Replace(oldValue, newValue);
            });
            return empty2 + text;
        }
    }
}
