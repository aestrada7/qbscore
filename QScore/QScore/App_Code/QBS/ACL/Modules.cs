using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;
using Aexis;
using QScore.lang;

namespace QBS.ACL
{
    /// <summary>
    /// The Modules Class has functions to define the given access to a user.
    /// </summary>
    public static class Modules
    {
        #region Constants
        public const int USER_ADMINISTRATION = 100;
        public const int ADD_USER = 101;
        public const int EDIT_USER = 102;
        public const int DELETE_USER = 103;
        public const int EDIT_PERMISSIONS = 104;
        public const int EDIT_PASSWORDS = 105;
        public const int EDIT_SELF_PASSWORD = 106;
        public const int EDIT_SELF_INFO = 107;
        public const int EXPORT_USERS = 108;
        public const int IMPORT_USERS = 109;
        public const int DATAFIELDS_ADMINISTRATION = 200;
        public const int ADD_FIELDS = 201;
        public const int EDIT_FIELDS = 202;
        public const int DELETE_FIELDS = 203;
        public const int LOG_ACCESS = 300;
        public const int CATALOGS = 400;
        public const int ROLE_CATALOG = 401;
        public const int ROLE_CATALOG_ADD = 402;
        public const int ROLE_CATALOG_EDIT = 403;
        public const int ROLE_CATALOG_DELETE = 404;
        public const int EVALUATION = 500;
        public const int EVAL_QUESTION_CATALOG = 501;
        public const int EVAL_QUESTION_CATALOG_ADD = 502;
        public const int EVAL_QUESTION_CATALOG_EDIT = 503;
        public const int EVAL_QUESTION_CATALOG_DELETE = 504;
        public const int EVAL_THEME_CATALOG = 505;
        public const int EVAL_THEME_CATALOG_ADD = 506;
        public const int EVAL_THEME_CATALOG_EDIT = 507;
        public const int EVAL_THEME_CATALOG_DELETE = 508;
        public const int EVAL_EXAM_CATALOG = 509;
        public const int EVAL_EXAM_CATALOG_ADD = 510;
        public const int EVAL_EXAM_CATALOG_EDIT = 511;
        public const int EVAL_EXAM_CATALOG_DELETE = 512;
        public const int EVAL_SOLVE_USER = 513;
        public const int EVAL_IMPORT_EXAMS = 514;
        public const int EVAL_IMPORT_RESULTS = 515;
        public const int EVAL_REPORTS = 516;
        public const int EVAL_ASSIGN = 517;
        #endregion

        public static string userAdministratorModuleList = USER_ADMINISTRATION.ToString() + "," + EDIT_USER.ToString() + "," + ADD_USER.ToString() + "," + DELETE_USER.ToString();
        public static string roleAdministratorModuleList = CATALOGS.ToString() + "," + ROLE_CATALOG.ToString() + "," + ROLE_CATALOG_ADD.ToString() + "," + ROLE_CATALOG_DELETE.ToString() + "," + ROLE_CATALOG_EDIT.ToString();
        public static string fieldsAdministratorModuleList = DATAFIELDS_ADMINISTRATION.ToString() + "," + ADD_FIELDS.ToString() + "," + EDIT_FIELDS.ToString() + "," + DELETE_FIELDS.ToString();
        public static string examAdministratorModuleList = EVAL_EXAM_CATALOG.ToString() + "," + EVAL_EXAM_CATALOG_ADD.ToString() + "," + EVAL_EXAM_CATALOG_DELETE.ToString() + "," + EVAL_EXAM_CATALOG_EDIT.ToString();
        public static string themeAdministratorModuleList = EVAL_THEME_CATALOG.ToString() + "," + EVAL_THEME_CATALOG_ADD.ToString() + "," + EVAL_THEME_CATALOG_DELETE.ToString() + "," + EVAL_THEME_CATALOG_EDIT.ToString();
        public static string questionAdministratorModuleList = EVAL_QUESTION_CATALOG.ToString() + "," + EVAL_QUESTION_CATALOG_ADD.ToString() + "," + EVAL_QUESTION_CATALOG_DELETE.ToString() + "," + EVAL_QUESTION_CATALOG_EDIT.ToString();

        /// <summary>
        /// Retrieves the modules that the current user has permission to access to.
        /// </summary>
        /// <param name="idUser">The IdUser of the current user.</param>
        /// <returns>A Dictionary(string, bool) of the existing modules and the access the user has.</returns>
        public static Dictionary<string, bool> GetModules(int idUser)
        {
            Dictionary<string, bool> moduleDictionary = new Dictionary<string, bool>();
            bool permissionGranted = false;
            bool permissionRevoked = false;
            bool moduleAccess = false;
            string sql = "SELECT IdRole FROM UserRoles WHERE IdUser = " + idUser.ToString();
            int idRole = Common.GetBDNum("IdRole", sql);
            moduleDictionary = GetRoleModules(idRole);
            List<Dictionary<string, string>> result = Common.GetRS("SELECT * FROM UserModules WHERE IdUser = " + idUser.ToString());
            foreach (Dictionary<string, string> records in result)
            {
                permissionGranted = records["GrantPermission"] == "1";
                permissionRevoked = records["RevokePermission"] == "1";
                moduleAccess = (permissionGranted) && (!permissionRevoked);
                moduleDictionary[records["IdModule"]] = moduleAccess;
                PropagateAccess(records["IdModule"], moduleAccess, moduleDictionary);
            }
            return moduleDictionary;
        }

        /// <summary>
        /// Retrieves the modules that the current role has permission to access to.
        /// </summary>
        /// <param name="idRole">The IdRole of the current role.</param>
        /// <returns>A Dictionary(string, bool) of the existing modules and the access the role has.</returns>
        public static Dictionary<string, bool> GetRoleModules(int idRole)
        {
            Dictionary<string, bool> moduleDictionary = new Dictionary<string, bool>();
            bool permissionGranted = false;
            bool permissionRevoked = false;
            bool moduleAccess = false;
            List<Dictionary<string, string>> modules = Common.GetRS("SELECT * FROM Modules");
            foreach (Dictionary<string, string> records in modules)
            {
                moduleDictionary.Add(records["IdModule"], false);
            }
            if (idRole != 0)
            {
                List<Dictionary<string, string>> result = Common.GetRS("SELECT * FROM RoleModules WHERE IdRole = " + idRole.ToString());
                foreach (Dictionary<string, string> records in result)
                {
                    permissionGranted = records["GrantPermission"] == "1";
                    permissionRevoked = records["RevokePermission"] == "1";
                    moduleAccess = (permissionGranted) && (!permissionRevoked);
                    moduleDictionary[records["IdModule"]] = moduleAccess;
                    PropagateAccess(records["IdModule"], moduleAccess, moduleDictionary);
                }
            }
            return moduleDictionary;
        }

        /// <summary>
        /// Retrieves the module tree structure, useful for editing. Recursive function.
        /// </summary>
        /// <param name="moduleList">An empty List&lt;Module&gt; that will eventually hold the information.</param>
        /// <param name="idModuleParent">The idModuleParent from which the tree will be shown.</param>
        /// <param name="level">The current depth level.</param>
        /// <returns>The same List&lt;Module&gt; but now with all the data.</returns>
        public static List<Module> GetModuleTree(List<Module> moduleList, int idModuleParent, int level)
        {
            List<Dictionary<string, string>> modules = Common.GetRS("SELECT * FROM Modules WHERE IdModuleParent = " + idModuleParent.ToString() + " ORDER BY IdModule;");
            foreach (Dictionary<string, string> records in modules)
            {
                Module module = new Module();
                module.IdModule = Convert.ToInt32(records["IdModule"]);
                module.IdModuleParent = Convert.ToInt32(records["IdModuleParent"]);
                module.Name = records["Name"];
                module.Depth = level;
                moduleList.Add(module);
                moduleList = GetModuleTree(moduleList, module.IdModule, module.Depth + 1);
            }
            return moduleList;
        }

        /// <summary>
        /// Propagates access to the children of a given module.
        /// </summary>
        /// <param name="key">The IdModule acting as a Dictionary Key</param>
        /// <param name="access">Boolean, determines if the access is granted or revoked to the specific module</param>
        /// <param name="modules">The current Dictionary holding the modules</param>
        private static void PropagateAccess(string key, bool access, Dictionary<string, bool> modules)
        {
            string childrenList = Common.GetBDList("IdModule", "SELECT IdModule FROM Modules WHERE IdModuleParent = " + key, false);
            if (!String.IsNullOrEmpty(childrenList))
            {
                string[] children = Common.CSVToArray(childrenList);
                foreach (string child in children)
                {
                    modules[child.ToString()] = access;
                    PropagateAccess(child.ToString(), access, modules);
                }
            }
        }

        /// <summary>
        /// Checks if the user has access to a given module. It's a wrapper to avoid constant ToString() calls.
        /// </summary>
        /// <param name="modules">A Dictionary containing the modules and the user access.</param>
        /// <param name="module">The module to check.</param>
        /// <returns>True if the user has access, false otherwise.</returns>
        public static bool Permission(Dictionary<string, bool> modules, int module)
        {
            try
            {
                return modules[module.ToString()];
            }
            catch (KeyNotFoundException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the user has access to a given module. It's a wrapper to avoid constant ToString() calls.
        /// Overloaded method, receives a string instead of an int.
        /// </summary>
        /// <param name="modules">A Dictionary containing the modules and the user access.</param>
        /// <param name="module">The module to check.</param>
        /// <returns>True if the user has access, false otherwise.</returns>
        public static bool Permission(Dictionary<string, bool> modules, string module)
        {
            return modules[module];
        }

        /// <summary>
        /// Receives a CSV list of modules, this is a lenient test, and returns true if any of the
        /// listed modules are available to the user.
        /// </summary>
        /// <param name="modules">A Dictionary containing the modules and the user access.</param>
        /// <param name="moduleCSV">The modules to check.</param>
        /// <returns>True if the user has access to ANY of the modules, false otherwise.</returns>
        public static bool PermissionOr(Dictionary<string, bool> modules, string moduleCSV)
        {
            bool retval = false;
            string[] moduleList = Common.CSVToArray(moduleCSV);
            for (int i = 0; i < moduleList.Length; i++)
            {
                retval = retval || Permission(modules, moduleList[i]);
            }
            return retval;
        }

        /// <summary>
        /// Receives a CSV list of modules, this is a strict test, and returns true only if all of the
        /// listed modules are available to the user.
        /// </summary>
        /// <param name="modules">A Dictionary containing the modules and the user access.</param>
        /// <param name="moduleCSV">The modules to check.</param>
        /// <returns>True if the user has access to ALL of the modules, false otherwise.</returns>
        public static bool PermissionAnd(Dictionary<string, bool> modules, string moduleCSV)
        {
            bool retval = true;
            string[] moduleList = Common.CSVToArray(moduleCSV);
            for (int i = 0; i < moduleList.Length; i++)
            {
                retval = retval && Permission(modules, moduleList[i]);
            }
            return retval;
        }

        /// <summary>
        /// Auxiliar Method. Lists all the modules and the access for the currently logged in user. Returns a string separated
        /// by HTML break lines (<br>).
        /// </summary>
        /// <param name="modules">A dictionary containing the modules.</param>
        /// <returns>A string with the list.</returns>
        public static string ListModules(Dictionary<string, bool> modules)
        {
            string retval = "";
            foreach (KeyValuePair<string, bool> module in modules)
            {
                retval += module.Key + " - " + module.Value.ToString() + "<br>";
            }
            return retval;
        }

        /// <summary>
        /// Returns a friendly (localized) string for the predefined modules in the database.
        /// </summary>
        /// <param name="text">The text containing the original name in the database.</param>
        /// <returns>The friendly (localized) text.</returns>
        public static string FriendlyModuleName(string text)
        {
            text = text.Replace("#USER_ADMINISTRATION#", Text.mdl_User_Administration);
            text = text.Replace("#ADD_USER#", Text.mdl_Add_User);
            text = text.Replace("#EDIT_USER#", Text.mdl_Edit_User);
            text = text.Replace("#DELETE_USER#", Text.mdl_Delete_User);
            text = text.Replace("#EDIT_PERMISSIONS#", Text.mdl_Edit_Permissions);
            text = text.Replace("#EDIT_PASSWORDS#", Text.mdl_Edit_Passwords);
            text = text.Replace("#EDIT_SELF_PASSWORD#", Text.mdl_Edit_Self_Password);
            text = text.Replace("#EDIT_SELF_INFO#", Text.mdl_Edit_Self_Info);
            text = text.Replace("#EXPORT_USERS#", Text.mdl_Export_Users);
            text = text.Replace("#IMPORT_USERS#", Text.mdl_Import_Users);
            text = text.Replace("#DATAFIELDS_ADMINISTRATION#", Text.mdl_Datafields_Administration);
            text = text.Replace("#ADD_FIELDS#", Text.mdl_Add_Fields);
            text = text.Replace("#EDIT_FIELDS#", Text.mdl_Edit_Fields);
            text = text.Replace("#DELETE_FIELDS#", Text.mdl_Delete_Fields);
            text = text.Replace("#LOG_ACCESS#", Text.mdl_Log_Access);
            text = text.Replace("#CATALOGS#", Text.mdl_Catalogs);
            text = text.Replace("#ROLE_CATALOG#", Text.mdl_Role_Catalog);
            text = text.Replace("#ROLE_CATALOG_ADD#", Text.Add);
            text = text.Replace("#ROLE_CATALOG_EDIT#", Text.Edit);
            text = text.Replace("#ROLE_CATALOG_DELETE#", Text.Delete);
            text = text.Replace("#EVALUATION#", Text.mdl_Evaluation);
            text = text.Replace("#EVAL_QUESTION_CATALOG#", Text.mdl_Eval_Question_Catalog);
            text = text.Replace("#EVAL_QUESTION_CATALOG_ADD#", Text.Add);
            text = text.Replace("#EVAL_QUESTION_CATALOG_EDIT#", Text.Edit);
            text = text.Replace("#EVAL_QUESTION_CATALOG_DELETE#", Text.Delete);
            text = text.Replace("#EVAL_THEME_CATALOG#", Text.mdl_Eval_Theme_Catalog);
            text = text.Replace("#EVAL_THEME_CATALOG_ADD#", Text.Add);
            text = text.Replace("#EVAL_THEME_CATALOG_EDIT#", Text.Edit);
            text = text.Replace("#EVAL_THEME_CATALOG_DELETE#", Text.Delete);
            text = text.Replace("#EVAL_EXAM_CATALOG#", Text.mdl_Eval_Exam_Catalog);
            text = text.Replace("#EVAL_EXAM_CATALOG_ADD#", Text.Add);
            text = text.Replace("#EVAL_EXAM_CATALOG_EDIT#", Text.Edit);
            text = text.Replace("#EVAL_EXAM_CATALOG_DELETE#", Text.Delete);
            text = text.Replace("#EVAL_SOLVE_USER#", Text.mdl_Eval_Solve_User);
            text = text.Replace("#EVAL_IMPORT_EXAMS#", Text.mdl_Eval_Import_Exams);
            text = text.Replace("#EVAL_IMPORT_RESULTS#", Text.mdl_Eval_Import_Results);
            text = text.Replace("#EVAL_REPORTS#", Text.mdl_Eval_Reports);
            text = text.Replace("#EVAL_ASSIGN#", Text.mdl_Eval_Assign);
            return text;
        }
    }

    public class Module
    {
        public int IdModule;
        public int IdModuleParent;
        public string Name;
        //Not in DB
        public int Depth;

        /// <summary>
        /// Default Constructor. No arguments received.
        /// </summary>
        public Module() { }

        /// <summary>
        /// Overload of the constructor. Receives a IdModule and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdModule of the module to retrieve information from.</param>
        public Module(int id)
        {
            List<Dictionary<string, string>> module = Common.GetRS("SELECT * FROM Modules WHERE IdModule = " + id);
            foreach (Dictionary<string, string> record in module)
            {
                IdModule = id;
                IdModuleParent = Convert.ToInt32(record["IdModuleParent"]);
                Name = record["Name"];
            }
        }
    }
}