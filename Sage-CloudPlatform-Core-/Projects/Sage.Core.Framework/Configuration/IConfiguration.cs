namespace Sage.Core.Framework.Configuration
{
    using System;

    public interface IConfiguration
    {
        /// <summary>
        /// Get's the configuration value.   returns null if key does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        String this[string key] { get; }


        /// <summary>
        /// Check the existence of a key in the configuration
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string key);
    }
}
