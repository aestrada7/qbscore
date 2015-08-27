using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Aexis.Web;
using Aexis;
using QScore.lang;
using QBS.ACL;
using QBS.Exams;
using Newtonsoft.Json;

namespace QBS.Data
{
    /// <summary>
    /// Partial Class for the Data Class. Includes definitions for the Search functions.
    /// </summary>
    public static partial class Data
    {
        public static List<DataDesc> ExtraList = new List<DataDesc>();

        /// <summary>
        /// Adds all the exam fields to the search.
        /// </summary>
        public static void SetExtraExam(bool persist)
        {
            if(!persist) ExtraList.Clear();
            DataDesc tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.EXAM_NAME;
            tmpData.Name = Text.Exam;
            tmpData.Kind = Data.OPTIONS;
            tmpData.AuxValues = Common.GetBDList("Exam", "SELECT Exam FROM Exam WHERE Status = " + ExamStatus.ACTIVE, false).ToString().Replace(",", "|");
            tmpData.CustomQuery = "IdExam IN (SELECT IdExam FROM Exam WHERE Exam = '#SEARCH_VALUE#')";
            tmpData.IdDataGroup = SearchEnum.EXAMS;
            tmpData.DataGroupName = Text.Exams;
            ExtraList.Add(tmpData);

            tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.EXAM_SCORE;
            tmpData.Name = Text.Score;
            NumberJSON numberJSON = new NumberJSON();
            numberJSON.from = 0;
            numberJSON.to = 100;
            numberJSON.maxLength = "4";
            tmpData.AuxValues = JsonConvert.SerializeObject(numberJSON);
            tmpData.Kind = Data.FLOAT;
            tmpData.CustomQuery = "CAST(Score AS Float) >= '#SEARCH_VALUE#' AND CAST(Score AS Float) <= '#SEARCH_VALUE2#'";
            tmpData.IdDataGroup = SearchEnum.EXAMS;
            ExtraList.Add(tmpData);

            tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.EXAM_STATUS;
            tmpData.Name = Text.Status;
            tmpData.Kind = Data.OPTIONS;
            tmpData.AuxValues = Text.Exam_Failed + "|" + Text.Exam_Incomplete + "|" + Text.Exam_Passed + "|" + Text.Exam_Pending;
            Dictionary<string, string> userStatusFriendly = new Dictionary<string, string>();
            userStatusFriendly.Add(Text.Exam_Failed, UserExamStatus.FAILED.ToString());
            userStatusFriendly.Add(Text.Exam_Incomplete, UserExamStatus.INCOMPLETE.ToString());
            userStatusFriendly.Add(Text.Exam_Passed, UserExamStatus.PASSED.ToString());
            userStatusFriendly.Add(Text.Exam_Pending, UserExamStatus.PENDING.ToString());
            tmpData.FriendlyOptions = userStatusFriendly;
            tmpData.CustomQuery = "Status IN (#SEARCH_VALUE#)";
            tmpData.IdDataGroup = SearchEnum.EXAMS;
            tmpData.DataGroupName = Text.Exams;
            ExtraList.Add(tmpData);

            tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.EXAM_DATE_COMPLETED;
            tmpData.Name = Text.DateCompleted;
            tmpData.Kind = Data.DATE;
            tmpData.CustomQuery = "CONVERT(DATETIME, DateComplete, 103) BETWEEN CONVERT(DATETIME, '#SEARCH_VALUE#', 103) AND CONVERT(DATETIME, '#SEARCH_VALUE2#', 103)";
            tmpData.IdDataGroup = SearchEnum.EXAMS;
            ExtraList.Add(tmpData);
        }

        /// <summary>
        /// Adds all the user fields to the search.
        /// </summary>
        public static void SetExtraListUsers()
        {
            ExtraList.Clear();
            DataDesc tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.USERNAME;
            tmpData.Name = Text.Username;
            tmpData.Kind = Data.SIMPLE_TEXT;
            tmpData.CustomQuery = "IdUser IN (SELECT IdUser FROM Users WHERE Username LIKE '%#SEARCH_VALUE#%')";
            tmpData.IdDataGroup = SearchEnum.USERS;
            tmpData.DataGroupName = Text.Users;
            ExtraList.Add(tmpData);

            tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.LAST_NAME;
            tmpData.Name = Text.LastName;
            tmpData.Kind = Data.SIMPLE_TEXT;
            tmpData.CustomQuery = "IdUser IN (SELECT IdUser FROM Users WHERE LastName LIKE '%#SEARCH_VALUE#%')";
            tmpData.IdDataGroup = SearchEnum.USERS;
            ExtraList.Add(tmpData);

            tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.MOTHER_LAST_NAME;
            tmpData.Name = Text.MotherLastName;
            tmpData.Kind = Data.SIMPLE_TEXT;
            tmpData.IdDataGroup = SearchEnum.USERS;
            tmpData.CustomQuery = "IdUser IN (SELECT IdUser FROM Users WHERE MotherLastName LIKE '%#SEARCH_VALUE#%')";
            ExtraList.Add(tmpData);
            
            tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.NAME;
            tmpData.Name = Text.Name;
            tmpData.Kind = Data.SIMPLE_TEXT;
            tmpData.CustomQuery = "IdUser IN (SELECT IdUser FROM Users WHERE Name LIKE '%#SEARCH_VALUE#%')";
            tmpData.IdDataGroup = SearchEnum.USERS;
            ExtraList.Add(tmpData);

            tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.REGISTRY_DATE;
            tmpData.Name = Text.RegistryDate;
            tmpData.Kind = Data.DATE;
            tmpData.CustomQuery = "IdUser IN (SELECT IdUser FROM Users WHERE CONVERT(DATETIME, RegistryDate, 103) BETWEEN CONVERT(DATETIME, '#SEARCH_VALUE#', 103) AND CONVERT(DATETIME, '#SEARCH_VALUE2#', 103))";
            tmpData.IdDataGroup = SearchEnum.USERS;
            ExtraList.Add(tmpData);

            tmpData = new DataDesc();
            tmpData.IdData = SearchEnum.STATUS;
            tmpData.Name = Text.Status;
            tmpData.Kind = Data.OPTIONS;
            tmpData.AuxValues = Text.Inactive + "|" + Text.Active + "|" + Text.Blocked;
            Dictionary<string, string> userStatusFriendly = new Dictionary<string, string>();
            userStatusFriendly.Add(Text.Inactive, Status.INACTIVE.ToString());
            userStatusFriendly.Add(Text.Active, Status.ACTIVE.ToString());
            userStatusFriendly.Add(Text.Blocked, Status.BLOCKED.ToString());
            tmpData.FriendlyOptions = userStatusFriendly;
            tmpData.CustomQuery = "IdUser IN (SELECT IdUser FROM Users WHERE Status = '#SEARCH_VALUE#')";
            tmpData.IdDataGroup = SearchEnum.USERS;
            ExtraList.Add(tmpData);
        }

        /// <summary>
        /// Builds the full search HTML for all the data fields that are currently defined.
        /// </summary>
        /// <returns>The full HTML search.</returns>
        public static string BuildSearchHTML()
        {
            string retval = "<div id='tabNav'><ul>";
            string tabBody = "";
            int currentIdDataGroup = -100;
            List<DataDesc> dataDescList = GetDataDesc();
            dataDescList.AddRange(ExtraList);
            foreach (DataDesc dataDesc in dataDescList)
            {
                if (dataDesc.IdDataGroup != currentIdDataGroup)
                {
                    if (currentIdDataGroup != -100)
                    {
                        tabBody += "</table></div>";
                    }
                    tabBody += "<div id='tab" + dataDesc.IdDataGroup + "'><table width='70%' align='center'>";
                    currentIdDataGroup = dataDesc.IdDataGroup;
                    DataGroup dataGroup = new DataGroup(dataDesc.IdDataGroup);
                    string dataGroupName = dataGroup.Name;
                    if (!String.IsNullOrEmpty(dataDesc.DataGroupName))
                    {
                        dataGroupName = dataDesc.DataGroupName;
                    }
                    retval += "<li><a href='#tab" + dataDesc.IdDataGroup + "'>" + dataGroupName + "</a></li>";
                }
                if (dataDesc.Kind != LABEL)
                {
                    string fieldName = dataDesc.Name;
                    dataDesc.GetSearchValue();
                    tabBody += "<tr><td width='30%'>" + fieldName + ":</td><td width='70%'>" + dataDesc.GetSearchHTML() + "</td></tr>";
                }
            }
            if (!String.IsNullOrEmpty(tabBody)) tabBody += "</table></div>";
            return retval + "</ul>" + tabBody + "</div>";
        }

        /// <summary>
        /// Returns the filters for the current selection.
        /// </summary>
        /// <returns>An SQL filter.</returns>
        public static string GetFilters()
        {
            string retval = "";
            bool isClearing = HttpContext.Current.Request.Form["clearSearch"] == "1";
            if (isClearing)
            {
                SessionHandler.ClearSessionVars();
            }
            List<DataDesc> dataDescList = GetDataDesc();
            dataDescList.AddRange(ExtraList);
            foreach (DataDesc dataDesc in dataDescList)
            {
                dataDesc.GetSearchValue();
                if (!isClearing)
                {
                    retval = Common.StrAdd(retval, " AND ", dataDesc.GetFilter());
                }
            }
            return retval;
        }

        /// <summary>
        /// Returns the friendly text for the current selection.
        /// </summary>
        /// <returns>A string.</returns>
        public static string GetSearchDesc()
        {
            string retval = "";
            List<DataDesc> dataDescList = GetDataDesc();
            dataDescList.AddRange(ExtraList);
            foreach (DataDesc dataDesc in dataDescList)
            {
                dataDesc.GetSearchValue();
                retval = Common.StrAdd(retval, "<br />", dataDesc.GetSearchDescription());
            }
            return retval;
        }
    }
    
    /// <summary>
    /// Partial Class for the DataDesc Class. Includes all the definitions for the Search functions.
    /// </summary>
    public partial class DataDesc
    {
        public string SearchValue = "";
        public string SearchValueAux = "";

        /// <summary>
        /// Retrieves the current search values from session or request.
        /// </summary>
        public void GetSearchValue()
        {
            bool isClearing = HttpContext.Current.Request.Form["clearSearch"] == "1";
            if (!isClearing)
            {
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["data_" + IdData]))
                {
                    SessionHandler.AddToSessionVars("data_" + IdData, HttpContext.Current.Request.Form["data_" + IdData]);
                }
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["data_" + IdData + "_aux"]))
                {
                    SessionHandler.AddToSessionVars("data_" + IdData + "_aux", HttpContext.Current.Request.Form["data_" + IdData + "_aux"]);
                }
                try
                {
                    SearchValue = SessionHandler.SessionVars["data_" + IdData];
                    Value = SearchValue;
                }
                catch (Exception ex) { }
                try
                {
                    SearchValueAux = SessionHandler.SessionVars["data_" + IdData + "_aux"];
                }
                catch (Exception ex) { }
            }
            else
            {
                SessionHandler.ClearSessionVars();
            }
        }

        /// <summary>
        /// Retrieves the search description.
        /// </summary>
        /// <returns>A string</returns>
        public string GetSearchDescription()
        {
            string retval = "";
            switch (Kind)
            {
                case Data.SIMPLE_TEXT:
                case Data.CHECKBOX:
                case Data.MULTI_TEXT:
                case Data.EMAIL:
                    if (!String.IsNullOrEmpty(SearchValue))
                    {
                        retval = Name + " " + Text.Contains + " '" + SearchValue + "'";
                    }
                    break;
                case Data.YES_NO:
                case Data.OPTIONS:
                    if (!String.IsNullOrEmpty(SearchValue))
                    {
                        retval = Name + " " + Text.Is + " '" + SearchValue + "'";
                    }
                    break;
                case Data.TREE:
                    if (!String.IsNullOrEmpty(SearchValue))
                    {
                        retval = Name + " " + Text.Is + " '" + GetTreeValue(SearchValue) + "'";
                    }
                    break;
                case Data.INTEGER:
                case Data.FLOAT:
                case Data.SLIDER:
                case Data.DATE:
                    if (!String.IsNullOrEmpty(SearchValue) || !String.IsNullOrEmpty(SearchValueAux))
                    {
                        if(String.IsNullOrEmpty(SearchValueAux))
                        {
                            retval = Name + " " + Text.IsGreaterOrEqualTo + " '" + SearchValue + "'";
                        }
                        else if (String.IsNullOrEmpty(SearchValue))
                        {
                            retval = Name + " " + Text.IsSmallerOrEqualTo + " '" + SearchValueAux + "'";
                        }
                        else
                        {
                            retval = Name + " " + Text.IsBetween + " '" + SearchValue + "' " + Text.And + "'" + SearchValueAux + "'";
                        }
                    }
                    break;
            }
            return retval;
        }

        /// <summary>
        /// Retrieves the filter for the current SearchValues.
        /// </summary>
        /// <returns>An SQL filter.</returns>
        public string GetFilter()
        {
            string retval = "";
            switch (Kind)
            {
                case Data.SIMPLE_TEXT:
                case Data.YES_NO:
                case Data.OPTIONS:
                case Data.CHECKBOX:
                case Data.MULTI_TEXT:
                case Data.EMAIL:
                    if (!String.IsNullOrEmpty(SearchValue))
                    {
                        if (!String.IsNullOrEmpty(CustomQuery))
                        {
                            string searchItem = SearchValue;
                            if (FriendlyOptions.Count != 0)
                            {
                                try
                                {
                                    searchItem = FriendlyOptions[SearchValue];
                                }
                                catch(KeyNotFoundException ex) { }
                            }
                            searchItem = Common.RemoveIntrusiveChars(searchItem);
                            if (!String.IsNullOrEmpty(searchItem))
                            {
                                retval = CustomQuery.Replace("#SEARCH_VALUE#", Common.SQSF(searchItem));
                            }
                        }
                        else
                        {
                            retval = "IdUser IN (SELECT IdUser FROM UserData WHERE IdData = " + IdData + " AND Value LIKE '%" + Common.SQSF(SearchValue) + "%')";
                        }
                    }
                    break;
                case Data.TREE:
                    if (!String.IsNullOrEmpty(SearchValue))
                    {
                        retval = "IdUser IN (SELECT IdUser FROM UserData WHERE IdData = " + IdData + " AND Value LIKE '" + Common.SQSF(SearchValue) + "%')";
                    }
                    break;
                case Data.INTEGER:
                case Data.FLOAT:
                case Data.SLIDER:
                    if (!String.IsNullOrEmpty(SearchValue) || !String.IsNullOrEmpty(SearchValueAux))
                    {
                        if (String.IsNullOrEmpty(SearchValueAux))
                        {
                            retval = "IdUser IN (SELECT IdUser FROM UserData WHERE IdData = " + IdData + " AND CAST(Value AS Float) >= " + Common.SQSF(SearchValue) + ")";
                        }
                        else if (String.IsNullOrEmpty(SearchValue))
                        {
                            retval = "IdUser IN (SELECT IdUser FROM UserData WHERE IdData = " + IdData + " AND CAST(Value AS Float) <= " + Common.SQSF(SearchValueAux) + ")";
                        }
                        else
                        {
                            retval = "IdUser IN (SELECT IdUser FROM UserData WHERE IdData = " + IdData + " AND CAST(Value AS Float) >= " + Common.SQSF(SearchValue) + " AND CAST(Value AS Float) <= " + Common.SQSF(SearchValueAux) + ")";
                        }
                        if (!String.IsNullOrEmpty(CustomQuery))
                        {
                            if (String.IsNullOrEmpty(SearchValue))
                            {
                                SearchValue = "-999999";
                            }
                            if (String.IsNullOrEmpty(SearchValueAux))
                            {
                                SearchValueAux = "999999";
                            }
                            retval = CustomQuery.Replace("#SEARCH_VALUE#", Common.SQSF(SearchValue)).Replace("#SEARCH_VALUE2#", Common.SQSF(SearchValueAux));
                        }
                    }
                    break;
                case Data.DATE:
                    if (!String.IsNullOrEmpty(SearchValue) || !String.IsNullOrEmpty(SearchValueAux))
                    {
                        if (String.IsNullOrEmpty(SearchValueAux))
                        {
                            retval = "IdUser IN (SELECT IdUser FROM UserData WHERE IdData = " + IdData + " AND CONVERT(DATETIME, Value, 103) >= CONVERT(DATETIME, '" + Common.SQSF(SearchValue) + "', 103))";
                        }
                        else if (String.IsNullOrEmpty(SearchValue))
                        {
                            retval = "IdUser IN (SELECT IdUser FROM UserData WHERE IdData = " + IdData + " AND CONVERT(DATETIME, Value, 103) <= CONVERT(DATETIME, '" + Common.SQSF(SearchValueAux) + "', 103))";
                        }
                        else
                        {
                            retval = "IdUser IN (SELECT IdUser FROM UserData WHERE IdData = " + IdData + " AND CONVERT(DATETIME, Value, 103) BETWEEN CONVERT(DATETIME, '" + Common.SQSF(SearchValue) + "', 103) AND CONVERT(DATETIME, '" + Common.SQSF(SearchValueAux) + "', 103))";
                        }
                        if (!String.IsNullOrEmpty(CustomQuery))
                        {
                            if (String.IsNullOrEmpty(SearchValue))
                            {
                                SearchValue = "01/01/1900";
                            }
                            if (String.IsNullOrEmpty(SearchValueAux))
                            {
                                SearchValueAux = "01/01/2500";
                            }
                            retval = CustomQuery.Replace("#SEARCH_VALUE#", Common.SQSF(SearchValue)).Replace("#SEARCH_VALUE2#", Common.SQSF(SearchValueAux));
                        }
                    }
                    break;
            }
            return retval;
        }

        /// <summary>
        /// Retrieves the HTML for the current field.
        /// </summary>
        /// <returns>An HTML String.</returns>
        public string GetSearchHTML()
        {
            string retval = "";
            string fieldId = "data_" + IdData.ToString();
            string validationFunction = "";
            switch (Kind)
            {
                case Data.SIMPLE_TEXT:
                case Data.YES_NO:
                case Data.OPTIONS:
                case Data.CHECKBOX:
                case Data.TREE:
                case Data.EMAIL:
                    retval = GetFieldHTML();
                    break;
                case Data.MULTI_TEXT:
                    retval = DrawInput.InputTextField(fieldId, SearchValue, "", "", "", "", "");
                    break;
                case Data.INTEGER:
                case Data.FLOAT:
                    NumberJSON numberJSON = JsonConvert.DeserializeObject<NumberJSON>(AuxValues);
                    if ((numberJSON.from == numberJSON.to) && (numberJSON.to == 0))
                    {
                        numberJSON.maxLength = "10";
                    }
                    if (Kind == Data.FLOAT)
                    {
                        validationFunction = "validateFloat($(this), event);";
                    }
                    else
                    {
                        validationFunction = "validateInt($(this), event);";
                    }
                    retval = Text.Between + "&nbsp";
                    retval += DrawInput.InputTextField(fieldId, SearchValue, numberJSON.maxLength, "", validationFunction, "width: 50px;", "title=\"" + Common.StrLang(Text.EnterValueBetweenXandY, numberJSON.from + "," + numberJSON.to) + "\" onKeyUp=\"" + validationFunction + "\"");
                    retval += "&nbsp;" + Text.And + "&nbsp;";
                    retval += DrawInput.InputTextField(fieldId + "_aux", SearchValueAux, numberJSON.maxLength, "", validationFunction, "width: 50px;", "title=\"" + Common.StrLang(Text.EnterValueBetweenXandY, numberJSON.from + "," + numberJSON.to) + "\" onKeyUp=\"" + validationFunction + "\"");
                    break;
                case Data.SLIDER:
                    validationFunction = "validateInt($(this), event);";
                    retval = Text.Between + "&nbsp";
                    retval += DrawInput.InputTextField(fieldId, SearchValue, "10", "", validationFunction, "width: 50px;", "onKeyUp=\"" + validationFunction + "\"");
                    retval += "&nbsp;" + Text.And + "&nbsp;";
                    retval += DrawInput.InputTextField(fieldId + "_aux", SearchValueAux, "10", "", validationFunction, "width: 50px;", "onKeyUp=\"" + validationFunction + "\"");
                    break;
                case Data.DATE:
                    retval = Text.Between + "&nbsp";
                    retval += DrawInput.InputTextField(fieldId, SearchValue, "", "datePicker", "", "width: 85px;", "");
                    retval += "&nbsp;" + Text.And + "&nbsp;";
                    retval += DrawInput.InputTextField(fieldId + "_aux", SearchValueAux, "", "datePicker", "", "width: 85px;", "");
                    break;
                default:
                    break;
            }
            return retval;
        }
    }
}