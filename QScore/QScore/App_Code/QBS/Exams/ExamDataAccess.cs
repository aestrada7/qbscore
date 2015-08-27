using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QBS.Data;
using QBS.ACL;
using QScore.lang;
using Aexis;
using Aexis.Web;

namespace QBS.Exams
{
    /// <summary>
    /// Provides generic functions for the ExamQuestion class.
    /// </summary>
    public static class ExamQuestions
    {
        /// <summary>
        /// Retrieves the question list as an HTML string.
        /// </summary>
        /// <param name="modules">The current user modules.</param>
        /// <param name="currentPage">The current page used.</param>
        /// <param name="filter">An SQL filter.</param>
        /// <returns>The HTML string with the question list.</returns>
        public static string GetQuestionList(Dictionary<string, bool> modules, int currentPage, string filter, bool forSelection)
        {
            string retval = "";
            string className = "";
            int total = 0;
            string sql = "SELECT COUNT(IdQuestion) AS HowMany FROM ExamQuestion";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            double recordsPerPage = Config.RecordsPerPage();
            double totalRecords = Common.GetBDNum("HowMany", sql);
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
            retval += "<tr>";

            //Table Header
            if (forSelection)
            {
                retval += "<th>&nbsp;</th>";
            }
            retval += "<th>" + Text.Identifier + "</th><th>" + Text.Question + "</th>";
            if(!forSelection)
            {
                retval += "<th>" + Text.Theme +"</th><th>" + Text.Status + "</th>";
            }
            retval += "<th>" + Text.DifficultyIndex + "</th></tr>";

            sql = "SELECT IdQuestion, Question FROM (SELECT IdQuestion, Question, ROW_NUMBER() OVER (ORDER BY IdQuestion) AS RowNum FROM ExamQuestion";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            sql += ") AS U WHERE U.RowNum BETWEEN ((" + currentPage + " - 1) * " + recordsPerPage + ") + 1 AND " + recordsPerPage + " * (" + currentPage + ")";
            string[] idQuestionList = Common.CSVToArray(Common.GetBDList("IdQuestion", sql, false));
            foreach (string idQuestion in idQuestionList)
            {
                try
                {
                    ExamQuestion question = new ExamQuestion(Convert.ToInt32(idQuestion));
                    ExamTheme theme = new ExamTheme(question.IdTheme);
                    string themeName = "";
                    if (String.IsNullOrEmpty(theme.Theme))
                    {
                        themeName = Text.None;
                    }
                    else
                    {
                        themeName = theme.Theme;
                    }
                    className = Common.SwitchClass(className);
                    string questionStatus = Text.Inactive;
                    if (question.Status == ExamQuestionStatus.ACTIVE)
                    {
                        questionStatus = Text.Active;
                    }
                    retval += "<tr class='" + className + "' id='q_" + idQuestion + "' onClick='showQuestion(" + idQuestion + ");'>";
                    if(forSelection)
                    {
                        retval += "<td width='5%' align='center'>" + DrawInput.InputCheckbox("q_chk_" + idQuestion, "", false, "questionChk", "", "", "") + "</td>";
                    }
                    retval += "<td width='5%' align='center'>" + question.IdQuestion + "</td>";
                    retval += "<td width='60%'>" + Common.BBCodeToHTML(question.Question) + "</td>";
                    if(!forSelection)
                    {
                        retval += "<td width='20%' align='center'>" + themeName + "</td>";
                        retval += "<td width='10%' align='center'>" + questionStatus + "</td>";
                    }
                    retval += "<td width='5%' align='center'>" + String.Format("{0:0.00}", question.DifficultyIndex("")) + "</td>";
                    retval += "</tr>";
                    total++;
                }
                catch (Exception ex) { }
            }

            retval += "</table>";

            //footer / pagination
            retval += "<div align='center' class='pagination'>";
            retval += "<div align='left' style='width: 50%; display: inline-block;'>" + Common.StrLang(Text.ShowingXofY, total.ToString() + "," + totalRecords.ToString()) + " " + Text.Question_s + "</div>";
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

    /// <summary>
    /// Provides generic functions for the ExamTheme class.
    /// </summary>
    public static class ExamThemes
    {
        /// <summary>
        /// Retrieves the theme list as an HTML string.
        /// </summary>
        /// <param name="modules">The current user modules.</param>
        /// <param name="currentPage">The current page used.</param>
        /// <param name="filter">An SQL filter.</param>
        /// <returns>The HTML string with the question list.</returns>
        public static string GetThemeList(Dictionary<string, bool> modules, int currentPage, string filter)
        {
            string retval = "";
            string className = "";
            int total = 0;
            string sql = "SELECT COUNT(IdTheme) AS HowMany FROM ExamTheme";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            double recordsPerPage = Config.RecordsPerPage();
            double totalRecords = Common.GetBDNum("HowMany", sql);
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
            retval += "<tr><th>" + Text.Identifier + "</th><th>" + Text.Theme + "</th></tr>";

            sql = "SELECT IdTheme, Theme FROM (SELECT IdTheme, Theme, ROW_NUMBER() OVER (ORDER BY IdTheme) AS RowNum FROM ExamTheme";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            sql += ") AS U WHERE U.RowNum BETWEEN ((" + currentPage + " - 1) * " + recordsPerPage + ") + 1 AND " + recordsPerPage + " * (" + currentPage + ")";
            string[] idThemeList = Common.CSVToArray(Common.GetBDList("IdTheme", sql, false));
            foreach (string idTheme in idThemeList)
            {
                try
                {
                    ExamTheme theme = new ExamTheme(Convert.ToInt32(idTheme));
                    className = Common.SwitchClass(className);
                    retval += "<tr class='" + className + "' onClick='showTheme(" + idTheme + ");'>";
                    retval += "<td width='10%' align='center'>" + theme.IdTheme + "</td>";
                    retval += "<td width='70%'>" + theme.Theme + "</td>";
                    retval += "</tr>";
                    total++;
                }
                catch (Exception ex) { }
            }

            retval += "</table>";

            //footer / pagination
            retval += "<div align='center' class='pagination'>";
            retval += "<div align='left' style='width: 50%; display: inline-block;'>" + Common.StrLang(Text.ShowingXofY, total.ToString() + "," + totalRecords.ToString()) + " " + Text.Theme_s + "</div>";
            retval += "<div align='right' style='width: 50%; display: inline-block;'>" + Common.StrLang(Text.PageXofY, currentPage.ToString() + "," + totalPages.ToString());
            retval += "&nbsp;<a href='#' class='dark' onClick='firstPage();'>&lt;&lt;</a>";
            retval += "&nbsp;<a href='#' class='dark' onClick='prevPage();'>&lt;</a>";
            retval += "&nbsp;<a href='#' class='dark' onClick='nextPage();'>&gt;</a>";
            retval += "&nbsp;<a href='#' class='dark' onClick='lastPage(" + totalPages + ");'>&gt;&gt;</a>";
            retval += "</div>";
            retval += "</div>";

            return retval;
        }

        public static string DrawThemeTreeSelect(string fieldId, string selected, int idTheme, string defaultText)
        {
            string retval = "";
            string themeJSON = GetThemeTree(0, idTheme);
            selected = Common.StrAdd(GetThemeTreeFromId(selected), "_", selected);
            retval = DrawInput.InputHiddenField(fieldId, selected, "", "");
            retval += "<div id='tree_" + fieldId + "'></div>";
            retval += "<script language='JavaScript'>";
            retval += "$('#tree_" + fieldId + "').treeSelectJSON({";
            retval += "  source: " + themeJSON + ",";
            retval += "  target: '" + fieldId + "',";
            retval += "  defaultText: '- " + defaultText + " -',";
            retval += "  preselected: '" + selected + "'";
            retval += "});";
            retval += "</script>";
            return retval;
        }

        /// <summary>
        /// Builds the JSON string for the theme tree.
        /// </summary>
        /// <param name="parentTheme">The current parent theme.</param>
        /// <param name="idTheme">The current theme, this will be excluded from the selection.</param>
        /// <returns>The JSON string with the theme tree.</returns>
        public static string GetThemeTree(int parentTheme, int currIdTheme)
        {
            string retval = "";
            string sql = "SELECT * FROM ExamTheme WHERE IdParentTheme = " + parentTheme + " AND IdTheme <> " + currIdTheme;
            List<Dictionary<string, string>> themes = Common.GetRS(sql);
            foreach (Dictionary<string, string> record in themes)
            {
                int idTheme = Convert.ToInt32(record["IdTheme"]);
                string theme = record["Theme"];
                int idParentTheme = Convert.ToInt32(record["IdParentTheme"]);
                string children = GetThemeTree(idTheme, currIdTheme);
                retval = Common.StrAdd(retval, ",", "{'id':" + idTheme + ",'value':'" + theme + "'");
                if (!String.IsNullOrEmpty(children))
                {
                    retval += ",'children':[" + children + "]";
                }
                retval += "}";
            }
            if (parentTheme == 0)
            {
                return "{'children': [" + retval + "]}";
            }
            else
            {
                return retval;
            }
        }

        /// <summary>
        /// Retrieves the full tree Id from a given IdTheme. Say a theme has a parent id of 20, and that theme (20) has a
        /// parent id of 77, this function will return 77_20. This is useful for the tree component.
        /// </summary>
        /// <param name="idTheme">The current IdTheme.</param>
        /// <returns>The full tree id</returns>
        public static string GetThemeTreeFromId(string idTheme)
        {
            string retval = "";
            int parentTheme = Common.GetBDNum("IdParentTheme", "SELECT IdParentTheme FROM ExamTheme WHERE IdTheme = " + idTheme);
            if (parentTheme != 0)
            {
                retval = GetThemeTreeFromId(parentTheme.ToString());
                retval = Common.StrAdd(retval, "_", parentTheme.ToString());
            }
            return retval;
        }

        /// <summary>
        /// Retrieves the IdTheme from a given TreeId. Say a given tree id of 12_77_20 is sent, this function will return the
        /// last value, in this case, '20'.
        /// </summary>
        /// <param name="treeId">The tree id to convert.</param>
        /// <returns>A valid IdTheme.</returns>
        public static int GetThemeIdFromTree(string treeId)
        {
            int retval = 0;
            if(!String.IsNullOrEmpty(treeId))
            {
                try
                {
                    retval = Convert.ToInt32(treeId.Substring(treeId.LastIndexOf('_') + 1));
                }
                catch (ArgumentOutOfRangeException ex) { }
                catch (FormatException ex) { }
            }
            return retval;
        }
    }

    /// <summary>
    /// Provides generic functions for the Exams class.
    /// </summary>
    public static class Exams
    {
        /// <summary>
        /// Retrieves a list of Exams.
        /// </summary>
        /// <param name="filter">A given SQL Statement filter.</param>
        /// <returns>A List of Exams. (List&lt;Exam&gt;)</returns>
        public static List<Exam> GetExams(string filter)
        {
            List<Exam> examList = new List<Exam>();
            string sql = "SELECT IdExam FROM Exam";
            filter = Common.StrAdd(filter, " AND ", "Status=" + ExamStatus.ACTIVE);
            sql = Common.StrAdd(sql, " WHERE ", filter);
            string[] idExamList = Common.CSVToArray(Common.GetBDList("IdExam", sql, false));
            foreach (string idExam in idExamList)
            {
                Exam exam = new Exam(Convert.ToInt32(idExam));
                examList.Add(exam);
            }
            return examList;
        }

        /// <summary>
        /// Retrieves the exam list as an HTML string.
        /// </summary>
        /// <param name="modules">The current user modules.</param>
        /// <param name="currentPage">The current page used.</param>
        /// <param name="filter">An SQL filter.</param>
        /// <returns>The HTML string with the question list.</returns>
        public static string GetExamList(Dictionary<string, bool> modules, int currentPage, string filter)
        {
            string retval = "";
            string className = "";
            int total = 0;
            string sql = "SELECT COUNT(IdExam) AS HowMany FROM Exam";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            double recordsPerPage = Config.RecordsPerPage();
            double totalRecords = Common.GetBDNum("HowMany", sql);
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
            retval += "<tr><th>" + Text.Identifier + "</th><th>" + Text.Exam + "</th><th>" + Text.Status + "</th></tr>";

            sql = "SELECT IdExam, Exam FROM (SELECT IdExam, Exam, ROW_NUMBER() OVER (ORDER BY IdExam) AS RowNum FROM Exam";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            sql += ") AS U WHERE U.RowNum BETWEEN ((" + currentPage + " - 1) * " + recordsPerPage + ") + 1 AND " + recordsPerPage + " * (" + currentPage + ")";
            string[] idExamList = Common.CSVToArray(Common.GetBDList("IdExam", sql, false));
            foreach (string idExam in idExamList)
            {
                try
                {
                    Exam exam = new Exam(Convert.ToInt32(idExam));
                    className = Common.SwitchClass(className);
                    string examStatus = Text.Inactive;
                    if (exam.Status == ExamStatus.ACTIVE)
                    {
                        examStatus = Text.Active;
                    }
                    retval += "<tr class='" + className + "' onClick='showExam(" + idExam + ");'>";
                    retval += "<td width='10%' align='center'>" + exam.IdExam + "</td>";
                    retval += "<td width='70%'>" + exam.ExamName + "</td>";
                    retval += "<td width='20%' align='center'>" + examStatus + "</td>";
                    retval += "</tr>";
                    total++;
                }
                catch (Exception ex) { }
            }

            retval += "</table>";

            //footer / pagination
            retval += "<div align='center' class='pagination'>";
            retval += "<div align='left' style='width: 50%; display: inline-block;'>" + Common.StrLang(Text.ShowingXofY, total.ToString() + "," + totalRecords.ToString()) + " " + Text.Exam_s + "</div>";
            retval += "<div align='right' style='width: 50%; display: inline-block;'>" + Common.StrLang(Text.PageXofY, currentPage.ToString() + "," + totalPages.ToString());
            retval += "&nbsp;<a href='#' class='dark' onClick='firstPage();'>&lt;&lt;</a>";
            retval += "&nbsp;<a href='#' class='dark' onClick='prevPage();'>&lt;</a>";
            retval += "&nbsp;<a href='#' class='dark' onClick='nextPage();'>&gt;</a>";
            retval += "&nbsp;<a href='#' class='dark' onClick='lastPage(" + totalPages + ");'>&gt;&gt;</a>";
            retval += "</div>";
            retval += "</div>";

            return retval;
        }

        /// <summary>
        /// Retrieves the exam list as an HTML string.
        /// </summary>
        /// <param name="modules">The current user modules.</param>
        /// <param name="currentPage">The current page used.</param>
        /// <param name="filter">An SQL filter.</param>
        /// <returns>The HTML string with the question list.</returns>
        public static string GetReportList(int currentPage, string filter)
        {
            string retval = "";
            string className = "";
            int total = 0;
            string sql = "SELECT COUNT(IdExam) AS HowMany FROM UserExam";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            double recordsPerPage = Config.RecordsPerPage();
            double totalRecords = Common.GetBDNum("HowMany", sql);
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
            retval += "<tr><th>" + Text.FullName + "</th><th>" + Text.Exam + "</th><th>" + Text.DateTime + "</th><th>" + Text.Status + "</th><th>" + Text.Score + "</th></tr>";

            sql = "SELECT IdUserExam FROM (SELECT IdUserExam, ROW_NUMBER() OVER (ORDER BY IdUserExam) AS RowNum FROM UserExam";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            sql += ") AS U WHERE U.RowNum BETWEEN ((" + currentPage + " - 1) * " + recordsPerPage + ") + 1 AND " + recordsPerPage + " * (" + currentPage + ")";
            string[] idUserExamList = Common.CSVToArray(Common.GetBDList("IdUserExam", sql, false));
            foreach (string idUserExam in idUserExamList)
            {
                try
                {
                    UserExam userExam = new UserExam(Convert.ToInt32(idUserExam));
                    User user = new User(userExam.IdUser);
                    Exam exam = new Exam(userExam.IdExam);
                    className = Common.SwitchClass(className);
                    if (userExam.Status == UserExamStatus.PENDING && Modules.Permission(SessionHandler.Modules, Modules.EVAL_SOLVE_USER))
                    {
                        retval += "<tr class='" + className + "' onClick='exam(" + idUserExam + ");'>";
                    }
                    else
                    {
                        retval += "<tr class='" + className + "' onClick='detail(" + idUserExam + ");'>";
                    }
                    retval += "<td width='30%'>" + user.FullName(true) + "</td>";
                    retval += "<td width='30%'>" + exam.ExamName + "</td>";
                    retval += "<td width='20%' align='center'>" + userExam.DateComplete + "</td>";
                    retval += "<td width='10%' align='center'>" + UserExamStatus.FriendlyText(userExam.Status) + "</td>";
                    retval += "<td width='10%' align='center'>" + String.Format("{0:0.00}", userExam.Score) + "</td>";
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

        /// <summary>
        /// Shows the exam list for the current user.
        /// </summary>
        /// <returns>An HTML with the list of available exams.</returns>
        public static string GetPendingExamsList()
        {
            int currentId = SessionHandler.Id;
            string retval = "";
            string className = "";
            string sql = "";
            int total = 0;

            retval = "<table width='100%'>";

            //Table Header
            retval += "<tr><th>#</th><th>" + Text.Exam + "</th><th>" + Text.Status + "</th></tr>";

            //Self-Enroll exams
            sql = "SELECT IdExam FROM Exam WHERE Status = " + ExamStatus.ACTIVE + " AND SelfEnroll = 1 AND IdExam NOT IN(SELECT IdExam FROM UserExam WHERE IdUser=" + currentId + ")";
            string[] idExamList = Common.CSVToArray(Common.GetBDList("IdExam", sql, false));
            foreach (string idExam in idExamList)
            {
                try
                {
                    Exam exam = new Exam(Convert.ToInt32(idExam));
                    className = Common.SwitchClass(className);
                    int examStatus = Common.GetBDNum("Status", "SELECT Status FROM UserExam WHERE IdExam = " + idExam + " AND IdUser = " + currentId);
                    string examStatusDesc = "";
                    if (examStatus == UserExamStatus.PENDING)
                    {
                        examStatusDesc = Text.Exam_Pending;
                    }
                    else if (examStatus == UserExamStatus.INCOMPLETE)
                    {
                        examStatusDesc = Text.Exam_Incomplete;
                    }
                    else
                    {
                        examStatusDesc = Text.Exam_Complete;
                    }
                    retval += "<tr class='" + className + "' onClick='exam(" + idExam + ");'>";
                    retval += "<td width='10%' align='center'>" + (total + 1) + "</td>";
                    retval += "<td width='70%'>" + exam.ExamName + "</td>";
                    retval += "<td width='20%' align='center'>" + examStatusDesc + "</td>";
                    retval += "</tr>";
                    total++;
                }
                catch (Exception ex) { }
            }
            //Assigned Exams
            sql = "SELECT IdUserExam FROM UserExam WHERE Status = " + UserExamStatus.PENDING + " AND IdUser=" + currentId;
            string[] idUserExamList = Common.CSVToArray(Common.GetBDList("IdUserExam", sql, false));
            foreach (string idUserExam in idUserExamList)
            {
                try
                {
                    UserExam userExam = new UserExam(Convert.ToInt32(idUserExam));
                    Exam exam = new Exam(userExam.IdExam);
                    className = Common.SwitchClass(className);
                    int examStatus = Common.GetBDNum("Status", "SELECT Status FROM UserExam WHERE IdUserExam=" + idUserExam);
                    string examStatusDesc = "";
                    if (examStatus == UserExamStatus.PENDING)
                    {
                        examStatusDesc = Text.Exam_Pending;
                    }
                    else if (examStatus == UserExamStatus.INCOMPLETE)
                    {
                        examStatusDesc = Text.Exam_Incomplete;
                    }
                    else
                    {
                        examStatusDesc = Text.Exam_Complete;
                    }
                    retval += "<tr class='" + className + "' onClick='exam(" + userExam.IdExam + ", " + userExam.IdUserExam + ");'>";
                    retval += "<td width='10%' align='center'>" + (total + 1) + "</td>";
                    retval += "<td width='70%'>" + exam.ExamName + "</td>";
                    retval += "<td width='20%' align='center'>" + examStatusDesc + "</td>";
                    retval += "</tr>";
                    total++;
                }
                catch (Exception ex) { }
            }
            if (total == 0)
            {
                retval += "<tr><td colspan='3' width='100%' class='alert'>" + Text.NoPendingExams + "</td></tr>";
            }

            retval += "</table>";
            return retval;
        }

        /// <summary>
        /// Starts the exam for the given IdUser.
        /// </summary>
        /// <param name="idUser">The given IdUser.</param>
        /// <param name="idExam">The given IdExam.</param>
        /// <param name="ignorePrevious">Ignores previous exams and builds new ones.</param>
        /// <returns>The IdUserExam for the current IdUser/IdExam combination.</returns>
        public static int ExamStart(int idUser, int idExam, bool ignorePrevious)
        {
            int idUserExam = Common.GetBDNum("IdUserExam", "SELECT MAX(IdUserExam) AS IdUserExam FROM UserExam WHERE IdUser = " + idUser + " AND IdExam = " + idExam);
            UserExam userExam;
            if((idUserExam != 0) && !ignorePrevious)
            {
                userExam = new UserExam(idUserExam);
            }
            else
            {
                userExam = new UserExam();
                userExam.IdUser = idUser;
                userExam.IdExam = idExam;
                userExam.Create();
            }
            return userExam.IdUserExam;
        }
    }

}