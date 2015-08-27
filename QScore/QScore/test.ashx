<%@ WebHandler Language="C#" Class="test" %>

using System;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Aexis;

public class test : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        DateTime someDate = new DateTime(1998, 5, 7, 10, 12, 44);
        int someDatesUnix = Common.TimeStampFromDateTime(someDate);
        DateTime someDateProcessed = Common.DateTimeFromTimeStamp(someDatesUnix);
        context.Response.Write(someDatesUnix + " " + someDateProcessed.ToString() + "<br>");
        int ctime = Common.GetCurrentTimeStamp();
        DateTime cdate = Common.DateTimeFromTimeStamp(ctime);
        context.Response.Write(ctime + " " + cdate.ToString());
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}