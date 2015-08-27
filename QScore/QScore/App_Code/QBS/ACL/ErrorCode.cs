using System;
using System.Configuration;
using System.Web;
using Aexis;

namespace QBS.ACL
{
    public static class ErrorCode
    {
        public const int NO_ERROR = 0;
        public const int ALREADY_EXISTS = 1;
        public const int MISSING_FIELDS = 2;
        public const int INVALID_FIELDS = 3;
        public const int CANT_DELETE = 4;
        public const int CANT_MODIFY = 5;
    }
}