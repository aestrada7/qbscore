using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aexis;
using QBS.ACL;
using QBS.Data;
using QBS.Exams;
using QScore.lang;

namespace QBS.Exams
{
    /// <summary>
    /// The ExamSession Class defines the structure of a single exam session, plus any actions it can have.
    /// </summary>
    public class ExamSession
    {
        public int IdSession = 0;
        public string Session = "";
        public int IdExam = 0;
        public int StartTimeStamp = 0;
        public int EndTimeStamp = 0;
        //not on db
        public DateTime StartTime = new DateTime();
        public DateTime EndTime = new DateTime();

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ExamSession() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdSession and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdSession of the user to retrieve information from.</param>
        public ExamSession(int id)
        {
            List<Dictionary<string, string>> option = Common.GetRS("SELECT * FROM ExamSession WHERE IdSession = " + id);
            foreach (Dictionary<string, string> record in option)
            {
                IdSession = id;
                Session = record["Session"];
                IdExam = Convert.ToInt32(record["IdExam"]);
                StartTimeStamp = Convert.ToInt32(record["StartTimeStamp"]);
                EndTimeStamp = Convert.ToInt32(record["EndTimeStamp"]);
                StartTime = Common.DateTimeFromTimeStamp(StartTimeStamp);
                EndTime = Common.DateTimeFromTimeStamp(EndTimeStamp);
            }
        }

        /// <summary>
        /// Synchronizes the TimeStamps with the provided DateTime objects.
        /// </summary>
        private void SyncTime()
        {
            StartTimeStamp = Common.TimeStampFromDateTime(StartTime);
            EndTimeStamp = Common.TimeStampFromDateTime(EndTime);
        }

        /// <summary>
        /// Saves a new session in the database.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            SyncTime();
            string sql = "INSERT INTO ExamSession(Session, IdExam, StartTimeStamp, EndTimeStamp) VALUES('" + Session + "', " + IdExam + ", " + StartTimeStamp + ", " + EndTimeStamp + ");";
            Common.BDExecute(sql);
            IdSession = Common.GetBDNum("lastId", "SELECT MAX(IdOption) AS lastId FROM ExamSession");
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Deletes the current session, plus any associated table data.
        /// </summary>
        /// <returns>NO_ERROR if deleted successfully, the error code otherwise.</returns>
        public int Delete()
        {
            string sql = "DELETE FROM ExamSession WHERE IdSession = " + IdSession.ToString();
            Common.BDExecute(sql);
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Updates the current's session data.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error code otherwise.</returns>
        public int Update()
        {
            SyncTime();
            string sql = "UPDATE ExamSession SET Session='" + Common.SQSF(Session) + "', IdExam=" + IdExam.ToString() + ", StartTimeStamp=" + StartTimeStamp + ", EndTimeStamp=" + EndTimeStamp + " WHERE IdSession = " + IdSession.ToString();
            Common.BDExecute(sql);
            return ErrorCode.NO_ERROR;
        }

    }

    /// <summary>
    /// The ExamOption Class defines the structure of a single exam option, plus any actions it can have.
    /// </summary>
    public class ExamOption
    {
        public int IdOption = 0;
        public int IdQuestion = 0;
        public string OptionText = "";
        public int Points = 0;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ExamOption() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdExamOption and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdExamOption of the user to retrieve information from.</param>
        public ExamOption(int id)
        {
            List<Dictionary<string, string>> option = Common.GetRS("SELECT * FROM ExamOption WHERE IdOption = " + id);
            foreach (Dictionary<string, string> record in option)
            {
                IdOption = id;
                IdQuestion = Convert.ToInt32(record["IdQuestion"]);
                OptionText = record["OptionText"];
                Points = Convert.ToInt32(record["Points"]);
            }
        }

        /// <summary>
        /// Saves a new option in the database.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            string sql = "INSERT INTO ExamOption(IdQuestion, OptionText, Points) VALUES(" + IdQuestion + ", '" + Common.SQSF(OptionText) + "', " + Points + ");";
            Common.BDExecute(sql);
            IdOption = Common.GetBDNum("lastId", "SELECT MAX(IdOption) AS lastId FROM ExamOption");
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Deletes the current option, plus any associated table data.
        /// </summary>
        /// <returns>NO_ERROR if deleted successfully, the error code otherwise.</returns>
        public int Delete()
        {
            string sql = "DELETE FROM ExamOption WHERE IdOption = " + IdOption.ToString();
            Common.BDExecute(sql);
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Updates the current's option data.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error code otherwise.</returns>
        public int Update()
        {
            string sql = "UPDATE ExamOption SET OptionText='" + Common.SQSF(OptionText) + "', Points=" + Points.ToString() + " WHERE IdOption = " + IdOption.ToString();
            Common.BDExecute(sql);
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Clones the current option.
        /// </summary>
        /// <param name="newIdQuestion">The new question Id.</param>
        /// <returns>The id of the cloned option.</returns>
        public int Clone(int newIdQuestion)
        {
            ExamOption clonedOption = new ExamOption();
            clonedOption.IdQuestion = newIdQuestion;
            clonedOption.OptionText = OptionText;
            clonedOption.Points = Points;
            clonedOption.Create();
            return clonedOption.IdOption;
        }

    }

    /// <summary>
    /// The ExamQuestion Class defines the structure of a single exam question, plus any actions it can have.
    /// </summary>
    public partial class ExamQuestion
    {
        public int IdQuestion = 0;
        public string Question = "";
        public int IdTheme = 0; //can be zero, no problem
        public int Status = ExamQuestionStatus.INACTIVE;
        public DateTime DateCreated = new DateTime();
        public DateTime DateModified = new DateTime();
        public int IdQuestionOriginal = 0;
        //Not on DB
        public List<ExamOption> options = new List<ExamOption>();

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ExamQuestion() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdExamQuestion and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdExamQuestion of the user to retrieve information from.</param>
        public ExamQuestion(int id)
        {
            List<Dictionary<string, string>> question = Common.GetRS("SELECT * FROM ExamQuestion WHERE IdQuestion = " + id);
            foreach (Dictionary<string, string> record in question)
            {
                IdQuestion = id;
                Question = record["Question"];
                IdTheme = Convert.ToInt32(record["IdTheme"]);
                Status = Convert.ToInt32(record["Status"]);
                DateCreated = Convert.ToDateTime(record["DateCreated"]);
                DateModified = Convert.ToDateTime(record["DateModified"]);
                IdQuestionOriginal = Convert.ToInt32(record["IdQuestionOriginal"]);
            }
            GatherOptions();
        }

        /// <summary>
        /// Gathers all options and saves them in memory.
        /// </summary>
        public void GatherOptions()
        {
            options = new List<ExamOption>();
            List<Dictionary<string, string>> option = Common.GetRS("SELECT * FROM ExamOption WHERE IdQuestion = " + IdQuestion);
            foreach (Dictionary<string, string> record in option)
            {
                ExamOption examOption = new ExamOption(Convert.ToInt32(record["IdOption"]));
                options.Add(examOption);
            }
        }

        /// <summary>
        /// Saves a new question in the database.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            if (String.IsNullOrWhiteSpace(Question))
            {
                return ErrorCode.MISSING_FIELDS;
            }
            else
            {
                string sql = "INSERT INTO ExamQuestion(Question, IdTheme, Status, DateCreated, DateModified, IdQuestionOriginal) VALUES('" + Common.SQSF(Question) + "', " + IdTheme + ", " + Status + ", getdate(), getdate(), " + IdQuestionOriginal + ");";
                Common.BDExecute(sql);
                IdQuestion = Common.GetBDNum("lastId", "SELECT MAX(IdQuestion) AS lastId FROM ExamQuestion");
                Log.Add(SessionHandler.Id, LogKind.CREATE, Modules.EVAL_QUESTION_CATALOG, IdQuestion, "");
                return ErrorCode.NO_ERROR;
            }
        }

        /// <summary>
        /// Deletes the current question, plus any associated table data.
        /// </summary>
        /// <returns>NO_ERROR if deleted successfully, the error code otherwise.</returns>
        public int Delete()
        {
            bool handPickedInExams = Common.GetBDNum("HowMany", "SELECT COUNT(IdExam) AS HowMany FROM ExamContent WHERE IdQuestion = " + IdQuestion.ToString()) > 0;
            bool hasBeenUsed = Common.GetBDNum("HowMany", "SELECT COUNT(IdUserExam) AS HowMany FROM UserExamQuestion WHERE IdQuestion = " + IdQuestion.ToString()) > 0;

            if (!handPickedInExams && !hasBeenUsed)
            {
                string sql = "DELETE FROM ExamQuestion WHERE IdQuestion = " + IdQuestion.ToString();
                Common.BDExecute(sql);
                GatherOptions();
                foreach (ExamOption option in options)
                {
                    option.Delete();
                }
                Log.Add(SessionHandler.Id, LogKind.DELETE, Modules.EVAL_QUESTION_CATALOG, IdQuestion, "");
                return ErrorCode.NO_ERROR;
            }
            else
            {
                return ErrorCode.CANT_DELETE;
            }
        }

        /// <summary>
        /// Updates the current's question data.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error code otherwise.</returns>
        public int Update()
        {
            bool hasBeenUsed = Common.GetBDNum("HowMany", "SELECT COUNT(IdUserExam) AS HowMany FROM UserExamQuestion WHERE IdQuestion = " + IdQuestion.ToString()) > 0;

            if (!hasBeenUsed)
            {
                if (String.IsNullOrWhiteSpace(Question))
                {
                    return ErrorCode.MISSING_FIELDS;
                }
                else
                {
                    string sql = "UPDATE ExamQuestion SET Question = '" + Common.SQSF(Question) + "', IdTheme = " + IdTheme + ", Status = " + Status + ", DateModified = getdate() WHERE IdQuestion = " + IdQuestion.ToString();
                    Common.BDExecute(sql);
                    foreach (ExamOption option in options)
                    {
                        option.Update();
                    }
                    Log.Add(SessionHandler.Id, LogKind.UPDATE, Modules.EVAL_QUESTION_CATALOG, IdQuestion, "");
                    return ErrorCode.NO_ERROR;
                }
            }
            else
            {
                Clone();
                return ErrorCode.NO_ERROR;
            }
        }

        /// <summary>
        /// Checks if a given question already exists.
        /// </summary>
        /// <returns>True if it exists, false otherwise.</returns>
        public bool Exists()
        {
            return Common.GetBDNum("IdQuestion", "SELECT IdQuestion FROM ExamQuestion WHERE Question = '" + Common.SQSF(Question) + "'") > 0;
        }

        /// <summary>
        /// Clones the current question.
        /// </summary>
        /// <returns>The id of the cloned question.</returns>
        public int Clone()
        {
            ExamQuestion clonedQuestion = new ExamQuestion();
            clonedQuestion.Question = Question;
            clonedQuestion.IdTheme = IdTheme;
            clonedQuestion.Status = Status;
            clonedQuestion.IdQuestionOriginal = IdQuestion;
            clonedQuestion.Create();
            foreach (ExamOption option in options)
            {
                option.Clone(clonedQuestion.IdQuestion);
            }
            return clonedQuestion.IdQuestion;
        }

        /// <summary>
        /// Returns the difficulty index of the current IdQuestion.
        /// </summary>
        /// <param name="filter">An SQL filter for the query. This is useful to get a difficulty index in different moments.</param>
        /// <returns>A double value with the difficulty index.</returns>
        public double DifficultyIndex(string filter)
        {
            int totalAnswers = 0;
            int correctAnswers = 0;
            double degreeOfDiff = 0;
            string sql = "SELECT COUNT(UQ.IdUserExam) AS TotalAnswers, SUM(CASE WHEN EO.Points > 0 THEN 1 ELSE 0 END) AS CorrectAnswers";
            sql += " FROM UserExamQuestion UQ INNER JOIN ExamOption EO ON EO.IdOption = UQ.IdOption INNER JOIN UserExam UE ON UQ.IdUserExam = UE.IdUserExam";
            sql += " INNER JOIN Exam E ON E.IdExam = UE.IdExam";
            sql += " WHERE UQ.IdQuestion = " + IdQuestion + " AND UQ.IdOption > 0 AND UE.Status IN(" + UserExamStatus.PASSED + "," + UserExamStatus.FAILED + ")";
            sql = Common.StrAdd(sql, " AND ", filter);
            List<Dictionary<string, string>> diff = Common.GetRS(sql);
            foreach (Dictionary<string, string> record in diff)
            {
                totalAnswers = Convert.ToInt32(record["TotalAnswers"]);
                try
                {
                    correctAnswers = Convert.ToInt32(record["CorrectAnswers"]);
                }
                catch (FormatException ex)
                {
                    correctAnswers = 1;
                }
            }
            try
            {
                degreeOfDiff = ((double)correctAnswers / (double)totalAnswers);
            }
            catch (DivideByZeroException ex) { }
            catch (NotFiniteNumberException ex) { }
            finally
            {
                if (totalAnswers == 0)
                {
                    degreeOfDiff = 0;
                }
            }
            return degreeOfDiff;
        }

        /// <summary>
        /// Returns the discrimination index of the current IdQuestion.
        /// </summary>
        /// <param name="filter">An SQL filter for the query. This is useful to get a discrimination index in different moments.</param>
        /// <returns>A double value with the discrimination index.</returns>
        public double DiscriminationIndex(string filter)
        {
            //Algorithm in DiscriminationIndex.pdf
            return 0;
        }

    }

    /// <summary>
    /// The ExamTheme Class defines the structure of a single exam theme, plus any actions it can have.
    /// </summary>
    public class ExamTheme
    {
        public int IdTheme = 0;
        public int IdParentTheme = 0;
        public string Theme = "";

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ExamTheme() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdExamTheme and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdExamTheme of the user to retrieve information from.</param>
        public ExamTheme(int id)
        {
            List<Dictionary<string, string>> theme = Common.GetRS("SELECT * FROM ExamTheme WHERE IdTheme = " + id);
            foreach (Dictionary<string, string> record in theme)
            {
                IdTheme = id;
                Theme = record["Theme"];
                IdParentTheme = Convert.ToInt32(record["IdParentTheme"]);
            }
        }

        /// <summary>
        /// Overload of the constructor. Recieves a Theme and populates the instance with its values (if it exists).
        /// </summary>
        /// <param name="theme">The Theme to look for.</param>
        public ExamTheme(string themeText)
        {
            List<Dictionary<string, string>> theme = Common.GetRS("SELECT * FROM ExamTheme WHERE Theme = '" + Common.SQSF(themeText) + "'");
            foreach (Dictionary<string, string> record in theme)
            {
                IdTheme = Convert.ToInt32(record["IdTheme"]); ;
                Theme = record["Theme"];
                IdParentTheme = Convert.ToInt32(record["IdParentTheme"]);
            }
        }

        /// <summary>
        /// Checks if the theme name is valid.
        /// </summary>
        /// <returns>NO_ERROR if the Theme is valid, the error number otherwise.</returns>
        private int ValidTheme()
        {
            int retval = ErrorCode.NO_ERROR;
            if (Common.GetBDNum("NameExists", "SELECT COUNT(IdTheme) AS NameExists FROM ExamTheme WHERE Theme='" + Common.SQSF(Theme) + "' AND IdTheme <> " + IdTheme.ToString()) > 0)
            {
                retval = ErrorCode.ALREADY_EXISTS;
            }
            else if (String.IsNullOrWhiteSpace(Theme))
            {
                retval = ErrorCode.MISSING_FIELDS;
            }
            return retval;
        }

        /// <summary>
        /// Saves a new theme in the database.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            int errorCode = ValidTheme();
            if (errorCode == ErrorCode.NO_ERROR)
            {
                string sql = "INSERT INTO ExamTheme(Theme, IdParentTheme) VALUES('" + Common.SQSF(Theme) + "', " + IdParentTheme + ");";
                Common.BDExecute(sql);
                IdTheme = Common.GetBDNum("lastId", "SELECT MAX(IdTheme) AS lastId FROM ExamTheme");
                Log.Add(SessionHandler.Id, LogKind.CREATE, Modules.EVAL_THEME_CATALOG, IdTheme, "");
            }
            return errorCode;
        }

        /// <summary>
        /// Deletes the current theme, plus any associated table data.
        /// </summary>
        /// <returns>NO_ERROR if deleted successfully, the error code otherwise.</returns>
        public int Delete()
        {
            bool handPickedInExams = Common.GetBDNum("HowMany", "SELECT COUNT(IdExam) AS HowMany FROM ExamContent WHERE IdTheme = " + IdTheme.ToString()) > 0;
            if (!handPickedInExams)
            {
                string sql = "DELETE FROM ExamTheme WHERE IdTheme = " + IdTheme.ToString();
                Common.BDExecute(sql);
                sql = "UPDATE ExamQuestion SET IdTheme = 0 WHERE IdTheme = " + IdTheme.ToString();
                Common.BDExecute(sql);
                Log.Add(SessionHandler.Id, LogKind.DELETE, Modules.EVAL_THEME_CATALOG, IdTheme, "");
                return ErrorCode.NO_ERROR;
            }
            else
            {
                return ErrorCode.CANT_DELETE;
            }
        }

        /// <summary>
        /// Updates the current's theme data.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error code otherwise.</returns>
        public int Update()
        {
            string sql = "UPDATE ExamTheme SET Theme = '" + Common.SQSF(Theme) + "', IdParentTheme = " + IdParentTheme + " WHERE IdTheme = " + IdTheme.ToString();
            Common.BDExecute(sql);
            Log.Add(SessionHandler.Id, LogKind.UPDATE, Modules.EVAL_THEME_CATALOG, IdTheme, "");
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Retrieves the number of available questions for the current theme.
        /// </summary>
        /// <returns>The number of available questions.</returns>
        public int NumberOfQuestions()
        {
            string ThemeAndChildrenThemes = IdTheme.ToString();
            ThemeAndChildrenThemes = GetChildrenThemes(IdTheme.ToString());
            int numberOfQuestions = Common.GetBDNum("HowMany", "SELECT COUNT(*) AS HowMany FROM ExamQuestion WHERE IdTheme IN(" + ThemeAndChildrenThemes + ") AND IdTheme NOT IN(0) AND Status=" + ExamQuestionStatus.ACTIVE);
            return numberOfQuestions;
        }

        /// <summary>
        /// Retrieves a list of the children themes for the given IdThemes.
        /// </summary>
        /// <param name="idThemes">A CSV list of themes.</param>
        /// <returns>A CSV list of the children themes.</returns>
        public string GetChildrenThemes(string idThemes)
        {
            string retval = "";
            string sql = "SELECT IdTheme FROM ExamTheme WHERE IdParentTheme IN (" + idThemes + ")";
            string tempChildren = Common.GetBDList("IdTheme", sql, false);
            retval = Common.StrAdd(retval, ",", idThemes);
            if (!String.IsNullOrEmpty(tempChildren))
            {
                retval = Common.StrAdd(retval, ",", GetChildrenThemes(tempChildren));
            }
            return retval;
        }

        /// <summary>
        /// Determines if a given IdTheme is a child of the current Theme.
        /// </summary>
        /// <param name="id">The IdTheme to check.</param>
        /// <returns>True if it is, false if it isn't.</returns>
        public bool IsChild(int id)
        {
            string sql = "SELECT IdParentTheme FROM ExamTheme WHERE IdTheme = " + id;
            int tempId = Common.GetBDNum("IdParentTheme", sql);
            if (tempId == 0)
            {
                return false;
            }
            else if (tempId == IdTheme)
            {
                return true;
            }
            else
            {
                if (IsChild(tempId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Determines if a given IdTheme is a parent of the current Theme.
        /// </summary>
        /// <param name="id">The IdTheme to check.</param>
        /// <returns>True if it is, false if it isn't.</returns>
        public bool IsParent(int id)
        {
            string sql = "SELECT IdTheme FROM ExamTheme WHERE IdParentTheme = " + id;
            int tempId = Common.GetBDNum("IdTheme", sql);
            if (tempId == 0)
            {
                return false;
            }
            else if (tempId == IdParentTheme)
            {
                return true;
            }
            else
            {
                if (IsParent(tempId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }

    /// <summary>
    /// The Exam Class defines the structure of a single exam, plus any actions it can have.
    /// </summary>
    public class Exam
    {
        public int IdExam = 0;
        public string ExamName = "";
        public int Status = ExamStatus.INACTIVE;
        public int SelfEnroll = 0;
        public int MasteryScore = 0;
        public int Shuffle = 0;
        public int QuestionsPerPage = 5;
        public string Instructions = "";
        public int MaxMins = 0;
        //not on db
        public List<ExamContent> examContents = new List<ExamContent>();

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Exam() { }

        /// <summary>
        /// Overload of the constructor. Receives a IdExam and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdExam of the user to retrieve information from.</param>
        public Exam(int id)
        {
            List<Dictionary<string, string>> exam = Common.GetRS("SELECT * FROM Exam WHERE IdExam = " + id);
            foreach (Dictionary<string, string> record in exam)
            {
                IdExam = id;
                ExamName = record["Exam"];
                Status = Convert.ToInt32(record["Status"]);
                SelfEnroll = Convert.ToInt32(record["SelfEnroll"]);
                MasteryScore = Convert.ToInt32(record["MasteryScore"]);
                Shuffle = Convert.ToInt32(record["Shuffle"]);
                QuestionsPerPage = Convert.ToInt32(record["QuestionsPerPage"]);
                Instructions = record["Instructions"];
                MaxMins = Convert.ToInt32(record["MaxMins"]);
            }
            List<Dictionary<string, string>> examContentList = Common.GetRS("SELECT * FROM ExamContent WHERE IdExam = " + id + " ORDER BY QuestionSequence");
            foreach (Dictionary<string, string> record in examContentList)
            {
                ExamContent examContent = new ExamContent(Convert.ToInt32(record["IdExamContent"]));
                examContents.Add(examContent);
            }
        }

        /// <summary>
        /// Checks if the exam name is valid.
        /// </summary>
        /// <returns>NO_ERROR if the Exam is valid, the error number otherwise.</returns>
        private int ValidExam()
        {
            int retval = ErrorCode.NO_ERROR;
            if (Common.GetBDNum("NameExists", "SELECT COUNT(IdExam) AS NameExists FROM Exam WHERE Exam='" + Common.SQSF(ExamName) + "' AND IdExam <> " + IdExam.ToString()) > 0)
            {
                retval = ErrorCode.ALREADY_EXISTS;
            }
            else if (String.IsNullOrWhiteSpace(ExamName))
            {
                retval = ErrorCode.MISSING_FIELDS;
            }
            return retval;
        }


        /// <summary>
        /// Saves a new exam in the database.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            int errorCode = ValidExam();
            if (errorCode == ErrorCode.NO_ERROR)
            {
                string sql = "INSERT INTO Exam(Exam, Status, SelfEnroll, MasteryScore, Shuffle, QuestionsPerPage, Instructions, MaxMins) VALUES('" + Common.SQSF(ExamName) + "', " + Status + ", " + SelfEnroll + ", " + MasteryScore + "," + Shuffle + ", " + QuestionsPerPage + ", '" + Common.SQSF(Instructions) + "', " + MaxMins + ");";
                Common.BDExecute(sql);
                IdExam = Common.GetBDNum("lastId", "SELECT MAX(IdExam) AS lastId FROM Exam");
                Log.Add(SessionHandler.Id, LogKind.CREATE, Modules.EVAL_EXAM_CATALOG, IdExam, "");
                return ErrorCode.NO_ERROR;
            }
            else
            {
                return errorCode;
            }
        }

        /// <summary>
        /// Deletes the current exam, plus any associated table data.
        /// </summary>
        /// <returns>NO_ERROR if deleted successfully, the error code otherwise.</returns>
        public int Delete()
        {
            bool hasBeenUsed = Common.GetBDNum("HowMany", "SELECT COUNT(IdExam) AS HowMany FROM UserExam WHERE IdExam = " + IdExam.ToString()) > 0;
            if (!hasBeenUsed)
            {
                string sql = "DELETE FROM Exam WHERE IdExam = " + IdExam.ToString();
                Common.BDExecute(sql);
                foreach (ExamContent examContent in examContents)
                {
                    examContent.Delete();
                }
                Log.Add(SessionHandler.Id, LogKind.DELETE, Modules.EVAL_EXAM_CATALOG_DELETE, IdExam, "");
                return ErrorCode.NO_ERROR;
            }
            else
            {
                return ErrorCode.CANT_DELETE;
            }
        }

        /// <summary>
        /// Updates the current's exam data.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error code otherwise.</returns>
        public int Update()
        {
            bool hasBeenUsed = Common.GetBDNum("HowMany", "SELECT COUNT(IdExam) AS HowMany FROM UserExam WHERE IdExam = " + IdExam.ToString()) > 0;
            if (!hasBeenUsed)
            {
                int errorCode = ValidExam();
                if (errorCode == ErrorCode.NO_ERROR)
                {
                    string sql = "UPDATE Exam SET Exam='" + Common.SQSF(ExamName) + "', Status=" + Status + ", SelfEnroll=" + SelfEnroll + ", MasteryScore=" + MasteryScore + ", Shuffle=" + Shuffle + ", QuestionsPerPage=" + QuestionsPerPage + ", Instructions='" + Common.SQSF(Instructions) + "', MaxMins=" + MaxMins + " WHERE IdExam = " + IdExam.ToString();
                    Common.BDExecute(sql);
                    foreach (ExamContent examContent in examContents)
                    {
                        examContent.Update();
                    }
                    Log.Add(SessionHandler.Id, LogKind.UPDATE, Modules.EVAL_EXAM_CATALOG_DELETE, IdExam, "");
                    return ErrorCode.NO_ERROR;
                }
                else
                {
                    return errorCode;
                }
            }
            else
            {
                return ErrorCode.CANT_MODIFY;
            }
        }
    }

    /// <summary>
    /// The ExamContent Class defines the structure of a single content of an exam, plus any actions it can have.
    /// </summary>
    public class ExamContent
    {
        public int IdExamContent = 0;
        public int IdExam = 0;
        public int IdTheme = 0;
        public int QuestionCount = 0;
        public int IdQuestion = 0;
        public int QuestionSequence = 0;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ExamContent() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdExamContent and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdExamContent of the user to retrieve information from.</param>
        public ExamContent(int id)
        {
            List<Dictionary<string, string>> examContent = Common.GetRS("SELECT * FROM ExamContent WHERE IdExamContent = " + id + " ORDER BY QuestionSequence");
            foreach (Dictionary<string, string> record in examContent)
            {
                IdExamContent = id;
                IdExam = Convert.ToInt32(record["IdExam"]);
                IdTheme = Convert.ToInt32(record["IdTheme"]);
                QuestionCount = Convert.ToInt32(record["QuestionCount"]);
                IdQuestion = Convert.ToInt32(record["IdQuestion"]);
                QuestionSequence = Convert.ToInt32(record["QuestionSequence"]);
            }
        }

        /// <summary>
        /// Saves a new exam content in the database.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            if (IdQuestion != 0)
            {
                QuestionSequence = Common.GetBDNum("QuestionSequence", "SELECT MAX(QuestionSequence) AS QuestionSequence FROM ExamContent WHERE IdExam = " + IdExam) + 1;
            }
            string sql = "INSERT INTO ExamContent(IdExam, IdTheme, QuestionCount, IdQuestion, QuestionSequence) VALUES(" + IdExam + ", " + IdTheme + ", " + QuestionCount + ", " + IdQuestion + ", " + QuestionSequence + ");";
            Common.BDExecute(sql);
            IdExamContent = Common.GetBDNum("lastId", "SELECT MAX(IdExamContent) AS lastId FROM ExamContent");
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Deletes the current exam content, plus any associated table data.
        /// </summary>
        /// <returns>NO_ERROR if deleted successfully, the error code otherwise.</returns>
        public int Delete()
        {
            string sql = "DELETE FROM ExamContent WHERE IdExamContent = " + IdExamContent;
            Common.BDExecute(sql);
            sql = "UPDATE ExamContent SET QuestionSequence = QuestionSequence - 1 WHERE IdExam = " + IdExam + " AND QuestionSequence > " + QuestionSequence;
            Common.BDExecute(sql);
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Updates the current's exam content data.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error code otherwise.</returns>
        public int Update()
        {
            string sql = "UPDATE ExamContent SET IdTheme=" + IdTheme + ", QuestionCount=" + QuestionCount + ", IdQuestion=" + IdQuestion + ", QuestionSequence=" + QuestionSequence + " WHERE IdExamContent = " + IdExamContent;
            Common.BDExecute(sql);
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Updates the Sequence for a given ExamContent.
        /// </summary>
        public void SetSequence()
        {
            string sql = "UPDATE ExamContent SET QuestionSequence=" + QuestionSequence + " WHERE IdExamContent = " + IdExamContent;
            Common.BDExecute(sql);
        }
        

    }

    /// <summary>
    /// The UserExam Class defines the structure of a single user's exam, plus any actions it can have.
    /// </summary>
    public class UserExam
    {
        public int IdUserExam = 0;
        public int IdUser = 0;
        public int IdExam = 0;
        public int Status = UserExamStatus.PENDING;
        public DateTime DateComplete = new DateTime();
        public double Score = 0;
        public List<UserExamQuestion> questions = new List<UserExamQuestion>();

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public UserExam() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdUserExam and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdUserExam of the user to retrieve information from.</param>
        public UserExam(int id)
        {
            ExamSetup(id);
        }

        /// <summary>
        /// Overload of the constructor. Receives an IdUser and an IdExam to populate the instance with its values.
        /// </summary>
        /// <param name="idExam">The IdExam of the exam.</param>
        /// <param name="idUser">The IdUser of the user.</param>
        public UserExam(int idExam, int idUser)
        {
            ExamSetup(Common.GetBDNum("IdUserExam", "SELECT MAX(IdUserExam) AS IdUserExam FROM UserExam WHERE IdExam = " + idExam + " AND IdUser = " + idUser));
        }

        /// <summary>
        /// Constructor private auxiliar function.
        /// </summary>
        /// <param name="id">IdUserExam</param>
        private void ExamSetup(int id)
        {
            List<Dictionary<string, string>> userExam = Common.GetRS("SELECT * FROM UserExam WHERE IdUserExam = " + id);
            foreach (Dictionary<string, string> record in userExam)
            {
                IdUserExam = id;
                IdUser = Convert.ToInt32(record["IdUser"]);
                IdExam = Convert.ToInt32(record["IdExam"]);
                Status = Convert.ToInt32(record["Status"]);
                try
                {
                    DateComplete = Convert.ToDateTime(record["DateComplete"]);
                }
                catch (FormatException ex) { }
                Score = Convert.ToDouble(record["Score"]);
            }
            List<Dictionary<string, string>> userExamQuestions = Common.GetRS("SELECT * FROM UserExamQuestion WHERE IdUserExam = " + id);
            foreach (Dictionary<string, string> record in userExamQuestions)
            {
                UserExamQuestion userExamQuestion = new UserExamQuestion(id, Convert.ToInt32(record["IdQuestion"]));
                userExamQuestion.QuestionSequence = Convert.ToInt32(record["QuestionSequence"]);
                questions.Add(userExamQuestion);
            }
        }

        /// <summary>
        /// Creates an exam given the selected configuration, also locks said exam.
        /// </summary>
        /// <returns>NO_ERROR if created successfully.</returns>
        public int Create()
        {
            string sql = "INSERT INTO UserExam(IdUser, IdExam, Status) VALUES(" + IdUser + ", " + IdExam + ", " + UserExamStatus.PENDING + ")";
            Common.BDExecute(sql);
            IdUserExam = Common.GetBDNum("lastId", "SELECT MAX(IdUserExam) AS lastId FROM UserExam");
            Exam exam = new Exam(IdExam);
            foreach (ExamContent examContent in exam.examContents)
            {
                if (examContent.IdTheme != 0)
                {
                    ExamTheme theme = new ExamTheme(examContent.IdTheme);
                    List<int> selectedIds = Common.GetIdList("IdQuestion", "SELECT TOP " + examContent.QuestionCount + " IdQuestion FROM ExamQuestion WHERE IdTheme IN (" + theme.GetChildrenThemes(examContent.IdTheme.ToString()) + ") ORDER BY newid();");
                    foreach (int selectedId in selectedIds)
                    {
                        UserExamQuestion examQuestion = new UserExamQuestion(IdUserExam, selectedId);
                        examQuestion.Create();
                    }
                }
                else if(examContent.IdQuestion != 0)
                {
                    UserExamQuestion examQuestion = new UserExamQuestion(IdUserExam, examContent.IdQuestion);
                    examQuestion.QuestionSequence = examContent.QuestionSequence;
                    examQuestion.Create();
                }
            }
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Grades the user exam.
        /// </summary>
        public void Grade()
        {
            int totalQuestions = 0;
            int totalPoints = 0;
            foreach (UserExamQuestion userExamQuestion in questions)
            {
                ExamQuestion question = new ExamQuestion(userExamQuestion.IdQuestion);
                foreach(ExamOption option in question.options)
                {
                    if (option.IdOption == userExamQuestion.IdOption)
                    {
                        totalPoints += option.Points;
                    }
                }
                totalQuestions++;
            }
            Score = ((double)totalPoints / (double)totalQuestions) * 100;
            Exam exam = new Exam(IdExam);
            if (exam.MasteryScore == 0)
            {
                Status = UserExamStatus.COMPLETE;
            }
            else if (Score >= exam.MasteryScore)
            {
                Status = UserExamStatus.PASSED;
            }
            else
            {
                Status = UserExamStatus.FAILED;
            }
            string sql = "UPDATE UserExam SET Status=" + Status + ", Score=" + Score + ", DateComplete=getdate() WHERE IdUserExam = " + IdUserExam;
            Common.BDExecute(sql);
            Log.Add(SessionHandler.Id, LogKind.GENERAL, Modules.EVALUATION, IdExam, Text.Exam + ": " + exam.ExamName + ", [IdU: " + IdUser + ", IdUE: " + IdUserExam + "]");
        }

    }

    /// <summary>
    /// The UserExamQuestion Class defines the structure of a single question for a user in an exam, plus any actions it can have.
    /// </summary>
    public class UserExamQuestion
    {
        public int IdUserExam = 0;
        public int IdQuestion = 0;
        public int IdOption = 0;
        public int QuestionSequence = 0;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public UserExamQuestion() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdUserExam and IdQuestion and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdUserExam of the user/exam.</param>
        /// <param name="idQuestion">The IdQuestion to retrieve information from.</param>
        public UserExamQuestion(int id, int idQuestion)
        {
            IdUserExam = id;
            IdQuestion = idQuestion;
            IdOption = Common.GetBDNum("IdOption", "SELECT IdOption FROM UserExamQuestion WHERE IdUserExam = " + IdUserExam + " AND IdQuestion = " + IdQuestion);
        }

        /// <summary>
        /// Saves a new question for the user/exam.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            string sql = "INSERT INTO UserExamQuestion(IdUserExam, IdQuestion, IdOption, QuestionSequence) VALUES(" + IdUserExam + ", " + IdQuestion + ", 0, " + QuestionSequence + ");";
            Common.BDExecute(sql);
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Updates the current user/exam question.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error code otherwise.</returns>
        public int Update()
        {
            string sql = "UPDATE UserExamQuestion SET IdOption=" + IdOption + " WHERE IdUserExam = " + IdUserExam + " AND IdQuestion = " + IdQuestion;
            Common.BDExecute(sql);
            return ErrorCode.NO_ERROR;
        }
    }

}