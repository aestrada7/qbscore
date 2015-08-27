using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace Aexis
{
    /// <summary>
    /// The Common Class of the Aexis Framework provides a lot of database wrapping functionality, as well as system configuration.
    /// </summary>
    public static class Common
    {
        public static HttpContextBase context; //For debug purposes.

        #region Connection String
        private static string connectionString = ConfigurationManager.ConnectionStrings["QBSConnectionString"].ToString();

        /// <summary>
        /// Sets the connection string to be used throughout the entire class. This method should be called before any other method 
        /// of this class.
        /// </summary>
        /// <param name="cstring">The connection string that will be used.</param>
        public static void SetConnectionString(string cstring)
        {
            connectionString = cstring;
        }

        /// <summary>
        /// Returns the given connection string.
        /// </summary>
        public static string GetConnectionString()
        {
            return connectionString;
        }
        #endregion

        #region System Configuration
        /// <summary>
        /// Gets the configuration value of the application from the given idConfig.
        /// </summary>
        /// <param name="idInfo">The configuration id.</param>
        /// <returns>The configuration value of the given idConfig.</returns>
        public static string ConfigValue(int idConfig)
        {
            return ConfigValueWithDefault(idConfig, "");
        }

        /// <summary>
        /// Gets the configuration value of the application from the given idInfo.
        /// </summary>
        /// <param name="idInfo">The configuration Id.</param>
        /// <param name="defaultValue">The default value if the configuration value is empty.</param>
        /// <returns>The configuration value, or the default if the configuration value is empty.</returns>
        public static string ConfigValueWithDefault(int idConfig, string defaultValue)
        {
            var retval = GetDescripcion("SystemConfig", "IdConfig", "Value", idConfig);
            return String.IsNullOrEmpty(retval) ? defaultValue : retval;
        }
        #endregion

        #region Common Database Functions
        /// <summary>
        /// Gets the single value of a table using the ID as condition.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <param name="idField">Name of the Id field.</param>
        /// <param name="valueField">Name of the value field.</param>
        /// <param name="id">The actual Id that will be used to filter.</param>
        /// <returns>The value of valueField filtered by id.</returns>
        public static string GetDescripcion(string table, string idField, string valueField, int id)
        {
            string sqlStatement = "SELECT " + valueField + " FROM " + table + " WHERE " + idField + "=" + id.ToString();
            return GetBD(valueField, sqlStatement);
        }

        /// <summary>
        /// Gets the value of a given field in a given sqlStatement.
        /// </summary>
        /// <param name="field">The field that will be retrieved.</param>
        /// <param name="sqlStatement">A valid SQL Statement that includes the given field.</param>
        /// <returns>The value of the given field in the sqlStatement.</returns>
        public static string GetBD(string field, string sqlStatement)
        {
            string retval = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlStatement, conn);
                    conn.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    if (sqlDataReader.HasRows && sqlDataReader.Read())
                    {
                        retval = sqlDataReader[field].ToString();
                    }
                    sqlCommand.Dispose();
                    sqlDataReader.Dispose();
                    conn.Dispose();
                }
                catch (SqlException ex)
                {
                    PrintSQLError(sqlStatement, ex.Message);
                }
            }
            return retval;
        }

        /// <summary>
        /// Wrapper for the GetBD function. It also gets the value of a given field in a given sqlStatement, but treats the result as an int.
        /// </summary>
        /// <param name="field">The field that will be retrieved.</param>
        /// <param name="sqlStatement">A valid SQL Statement that includes the given field.</param>
        /// <returns>The value of the given field in the sqlStatement, as an int.</returns>
        public static int GetBDNum(string field, string sqlStatement)
        {
            try
            {
                return Convert.ToInt32(GetBD(field, sqlStatement));
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Builds a list of the given field in a given sqlStatement.
        /// </summary>
        /// <param name="field">The field that will be listed.</param>
        /// <param name="sqlStatement">A valid SQL Statement that includes the given field.</param>
        /// <param name="asString">Boolean value. If true, it treats the CSV values as a string, adding the appropriate apostrophes needed.</param>
        /// <returns>A Comma Separated Value list of the given field in a given sqlStatement.</returns>
        public static string GetBDList(string field, string sqlStatement, bool asString)
        {
            string retval = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlStatement, conn);
                    conn.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.HasRows && sqlDataReader.Read())
                    {
                        var data = sqlDataReader[field].ToString();
                        if (asString) data = "'" + data + "'";
                        retval = StrAdd(retval, ",", data);
                    }
                    sqlDataReader.Close();
                    sqlCommand.Dispose();
                    sqlDataReader.Dispose();
                    conn.Dispose();
                }
                catch (SqlException ex)
                {
                    PrintSQLError(sqlStatement, ex.Message);
                }
            }
            return retval;
        }

        /// <summary>
        /// Builds a List of ints of the given field in a given sqlStatement.
        /// </summary>
        /// <param name="field">The field that will be listed.</param>
        /// <param name="sqlStatement">A valid SQL Statement that includes the given field.</param>
        /// <returns>A List&lt;int&gt; of the given field in a given sqlStatement.</returns>
        public static List<int> GetIdList(string field, string sqlStatement)
        {
            List<int> retval = new List<int>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlStatement, conn);
                    conn.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.HasRows && sqlDataReader.Read())
                    {
                        retval.Add(Convert.ToInt32(sqlDataReader[field].ToString()));
                    }
                    sqlDataReader.Close();
                    sqlCommand.Dispose();
                    sqlDataReader.Dispose();
                    conn.Dispose();
                }
                catch (SqlException ex)
                {
                    PrintSQLError(sqlStatement, ex.Message);
                }
            }
            return retval;
        }

        /// <summary>
        /// Returns a List of objects with the number of values, every property is the name of the field.
        /// </summary>
        /// <param name="sqlStatement">A valid SQL Statement.</param>
        /// <returns>A List of objects.</returns>
        public static List<Dictionary<string, string>> GetRS(string sqlStatement)
        {
            List<string> fieldsList = new List<string>();
            List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlStatement, conn);
                    conn.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    for (int i = 0; i < sqlDataReader.FieldCount; i++)
                    {
                        fieldsList.Add(sqlDataReader.GetName(i));
                    }
                    while (sqlDataReader.HasRows && sqlDataReader.Read())
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        foreach (string field in fieldsList)
                        {
                            dictionary.Add(field, sqlDataReader[field].ToString());
                        }
                        records.Add(dictionary);
                    }
                    sqlDataReader.Close();
                    sqlCommand.Dispose();
                    sqlDataReader.Dispose();
                    conn.Dispose();
                }
                catch (SqlException ex)
                {
                    PrintSQLError(sqlStatement, ex.Message);
                }
            }
            return records;
        }

        /// <summary>
        /// Retrieves the existing fields in a given table.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns>A CSV of the fields.</returns>
        public static string GetTableFields(string table)
        {
            string retval = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand("SP_COLUMNS " + table, conn);
                    conn.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.HasRows && sqlDataReader.Read())
                    {
                        var name = sqlDataReader["COLUMN_NAME"].ToString();
                        var kind = sqlDataReader["TYPE_NAME"].ToString(); //unused
                        retval = StrAdd(retval, ",", name);
                    }
                    sqlDataReader.Close();
                    sqlCommand.Dispose();
                    sqlDataReader.Dispose();
                    conn.Dispose();
                }
                catch (SqlException ex)
                {
                    PrintSQLError(table, ex.Message);
                }
            }
            return retval;
        }

        /// <summary>
        /// Checks if a given sqlStatement exists or not in the database.
        /// </summary>
        /// <param name="sqlStatement">A valid SQL Statement.</param>
        /// <returns>True if it exists, false otherwise.</returns>
        public static bool BDExists(string sqlStatement)
        {
            return GetBDNum("existe", "SELECT (CASE WHEN EXISTS(" + sqlStatement + ") THEN 1 ELSE 0 END) AS existe") != 0;
        }

        /// <summary>
        /// Executes a given SQL Statement.
        /// </summary>
        /// <param name="sqlStatement">A valid SQL Statement.</param>
        public static void BDExecute(string sqlStatement)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand sqlCommand = new SqlCommand(sqlStatement, conn);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    conn.Dispose();
                }
                catch (SqlException ex)
                {
                    PrintSQLError(sqlStatement, ex.Message);
                }
            }
        }

        /// <summary>
        /// Prints a given SQL Error.
        /// </summary>
        /// <param name="sqlStatement">The SQL Statement that generated the exception.</param>
        /// <param name="exceptionMessage">The Exception's message.</param>
        public static void PrintSQLError(string sqlStatement, string exceptionMessage)
        {
            try
            {
                if (ConfigurationManager.AppSettings["Environment"].ToString() == "Production")
                {
                    HttpContext.Current.Response.Write("Database Error: " + exceptionMessage);
                }
                else
                {
                    HttpContext.Current.Response.Write("SQL Error: " + sqlStatement + "<br />" + exceptionMessage);
                }
            }
            catch (NullReferenceException ex)
            {
                HttpContext.Current.Response.Write("SQL Error: " + sqlStatement + "<br />" + exceptionMessage);
            }
        }
        #endregion

        #region Security
        /// <summary>
        /// Removes unsafe characters prior to entering them to the database
        /// </summary>
        /// <param name="value">The varchar value</param>
        /// <returns>The safe string</returns>
        public static string SQSF(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                value = RemoveIntrusiveSQL(value);
                value = RemoveIntrusiveJS(value);
                value = value.Replace("&NBSP;", "&nbsp;");
                value = value.Replace("'", "''");
            }
            return value;
        }

        /// <summary>
        /// Removes unsafe characters
        /// </summary>
        /// <param name="value">The varchar value</param>
        /// <returns>The safe string</returns>
        public static string RemoveIntrusiveChars(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                value = value.Replace(";", "");
                value = value.Replace(")", "");
                value = value.Replace("(", "");
                value = value.Replace("'", "");
                value = value.Replace("\\", "");
            }
            return value;
        }

        /// <summary>
        /// Removes SQL Commands from a string, to avoid SQL injection.
        /// </summary>
        /// <param name="value">The string to clean</param>
        /// <returns>The string without intrusive SQL</returns>
        public static string RemoveIntrusiveSQL(string value)
        {
            value = Regex.Replace(value, ";|insert |delete |drop |update |exec |convert |db_name|alter |modify |varchar |shell |declare |create |union ", "");
            return value;
        }

        /// <summary>
        /// Removes JavaScript from a string, to avoid XSS.
        /// </summary>
        /// <param name="value">The string to clean</param>
        /// <returns>The string without intrusive JavaScript</returns>
        public static string RemoveIntrusiveJS(string value)
        {
            value = Regex.Replace(value, "alert(|);|:expression", "");
            return value;
        }
        #endregion

        #region Common Functions
        /// <summary>
        /// Concatenates a given string1 with a given string2 using the connector between them. If either string1 or string2 are empty,
        /// it returns the one that isn't empty without concatenating.
        /// </summary>
        /// <param name="string1">A string.</param>
        /// <param name="connector">A string used to concatenate string1 and string2.</param>
        /// <param name="string2">A second string.</param>
        /// <returns>The concatenated string.</returns>
        public static string StrAdd(string string1, string connector, string string2)
        {
            var retval = string1;
            if (!String.IsNullOrEmpty(string1) && !String.IsNullOrEmpty(string2)) retval = retval + connector;
            return retval + string2;
        }

        /// <summary>
        /// Replaces tokens %%0, %%1, ... %%n on a given string with the values given on the csv parameter.
        /// </summary>
        /// <param name="string1">The string containing the tokens.</param>
        /// <param name="csv">The CSV which includes the replacement for all these tokens.</param>
        /// <returns>The newly formed string</returns>
        public static string StrLang(string string1, string csv)
        {
            string[] tokens = CSVToArray(csv);
            int i = 0;
            for (i = 0; i < tokens.Length; i++)
            {
                string1 = string1.Replace("%%" + i, tokens[i]);
            }
            return string1;
        }

        /// <summary>
        /// Transforms a given CSV string into an Array.
        /// </summary>
        /// <param name="csv">The given CSV string.</param>
        /// <returns>The Array.</returns>
        public static string[] CSVToArray(string csv)
        {
            if (String.IsNullOrEmpty(csv))
            {
                return "".Split(',');
            }
            else
            {
                return csv.Split(',');
            }
        }

        /// <summary>
        /// Transforms a given CSV string into an Array.
        /// </summary>
        /// <param name="separator">The separator to use.</param>
        /// <param name="csv">The given CSV string.</param>
        /// <returns>The Array.</returns>
        public static string[] CSVToArray(char separator, string csv)
        {
            return csv.Split(separator);
        }

        /// <summary>
        /// Returns the Length of a CSV string.
        /// </summary>
        /// <param name="csv">The given CSV string.</param>
        /// <returns>The CSV Length.</returns>
        public static int CSVCount(string csv)
        {
            return CSVToArray(csv).Length;
        }

        /// <summary>
        /// Converts a given string into an encrypted string using the SHA512 one-way encryption method.
        /// </summary>
        /// <param name="value">The string to encrypt.</param>
        /// <returns>An encoded string.</returns>
        public static string EncryptSHA512(string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            SHA512 sha512M = new SHA512Managed();
            var hash = sha512M.ComputeHash(data);
            return hash.ToString();
        }

        /// <summary>
        /// Shuffles a given List.
        /// </summary>
        /// <typeparam name="T">The kind of list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        #endregion

        #region Time Functions
        /// <summary>
        /// Returns the current Unix TimeStamp.
        /// </summary>
        /// <returns>An Int32 with the current Unix TimeStamp.</returns>
        public static Int32 GetCurrentTimeStamp()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        /// <summary>
        /// Returns a Unix TimeStamp for the provided DateTime Object.
        /// </summary>
        /// <param name="dateTime">The DateTime Object to convert.</param>
        /// <returns>An Int32 with the Unix TimeStamp.</returns>
        public static Int32 TimeStampFromDateTime(DateTime dateTime)
        {
            return (Int32)(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        /// <summary>
        /// Converts a Unix TimeStamp to a local time DateTime Object.
        /// </summary>
        /// <param name="timeStamp">The Unix TimeStamp to convert.</param>
        /// <returns>A DateTime Object.</returns>
        public static DateTime DateTimeFromTimeStamp(double timeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(timeStamp).ToLocalTime();
            return dateTime;
        }

        #endregion

        #region Request
        /// <summary>
        /// Checks if a field is missing or invalid
        /// </summary>
        /// <param name="fieldName">The name of the field</param>
        /// <param name="requestValue">The requested field</param>
        /// <returns>The name of the field in case it's missing</returns>
        public static string MissingField(string fieldName, string requestValue)
        {
            if (String.IsNullOrEmpty(requestValue))
            {
                return fieldName;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Appearance
        /// <summary>
        /// Switches from one class to another, useful for table rows.
        /// </summary>
        /// <param name="value">The current CSS class name</param>
        /// <returns>The new CSS class name</returns>
        public static string SwitchClass(string value)
        {
            string retval = "cellDark";
            if (value == retval)
            {
                retval = "cellLight";
            }
            return retval;
        }

        /// <summary>
        /// Converts a string with BBCode into a valid HTML string.
        /// </summary>
        /// <param name="value">The BBCode string.</param>
        /// <returns>The HTML string.</returns>
        public static string BBCodeToHTML(string value)
        {
            //string retval = value.Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
            string retval = value;
            Regex regExp;

            ////Line Breaks
            regExp = new Regex(@"\r\n?|\n");
            retval = regExp.Replace(retval, "<br />");

            ////Regex for URL tag without anchor 
            regExp = new Regex(@"\[url\]([^\]]+)\[\/url\]");
            retval = regExp.Replace(retval, "<a href=\"$1\">$1</a>");

            ////Regex for URL with anchor 
            regExp = new Regex(@"\[url=([^\]]+)\]([^\]]+)\[\/url\]");
            retval = regExp.Replace(retval, "<a href=\"$1\">$2</a>");

            ////Image regex without size
            regExp = new Regex(@"\[img\]([^\]]+)\[\/img\]");
            retval = regExp.Replace(retval, "<img src=\"$1\" />");

            ////Image regex with size
            regExp = new Regex(@"\[img=([^\]]+)x([^\]]+)\]([^\]]+)\[\/img\]");
            retval = regExp.Replace(retval, "<img src=\"$3\" width=\"$1\" height=\"$2\" />");

            ////Bold text 
            regExp = new Regex(@"\[b\](.+?)\[\/b\]");
            retval = regExp.Replace(retval, "<b>$1</b>");

            ////Italic text 
            regExp = new Regex(@"\[i\](.+?)\[\/i\]");
            retval = regExp.Replace(retval, "<i>$1</i>");

            ////Centered text
            regExp = new Regex(@"\[center\](.+?)\[\/center\]");
            retval = regExp.Replace(retval, "<center>$1</center>");

            ////Centered text
            regExp = new Regex(@"\[center\](.+?)\[\/center\]");
            retval = regExp.Replace(retval, "<center>$1</center>");

            ////Superscript text 
            regExp = new Regex(@"\[sup\](.+?)\[\/sup\]");
            retval = regExp.Replace(retval, "<sup>$1</sup>");

            ////Subscript text 
            regExp = new Regex(@"\[sub\](.+?)\[\/sub\]");
            retval = regExp.Replace(retval, "<sub>$1</sub>");

            ////Unsorted list
            regExp = new Regex(@"\[ul\](.+?)\[\/ul\]");
            retval = regExp.Replace(retval, "<ul>$1</ul>");

            ////Ordered list
            regExp = new Regex(@"\[ol\](.+?)\[\/ol\]");
            retval = regExp.Replace(retval, "<ol>$1</ol>");

            ////List item
            regExp = new Regex(@"\[li\](.+?)\[\/li\]");
            retval = regExp.Replace(retval, "<li>$1</li>");

            ////Table
            regExp = new Regex(@"\[table\](.+?)\[\/table\]");
            retval = regExp.Replace(retval, "<table>$1</table>");

            ////Table Header
            regExp = new Regex(@"\[th\](.+?)\[\/th\]");
            retval = regExp.Replace(retval, "<th>$1</th>");

            ////Table Row
            regExp = new Regex(@"\[tr\](.+?)\[\/tr\]");
            retval = regExp.Replace(retval, "<tr>$1</tr>");

            ////Table Division
            regExp = new Regex(@"\[td\](.+?)\[\/td\]");
            retval = regExp.Replace(retval, "<td>$1</td>");

            ////Code
            regExp = new Regex(@"\[code\](.+?)\[\/code\]");
            retval = regExp.Replace(retval, "<code>$1</code>");

            ////Font
            regExp = new Regex(@"\[font=([^\]]+)\](.+?)\[\/font\]");
            retval = regExp.Replace(retval, "<span style=\"font-family: '$1'\">$2</span>");

            ////Font size
            regExp = new Regex(@"\[size=([^\]]+)\]([^\]]+)\[\/size\]");
            string fontSize = regExp.Replace(retval, "$1");
            string fontSizePx = "12";
            switch (fontSize)
            {
                case "1":
                    fontSizePx = "10";
                    break;
                case "2":
                    fontSizePx = "13";
                    break;
                case "3":
                    fontSizePx = "16";
                    break;
                case "4":
                    fontSizePx = "18";
                    break;
                case "5":
                    fontSizePx = "24";
                    break;
                case "6":
                    fontSizePx = "32";
                    break;
                case "7":
                    fontSizePx = "48";
                    break;
            }
            retval = regExp.Replace(retval, "<span style=\"font-size: " + fontSizePx + "px\">$2</span>");

            ////Font color
            regExp = new Regex(@"\[color=([^\]]+)\]([^\]]+)\[\/color\]");
            retval = regExp.Replace(retval, "<span style=\"color: $1\">$2</span>");

            return retval;
        }

        /// <summary>
        /// Converts a given Hex string color to a Color.
        /// </summary>
        /// <param name="hex">The color to convert (ex. 0077CC)</param>
        /// <param name="alpha">The alpha value from 0-255.</param>
        /// <returns>A Color with the Hex color.</returns>
        public static Color HexToColor(string hex, int alpha)
        {
            if (String.IsNullOrEmpty(hex) || (hex.Length != 6))
            {
                return Color.Empty;
            }
            string r = hex.Substring(0, 2);
            string g = hex.Substring(2, 2);
            string b = hex.Substring(4, 2);
            Color color = Color.Empty;
            try
            {
                int ri = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                int gi = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                int bi = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                color = Color.FromArgb(alpha, ri, gi, bi);
            }
            catch (Exception ex)
            {
                return Color.Empty;
            }
            return color;
        }

        /// <summary>
        /// Returns a Hex color between two given hex colors. Useful for tables.
        /// </summary>
        /// <param name="hexColor1">The first color.</param>
        /// <param name="hexColor2">The second color.</param>
        /// <param name="value">hexColor1 is the equivalent for 0%, hexColor2 the equivalent for 100%. Value skews the result between these two. Value must be a number between 0 and 1.</param>
        /// <returns>A Hex color.</returns>
        public static string HexColorFromValue(string hexColor1, string hexColor2, double value)
        {
            if (String.IsNullOrEmpty(hexColor1) || (hexColor1.Length != 6))
            {
                return "";
            }
            if (String.IsNullOrEmpty(hexColor2) || (hexColor2.Length != 6))
            {
                return "";
            }
            else
            {
                string r = hexColor1.Substring(0, 2);
                string g = hexColor1.Substring(2, 2);
                string b = hexColor1.Substring(4, 2);
                string r2 = hexColor2.Substring(0, 2);
                string g2 = hexColor2.Substring(2, 2);
                string b2 = hexColor2.Substring(4, 2);
                try
                {
                    int ri = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                    int gi = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                    int bi = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                    int ri2 = Int32.Parse(r2, System.Globalization.NumberStyles.HexNumber);
                    int gi2 = Int32.Parse(g2, System.Globalization.NumberStyles.HexNumber);
                    int bi2 = Int32.Parse(b2, System.Globalization.NumberStyles.HexNumber);
                    double rf = (ri * value) + (ri2 * (1 - value));
                    double gf = (gi * value) + (gi2 * (1 - value));
                    double bf = (bi * value) + (bi2 * (1 - value));
                    return Convert.ToInt32(rf).ToString("X2") + Convert.ToInt32(gf).ToString("X2") + Convert.ToInt32(bf).ToString("X2");
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
        #endregion
    }
}
