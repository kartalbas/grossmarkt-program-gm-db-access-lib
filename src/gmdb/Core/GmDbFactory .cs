namespace gmdb.Core
{
    using System;
    
    public class GmDbFactory
    {
        /// <summary>
        /// Creates a new instance of the GmDb class
        /// </summary>
        /// <param name="gmPath">Path to GM database</param>
        /// <param name="gmUserData">User data folder</param>
        /// <returns>An IGmDb implementation</returns>
        public static IGmDb Create(string gmPath, string gmUserData)
        {
            return new GmDb(gmPath, gmUserData);
        }
        
        /// <summary>
        /// Creates a new instance with custom configuration
        /// </summary>
        /// <param name="gmPath">Path to GM database</param>
        /// <param name="gmUserData">User data folder</param>
        /// <param name="enableLogging">Whether to enable logging</param>
        /// <returns>An IGmDb implementation</returns>
        public static IGmDb Create(string gmPath, string gmUserData, bool enableLogging)
        {
            var gmDb = new GmDb(gmPath, gmUserData);
            
            if (enableLogging)
            {
                gmDb.InitTraceSource();
            }
            
            return gmDb;
        }
    }
}