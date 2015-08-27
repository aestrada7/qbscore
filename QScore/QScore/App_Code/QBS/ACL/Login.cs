using System;
using System.Configuration;
using System.Web;
using Aexis;
using QScore.lang;

namespace QBS.ACL
{
    public static class Login
    {
        /// <summary>
        /// Tries to authenticate a user in the system.
        /// </summary>
        /// <param name="username">The given username.</param>
        /// <param name="password">The given password.</param>
        /// <param name="noLog">Boolean, if true it won't log access. Useful for checking passwords.</param>
        /// <returns>An int of the user (IdUser) if it exists and the username/password combination is correct. 
        /// Otherwise returns 0.</returns>
        public static int Authenticate(string username, string password, bool noLog)
        {
            string sql = "SELECT IdUser FROM Users WHERE Status = 1 AND Username = '" + Common.SQSF(username) + "' ";
            int idUser = 0;
            if (Config.UseCryptoPassword()) password = Common.EncryptSHA512(password);
            if (Config.CaseSensitiveUsername()) sql += "COLLATE SQL_Latin1_General_CP1_CS_AS ";
            sql += "AND Password = '" + Common.SQSF(password) + "'";
            if (Config.CaseSensitivePassword()) sql += " COLLATE SQL_Latin1_General_CP1_CS_AS";
            idUser = Common.GetBDNum("IdUser", sql);
            if (!noLog)
            {
                string userIP = String.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (idUser != 0)
                {
                    Log.Add(idUser, LogKind.SESSION, 0, 0, "#SESSION_START#" + " IP: [" + userIP + "], Agent: [" + HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToString() + "]");
                }
                else
                {
                    Log.Add(0, LogKind.SESSION, 0, 0, "#LOGIN_FAILED#" + " Username: [" + username + "], IP: [" + userIP + "]");
                }
            }
            return idUser;
        }
    }
}