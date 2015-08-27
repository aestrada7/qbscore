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
    /// The Users Class defines several static functions pertaining to the users.
    /// </summary>
    public static class Users
    {
        /// <summary>
        /// Retrieves a list of Users.
        /// </summary>
        /// <param name="filter">A given SQL Statement filter.</param>
        /// <returns>A List of Users. (List&lt;User&gt;)</returns>
        public static List<User> GetUsers(string filter)
        {
            List<User> userList = new List<User>();
            string sql = "SELECT IdUser FROM Users";
            filter = Common.StrAdd(filter, " AND ", "IdUser > 0");
            sql = Common.StrAdd(sql, " WHERE ", filter);
            string[] idUserList = Common.CSVToArray(Common.GetBDList("IdUser", sql, false));
            foreach (string idUser in idUserList)
            {
                try
                {
                    User user = new User(Convert.ToInt32(idUser));
                    userList.Add(user);
                }
                catch (Exception ex) { }
            }
            return userList;
        }

        /// <summary>
        /// Retrieves and returns the user list.
        /// </summary>
        /// <param name="modules">The modules for the current user.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="filter">A given SQL Statement filter.</param>
        /// <returns>An HTML including the user list.</returns>
        public static string GetUserList(Dictionary<string, bool> modules, int currentPage, string filter)
        {
            string retval = "";
            string className = "";
            int total = 0;
            string sql = "SELECT COUNT(IdUser) AS HowMany FROM Users";
            filter = Common.StrAdd(filter, " AND ", "IdUser > 0");
            sql = Common.StrAdd(sql, " WHERE ", filter);
            double recordsPerPage = Config.RecordsPerPage();
            double totalRecords = Common.GetBDNum("HowMany", sql);
            int totalPages = 1;
            try
            {
                Convert.ToInt32(Math.Ceiling(totalRecords / recordsPerPage));
            }
            catch (OverflowException ex) { }

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
            retval += "<tr><th width='20px'>" + Text.Identifier + "</th><th>" + Text.Name + "</th><th>" + Text.Username + "</th><th>" + Text.RegistryDate + "</th></tr>";

            sql = "SELECT IdUser, LastName FROM (SELECT IdUser, LastName, ROW_NUMBER() OVER (ORDER BY LastName) AS RowNum FROM Users";
            sql = Common.StrAdd(sql, " WHERE ", filter);
            sql += ") AS U WHERE U.RowNum BETWEEN ((" + currentPage + " - 1) * " + recordsPerPage + ") + 1 AND " + recordsPerPage + " * (" + currentPage + ")";
            string[] idUserList = Common.CSVToArray(Common.GetBDList("IdUser", sql, false));
            foreach (string idUser in idUserList)
            {
                try
                {
                    User user = new User(Convert.ToInt32(idUser));
                    string fullName = Common.StrAdd(user.LastName, " ", user.MotherLastName);
                    fullName = Common.StrAdd(fullName, " ", user.Name);
                    className = Common.SwitchClass(className);
                    string extraStyle = "";
                    if (user.Status == Status.BLOCKED)
                    {
                        extraStyle = "color: #CC0000;";
                    }
                    else if (user.Status == Status.INACTIVE)
                    {
                        extraStyle = "font-style: italic;";
                    }
                    retval += "<tr class='" + className + "' onClick='showUser(" + idUser + ");' style='" + extraStyle + "'>";
                    //retval += "<td>" + DrawInput.InputCheckbox("user_" + idUser, idUser, false, "", "", "", "") + "</td>";
                    retval += "<td>" + user.IdUser + "</td>";
                    retval += "<td>" + fullName + "</td>";
                    retval += "<td>" + user.Username + "</td>";
                    retval += "<td>" + user.RegistryDate.ToShortDateString() + "</td>";
                    retval += "</tr>";
                    total++;
                }
                catch (Exception ex) { }
            }

            retval += "</table>";

            //footer / pagination
            retval += "<div align='center' class='pagination'>";
            retval += "<div align='left' style='width: 50%; display: inline-block;'>" + Common.StrLang(Text.ShowingXofY, total.ToString() + "," + totalRecords.ToString()) + " " + Text.Users + "</div>";
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
    /// The User Class defines the structure of a single user, plus any actions it can have.
    /// </summary>
    public class User
    {
        //Fields on Table
        public int IdUser = 0;
        public string IdExternal = "";
        public string Username = "";
        public string Password = "";
        public string Name = "";
        public string LastName = "";
        public string MotherLastName = "";
        public int Status = 0;
        public DateTime RegistryDate = new DateTime();
        public int PrivacyAccepted = 0;
        //Related Fields
        public int IdRole = 0;
        public List<UserData> userDataList = new List<UserData>();
        //Auxiliar Fields
        public string userFieldsError = "";

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public User() { }

        /// <summary>
        /// Overload of the constructor. Receives a IdUser and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdUser of the user to retrieve information from.</param>
        public User(int id)
        {
            List<Dictionary<string, string>> user = Common.GetRS("SELECT * FROM Users WHERE IdUser = " + id);
            foreach (Dictionary<string, string> record in user)
            {
                IdUser = id;
                IdExternal = record["IdExternal"];
                Username = record["Username"];
                Password = record["Password"];
                Name = record["Name"];
                LastName = record["LastName"];
                MotherLastName = record["MotherLastName"];
                Status = Convert.ToInt32(record["Status"]);
                RegistryDate = Convert.ToDateTime(record["RegistryDate"]);
                PrivacyAccepted = Convert.ToInt32(record["PrivacyAccepted"]);
                IdRole = Common.GetBDNum("IdRole", "SELECT IdRole FROM UserRoles WHERE IdUser = " + id);
            }
            List<Dictionary<string, string>> userDataDB = Common.GetRS("SELECT * FROM UserData WHERE IdUser = " + id);
            foreach (Dictionary<string, string> record in userDataDB)
            {
                UserData userData = new UserData(id, Convert.ToInt32(record["IdData"]));
                userDataList.Add(userData);
            }
        }

        /// <summary>
        /// Saves a new user in the database.
        /// </summary>
        /// <returns>The newly generated IdUser.</returns>
        public int Create()
        {
            bool usernameExists = Common.GetBDNum("IdUser", "SELECT IdUser FROM Users WHERE Username = '" + Common.SQSF(Username) + "'") > 0;
            if (usernameExists)
            {
                return 0;
            }
            else
            {
                if (Config.UseCryptoPassword()) Password = Common.EncryptSHA512(Password);
                string sql = "INSERT INTO Users(IdExternal, Username, Password, Name, LastName, MotherLastName, Status, RegistryDate, PrivacyAccepted) VALUES('" + Common.SQSF(IdExternal) + "', '" + Common.SQSF(Username) + "', '" + Common.SQSF(Password) + "', '" + Common.SQSF(Name) + "', '" + Common.SQSF(LastName) + "', '" + Common.SQSF(MotherLastName) + "', 1, getdate(), 0);";
                Common.BDExecute(sql);
                IdUser = Common.GetBDNum("lastId", "SELECT MAX(IdUser) AS lastId FROM Users");
                sql = "INSERT INTO UserRoles(IdUser, IdRole) VALUES(" + IdUser + ", " + IdRole + ")";
                Common.BDExecute(sql);
                Log.Add(SessionHandler.Id, LogKind.CREATE, Modules.USER_ADMINISTRATION, IdUser, Common.SQSF(Username));
                return IdUser;
            }
        }

        /// <summary>
        /// Deactivates the current user.
        /// </summary>
        /// <returns>NO_ERROR if deactivated successfully, the error code otherwise.</returns>
        public int Deactivate()
        {
            string sql = "UPDATE Users SET Status = 0 WHERE IdUser = " + IdUser.ToString();
            Common.BDExecute(sql);
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Deletes the current user, plus any associated table data.
        /// </summary>
        /// <returns>NO_ERROR if deleted successfully, the error code otherwise.</returns>
        public int Delete()
        {
            string sql = "DELETE FROM Users WHERE IdUser = " + IdUser.ToString();
            Common.BDExecute(sql);
            sql = "DELETE FROM UserData WHERE IdUser = " + IdUser.ToString();
            Common.BDExecute(sql);
            sql = "DELETE FROM UserRoles WHERE IdUser = " + IdUser.ToString();
            Common.BDExecute(sql);
            sql = "DELETE FROM UserModules WHERE IdUser = " + IdUser.ToString();
            Common.BDExecute(sql);
            Log.Add(SessionHandler.Id, LogKind.DELETE, Modules.USER_ADMINISTRATION, IdUser, Common.SQSF(Username));
            return ErrorCode.NO_ERROR;
        }

        /// <summary>
        /// Updates the current's user data.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error code otherwise.</returns>
        public int Update()
        {
            userFieldsError = UpdateUserFields();
            UpdatePassword(true);
            string sql = "UPDATE Users SET IdExternal='" + Common.SQSF(IdExternal) + "', Username='" + Common.SQSF(Username) + "', Name='" + Common.SQSF(Name) + "', LastName='" + Common.SQSF(LastName) + "', MotherLastName='" + Common.SQSF(MotherLastName) + "', Status=" + Status + " WHERE IdUser = " + IdUser.ToString();
            Common.BDExecute(sql);
            int currentIdRole = Common.GetBDNum("IdRole", "SELECT IdRole FROM UserRoles WHERE IdUser = " + IdUser);
            if (currentIdRole != IdRole)
            {
                sql = "DELETE FROM UserRoles WHERE IdUser = " + IdUser;
                Common.BDExecute(sql);
                sql = "INSERT INTO UserRoles(IdUser, IdRole) VALUES(" + IdUser + ", " + IdRole + ")";
                Common.BDExecute(sql);
            }
            Log.Add(SessionHandler.Id, LogKind.UPDATE, Modules.USER_ADMINISTRATION, IdUser, Common.SQSF(Username));
            if (!String.IsNullOrEmpty(userFieldsError))
            {
                return ErrorCode.INVALID_FIELDS;
            }
            else
            {
                return ErrorCode.NO_ERROR;
            }
        }

        /// <summary>
        /// Updates the password for the given user.
        /// </summary>
        /// <param name="noLog">If true, doesn't log the password change.</param>
        public void UpdatePassword(bool noLog)
        {
            if (Config.UseCryptoPassword()) Password = Common.EncryptSHA512(Password);
            string sql = "UPDATE Users SET Password='" + Common.SQSF(Password) + "' WHERE IdUser = " + IdUser;
            Common.BDExecute(sql);
            if (!noLog)
            {
                Log.Add(SessionHandler.Id, LogKind.UPDATE, Modules.EDIT_SELF_PASSWORD, IdUser, Common.SQSF(Username));
            }
        }

        /// <summary>
        /// Attempts to update the user fields.
        /// </summary>
        /// <returns>If an error is found, returns the description, otherwise it returns an empty string.</returns>
        public string UpdateUserFields()
        {
            string errorMsg = "";
            bool requiredFieldsSet = true;
            List<DataDesc> dataDescList = Data.Data.GetDataDesc();
            foreach (DataDesc dataDesc in dataDescList)
            {
                UserData userData = new UserData(IdUser, dataDesc.IdData);
                userData.Value = HttpContext.Current.Request.Form["data_" + dataDesc.IdData];
                if (!dataDesc.IsValid(userData.Value) && dataDesc.Required == 1)
                {
                    requiredFieldsSet = false;
                    errorMsg = Text.RequiredFieldsMissing + "<br>";
                }
            }
            foreach (DataDesc dataDesc in dataDescList)
            {
                UserData userData = new UserData(IdUser, dataDesc.IdData);
                userData.Value = HttpContext.Current.Request.Form["data_" + dataDesc.IdData];
                if (dataDesc.IsValid(userData.Value))
                {
                    if(requiredFieldsSet) userData.Update();
                }
                else
                {
                    errorMsg = Common.StrAdd(errorMsg, "<br>", dataDesc.GetErrorMessage());
                }
            }
            if (!String.IsNullOrEmpty(errorMsg)) errorMsg += "<br>" + Text.ChangesNotSaved;
            return errorMsg;
        }

        /// <summary>
        /// Builds a table with all the user fields and returns the string.
        /// </summary>
        /// <returns>An HTML string.</returns>
        public string GetUserFields(bool isSelfInfo)
        {
            //tal vez esta función no deba ir aqui...
            string retval = "<div id='tabNav'><ul>";
            string tabBody = "";
            int currentIdDataGroup = 0;
            List<DataDesc> dataDescList = Data.Data.GetDataDesc();
            foreach (DataDesc dataDesc in dataDescList)
            {
                foreach (UserData userData in userDataList)
                {
                    if (userData.IdData == dataDesc.IdData)
                    {
                        if (!String.IsNullOrEmpty(userData.Value))
                        {
                            dataDesc.Value = userData.Value;
                        }
                    }
                }
                if (dataDesc.IdDataGroup != currentIdDataGroup)
                {
                    if (currentIdDataGroup != 0)
                    {
                        tabBody += "</table></div>";
                    }
                    tabBody += "<div id='tab" + dataDesc.IdDataGroup + "'><table width='70%' align='center'>";
                    currentIdDataGroup = dataDesc.IdDataGroup;
                    DataGroup dataGroup = new DataGroup(dataDesc.IdDataGroup);
                    retval += "<li><a href='#tab" + dataDesc.IdDataGroup + "'>" + dataGroup.Name + "</a></li>";
                }
                if (dataDesc.Kind != Data.Data.LABEL)
                {
                    bool canBeShown = false;
                    if (dataDesc.InvisibleToSelf == 1)
                    {
                        if (isSelfInfo)
                        {
                            if (Modules.Permission(SessionHandler.Modules, Modules.USER_ADMINISTRATION))
                            {
                                canBeShown = true;
                            }
                        }
                    }
                    else
                    {
                        canBeShown = true;
                    }
                    if (canBeShown)
                    {
                        string fieldName = dataDesc.Name;
                        if (dataDesc.Required == 1) fieldName = "<span class='required'>" + fieldName + "</span>";
                        string valueFromRequest = HttpContext.Current.Request.Form["data_" + dataDesc.IdData];
                        //if (dataDesc.Value == "") dataDesc.Value = valueFromRequest;
                        if (!String.IsNullOrEmpty(valueFromRequest)) dataDesc.Value = valueFromRequest;
                        tabBody += "<tr><td width='30%'>" + fieldName + ":</td><td width='70%'>" + dataDesc.GetFieldHTML() + "</td></tr>";
                    }
                }
                else
                {
                    tabBody += "<tr><td colspan='2'>" + dataDesc.GetFieldHTML() + "</td></tr>";
                }
            }
            if (!String.IsNullOrEmpty(tabBody)) tabBody += "</table></div>";
            return retval + "</ul>" + tabBody + "</div>";
        }

        /// <summary>
        /// Builds and returns the full name of the user.
        /// </summary>
        /// <param name="surnameFirst">If true, shows last names first, otherwise starts with given names, then surnames.</param>
        /// <returns>The full name.</returns>
        public string FullName(bool surnameFirst)
        {
            string retval = "";
            if (surnameFirst)
            {
                retval = LastName;
                retval = Common.StrAdd(retval, " ", MotherLastName);
                retval = Common.StrAdd(retval, " ", Name);
            }
            else
            {
                retval = Name;
                retval = Common.StrAdd(retval, " ", LastName);
                retval = Common.StrAdd(retval, " ", MotherLastName);
            }
            return retval;
        }
    }

    /// <summary>
    /// The UserData Class defines the structure of a single user data, plus any actions it can have.
    /// </summary>
    public class UserData
    {
        //Fields on Table
        public int IdUser = 0;
        public int IdData = 0;
        public string Value = "";

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public UserData() { }

        /// <summary>
        /// Overload of the constructor. Receives a IdUser and IdData and populates the instance with its values.
        /// </summary>
        /// <param name="idUser">The IdUser of the user to retrieve information from.</param>
        /// <param name="idData">The IdData of the user to retrieve information from.</param>
        public UserData(int idUser, int idData)
        {
            IdUser = idUser;
            IdData = idData;
            Value = Common.GetBD("Value", "SELECT Value FROM UserData WHERE IdUser = " + IdUser.ToString() + " AND IdData = " + IdData.ToString());
        }

        /// <summary>
        /// Updates or Inserts the new value.
        /// </summary>
        public bool Update()
        {
            Delete();
            Common.BDExecute("INSERT INTO UserData(IdUser, IdData, Value) VALUES(" + IdUser.ToString() + ", " + IdData.ToString() + ", '" + Common.SQSF(Value) + "')");
            return true;
        }

        /// <summary>
        /// Deletes the current UserData value.
        /// </summary>
        public bool Delete()
        {
            Common.BDExecute("DELETE FROM UserData WHERE IdUser = " + IdUser.ToString() + " AND IdData = " + IdData.ToString());
            return true;
        }
    }

    public struct Status
    {
        public static int INACTIVE = 0;
        public static int ACTIVE = 1;
        public static int BLOCKED = 2;
    }
}