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

namespace QBS.IO
{
    public class ImportExport
    {
        public List<string> ColumnTitles = new List<string>();
        private DataTable FullDataTable = new DataTable();

        /// <summary>
        /// Default constructor
        /// </summary>
        public ImportExport() {}

        /// <summary>
        /// Retrieves the data from the given filters.
        /// </summary>
        /// <param name="userFilter">An SQL query.</param>
        /// <returns>A DataTable with the full Data.</returns>
        public DataTable ExportData(string userFilter)
        {
            FullDataTable = new DataTable();
            ColumnTitles = new List<string>();
            UserSection(true);
            List<DataDesc> selectedDataDescList = new List<DataDesc>();
            List<DataDesc> dataDescList = Data.Data.GetDataDesc();
            foreach (DataDesc dataDesc in dataDescList)
            {
                if (HttpContext.Current.Request.Form["exp_data_" + dataDesc.IdData] == "1")
                {
                    selectedDataDescList.Add(dataDesc);
                    Type dataType = typeof(string);
                    if (dataDesc.Kind == Data.Data.FLOAT || dataDesc.Kind == Data.Data.INTEGER || dataDesc.Kind == Data.Data.SLIDER)
                    {
                        dataType = typeof(double);
                    }
                    FullDataTable.Columns.Add(dataDesc.IdData.ToString(), dataType);
                    ColumnTitles.Add(dataDesc.Name);
                }
            }
            //get filtered users and only retrieve selected columns' data
            string sql = "SELECT DISTINCT(IdUser) AS IdUser FROM Users";
            sql = Common.StrAdd(sql, " WHERE ", userFilter);
            string[] idUserList = Common.CSVToArray(Common.GetBDList("IdUser", sql, false));
            foreach (string idUser in idUserList)
            {
                var row = FullDataTable.NewRow();
                try
                {
                    User user = new User(Convert.ToInt32(idUser));
                    if (row.Table.Columns.Contains("usr_id"))
                    {
                        row["usr_id"] = user.IdUser;
                    }
                    if (row.Table.Columns.Contains("usr_idexternal"))
                    {
                        row["usr_idexternal"] = user.IdExternal;
                    }
                    if (row.Table.Columns.Contains("usr_username"))
                    {
                        row["usr_username"] = user.Username;
                    }
                    if (row.Table.Columns.Contains("usr_lastname"))
                    {
                        row["usr_lastname"] = user.LastName;
                    }
                    if (row.Table.Columns.Contains("usr_mlastname"))
                    {
                        row["usr_mlastname"] = user.MotherLastName;
                    }
                    if (row.Table.Columns.Contains("usr_name"))
                    {
                        row["usr_name"] = user.Name;
                    }
                    if (row.Table.Columns.Contains("usr_regdate"))
                    {
                        row["usr_regdate"] = user.RegistryDate;
                    }
                    if (row.Table.Columns.Contains("usr_status"))
                    {
                        string userStatus = "";
                        if (user.Status == Status.INACTIVE)
                        {
                            userStatus = Text.Inactive;
                        }
                        else if (user.Status == Status.ACTIVE)
                        {
                            userStatus = Text.Active;
                        }
                        else if (user.Status == Status.BLOCKED)
                        {
                            userStatus = Text.Blocked;
                        }
                        row["usr_status"] = userStatus;
                    }
                    foreach (DataDesc dataDesc in selectedDataDescList)
                    {
                        string currentValue = "";
                        UserData userData = new UserData(user.IdUser, dataDesc.IdData);
                        if (dataDesc.Kind == Data.Data.TREE)
                        {
                            currentValue = dataDesc.GetTreeValue(userData.Value);
                        }
                        else
                        {
                            currentValue = userData.Value;
                        }
                        if (dataDesc.Kind == Data.Data.FLOAT || dataDesc.Kind == Data.Data.INTEGER || dataDesc.Kind == Data.Data.SLIDER)
                        {
                            double currentNum = 0;
                            try
                            {
                                currentNum = Convert.ToDouble(currentValue);
                            }
                            catch (Exception ex) { }
                            row[dataDesc.IdData.ToString()] = currentNum;
                        }
                        else
                        {
                            row[dataDesc.IdData.ToString()] = currentValue;
                        }
                    }
                }
                catch(Exception ex) {}
                FullDataTable.Rows.Add(row);
            }
            return FullDataTable;
        }

        /// <summary>
        /// Retrieves all columns, or only selected ones if is exporting.
        /// </summary>
        /// <param name="isExporting">Boolean to determine if is exporting or importing.</param>
        private void UserSection(bool isExporting)
        {
            string columnName = "";
            if (isExporting && HttpContext.Current.Request.Form["usr_id"] == "1")
            {
                FullDataTable.Columns.Add("usr_id", typeof(Int32));
                ColumnTitles.Add(Text.Identifier);
            }
            if ((!isExporting) || (isExporting && HttpContext.Current.Request.Form["usr_idexternal"] == "1"))
            {
                FullDataTable.Columns.Add("usr_idexternal", typeof(string));
                ColumnTitles.Add(Text.ExternalIdentifier);
            }
            if ((!isExporting) || (isExporting && HttpContext.Current.Request.Form["usr_username"] == "1"))
            {
                FullDataTable.Columns.Add("usr_username", typeof(string));
                columnName = Text.Username;
                if (!isExporting)
                {
                    columnName += " (*)";
                }
                ColumnTitles.Add(columnName);
            }
            if ((!isExporting) || (isExporting && HttpContext.Current.Request.Form["usr_lastname"] == "1"))
            {
                FullDataTable.Columns.Add("usr_lastname", typeof(string));
                columnName = Text.LastName;
                if (!isExporting)
                {
                    columnName += " (*)";
                }
                ColumnTitles.Add(columnName);
            }
            if ((!isExporting) || (isExporting && HttpContext.Current.Request.Form["usr_mlastname"] == "1"))
            {
                FullDataTable.Columns.Add("usr_mlastname", typeof(string));
                ColumnTitles.Add(Text.MotherLastName);
            }
            if ((!isExporting) || (isExporting && HttpContext.Current.Request.Form["usr_name"] == "1"))
            {
                FullDataTable.Columns.Add("usr_name", typeof(string));
                columnName = Text.Name;
                if (!isExporting)
                {
                    columnName += " (*)";
                }
                ColumnTitles.Add(columnName);
            }
            if (isExporting && HttpContext.Current.Request.Form["usr_regdate"] == "1")
            {
                FullDataTable.Columns.Add("usr_regdate", typeof(string));
                ColumnTitles.Add(Text.RegistryDate);
            }
            if (isExporting && HttpContext.Current.Request.Form["usr_status"] == "1")
            {
                FullDataTable.Columns.Add("usr_status", typeof(string));
                ColumnTitles.Add(Text.Status);
            }
            if (!isExporting)
            {
                FullDataTable.Columns.Add("usr_password", typeof(string));
                ColumnTitles.Add(Text.Password + " (*)");
            }
        }

        /// <summary>
        /// Builds the layout with all the current active fields.
        /// </summary>
        /// <returns>A datatable with all the active fields.</returns>
        public DataTable BuildLayout()
        {
            FullDataTable = new DataTable();
            ColumnTitles = new List<string>();
            UserSection(false);
            List<DataDesc> dataDescList = Data.Data.GetDataDesc();
            foreach (DataDesc dataDesc in dataDescList)
            {
                Type dataType = typeof(string);
                if (dataDesc.Kind == Data.Data.FLOAT || dataDesc.Kind == Data.Data.INTEGER || dataDesc.Kind == Data.Data.SLIDER)
                {
                    dataType = typeof(double);
                }
                if (dataDesc.Kind != Data.Data.LABEL)
                {
                    FullDataTable.Columns.Add(dataDesc.IdData.ToString(), dataType);
                    string columnName = dataDesc.Name;
                    if (dataDesc.Required == 1)
                    {
                        columnName += " (*)";
                    }
                    ColumnTitles.Add(columnName);
                }
            }
            return FullDataTable;
        }

        /// <summary>
        /// Builds the Exam import layout.
        /// </summary>
        /// <returns>A DataTable with the full exam.</returns>
        public DataTable BuildExamLayout()
        {
            FullDataTable = new DataTable();
            ColumnTitles = new List<string>();
            FullDataTable.Columns.Add("question", typeof(string));
            ColumnTitles.Add(Text.Question);
            FullDataTable.Columns.Add("theme", typeof(string));
            ColumnTitles.Add(Text.Theme);
            for (int i = 1; i <= 10; i++)
            {
                FullDataTable.Columns.Add("option" + i, typeof(string));
                ColumnTitles.Add(Text.Option + " " + i);
                FullDataTable.Columns.Add("points" + i, typeof(Int32));
                ColumnTitles.Add(Text.Points);
            }
            return FullDataTable;
        }

        /// <summary>
        /// Retrieves the exam data from the given filters.
        /// </summary>
        /// <param name="userFilter">An SQL query.</param>
        /// <returns>A DataTable with the full Data.</returns>
        public DataTable ExportDataExam(string filter)
        {
            FullDataTable = new DataTable();
            ColumnTitles = new List<string>();
            string sql = "SELECT IdUserExam FROM UserExam";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            string[] idUserExamList = Common.CSVToArray(Common.GetBDList("IdUserExam", sql, false));
            FullDataTable.Columns.Add("usr_name", typeof(string));
            ColumnTitles.Add(Text.FullName);
            FullDataTable.Columns.Add("exam_name", typeof(string));
            ColumnTitles.Add(Text.Exam);
            FullDataTable.Columns.Add("exam_date", typeof(string));
            ColumnTitles.Add(Text.DateTime);
            FullDataTable.Columns.Add("exam_status", typeof(string));
            ColumnTitles.Add(Text.Status);
            FullDataTable.Columns.Add("exam_score", typeof(double));
            ColumnTitles.Add(Text.Score);
            foreach (string idUserExam in idUserExamList)
            {
                var row = FullDataTable.NewRow();
                UserExam userExam = new UserExam(Convert.ToInt32(idUserExam));
                User user = new User(userExam.IdUser);
                Exam exam = new Exam(userExam.IdExam);
                row["usr_name"] = user.FullName(true);
                row["exam_name"] = exam.ExamName;
                row["exam_date"] = userExam.DateComplete;
                row["exam_status"] = UserExamStatus.FriendlyText(userExam.Status);
                row["exam_score"] = String.Format("{0:0.00}", userExam.Score);
                FullDataTable.Rows.Add(row);
            }
            return FullDataTable;
        }
    }
}