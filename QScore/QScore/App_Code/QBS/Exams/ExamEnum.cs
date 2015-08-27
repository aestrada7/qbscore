using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QScore.lang;

namespace QBS.Exams
{
    /// <summary>
    /// The ExamQuestionStatus Enumeration is a helper class.
    /// </summary>
    public static class ExamQuestionStatus
    {
        public const int INACTIVE = 0;
        public const int ACTIVE = 1;
    }

    /// <summary>
    /// The ExamStatus Enumeration is a helper class.
    /// </summary>
    public static class ExamStatus
    {
        public const int INACTIVE = 0;
        public const int ACTIVE = 1;
    }

    /// <summary>
    /// The UserExamStatus Enumeration is a helper class.
    /// </summary>
    public static class UserExamStatus
    {
        public const int PENDING = 0;
        public const int INCOMPLETE = 1;
        public const int PASSED = 2;
        public const int FAILED = 3;
        public const int COMPLETE = 4;

        /// <summary>
        /// Returns a friendly text for the current status.
        /// </summary>
        /// <param name="status">The int with the status.</param>
        /// <returns>The friendly text.</returns>
        public static string FriendlyText(int status)
        {
            string retval = "";
            if (status == PENDING)
            {
                retval = Text.Exam_Pending;
            }
            else if (status == INCOMPLETE)
            {
                retval = Text.Exam_Incomplete;
            }
            else if (status == PASSED)
            {
                retval = Text.Exam_Passed;
            }
            else if (status == FAILED)
            {
                retval = Text.Exam_Failed;
            }
            else if (status == COMPLETE)
            {
                retval = Text.Exam_Complete;
            }
            return retval;
        }
    }
}