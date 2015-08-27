<%@ WebHandler Language="C#" Class="PieChart" %>

using System;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

public class PieChart : IHttpHandler
{

    /* --- Parameters --- */
    public string values;
    public string colors;
    public string titles;
    public string _units;
    public string px;

    /* --- Variables --- */
    private int gridWidth;
    private int gridHeight;
    private int gridY;
    private int gridX;
    private string units;
    private double radius = 120;
    private double total;
    private string[] valueArray;
    private string[] colorArray;
    private string[] titleArray;
    private ArrayList dataArray = new ArrayList();

    public int width = 400;
    public int height = 400;
    private HttpContext _context;
    private Color _DarkBlue = Color.FromArgb(255, 26, 59, 105);
    private Color _LightBlue = Color.FromArgb(255, 238, 242, 250); //EEF2FA
    private Color _DarkGrey = Color.FromArgb(255, 60, 60, 60);
    private Color _AlphaRed = Color.FromArgb(15, 204, 00, 00); //CC0000
    private Color _Red = Color.FromArgb(255, 204, 00, 00); //CC0000
    private Color _AlphaBlue = Color.FromArgb(15, 0, 119, 204); //0077CC
    private Color _Blue = Color.FromArgb(255, 0, 119, 204); //0077CC
    private Color _Grey = Color.FromArgb(255, 155, 159, 166); //9B9FA6
    private Pen bluePen;
    private Pen greyPen;
    private Pen redPen;
    private Font Calibri = new System.Drawing.Font("Calibri", 14);
    private Font CalibriSmall = new System.Drawing.Font("Calibri", 10);
    private Font CalibriVerySmall = new System.Drawing.Font("Calibri", 8);

    public void ProcessRequest(HttpContext context)
    {
        _context = context;
        context.Response.Clear();

        values = context.Request.QueryString["values"];
        titles = context.Request.QueryString["titles"];
        colors = context.Request.QueryString["colors"];
        _units = context.Request.QueryString["units"];
        px = context.Request.QueryString["px"];

        /* --- Test Values --- */
        values = "43.1,77.2,19.12";
        titles = "LOL,A,B";
        colors = "000000,CC0000,0077CC";
        _units = "points";
        values = "50,10,10,10,9,1";
        titles = "A,B,C,D,E,F";
        colors = "CC0000,0077CC,FFCC00,AAAAAA,00FF00,FFFFFF";
        _units = "points";
    /* --- Test Values --- */

        int _px = Convert.ToInt32(px);
        if (_px != 0)
        {
            width = _px;
            height = _px;
            radius = (_px * 120) / 400;
        }
        Bitmap bitmap = new Bitmap(width, height);
        Graphics graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        /* --- LineStyle --- */
        bluePen = new Pen(_Blue, 2);
        redPen = new Pen(_Red, 2);
        redPen.DashCap = DashCap.Round;
        redPen.DashPattern = new float[] { 4, 2, 1, 3 };
        greyPen = new Pen(_Grey, 1);
        graphics.FillRectangle(Brushes.White, 0, 0, width, height);

        drawBackground(graphics, width, height);
        startUp(graphics);

        /* --- End --- */
        CalibriVerySmall.Dispose();
        CalibriSmall.Dispose();
        Calibri.Dispose();
        graphics.Dispose();
        bitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        bitmap.Dispose();
        context.Response.AddHeader("Content-Type", "image/jpeg");
        context.Response.End();
    }

    private void startUp(Graphics gObj)
    {
        if ((values == null) || (values == "")) values = "0,0";
        if ((colors == null) || (colors == "")) colors = "000000,FFFFFF";
        if ((titles == null) || (titles == "")) titles = ",";
        valueArray = values.Split(',');
        colorArray = colors.Split(',');
        titleArray = titles.Split(',');
        if (valueArray.Length != colorArray.Length)
        {
            throw new Exception("equis");
        }
        units = _units;
        calcData();
        draw(gObj);
    }

    private void drawBackground(Graphics gObj, int w, int h)
    {
        Brush brGradient = new LinearGradientBrush(new Rectangle(10, 10, w - 20, h - 20), Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 230, 230, 230), 90, false);
        Brush brDarkBlue = new SolidBrush(_DarkBlue);
        Brush brLightBlue = new SolidBrush(_LightBlue);
        gridWidth = width;
        gridHeight = height;
        gridY = 40;
        gridX = 90;
        gObj.DrawRectangle(greyPen, 7, 7, w - 14, h - 14);
        gObj.FillRectangle(brGradient, 10, 10, w - 20, h - 20);
        brLightBlue.Dispose();
        brDarkBlue.Dispose();
        brGradient.Dispose();
    }

    private void calcData()
    {
        int i;
        for (i = 0; i <= valueArray.Length - 1; i++)
        {
            total += Convert.ToDouble(valueArray[i]);
        }
        for (i = 0; i <= valueArray.Length - 1; i++)
        {
            string tempTitle = "";
            double perc = Convert.ToDouble(valueArray[i]) / total;
            try
            {
                tempTitle = titleArray[i];
            }
            catch (IndexOutOfRangeException) { }
            FrsObject currObj = new FrsObject();
            currObj.value = valueArray[i];
            currObj.percentage = perc;
            currObj.color = colorArray[i];
            currObj.title = tempTitle;
            dataArray.Add(currObj);
        }
    }

    private void draw(Graphics gObj)
    {
        int i;
        double k;
        double currRadians = 1;
        double xStart = gridWidth / 2;
        double yStart = gridHeight / 2;
        Color currColor;
        for (i = dataArray.Count - 1; i >= 0; i--)
        {
            FrsObject tmpObj = (FrsObject)dataArray[i];
            currColor = hexStringToColor(tmpObj.color, 40);
            Pen cPen = new Pen(currColor, (float)1.8); //1.8 para mostrar degradado (y el k += .001)
            Brush brDarkGrey = new SolidBrush(_DarkGrey);
            bool notDrawn = true;

            for (k = 0; k <= (tmpObj.percentage * 2); k += .001)
            {
                gObj.DrawLine(cPen, (float)(xStart), (float)(yStart), (float)(xStart + Math.Sin((currRadians + k) * Math.PI) * radius), (float)(yStart + Math.Cos((currRadians + k) * Math.PI) * radius));
                if ((k >= tmpObj.percentage) && (notDrawn))
                {
                    gObj.DrawString(tmpObj.title, Calibri, brDarkGrey, (float)(xStart - 10 + Math.Sin((currRadians + k) * Math.PI) * (radius + 10)), (float)(yStart - 10 + Math.Cos((currRadians + k) * Math.PI) * (radius + 10)));
                    notDrawn = false;
                }
            }
            currRadians += (tmpObj.percentage * 2);

            brDarkGrey.Dispose();
            cPen.Dispose();
        }
    }

    public static Color hexStringToColor(string hc, int alpha)
    {
        if (hc.Length != 6)
        {
            return Color.Empty;
        }
        string r = hc.Substring(0, 2);
        string g = hc.Substring(2, 2);
        string b = hc.Substring(4, 2);
        Color color = Color.Empty;
        try
        {
            int ri = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
            int gi = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
            int bi = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
            color = Color.FromArgb(alpha, ri, gi, bi);
        }
        catch
        {
            return Color.Empty;
        }
        return color;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}