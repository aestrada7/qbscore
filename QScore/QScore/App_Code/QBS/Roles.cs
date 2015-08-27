using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aexis;
using Aexis.Web;
using QScore.lang;
using QBS.ACL;

namespace QBS
{
    /// <summary>
    /// The Roles Class defines several static functions pertaining to the roles.
    /// </summary>
    public static class Roles
    {
        public const int DEFAULT_ROLE_ADMINISTRATOR = 1;
        public const int DEFAULT_ROLE_USER = 2;

        /// <summary>
        /// Retrieves and returns the role list.
        /// </summary>
        /// <returns>An HTML including the role list.</returns>
        public static string GetRoleList(Dictionary<string, bool> modules)
        {
            string retval = "";
            string className = "";
            int total = 0;

            retval = "<table width='100%'>";

            //Table Header
            retval += "<tr><th>" + Text.Name + "</th></tr>";

            string[] idRoleList = Common.CSVToArray(Common.GetBDList("IdRole", "SELECT IdRole, Role FROM Roles ORDER BY Role", false));
            foreach (string idRole in idRoleList)
            {
                try
                {
                    Role role = new Role(Convert.ToInt32(idRole));
                    className = Common.SwitchClass(className);
                    retval += "<tr class='" + className + "' onClick='showRole(" + idRole + ");'>";
                    retval += "<td>" + role.RoleName + "</td>";
                    retval += "</tr>";
                    total++;
                }
                catch (Exception ex) { }
            }
            retval += "</table>";

            //Table Footer
            retval += "<div align='center' class='pagination'>";
            retval += "<div align='left' style='width: 100%; display: inline-block;'>" + Common.StrLang(Text.ShowingXofY, total.ToString() + "," + total.ToString()) + " " + Text.Role_s + "</div>";
            retval += "</div>";
            
            return retval;
        }

        /// <summary>
        /// Retrieves the HTML of a given role.
        /// </summary>
        /// <param name="idRole">The given idRole.</param>
        /// <returns>An HTML including the formatted role list and the currently selected permissions for the role.</returns>
        public static string GetRoleInfo(int idRole)
        {
            string retval = "";
            string className = "";
            Dictionary<string, bool> roleModules = Modules.GetRoleModules(idRole);

            //Table Header
            retval = "<table align='center' width='70%'>";
            retval += "<tr><th width='70%'>" + Text.Module + "<th>" + Text.GrantPermission + "</th><th>" + Text.RevokePermission + "</th></tr>";

            List<Module> moduleList = new List<Module>();
            moduleList = Modules.GetModuleTree(moduleList, 0, 0);

            foreach (Module tempModule in moduleList)
            {
                string elementName = tempModule.IdModule.ToString();
                bool isGranted = Common.GetBDNum("GrantPermission", "SELECT GrantPermission FROM RoleModules WHERE IdRole = " + idRole.ToString() + " AND IdModule = " + tempModule.IdModule.ToString()) == 1;
                bool isRevoked = Common.GetBDNum("RevokePermission", "SELECT RevokePermission FROM RoleModules WHERE IdRole = " + idRole.ToString() + " AND IdModule = " + tempModule.IdModule.ToString()) == 1;
                className = Common.SwitchClass(className);
                retval += "<tr class='" + className + "'>";
                retval += "<td>" + DepthSpacing(tempModule.Depth) + Modules.FriendlyModuleName(tempModule.Name) + "</td>";
                retval += "<td align='center'>" + DrawInput.InputCheckbox("grt_" + elementName, "1", isGranted, "", "", "", "") + "</td>";
                retval += "<td align='center'>" + DrawInput.InputCheckbox("rvk_" + elementName, "1", isRevoked, "", "", "", "") + "</td>";
                retval += "</tr>";
                retval += "<script language='JavaScript'>roles[roles.length] = new Role(" + tempModule.IdModule.ToString() + ", " + tempModule.IdModuleParent.ToString() + ");</script>";
            }
            
            //Table Footer
            retval += "</table>";

            return retval;
        }

        /// <summary>
        /// Auxiliar function to add spacing to the children of a given role.
        /// </summary>
        /// <param name="depth">The current element's depth level.</param>
        /// <returns>A string with HTML spacing.</returns>
        private static string DepthSpacing(int depth)
        {
            string retval = "";
            for (int i = 0; i < depth; i++)
            {
                retval += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            return retval;
        }
    }

    /// <summary>
    /// The Role Class defines the structure of a single role, plus any actions it can have.
    /// </summary>
    public class Role
    {
        //Fields on Table
        public int IdRole = 0;
        public string RoleName = "";
        //On auxiliar table
        public Dictionary<int, RoleModule> RoleModules = new Dictionary<int, RoleModule>();

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Role() { }

        /// <summary>
        /// Overload of the constructor. Receives an IdRole and populates the instance with its values.
        /// </summary>
        /// <param name="id">The IdRole of the role to retrieve information from.</param>
        public Role(int id)
        {
            List<Dictionary<string, string>> role = Common.GetRS("SELECT * FROM Roles WHERE IdRole = " + id);
            foreach (Dictionary<string, string> record in role)
            {
                IdRole = id;
                RoleName = record["Role"].Replace("#ADMINISTRATOR#", Text.role_Administrator).Replace("#USER#", Text.role_User);
            }
            PopulateRoleModules();
        }

        /// <summary>
        /// Populates the RoleModules Dictionary.
        /// </summary>
        private void PopulateRoleModules()
        {
            RoleModules.Clear();
            List<Dictionary<string, string>> modules = Common.GetRS("SELECT * FROM RoleModules WHERE IdRole = " + IdRole);
            foreach (Dictionary<string, string> record in modules)
            {
                RoleModule roleModule = new RoleModule();
                roleModule.IdRole = IdRole;
                roleModule.IdModule = Convert.ToInt32(record["IdModule"]);
                roleModule.GrantPermission = Convert.ToInt32(record["GrantPermission"]);
                roleModule.RevokePermission = Convert.ToInt32(record["RevokePermission"]);
                RoleModules.Add(roleModule.IdModule, roleModule);
            }
        }

        /// <summary>
        /// Updates the role modules for the current IdRole.
        /// </summary>
        /// <param name="request">An HttpRequestBase including the grt_&lt;IdModule&gt; and &lt;rvk_IdModule&gt; keys.</param>
        public void UpdateRoleModules(HttpRequestBase request)
        {
            DeleteModules();
            List<Dictionary<string, string>> modules = Common.GetRS("SELECT * FROM Modules");
            foreach (Dictionary<string, string> record in modules)
            {
                RoleModule roleModule = new RoleModule();
                roleModule.IdRole = IdRole;
                roleModule.IdModule = Convert.ToInt32(record["IdModule"]);
                roleModule.GrantPermission = Convert.ToInt32(request["grt_" + record["IdModule"]]);
                roleModule.RevokePermission = Convert.ToInt32(request["rvk_" + record["IdModule"]]);
                roleModule.Insert();
            }
            PopulateRoleModules();
        }

        /// <summary>
        /// Checks if the role is valid.
        /// </summary>
        /// <returns>NO_ERROR if the roleName is valid, the error number otherwise.</returns>
        private int ValidRole()
        {
            int retval = ErrorCode.NO_ERROR;
            if (Common.GetBDNum("NameExists", "SELECT COUNT(IdRole) AS NameExists FROM Roles WHERE Role='" + Common.SQSF(RoleName) + "' AND IdRole <> " + IdRole.ToString()) > 0)
            {
                retval = ErrorCode.ALREADY_EXISTS;
            }
            else if (String.IsNullOrWhiteSpace(RoleName))
            {
                retval = ErrorCode.MISSING_FIELDS;
            }
            return retval;
        }

        /// <summary>
        /// Saves a new role in the database.
        /// </summary>
        /// <returns>NO_ERROR if created successfully, the error number otherwise.</returns>
        public int Create()
        {
            int retval = ValidRole();
            if (retval == ErrorCode.NO_ERROR)
            {
                string sql = "INSERT INTO Roles(Role) VALUES('" + Common.SQSF(RoleName) + "');";
                Common.BDExecute(sql);
                IdRole = Common.GetBDNum("lastId", "SELECT MAX(IdRole) AS lastId FROM Roles");
                Log.Add(SessionHandler.Id, LogKind.CREATE, Modules.ROLE_CATALOG, IdRole, Common.SQSF(RoleName));
            }
            return retval;
        }

        /// <summary>
        /// Deletes the current role, plus any associated table data.
        /// </summary>
        /// <returns>True if deleted successfully, false otherwise.</returns>
        public bool Delete()
        {
            bool retval = false;
            string sql = "DELETE FROM Roles WHERE IdRole = " + IdRole.ToString();
            Common.BDExecute(sql);
            sql = "DELETE FROM UserRoles WHERE IdRole = " + IdRole.ToString();
            Common.BDExecute(sql);
            DeleteModules();
            Log.Add(SessionHandler.Id, LogKind.DELETE, Modules.ROLE_CATALOG, IdRole, Common.SQSF(RoleName));
            retval = true;
            return retval;
        }

        /// <summary>
        /// Updates the current role data.
        /// </summary>
        /// <returns>NO_ERROR if updated successfully, the error number otherwise.</returns>
        public int Update()
        {
            int retval = ValidRole();
            if (retval == ErrorCode.NO_ERROR)
            {
                string sql = "UPDATE Roles SET Role = '" + Common.SQSF(RoleName) + "' WHERE IdRole = " + IdRole.ToString();
                Common.BDExecute(sql);
                Log.Add(SessionHandler.Id, LogKind.UPDATE, Modules.ROLE_CATALOG, IdRole, Common.SQSF(RoleName));
            }
            return retval;
        }

        /// <summary>
        /// Deletes every role module record associated with this role.
        /// </summary>
        public void DeleteModules()
        {
            string sql = "DELETE FROM RoleModules WHERE IdRole = " + IdRole.ToString();
            Common.BDExecute(sql);
        }
    }

    /// <summary>
    /// The RoleModule Class mimics it's database counterpart.
    /// </summary>
    public class RoleModule
    {
        public int IdRole;
        public int IdModule;
        public int GrantPermission;
        public int RevokePermission;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public RoleModule() { }

        /// <summary>
        /// Inserts a new record.
        /// </summary>
        public void Insert()
        {
            string sql = "INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(" + IdRole.ToString() + ", " + IdModule.ToString() + ", " + GrantPermission.ToString() + ", " + RevokePermission.ToString() + ")";
            Common.BDExecute(sql);
        }
    }
}