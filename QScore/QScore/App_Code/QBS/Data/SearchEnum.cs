using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QBS.Data
{
    /// <summary>
    /// Provides friendly names for extra fields used in the search classes.
    /// </summary>
    public static class SearchEnum
    {
        //Data Groups
        public const int USERS = -1;
        public const int EXAMS = -2;

        //Users
        public const int USERNAME = -1;
        public const int LAST_NAME = -2;
        public const int MOTHER_LAST_NAME = -3;
        public const int NAME = -4;
        public const int REGISTRY_DATE = -5;
        public const int STATUS = -6;

        //Exams
        public const int EXAM_NAME = -11;
        public const int EXAM_SCORE = -12;
        public const int EXAM_STATUS = -13;
        public const int EXAM_DATE_COMPLETED = -14;
    }
}