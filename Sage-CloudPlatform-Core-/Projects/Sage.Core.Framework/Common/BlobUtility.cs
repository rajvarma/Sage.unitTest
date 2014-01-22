using Sage.Core.Framework.Configuration;

namespace Sage.Core.Framework.Common
{
    public static class BlobUtility
    {

        public static string GetConnectionString(IConfigurationManager configurationManager)
        {
            return configurationManager.SystemConfiguration[SystemStorageConnectionString];
        }

        public const string NameRegEx = @"[a-z0-9$&!*`,.()+-]+";
        public const string PathRegEx = @"^[a-z0-9]+(-[a-z0-9]+)*$";

        #region Private properties

        private const string SystemStorageConnectionString = "SystemStorageConnectionString";

        #endregion
    }
}
