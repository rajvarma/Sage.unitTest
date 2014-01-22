namespace Sage.Core.Framework.Configuration
{
    public class BaseConfigurationManager : IConfigurationManager
    {
        public BaseConfigurationManager()
        {
            this.SystemConfiguration = new BaseConfiguration();
            this.ApplicationConfiguration = new BaseConfiguration();
        }

        public IConfiguration SystemConfiguration { get; private set; }
        public IConfiguration ApplicationConfiguration { get; private set; }
    }
}
