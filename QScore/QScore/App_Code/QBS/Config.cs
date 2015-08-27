using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aexis;

namespace QBS
{
    /// <summary>
    /// The Config Class wraps certain functions for ease of use.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Returns the given name for the application.
        /// </summary>
        /// <returns>Returns the given name for the application.</returns>
        public static string AppName()
        {
            return Common.ConfigValue(1);
        }

        /// <summary>
        /// Returns the current system version.
        /// </summary>
        /// <returns>Returns the current system version.</returns>
        public static string Version()
        {
            return Common.ConfigValue(2);
        }

        /// <summary>
        /// Checks and retrieves the configuration for this item.
        /// </summary>
        /// <returns>True if configured this way, false otherwise.</returns>
        public static bool CaseSensitiveUsername()
        {
            return Convert.ToInt32(Common.ConfigValueWithDefault(3, "0")) == 1;
        }

        /// <summary>
        /// Checks and retrieves the configuration for this item.
        /// </summary>
        /// <returns>True if configured this way, false otherwise.</returns>
        public static bool CaseSensitivePassword()
        {
            return Convert.ToInt32(Common.ConfigValueWithDefault(4, "0")) == 1;
        }

        /// <summary>
        /// Checks and retrieves the configuration for this item.
        /// </summary>
        /// <returns>True if configured this way, false otherwise.</returns>
        public static bool UseCryptoPassword()
        {
            return Convert.ToInt32(Common.ConfigValueWithDefault(5, "0")) == 1;
        }

        /// <summary>
        /// Returns the number of records per page configured.
        /// </summary>
        /// <returns>Number of records per page configured.</returns>
        public static int RecordsPerPage()
        {
            int defaultRecordNumber = 20;
            int records = Convert.ToInt32(Common.ConfigValueWithDefault(6, defaultRecordNumber.ToString()));
            return records != 0 ? records : defaultRecordNumber;
        }
    }
}