
namespace Sage.Core.Framework.Configuration
{
    public interface IConfigurationManager
    {
        IConfiguration SystemConfiguration { get; }

        IConfiguration ApplicationConfiguration { get; }
    }
}
