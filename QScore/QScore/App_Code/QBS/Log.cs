using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aexis;
using Aexis.Web;
using QScore.lang;
using QBS.ACL;
using QBS.Data;

namespace QBS
{
    /// <summary>
    /// The LogKind is an enumerator for the different values the Kind property can use.
    /// </summary>
    public static class LogKind
    {
        public const int GENERAL = 0;
        public const int CREATE = 1;
        public const int UPDATE = 2;
        public const int DELETE = 3;
        public const int SESSION = 4;

        /// <summary>
        /// Gives the friendly name of a Log Kind.
        /// </summary>
        /// <param name="kind">An integer containing the LogKind key.</param>
        /// <returns>The friendly name of a Log Kind.</returns>
        public static string Name(int kind)
        {
            string retval = "";
            switch (kind)
            {
                case GENERAL:
                    retval = Text.General;
                    break;
                case CREATE:
                    retval = Text.log_Create;
                    break;
                case UPDATE:
                    retval = Text.log_Update;
                    break;
                case DELETE:
                    retval = Text.log_Delete;
                    break;
                case SESSION:
                    retval = Text.Session;
                    break;
            }
            return retval;
        }
    }

    /// <summary>
    /// The SystemLog Class mimics its database counterpart. To add a value use the static class/method Log.Add
    /// </summary>
    public class SystemLog
    {
        public int IdLog = 0;
        public int IdUser = 0;
        public int Kind = LogKind.GENERAL;
        public int IdModule = 0;
        public int IdRelated = 0;
        public string Description = "";
        public DateTime TimeStamp = new DateTime();

        /// <summary>
        /// Constructor, no arguments received.
        /// </summary>
        public SystemLog() { }

        /// <summary>
        /// Overload of the constructor. Receives a IdLog and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdLog of the log entry to retrieve information from.</param>
        public SystemLog(int id)
        {
            List<Dictionary<string, string>> log = Common.GetRS("SELECT * FROM SystemLog WHERE IdLog = " + id);
            foreach (Dictionary<string, string> record in log)
            {
                IdLog = id;
                IdUser = Convert.ToInt32(record["IdUser"]);
                Kind = Convert.ToInt32(record["Kind"]);
                IdModule = Convert.ToInt32(record["IdModule"]);
                IdRelated = Convert.ToInt32(record["IdRelated"]);
                Description = record["Description"];
                TimeStamp = Convert.ToDateTime(record["TimeStamp"]);
            }
        }

        /// <summary>
        /// Creates a new entry on the log.
        /// </summary>
        public void Create()
        {
            string sql = "INSERT INTO SystemLog(IdUser, Kind, IdModule, IdRelated, Description, TimeStamp) VALUES(" + IdUser.ToString() + ", " + Kind.ToString() + ", " + IdModule.ToString() + ", " + IdRelated.ToString() + ", '" + Common.SQSF(Description) + "', getdate());";
            Common.BDExecute(sql);
        }
    }

    /// <summary>
    /// The QBSLog Class mimics its database counterpart. Used to create simple files or logs, especially for imports.
    /// </summary>
    public class QBSLog
    {
        public int IdLog = 0;
        public string Name = "";
        public string Content = "";

        /// <summary>
        /// Constructor, no arguments received.
        /// </summary>
        public QBSLog() { }

        /// <summary>
        /// Overload of the constructor. Receives a IdLog and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdLog of the log entry to retrieve information from.</param>
        public QBSLog(int id)
        {
            List<Dictionary<string, string>> log = Common.GetRS("SELECT * FROM QBSLog WHERE IdLog = " + id);
            foreach (Dictionary<string, string> record in log)
            {
                IdLog = id;
                Name = record["Name"];
                Content = record["Content"];
            }
        }

        /// <summary>
        /// Creates a new entry on the log.
        /// </summary>
        public static int Create(string Content, string Name)
        {
            string sql = "INSERT INTO QBSLog(Content, Name) VALUES('" + Common.SQSF(Content) + "', '" + Common.SQSF(Name) +"');";
            Common.BDExecute(sql);
            return Common.GetBDNum("lastId", "SELECT MAX(IdLog) AS lastId FROM QBSLog");
        }
    }

    /// <summary>
    /// A Static Class that works as a wrapper for the SystemLog Class. Also includes the a method to list every entry to the log.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Adds an entry to the System Log.
        /// </summary>
        /// <param name="idUser">The current IdUser performing an action.</param>
        /// <param name="kind">The LogKind.Kind of action being performed.</param>
        /// <param name="idModule">The current IdModule.</param>
        /// <param name="idRelated">The current IdRelated (if present), this takes the value of any entity.</param>
        /// <param name="desc">An associated description (if provided).</param>
        public static void Add(int idUser, int kind, int idModule, int idRelated, string desc)
        {
            SystemLog systemLog = new SystemLog();
            systemLog.IdUser = idUser;
            systemLog.Kind = kind;
            systemLog.IdModule = idModule;
            systemLog.IdRelated = idRelated;
            systemLog.Description = desc;
            systemLog.Create();
        }

        /// <summary>
        /// Retrieves an HTML String with the full listing of records.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="filter">A given SQL Statement filter.</param>
        /// <returns>An HTML including the log list.</returns>
        public static string GetLogList(int currentPage, string filter)
        {
            string retval = "";
            string className = "";
            int total = 0;
            double recordsPerPage = Config.RecordsPerPage();
            string sql = "SELECT COUNT(IdLog) AS howMany FROM SystemLog";
            if (SessionHandler.Id != -1)
            {
                filter = Common.StrAdd(filter, " AND ", "IdUser <> -1");
            }
            sql = Common.StrAdd(sql, " WHERE ", filter);
            double totalRecords = Common.GetBDNum("howMany", sql);
            int totalPages = Convert.ToInt32(Math.Ceiling(totalRecords / recordsPerPage));

            if (currentPage > totalPages)
            {
                currentPage = totalPages;
            }
            if (currentPage == 0)
            {
                currentPage = 1;
            }

            retval = "<table width='100%'>";

            //Table Header
            retval += "<tr><th>" + Text.DateTime + "</th><th>" + Text.Username + "</th><th>" + Text.Action + "</th><th>" + Text.Description + "</th></tr>";

            sql = "SELECT IdLog, TimeStamp FROM (SELECT IdLog, TimeStamp, ROW_NUMBER() OVER (ORDER BY TimeStamp DESC) AS RowNum FROM SystemLog";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            sql += ") AS SL WHERE SL.RowNum BETWEEN ((" + currentPage + " - 1) * " + recordsPerPage + ") + 1 AND " + recordsPerPage + " * (" + currentPage + ")";
            string[] idLogList = Common.CSVToArray(Common.GetBDList("IdLog", sql, false));
            foreach (string idLog in idLogList)
            {
                try
                {
                    SystemLog log = new SystemLog(Convert.ToInt32(idLog));
                    User user = new User(log.IdUser);
                    string description = log.Description.Replace("#SESSION_START#", Text.SessionStarted + ": ").Replace("#SESSION_END#", Text.SessionClosed).Replace("#LOGIN_FAILED#", Text.LoginFailed + ": ").Replace("#FIELD_SEQUENCE#", Text.FieldSequence);
                    if (log.IdModule != 0)
                    {
                        Module module = new Module(log.IdModule);
                        description = Common.StrAdd(Modules.FriendlyModuleName(module.Name), ": ", description);
                        description += " [" + log.IdRelated + "]";
                        if (log.IdModule == Modules.IMPORT_USERS || log.IdModule == Modules.EVAL_IMPORT_EXAMS || log.IdModule == Modules.EVAL_ASSIGN || log.IdModule == Modules.EVAL_IMPORT_RESULTS)
                        {
                            description += "&nbsp;[<a href='#' class='dark' onClick='viewLog(" + log.IdRelated + "); return false;'>" + Text.Detail + "</a>]";
                        }
                    }
                    className = Common.SwitchClass(className);
                    retval += "<tr class='" + className + "'>";
                    retval += "<td>" + log.TimeStamp.ToString() + "</td>";
                    retval += "<td>" + user.Username + "</td>";
                    retval += "<td>" + LogKind.Name(log.Kind) + "</td>";
                    retval += "<td>" + description + "</td>";
                    retval += "</tr>";
                    total++;
                }
                catch (Exception ex) { }
            }
            retval += "</table>";

            //footer / pagination
            retval += "<div align='center' class='pagination'>";
            retval += "<div align='left' style='width: 50%; display: inline-block;'>" + Common.StrLang(Text.ShowingXofY, total.ToString() + "," + totalRecords.ToString()) + " " + Text.Records + "</div>";
            retval += "<div align='right' style='width: 50%; display: inline-block;'>" + Common.StrLang(Text.PageXofY, currentPage.ToString() + "," + totalPages.ToString());
            retval += "&nbsp;<a href='#' class='dark' onClick='firstPage();'>&lt;&lt;</a>";
            retval += "&nbsp;<a href='#' class='dark' onClick='prevPage();'>&lt;</a>";
            retval += "&nbsp;<a href='#' class='dark' onClick='nextPage();'>&gt;</a>";
            retval += "&nbsp;<a href='#' class='dark' onClick='lastPage(" + totalPages + ");'>&gt;&gt;</a>";
            retval += "</div>";
            retval += "</div>";

            return retval;
        }

    }
}