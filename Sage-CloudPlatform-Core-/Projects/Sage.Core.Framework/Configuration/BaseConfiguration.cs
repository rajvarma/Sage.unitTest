namespace Sage.Core.Framework.Configuration
{
    using System.Collections.Specialized;
    using System.Configuration;

    internal class BaseConfiguration : IConfiguration
    {
        private readonly StringDictionary configurationItems;

        public BaseConfiguration() : this(new StringDictionary())
        {

        }
        
        public BaseConfiguration(StringDictionary configurationItems)
        {
            this.configurationItems = configurationItems;
        }
        
        public string this[string key]
        {
            get { return this.GetConfigurationValue(key); }
        }

        public bool ContainsKey(string key)
        {
            return this.configurationItems.ContainsKey(key);
        }

        protected string GetConfigurationValue(string key)
        {
            if (this.ContainsKey(key))
                return this.configurationItems[key];

            var value = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrEmpty(value))
            {
                this.configurationItems.Add(key, value);
            }
            return value;
        }
    }
}
