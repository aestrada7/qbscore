using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Aexis.Web;
using Aexis;
using QScore.lang;
using QBS.ACL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QBS.Data
{
    public static partial class Data
    {
        #region Constants
        public const int SIMPLE_TEXT = 0;
        public const int YES_NO = 1;
        public const int OPTIONS = 2;
        public const int INTEGER = 3;
        public const int FLOAT = 4;
        public const int DATE = 5;
        public const int CHECKBOX = 6;
        public const int TREE = 7;
        public const int MULTI_TEXT = 8;
        public const int SLIDER = 9;
        public const int EMAIL = 10;
        public const int LABEL = 11;
        #endregion

        /// <summary>
        /// Retrieves the entire existing and active Data Fields in a DataDesc List.
        /// </summary>
        /// <returns>A DataDesc List.</returns>
        public static List<DataDesc> GetDataDesc()
        {
            string sql = "SELECT DataDesc.* FROM DataDesc INNER JOIN DataGroup ON DataDesc.IdDataGroup = DataGroup.IdDataGroup";
            string filter = "Inactive = 0";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            sql += " ORDER BY GroupSequence, FieldSequence";
            List<DataDesc> dataDescList = new List<DataDesc>();
            List<Dictionary<string, string>> dataDescDB = Common.GetRS(sql);
            foreach (Dictionary<string, string> record in dataDescDB)
            {
                DataDesc dataDesc = new DataDesc(Convert.ToInt32(record["IdData"]));
                dataDescList.Add(dataDesc);
            }
            return dataDescList;
        }

        /// <summary>
        /// Retrieves and returns the current Data Fields list.
        /// </summary>
        /// <returns>An HTML including the data fields list.</returns>
        public static string GetDataFieldsList()
        {
            string retval = "";
            string className = "";
            int total = 0;
            string dataGroupList = "";

            retval = "<table width='60%' align='center'><tr><td><ul id='dataGroups' style='list-style-type: none; margin: 0; padding: 0; width: 100%'>";

            string[] idDataGroupList = Common.CSVToArray(Common.GetBDList("IdDataGroup", "SELECT IdDataGroup, Name FROM DataGroup ORDER BY GroupSequence", false));
            foreach (string idDataGroup in idDataGroupList)
            {
                if (!String.IsNullOrEmpty(idDataGroup))
                {
                    DataGroup dataGroup = new DataGroup(Convert.ToInt32(idDataGroup));
                    dataGroupList = Common.StrAdd(dataGroupList, ",", "#idDG_" + dataGroup.IdDataGroup);
                    retval += "<li id='idGroup_" + dataGroup.IdDataGroup + "'><span class='moveIcon'><img src='images/move.png' /></span><div class='cellTitle' align='center' style='height: 32px;'>" + dataGroup.Name + "</div>";
                    retval += "<ul id='idDG_" + dataGroup.IdDataGroup + "' class='dragEnabledConn' style='list-style-type: none; margin: 0; padding: 8; width: 100%'>";

                    string[] idDataList = Common.CSVToArray(Common.GetBDList("IdData", "SELECT IdData, Name FROM DataDesc WHERE IdDataGroup = " + idDataGroup + " ORDER BY FieldSequence", false));
                    foreach (string idData in idDataList)
                    {
                        try
                        {
                            DataDesc dataDesc = new DataDesc(Convert.ToInt32(idData));
                            className = Common.SwitchClass(className);
                            retval += "<li id='idDI_" + dataDesc.IdData + "' class='" + className + "' style='height: 32px'><span class='moveIcon'><img src='images/move.png' /></span><span onClick='editField(" + idData + ");' style='margin-left: 30px;'>" + dataDesc.Name + " (" + Data.FriendlyName(dataDesc.Kind) + ")" + (dataDesc.Inactive == 1 ? " <i>" + Text.Inactive + "</i>" : "") + "</span></li>";
                            total++;
                        }
                        catch (Exception ex) { }
                    }
                    retval += "</ul>";
                    retval += "<div align='right'>";
                    if (Modules.PermissionOr(SessionHandler.Modules, Modules.EDIT_FIELDS + "," + Modules.DATAFIELDS_ADMINISTRATION))
                    {
                        retval += DrawInput.InputButton("editGrp_" + dataGroup.IdDataGroup, Text.Edit, "", "editGroup(" + dataGroup.IdDataGroup + ", '" + dataGroup.Name + "'); return false;", "background-image: url(./images/edit.png); background-repeat: no-repeat; height: 30px; padding-left: 30px;", "");
                    }
                    if (Modules.PermissionOr(SessionHandler.Modules, Modules.DELETE_FIELDS + "," + Modules.DATAFIELDS_ADMINISTRATION))
                    {
                        retval += DrawInput.InputButton("deleteGrp_" + dataGroup.IdDataGroup, Text.Delete, "", "deleteGroup(" + dataGroup.IdDataGroup + "); return false;", "background-image: url(./images/delete.png); background-repeat: no-repeat; height: 30px; padding-left: 30px;", "");
                    }
                    retval += "</div></li>";
                }
            }
            retval += "</ul>";

            if (!String.IsNullOrEmpty(dataGroupList))
            {
                retval += "<script type='text/javascript'>";
                retval += "$('" + dataGroupList + "').sortable({ connectWith: '.dragEnabledConn', placeholder: 'sort-placeholder', handle: '.moveIcon', stop: function(event, ui) { invalidateList($(this).attr('id'), $(this).sortable('toArray')); }, receive: function(event, ui) { invalidateList($(this).attr('id'), $(this).sortable('toArray')); } }).disableSelection();";
                retval += "$('#dataGroups').sortable({ handle: '.moveIcon', stop: function(event, ui) { invalidateList($(this).attr('id'), $(this).sortable('toArray')); } }).disableSelection();";
                retval += "</script>";
            }

            //Footer
            retval += "<div align='center' class='pagination'>";
            retval += "<div align='left' style='width: 100%; display: inline-block;'>" + Common.StrLang(Text.ShowingXofY, total.ToString() + "," + total.ToString()) + " " + Text.Field_s + "</div>";
            retval += "</div></td></tr></table>";

            return retval;
        }

        /// <summary>
        /// Returns the friendly name of the given kind.
        /// </summary>
        /// <param name="kind">A given kind.</param>
        /// <returns>A string with the friendly name.</returns>
        public static string FriendlyName(int kind)
        {
            string retval = "";
            switch (kind)
            {
                case SIMPLE_TEXT:
                    retval = Text.fld_SimpleText;
                    break;
                case YES_NO:
                    retval = Text.fld_YesNo;
                    break;
                case OPTIONS:
                    retval = Text.fld_Options;
                    break;
                case INTEGER:
                    retval = Text.fld_Integer;
                    break;
                case FLOAT:
                    retval = Text.fld_Float;
                    break;
                case DATE:
                    retval = Text.fld_Date;
                    break;
                case CHECKBOX:
                    retval = Text.fld_Checkbox;
                    break;
                case TREE:
                    retval = Text.fld_Tree;
                    break;
                case MULTI_TEXT:
                    retval = Text.fld_Multitext;
                    break;
                case SLIDER:
                    retval = Text.fld_Slider;
                    break;
                case EMAIL:
                    retval = Text.fld_Email;
                    break;
                case LABEL:
                    retval = Text.fld_Label;
                    break;
            }
            return retval;
        }
    }

    /// <summary>
    /// The DataDesc Class defines the structure of a single data field.
    /// </summary>
    public partial class DataDesc
    {
        public int IdData = 0;
        public int IdDataGroup = 0;
        public int Kind = 0;
        public string Name = "";
        public string ShortName = "";
        public string AuxValues = "";
        public string RegExValidation = "";
        public int Required = 0;
        public int InvisibleToSelf = 0;
        public int Inactive = 0;
        public int FieldSequence = 0;
        //Not in DB
        public string Value = "";
        public string ErrorMessage = "";
        public string CustomQuery = "";
        public Dictionary<string, string> FriendlyOptions = new Dictionary<string, string>();
        public string DataGroupName = "";

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public DataDesc() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdData and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdData of the data desc to retrieve information from.</param>
        public DataDesc(int id)
        {
            List<Dictionary<string, string>> dataDesc = Common.GetRS("SELECT * FROM DataDesc WHERE IdData = " + id);
            foreach (Dictionary<string, string> record in dataDesc)
            {
                IdData = id;
                IdDataGroup = Convert.ToInt32(record["IdDataGroup"]);
                Kind = Convert.ToInt32(record["Kind"]);
                Name = record["Name"];
                ShortName = record["ShortName"];
                AuxValues = record["AuxValues"];
                RegExValidation = record["RegExValidation"];
                Required = Convert.ToInt32(record["Required"]);
                InvisibleToSelf = Convert.ToInt32(record["InvisibleToSelf"]);
                Inactive = Convert.ToInt32(record["Inactive"]);
                FieldSequence = Convert.ToInt32(record["FieldSequence"]);
            }
        }

        /// <summary>
        /// Saves a new Data Desc in the database.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            int retval = ValidDataDesc();
            if (retval == ErrorCode.NO_ERROR)
            {
                int nextSequence = Common.GetBDNum("NextSequence", "SELECT MAX(FieldSequence) AS NextSequence FROM DataDesc WHERE IdDataGroup = " + IdDataGroup) + 1;
                string sql = "INSERT INTO DataDesc(IdDataGroup, Kind, Name, ShortName, AuxValues, RegExValidation, Required, InvisibleToSelf, Inactive, FieldSequence) VALUES(" + IdDataGroup + ", " + Kind + ", '" + Common.SQSF(Name) + "', '" + Common.SQSF(ShortName) + "', '" + Common.SQSF(AuxValues) + "', '" + Common.SQSF(RegExValidation) + "', " + Required + ", " + InvisibleToSelf + ", " + 1 + ", " + nextSequence + ");";
                Common.BDExecute(sql);
                IdData = Common.GetBDNum("lastId", "SELECT MAX(IdData) AS lastId FROM DataDesc");
                Log.Add(SessionHandler.Id, LogKind.CREATE, Modules.DATAFIELDS_ADMINISTRATION, IdData, Common.SQSF(Name));
            }
            return retval;
        }

        /// <summary>
        /// Updates the current Data Desc.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error number otherwise.</returns>
        public int Update()
        {
            int retval = ValidDataDesc();
            if (retval == ErrorCode.NO_ERROR)
            {
                string sql = "UPDATE DataDesc SET IdDataGroup=" + IdDataGroup + ", Kind=" + Kind + ", Name='" + Common.SQSF(Name) + "', ShortName='" + Common.SQSF(ShortName) + "', AuxValues='" + Common.SQSF(AuxValues) + "', RegExValidation='" + Common.SQSF(RegExValidation) + "', Required=" + Required + ", InvisibleToSelf=" + InvisibleToSelf + ", Inactive=" + Inactive + ", FieldSequence=" + FieldSequence + " WHERE IdData = " + IdData.ToString();
                Common.BDExecute(sql);
                Log.Add(SessionHandler.Id, LogKind.UPDATE, Modules.DATAFIELDS_ADMINISTRATION, IdData, Common.SQSF(Name));
            }
            return retval;
        }

        /// <summary>
        /// Deletes the current Data Desc.
        /// </summary>
        /// <returns>NO_ERROR if deleted successfully, the error number otherwise.</returns>
        public int Delete()
        {
            int totalRecords = Common.GetBDNum("HowMany", "SELECT COUNT(*) AS HowMany FROM UserData WHERE IdData = " + IdData.ToString());
            int retval = totalRecords == 0 ? ErrorCode.NO_ERROR : ErrorCode.CANT_DELETE;
            if (retval == ErrorCode.NO_ERROR)
            {
                string sql = "DELETE FROM DataDesc WHERE IdData = " + IdData.ToString();
                Common.BDExecute(sql);
                Log.Add(SessionHandler.Id, LogKind.DELETE, Modules.DATAFIELDS_ADMINISTRATION, IdData, Common.SQSF(Name));
            }
            return retval;
        }

        /// <summary>
        /// Checks if the Data Desc is valid.
        /// </summary>
        /// <returns>NO_ERROR if the name is valid, the error number otherwise.</returns>
        private int ValidDataDesc()
        {
            int retval = ErrorCode.NO_ERROR;
            if (Common.GetBDNum("NameExists", "SELECT COUNT(IdData) AS NameExists FROM DataDesc WHERE Name='" + Common.SQSF(Name) + "' AND IdData <> " + IdData.ToString()) > 0)
            {
                retval = ErrorCode.ALREADY_EXISTS;
            }
            else if (String.IsNullOrWhiteSpace(Name))
            {
                retval = ErrorCode.MISSING_FIELDS;
            }
            return retval;
        }

        /// <summary>
        /// Updates the Sequence and DataGroup for a given Data Field.
        /// </summary>
        public void SetSequence()
        {
            string sql = "UPDATE DataDesc SET FieldSequence=" + FieldSequence + ", IdDataGroup=" + IdDataGroup + " WHERE IdData = " + IdData;
            Common.BDExecute(sql);
        }
        
        /// <summary>
        /// Builds the HTML code for a given field.
        /// </summary>
        /// <returns>The HTML string of the field.</returns>
        public string GetFieldHTML()
        {
            string retval = "";
            string fieldId = "data_" + IdData.ToString();
            string validationFunction = "";
            switch (Kind)
            {
                case Data.SIMPLE_TEXT:
                    retval = DrawInput.InputTextField(fieldId, Value, "", "", "", "", "");
                    break;
                case Data.YES_NO:
                    retval = DrawInput.InputSelect(fieldId, Value, Common.CSVToArray("," + Text.Yes + "," + Text.No), "", "", "", "");
                    break;
                case Data.OPTIONS:
                    retval = DrawInput.InputSelect(fieldId, Value, Common.CSVToArray('|', "|" + AuxValues), "", "", "", "");
                    break;
                case Data.INTEGER:
                    NumberJSON numberJSON = JsonConvert.DeserializeObject<NumberJSON>(AuxValues);
                    if ((numberJSON.from == numberJSON.to) && (numberJSON.to == 0))
                    {
                        validationFunction = "validateInt($(this), event);";
                        numberJSON.maxLength = "10";
                    }
                    else
                    {
                        validationFunction = "validateInt($(this), event, " + numberJSON.from + ", " + numberJSON.to + ");";
                    }
                    retval = DrawInput.InputTextField(fieldId, Value, numberJSON.maxLength, "", validationFunction, "width: 50px;", "title=\"" + Common.StrLang(Text.EnterValueBetweenXandY, numberJSON.from + "," + numberJSON.to) + "\" onKeyUp=\"validateInt($(this), event);\"");
                    break;
                case Data.FLOAT:
                    NumberJSON numberJSONf = JsonConvert.DeserializeObject<NumberJSON>(AuxValues);
                    if ((numberJSONf.from == numberJSONf.to) && (numberJSONf.to == 0))
                    {
                        validationFunction = "validateFloat($(this), event);";
                        numberJSONf.maxLength = "10";
                    }
                    else
                    {
                        validationFunction = "validateFloat($(this), event, " + numberJSONf.from + ", " + numberJSONf.to + ");";
                    }
                    retval = DrawInput.InputTextField(fieldId, Value, numberJSONf.maxLength, "", validationFunction, "width: 50px;", "title=\"" + Common.StrLang(Text.EnterValueBetweenXandY, numberJSONf.from + "," + numberJSONf.to) + "\" onKeyUp=\"validateFloat($(this), event);\"");
                    break;
                case Data.DATE:
                    retval = DrawInput.InputTextField(fieldId, Value, "", "datePicker", "", "width: 85px;", "");
                    break;
                case Data.CHECKBOX:
                    retval = DrawInput.InputCheckbox(fieldId, Value, (Value == "1"), "", "", "", "onChange='toggleCheckbox($(this));'");
                    break;
                case Data.TREE:
                    retval = DrawInput.InputHiddenField(fieldId, Value, "", "");
                    retval += "<div id='tree_" + fieldId + "'></div>";
                    retval += "<script language='JavaScript'>";
                    retval += "$('#tree_" + fieldId + "').treeSelectJSON({";
                    retval += "  source: " + AuxValues + ",";
                    retval += "  target: '" + fieldId + "',";
                    retval += "  defaultText: '- " + Common.SQSF(Text.Select) + " -',";
                    retval += "  preselected: '" + Value + "'";
                    retval += "});";
                    retval += "</script>";
                    break;
                case Data.MULTI_TEXT:
                    retval = DrawInput.InputTextArea(fieldId, Value, "4", "", "", "", "width: 100%;", "");
                    break;
                case Data.SLIDER:
                    if (String.IsNullOrEmpty(Value)) Value = "0";
                    retval = DrawInput.InputHiddenField(fieldId, Value, "", "");
                    retval += "<div id='" + fieldId + "_slider'></div>";
                    retval += "<script language='JavaScript'>";
                    retval += "$('#" + fieldId + "_slider').slider({ value: " + Value + ", max: " + AuxValues + ", change: function() { $('#" + fieldId + "').val($('#" + fieldId + "_slider').slider('option', 'value')) }});";
                    retval += "</script>";
                    break;
                case Data.EMAIL:
                    retval = DrawInput.InputTextField(fieldId, Value, "", "", "validateEmail($(this));", "", "title=\"" + Text.InvalidEmail + "\" placeholder=\"mail@mail.com\"");
                    break;
                case Data.LABEL:
                    retval = "<div align='center'>" + AuxValues + "</div>";
                    break;
                default:
                    break;
            }
            return retval;
        }

        /// <summary>
        /// Validates the given Value.
        /// </summary>
        /// <returns>True if valid, false otherwise.</returns>
        public bool IsValid(string newValue)
        {
            bool retval = false;
            Value = newValue;
            Regex regEx;
            if (!String.IsNullOrEmpty(Value))
            {
                switch (Kind)
                {
                    case Data.SIMPLE_TEXT:
                        if (RegExValidation != "")
                        {
                            regEx = new Regex(RegExValidation, RegexOptions.IgnoreCase);
                            if (regEx.IsMatch(Value))
                            {
                                retval = true;
                            }
                            else
                            {
                                ErrorMessage = Name + ": " + Text.InvalidData;
                            }
                        }
                        else
                        {
                            retval = true;
                        }
                        break;
                    case Data.YES_NO:
                        if ((Value == Text.Yes) || (Value == Text.No))
                        {
                            retval = true;
                        }
                        else
                        {
                            ErrorMessage = Name + ": " + Text.InvalidData;
                        }
                        break;
                    case Data.OPTIONS:
                        string[] values = Common.CSVToArray('|', "|" + AuxValues);
                        foreach (string val in values)
                        {
                            if (Value == val)
                            {
                                retval = true;
                                break;
                            }
                        }
                        if (!retval)
                        {
                            ErrorMessage = Name + ": " + Text.InvalidData;
                        }
                        break;
                    case Data.INTEGER:
                    case Data.FLOAT:
                        NumberJSON numberJSON = JsonConvert.DeserializeObject<NumberJSON>(AuxValues);
                        if ((numberJSON.from == numberJSON.to) && (numberJSON.to == 0))
                        {
                            retval = true;
                        }
                        else
                        {
                            if ((Convert.ToDouble(Value) >= Convert.ToDouble(numberJSON.from)) && (Convert.ToDouble(Value) <= Convert.ToDouble(numberJSON.to)))
                            {
                                retval = true;
                            }
                            else
                            {
                                ErrorMessage = Name + ": " + Common.StrLang(Text.EnterValueBetweenXandY, numberJSON.from + "," + numberJSON.to);
                            }
                        }
                        break;
                    case Data.DATE:
                        retval = true;
                        break;
                    case Data.CHECKBOX:
                        if((Value == "1") || (Value == "0"))
                        {
                            retval = true;
                        }
                        break;
                    case Data.TREE:
                        retval = true;
                        break;
                    case Data.MULTI_TEXT:
                        retval = true;
                        break;
                    case Data.SLIDER:
                        retval = true;
                        break;
                    case Data.EMAIL:
                        string pattern = @"[-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}";
                        regEx = new Regex(pattern, RegexOptions.IgnoreCase);
                        if (regEx.IsMatch(Value))
                        {
                            retval = true;
                        }
                        else
                        {
                            ErrorMessage = Name + ": " + Text.InvalidEmail;
                        }
                        break;
                    case Data.LABEL:
                        retval = true;
                        break;
                    default:
                        break;
                }
            }
            else if (Required == 1)
            {
                ErrorMessage = Name + ": " + Text.MissingFields;
            }
            else
            {
                retval = true;
            }
            return retval;
        }

        /// <summary>
        /// Retrieves the tree value from the current DataDesc.
        /// </summary>
        /// <param name="value">The value to look for.</param>
        /// <returns>A friendly text value.</returns>
        public string GetTreeValue(string value)
        {
            string retval = "";
            if (Kind == Data.TREE && !String.IsNullOrEmpty(value))
            {
                string[] ids = value.Split('_');
                JObject json = JObject.Parse(AuxValues);
                JToken currJson = json["children"][Convert.ToInt32(ids[0]) - 1];
                retval = (string)currJson["value"];
                for (int i = 1; i < ids.Length; i++)
                {
                    for (int k = 0; k < currJson["children"].Count(); k++)
                    {
                        if (Convert.ToInt32((string)currJson["children"][k]["id"]) == Convert.ToInt32(ids[i]))
                        {
                            currJson = currJson["children"][k];
                            break;
                        }
                    }
                    retval = Common.StrAdd(retval, " - ", (string)currJson["value"]);
                }
            }
            return retval;
        }

        /// <summary>
        /// Retrieves the Ids for a given value.
        /// </summary>
        /// <param name="value">A string as formatted by GetTreeValue ([element] - [child1] - [child2])</param>
        /// <returns>The Ids for the given value (1_2_1)</returns>
        public string GetTreeIdsFromValue(string value)
        {
            string retval = "";
            int i;
            if (Kind == Data.TREE && !String.IsNullOrEmpty(value))
            {
                JObject json = JObject.Parse(AuxValues);
                JToken currJson = json;
                string[] values = value.Split('-');
                try
                {
                    foreach (string currVal in values)
                    {
                        string tmpVal = currVal.Trim();
                        for (i = 0; i < currJson["children"].Count(); i++)
                        {
                            string currJsonValue = (string)currJson["children"][i]["value"];
                            if (currJsonValue.ToUpper() == tmpVal.ToUpper())
                            {
                                currJson = currJson["children"][i];
                                retval = Common.StrAdd(retval, "_", (string)currJson["id"]);
                            }
                        }
                    }
                }
                catch (Exception ex) { }
            }
            return retval;
        }

        /// <summary>
        /// Retrieves the current error message (if it exists).
        /// </summary>
        /// <returns>A string containing the error message.</returns>
        public string GetErrorMessage()
        {
            return ErrorMessage;
        }
    }

    /// <summary>
    /// The DataGroup Class defines the structure of a data group.
    /// </summary>
    public class DataGroup
    {
        public int IdDataGroup = 0;
        public string Name = "";
        public int GroupSequence = 0;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public DataGroup() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdDataGroup and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdDataGroup of the data group to retrieve information from.</param>
        public DataGroup(int id)
        {
            List<Dictionary<string, string>> dataGroup = Common.GetRS("SELECT * FROM DataGroup WHERE IdDataGroup = " + id);
            foreach (Dictionary<string, string> record in dataGroup)
            {
                IdDataGroup = id;
                Name = record["Name"];
                GroupSequence = Convert.ToInt32(record["GroupSequence"]);
            }
        }

        /// <summary>
        /// Updates the Sequence for a given Data Group.
        /// </summary>
        public void SetSequence()
        {
            string sql = "UPDATE DataGroup SET GroupSequence=" + GroupSequence + " WHERE IdDataGroup = " + IdDataGroup;
            Common.BDExecute(sql);
        }

        /// <summary>
        /// Saves a new Data Group in the database.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            int retval = ValidDataGroup();
            if (retval == ErrorCode.NO_ERROR)
            {
                int nextSequence = Common.GetBDNum("NextSequence", "SELECT MAX(GroupSequence) AS NextSequence FROM DataGroup") + 1;
                string sql = "INSERT INTO DataGroup(Name, GroupSequence) VALUES('" + Common.SQSF(Name) + "', " + nextSequence + ");";
                Common.BDExecute(sql);
                IdDataGroup = Common.GetBDNum("lastId", "SELECT MAX(IdDataGroup) AS lastId FROM DataGroup");
                Log.Add(SessionHandler.Id, LogKind.CREATE, Modules.DATAFIELDS_ADMINISTRATION, IdDataGroup, Text.DataGroup + ": " + Common.SQSF(Name));
            }
            return retval;
        }

        /// <summary>
        /// Updates the current Data Group.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error number otherwise.</returns>
        public int Update()
        {
            int retval = ValidDataGroup();
            if (retval == ErrorCode.NO_ERROR)
            {
                string sql = "UPDATE DataGroup SET Name = '" + Common.SQSF(Name) + "' WHERE IdDataGroup = " + IdDataGroup.ToString();
                Common.BDExecute(sql);
                Log.Add(SessionHandler.Id, LogKind.UPDATE, Modules.DATAFIELDS_ADMINISTRATION, IdDataGroup, Text.DataGroup + ": " + Common.SQSF(Name));
            }
            return retval;
        }

        /// <summary>
        /// Deletes the current Data Group.
        /// </summary>
        /// <returns>NO_ERROR if deleted successfully, the error number otherwise.</returns>
        public int Delete()
        {
            int currNumberOfChildren = Common.GetBDNum("HowMany", "SELECT COUNT(*) AS HowMany FROM DataDesc WHERE IdDataGroup = " + IdDataGroup.ToString());
            int retval = currNumberOfChildren == 0 ? ErrorCode.NO_ERROR : ErrorCode.CANT_DELETE;
            if (retval == ErrorCode.NO_ERROR)
            {
                string sql = "DELETE FROM DataGroup WHERE IdDataGroup = " + IdDataGroup.ToString();
                Common.BDExecute(sql);
                Log.Add(SessionHandler.Id, LogKind.DELETE, Modules.DATAFIELDS_ADMINISTRATION, IdDataGroup, Text.DataGroup + ": " + Common.SQSF(Name));
            }
            return retval;
        }

        /// <summary>
        /// Checks if the Data Group is valid.
        /// </summary>
        /// <returns>NO_ERROR if the name is valid, the error number otherwise.</returns>
        private int ValidDataGroup()
        {
            int retval = ErrorCode.NO_ERROR;
            if (Common.GetBDNum("NameExists", "SELECT COUNT(IdDataGroup) AS NameExists FROM DataGroup WHERE Name='" + Common.SQSF(Name) + "' AND IdDataGroup <> " + IdDataGroup.ToString()) > 0)
            {
                retval = ErrorCode.ALREADY_EXISTS;
            }
            else if (String.IsNullOrWhiteSpace(Name))
            {
                retval = ErrorCode.MISSING_FIELDS;
            }
            return retval;
        }

    }

    /// <summary>
    /// Auxiliar Class that mimics the structure given on the JSON string.
    /// </summary>
    public class NumberJSON
    {
        public string maxLength = "";
        public int from = 0;
        public int to = 0;
    }
}