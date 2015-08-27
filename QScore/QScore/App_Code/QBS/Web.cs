using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace QBS
{
    /// <summary>
    /// Common methods needed for the web.
    /// </summary>
    public static class Web
    {
        /// <summary>
        /// Wraps the code to build the WYSIWYG editor.
        /// </summary>
        /// <returns>An HTML String</returns>
        public static string MakeWYSIWYG()
        {
            string retval = "";
            retval += "<script type='text/javascript'>";
            retval += WYSIWYGBase();
            retval += "</script>";
            return retval;
        }

        /// <summary>
        /// Wraps the code as a JavaScript function.
        /// </summary>
        /// <returns>A String with the JavaScript code.</returns>
        public static string WYSIWYGAsFunction()
        {
            string retval = "";
            retval += "function makeWYSIWYG() {";
            retval += WYSIWYGBase();
            retval += "}";
            return retval;
        }

        /// <summary>
        /// Base code to build the WYSIWYG editor.
        /// </summary>
        /// <returns>A String with the JavaScript code.</returns>
        private static string WYSIWYGBase()
        {
            string retval = "";
            retval += "$('textarea').sceditor({";
            retval += "plugins: 'bbcode',";
            retval += "style: 'css/jquery.sceditor.default.css',";
            retval += "toolbar: 'bold,italic,underline,strike,subscript,superscript|left,center,right,justify,code|font,size,color|bulletlist,orderedlist,table,horizontalrule|image,email,link|maximize',";
            retval += "height: '200',";
            retval += "fonts: 'Arial,Arial Black,Calibri,Comic Sans MS,Courier New,Georgia,Impact,Sans-serif,Segoe UI,Serif,Times New Roman,Trebuchet MS,Verdana',";
            retval += "locale: '" + CultureInfo.CurrentCulture + "',";
            retval += "colors: '#000000,#333333,#666666,#999999,#CCCCCC|#330000,#660000,#990000,#CC0000,#FF0000|#333300,#666600,#999900,#CCCC00,#FFFF00|#003300,#006600,#009900,#00CC00,#00FF00|#003333,#006666,#009999,#00CCCC,#00FFFF|#000033,#000066,#000099,#0000CC,#0000FF|#331100,#663300,#996600,#CC9900,#FFCC00|#003311,#006633,#009966,#00CC99,#00FFCC|#110033,#330066,#660099,#9900CC,#CC00FF',";
            retval += "id: 'editorInstance'";
            retval += "});";
            return retval;
        }
    }
}