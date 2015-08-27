using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aexis;
using QScore.lang;
using QBS.ACL;
using QBS.Data;

namespace QBS
{
    /// <summary>
    /// Auxiliar Class for easier session handling.
    /// </summary>
    public static class SessionHandler
    {
        private static string _id = "id";
        private static string _modules = "modules";
        private static string _history = "history";
        private static string _sessionVars = "sessionvars";
        
        /// <summary>
        /// Gets or sets the current UserId.
        /// </summary>
        public static int Id
        {
            get
            {
                try
                {
                    return (int)HttpContext.Current.Session[_id];
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            set
            {
                HttpContext.Current.Session[_id] = value;
            }
        }

        /// <summary>
        /// Gets or sets the current Modules.
        /// </summary>
        public static Dictionary<string, bool> Modules
        {
            get
            {
                try
                {
                    return (Dictionary<string, bool>)HttpContext.Current.Session[_modules];
                }
                catch (Exception ex)
                {
                    return new Dictionary<string, bool>();
                }
            }
            set
            {
                HttpContext.Current.Session[_modules] = value;
            }
        }

        /// <summary>
        /// Gets or sets the current History.
        /// </summary>
        public static List<string> History
        {
            get
            {
                try
                {
                    return (List<string>)HttpContext.Current.Session[_history];
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
            }
            set
            {
                HttpContext.Current.Session[_history] = value;
            }
        }

        /// <summary>
        /// Gets or sets the current Session Variables.
        /// </summary>
        public static Dictionary<string, string> SessionVars
        {
            get
            {
                try
                {
                    return (Dictionary<string, string>)HttpContext.Current.Session[_sessionVars];
                }
                catch (Exception ex)
                {
                    return new Dictionary<string, string>();
                }
            }
            set
            {
                HttpContext.Current.Session[_sessionVars] = value;
            }
        }

        /// <summary>
        /// Retrieves the current HttpContext.
        /// </summary>
        /// <returns>The current HttpContext</returns>
        public static HttpContext GetContext()
        {
            return HttpContext.Current;
        }

        /// <summary>
        /// Adds a string to a Dictionary. Mimics the session object but keeps it cleaner, for ease of disposal.
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="value">Value to add.</param>
        public static void AddToSessionVars(string key, string value)
        {
            Dictionary<string, string> currentSessionVars = SessionHandler.SessionVars;
            try
            {
                currentSessionVars.Add(key, value);
            }
            catch (ArgumentException)
            {
                currentSessionVars[key] = value;
            }
            catch (Exception) {}
            SessionHandler.SessionVars = currentSessionVars;
        }

        /// <summary>
        /// Returns the value of the SessionVar Dictionary.
        /// </summary>
        /// <param name="key">The key to retrieve the value for.</param>
        /// <returns>Retrieves the current value.</returns>
        public static string SessionVar(string key)
        {
            string retval = "";
            try
            {
                retval = SessionHandler.SessionVars[key];
            }
            catch(Exception ex) {}
            return retval;
        }

        /// <summary>
        /// Clears the session vars.
        /// </summary>
        public static void ClearSessionVars()
        {
            SessionHandler.SessionVars = new Dictionary<string, string>();
        }

        /// <summary>
        /// Adds the given page to the current history.
        /// </summary>
        /// <param name="page">The URL of the page, complete with the ?params</param>
        public static void AddToHistory(string page)
        {
            List<string> currentHistory = SessionHandler.History;
            currentHistory.Add(page);
            SessionHandler.History = currentHistory;
        }

        /// <summary>
        /// Adds the given page to the current history, additionaly; it deletes the entire history.
        /// </summary>
        /// <param name="page"></param>
        public static void ClearAndAddToHistory(string page)
        {
            ClearHistory();
            AddToHistory(page);
        }

        /// <summary>
        /// Clears the entire history;
        /// </summary>
        public static void ClearHistory()
        {
            SessionHandler.History = new List<string>();
        }

        /// <summary>
        /// Removes the last page in the history.
        /// </summary>
        public static void RemoveLastPageFromHistory()
        {
            List<string> currentHistory = SessionHandler.History;
            currentHistory.RemoveAt(currentHistory.Count - 1);
            SessionHandler.History = currentHistory;
        }

        /// <summary>
        /// Retrieves the last page in the history.
        /// </summary>
        /// <returns>The URL of the page, complete with ?params.</returns>
        public static string LastPageFromHistory()
        {
            string retval = "";
            List<string> currentHistory = SessionHandler.History;
            try
            {
                retval = currentHistory.ElementAt(currentHistory.Count - 1);
            }
            catch(ArgumentOutOfRangeException ex) {}
            return retval;
        }

        /// <summary>
        /// Retrieves a key from POST and saves it to session, otherwise retrieves directly from the Session object.
        /// </summary>
        /// <param name="key">The Key to lookup.</param>
        /// <returns>The current Form or Session value.</returns>
        public static string RequestSession(string key)
        {
            string retval = "";
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form[key]))
            {
                SessionHandler.AddToSessionVars(key, HttpContext.Current.Request.Form[key]);
            }
            try
            {
                retval = SessionHandler.SessionVars[key];
            }
            catch (Exception ex) { }
            return retval;
        }

        /// <summary>
        /// Retrieves a key from POST and saves it to session, otherwise retrieves directly from the Session object.
        /// </summary>
        /// <param name="key">The Key to lookup.</param>
        /// <returns>The current Form or Session value as an integer.</returns>
        public static int RequestSessionInt(string key)
        {
            int retval = 0;
            try
            {
                retval = Convert.ToInt32(RequestSession(key));
            }
            catch (Exception ex) { }
            return retval;
        }

    }
}