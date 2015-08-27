using System;
using System.Collections.Generic;
using System.Web;
using Aexis;
using QScore.lang;

namespace QBS.ACL
{
    /// <summary>
    /// The Menu Class has functions to define and show the menus for the current user.
    /// </summary>
    public static class Menu
    {
        #region Constants
        public const string MAIN_PAGE = "./main.cshtml";
        public const string USER_ADMINISTRATION = "./users.cshtml";
        public const string USER_INFO = "./userInfo.cshtml";
        public const string ADD_USER = "./newUser.cshtml";
        public const string DATA_FIELD_ADMINISTRATION = "./dataFields.cshtml";
        public const string DATA_FIELD_EDIT = "./dataFieldInfo.cshtml";
        public const string EXPORT_USERS = "./userExport.cshtml";
        public const string IMPORT_USERS = "./userImport.cshtml";
        public const string LOGOUT = "./logout.cshtml";
        public const string LOGIN = "./login.cshtml";
        public const string SESSION_LOST = "sessionLost.cshtml";
        public const string ROLE_CATALOG = "./roles.cshtml";
        public const string ROLE_EDIT = "./roleInfo.cshtml";
        public const string SYSTEM_CONFIGURATION = "./syscon.cshtml";
        public const string SYSTEM_LOG = "./syslog.cshtml";
        public const string EDIT_SELF_INFO = "./selfInfo.cshtml";
        public const string EDIT_SELF_PASSWORD = "./selfPwd.cshtml";
        public const string EVAL_EXAM_CATALOG = "./exams.cshtml";
        public const string EVAL_EXAM_CATALOG_EDIT = "./examInfo.cshtml";
        public const string EVAL_THEME_CATALOG = "./examThemes.cshtml";
        public const string EVAL_THEME_CATALOG_EDIT = "./examThemeInfo.cshtml";
        public const string EVAL_QUESTION_CATALOG = "./examQuestions.cshtml";
        public const string EVAL_QUESTION_CATALOG_EDIT = "./examQuestionInfo.cshtml";
        public const string EVAL_RESULTS = "./examRes.cshtml";
        public const string EVAL_RESULTS_DETAIL = "./examResInfo.cshtml";
        public const string EVAL_APPLICATION = "./examApplication.cshtml";
        public const string EVAL_EXAM = "./exam.cshtml";
        public const string EVAL_ASSIGN = "./examAssign.cshtml";
        public const string EVAL_IMPORT_EXAMS = "./examImport.cshtml";
        public const string EVAL_IMPORT_RESULTS = "./examImportRes.cshtml";
        public const string HELP_ROLE_CATALOG = "./help/roles.pdf";
        public const string HELP_DATA_FIELD_ADMINISTRATION = "./help/dataFields.pdf";
        public const string HELP_USER_ADMINISTRATION = "./help/users.pdf";
        public const string HELP_EVAL = "./help/eval.pdf";
        #endregion

        /// <summary>
        /// Builds the menu for the current user.
        /// </summary>
        /// <param name="modules">The module Dictionary for the current user.</param>
        /// <returns>A string containing the HTML markup.</returns>
        public static string GetDropDown(Dictionary<string, bool> modules, int userId)
        {
            string retval = "<div id='dropdown' class='ddsmoothmenu'><ul>";

            //Main Page
            string mainPage = "<a href='" + MAIN_PAGE + "'>" + Text.MainPage + "</a>";
            retval = Common.StrAdd(retval, "<li>", Common.StrAdd(mainPage, "", "</li>"));

            //Help Stuff
            string helpItems = "";

            //Configuration
            string configuration = "";
            if (userId == -1)
            {
                configuration += "<li><a href='" + SYSTEM_CONFIGURATION + "'>" + Text.SystemConfiguration + "-" + Text.Options + "</a></li>";
            }
            if (Modules.PermissionOr(modules, Modules.roleAdministratorModuleList))
            {
                configuration += "<li><a href='" + ROLE_CATALOG + "'>" + Text.mdl_Role_Catalog + "</a></li>";
                helpItems += "<li><a href='" + HELP_ROLE_CATALOG + "' target='_blank'>" + Text.mdl_Role_Catalog + "</a></li>";
            }
            if (Modules.PermissionOr(modules, Modules.fieldsAdministratorModuleList))
            {
                configuration += "<li><a href='" + DATA_FIELD_ADMINISTRATION + "'>" + Text.mdl_Datafields_Administration + "</a></li>";
                helpItems += "<li><a href='" + HELP_DATA_FIELD_ADMINISTRATION + "' target='_blank'>" + Text.mdl_Datafields_Administration + "</a></li>";
            }
            if (Modules.Permission(modules, Modules.LOG_ACCESS))
            {
                configuration += "<li><a href='" + SYSTEM_LOG + "'>" + Text.mdl_Log_Access + "</a></li>";
            }
            if (configuration != "")
            {
                configuration = "<a href='#'>" + Text.SystemConfiguration + "</a><ul>" + configuration + "</ul>";
            }
            retval = Common.StrAdd(retval, "<li>", Common.StrAdd(configuration, "", "</li>"));

            //User Administration
            string userAdministration = "";
            if (Modules.PermissionOr(modules, Modules.userAdministratorModuleList))
            {
                userAdministration += "<li><a href='" + USER_ADMINISTRATION + "'>" + Text.mdl_User_Administration + "</a></li>";
                helpItems += "<li><a href='" + HELP_USER_ADMINISTRATION + "' target='_blank'>" + Text.mdl_User_Administration + "</a></li>";
            }
            if (Modules.Permission(modules, Modules.ADD_USER))
            {
                userAdministration += "<li><a href='" + ADD_USER + "'>" + Text.mdl_Add_User + "</a></li>";
            }
            if (Modules.Permission(modules, Modules.EXPORT_USERS))
            {
                userAdministration += "<li><a href='" + EXPORT_USERS + "'>" + Text.mdl_Export_Users + "</a></li>";
            }
            if (Modules.Permission(modules, Modules.IMPORT_USERS))
            {
                userAdministration += "<li><a href='" + IMPORT_USERS + "'>" + Text.mdl_Import_Users + "</a></li>";
            }
            if (userAdministration != "")
            {
                userAdministration = "<a href='#'>" + Text.mdl_User_Administration + "</a><ul>" + userAdministration + "</ul>";
            }
            retval = Common.StrAdd(retval, "<li>", Common.StrAdd(userAdministration, "", "</li>"));

            //Evaluation
            string evaluation = "";
            evaluation += "<li><a href='" + EVAL_APPLICATION + "'>" + Text.EvalApplication + "</a></li>";
            if (Modules.Permission(modules, Modules.EVAL_REPORTS))
            {
                evaluation += "<li><a href='" + EVAL_RESULTS + "'>" + Text.mdl_Eval_Reports + "</a></li>";
            }
            if (Modules.Permission(modules, Modules.EVAL_ASSIGN))
            {
                evaluation += "<li><a href='" + EVAL_ASSIGN + "'>" + Text.mdl_Eval_Assign + "</a></li>";
            }
            if (Modules.PermissionOr(modules, Modules.examAdministratorModuleList))
            {
                evaluation += "<li><a href='" + EVAL_EXAM_CATALOG + "'>" + Text.mdl_Eval_Exam_Catalog + "</a></li>";
                helpItems += "<li><a href='" + HELP_EVAL + "' target='_blank'>" + Text.mdl_Evaluation + "</a></li>";
            }
            if (Modules.PermissionOr(modules, Modules.themeAdministratorModuleList))
            {
                evaluation += "<li><a href='" + EVAL_THEME_CATALOG + "'>" + Text.mdl_Eval_Theme_Catalog + "</a></li>";
            }
            if (Modules.PermissionOr(modules, Modules.questionAdministratorModuleList))
            {
                evaluation += "<li><a href='" + EVAL_QUESTION_CATALOG + "'>" + Text.mdl_Eval_Question_Catalog + "</a></li>";
            }
            if (Modules.Permission(modules, Modules.EVAL_IMPORT_EXAMS))
            {
                evaluation += "<li><a href='" + EVAL_IMPORT_EXAMS + "'>" + Text.mdl_Eval_Import_Exams + "</a></li>";
            }
            if (Modules.Permission(modules, Modules.EVAL_IMPORT_RESULTS))
            {
                evaluation += "<li><a href='" + EVAL_IMPORT_RESULTS + "'>" + Text.mdl_Eval_Import_Results + "</a></li>";
            }
            if (evaluation != "")
            {
                evaluation = "<a href='#'>" + Text.mdl_Evaluation + "</a><ul>" + evaluation + "</ul>";
            }
            retval = Common.StrAdd(retval, "<li>", Common.StrAdd(evaluation, "", "</li>"));

            //Help
            if (helpItems != "")
            {
                helpItems = "<a href='#'>" + Text.Help + "</a><ul>" + helpItems + "</ul>";
            }
            retval = Common.StrAdd(retval, "<li>", Common.StrAdd(helpItems, "", "</li>"));

            //My Account
            string myAccount = "<a href='#'>" + Text.MyAccount + "</a><ul>";
            myAccount += "<li><a href='" + LOGOUT + "'>" + Text.Logout + "</a></li>";
            if (Modules.Permission(modules, Modules.EDIT_SELF_INFO))
            {
                myAccount += "<li><a href='" + EDIT_SELF_INFO + "'>" + Text.mdl_Edit_Self_Info + "</a></li>";
            }
            if (Modules.Permission(modules, Modules.EDIT_SELF_PASSWORD))
            {
                myAccount += "<li><a href='" + EDIT_SELF_PASSWORD + "'>" + Text.mdl_Edit_Self_Password + "</a></li>";
            }
            myAccount += "</ul>";
            retval = Common.StrAdd(retval, "<li>", Common.StrAdd(myAccount, "", "</li>"));

            retval += "<br style='clear: left' /></ul></div>";
            return retval;
        }
        
        /// <summary>
        /// Builds the tiles available for the current user.
        /// </summary>
        /// <param name="modules">The module Dictionary for the current user.</param>
        /// <returns>A string containing the HTML markup.</returns>       
        public static string GetTiles(Dictionary<string, bool> modules)
        {
            string retval = "";

            //My Account
            retval += "<div id='tile_logout' class='tile' style='border-color: #7d5100;'><a href='" + LOGOUT + "' alt='" + Text.Logout + "'><img src='images/tiles/Logout.png' border='0' /></a>&nbsp;</div>";
            retval += "<div id='tile_self_info' class='tile' style='border-color: #073119;'><a href='" + EDIT_SELF_INFO + "' alt='" + Text.MyAccount + "'><img src='images/tiles/MyAccount.png' border='0' /></a>&nbsp;</div>";
            
            //User Administration
            if (Modules.PermissionOr(modules, Modules.userAdministratorModuleList))
            {
                retval += "<div id='tile_user_administration' class='tile' style='border-color: #025b72;'><a href='" + USER_ADMINISTRATION + "' alt='" + Text.mdl_User_Administration + "'><img src='images/tiles/UserAdministration.png' border='0' /></a>&nbsp;</div>";
            }

            //Exam Application
            retval += "<div id='tile_eval_app' class='tile' style='border-color: #025b72;'><a href='" + EVAL_APPLICATION + "' alt='" + Text.EvalApplication + "'><img src='images/tiles/ExamApplication.png' border='0' /></a>&nbsp;</div>";

            //Exam Reports
            if (Modules.Permission(modules, Modules.EVAL_REPORTS))
            {
                retval += "<div id='tile_eval_res' class='tile' style='border-color: #531e00;'><a href='" + EVAL_RESULTS + "' alt='" + Text.mdl_Eval_Reports + "'><img src='images/tiles/ExamReports.png' border='0' /></a>&nbsp;</div>";
            }

            return retval;
        }

    }
}