<%@ WebHandler Language="C#" Class="fileUpload" %>

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

public class fileUpload : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        bool withError = false;
        string strout = "";
        string fileName = "";
        string fileExt = "";
        string fullPath = "";
        string dir = "./data/";
        string extAllowed = "jpg,jpeg,gif,png,bmp,svg";
        bool extensionAllowed = true;
        HttpPostedFile file = null;
                
        try
        {
            file = context.Request.Files[0];

            fileName = RemoveDiacritics(Path.GetFileName(file.FileName));
            fileExt = Path.GetExtension(file.FileName);
            if(!String.IsNullOrEmpty(extAllowed))
            {
                extensionAllowed = false;
                string[] allowedExtensions = extAllowed.Split(',');
                foreach (string allowedExtension in allowedExtensions)
                {
                    if (fileExt.Replace(".", "").ToUpper() == allowedExtension.ToUpper())
                    {
                        extensionAllowed = true;
                        break;
                    }
                }
            }
            fullPath = dir + fileName;

            if (extensionAllowed)
            {
                Stream stream = file.InputStream;
                string fullFolder = "";
                string physicalPath = "";
                fullFolder = context.Server.MapPath(dir);
                physicalPath = context.Server.MapPath(fullPath);
                byte[] fileData = new byte[file.ContentLength];
                stream.Read(fileData, 0, file.ContentLength);
                if (!Directory.Exists(fullFolder))
                {
                    Directory.CreateDirectory(fullFolder);
                }
                file.SaveAs(physicalPath);
            }
            else
            {
                withError = true;
            }
        }
        catch (Exception ex)
        {
            withError = true;
        }

        if (!withError)
        {
            strout = "\"status\": \"success\",";
        }
        else
        {
            strout = "\"status\": \"error\",";
        }

        strout += "\"uploadedFile\": \"" + fileName + "\",";
        strout += "\"extension\": \"" + fileExt + "\"";
        for (int i = 0; i < context.Request.QueryString.Count; i++)
        {
            strout += ",\"" + context.Request.QueryString.Keys.Get(i) + "\": \"" + context.Request.QueryString[i].Replace("\t", " ") + "\"";
            if (i != context.Request.QueryString.Count - 1) strout += ",";
        }
        for (int i = 0; i < context.Request.Form.Count; i++)
        {
            strout += ",\"" + context.Request.Form.Keys.Get(i) + "\": \"" + context.Request.Form[i].Replace("\t", " ") + "\"";
            if (i != context.Request.Form.Count - 1) strout += ",";
        }
        context.Response.Write("[{" + strout + "}]");
    }

    public string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}