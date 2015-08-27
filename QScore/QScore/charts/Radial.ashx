<%@ WebHandler Language="C#" Class="Radial" %>

using System;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aexis;

public class Radial : IHttpHandler
{

    /* --- Parameters --- */
    public string maxValue;
    public string minValue;
    public string divisionEach;
    public string showScale;
    public string values;
    public string colors;
    public string labels;
    public string showFill;
    public string w;
    public string h;

    /* --- Variables --- */
    private double _maxValue;
    private double _minValue;
    private double _divisionEach;
    private bool _showScale;
    private bool _showFill;
    private string[] _colors;
    private string[] valuePerPoint;
    private double valuesToDisplay = 0;
    private double gridDivisions = 0;
    private double gridCenter = 0;
    private double gridDiameter = 0;
    private double gridCircumference = 0;
    private double lineRotation = 0;
    private string[] drawPoints;
    private string[] labelPerPoint;

    public int width = 550;
    public int height = 400;
    private HttpContext _context;
    private Color _DarkBlue = Color.FromArgb(255, 26, 59, 105);
    private Color _LightBlue = Color.FromArgb(255, 238, 242, 250);
    private Color _DarkGrey = Color.FromArgb(255, 60, 60, 60);
    private Color _AlphaRed = Color.FromArgb(40, 204, 00, 00); //CC0000
    private Color _Red = Color.FromArgb(255, 204, 00, 00); //CC0000
    private Color _AlphaBlue = Color.FromArgb(40, 0, 119, 204); //0077CC
    private Color _Blue = Color.FromArgb(255, 0, 119, 204); //0077CC
    private Color _Grey = Color.FromArgb(255, 155, 159, 166); //9B9FA6
    private Pen bluePen;
    private Pen greyPen;
    private Pen redPen;
    private Pen dashPen;
    private Font Calibri = new System.Drawing.Font("Segoe UI", 12);

    private int gridWidth;
    private int gridHeight;
    private int gridY;
    private int gridX;

    private int maxR = 0;

    public void ProcessRequest(HttpContext context)
    {
        _context = context;
        context.Response.Clear();

        maxValue = context.Request.QueryString["max"];
        minValue = context.Request.QueryString["min"];
        divisionEach = context.Request.QueryString["div"];
        showScale = context.Request.QueryString["showScale"];
        values = context.Request.QueryString["values"];
        colors = context.Request.QueryString["colors"];
        labels = context.Request.QueryString["labels"];
        showFill = context.Request.QueryString["showFill"];
        w = context.Request["width"];
        h = context.Request["height"];

        try
        {
            width = String.IsNullOrEmpty(w) ? 550 : Convert.ToInt32(w);
            height = String.IsNullOrEmpty(h) ? 400 : Convert.ToInt32(h);
        }
        catch (Exception ex) { }
        
        /* --- Test Values ---
        maxValue = "150";
        minValue = "0";
        divisionEach = "25";
        showScale = "true";
        values = "10,20,30,40,50,60,70,80,90,100,110,120@40,50,60,70,80,90,100,110,120,10,20,30@77,12,45,120,33,1,0,20,89,10,22,140";
        colors = "CC0000,0077CC,000000";
        labels = "HOLA,B,C,D,E,F,G,HOLA,1,2,3,4";
        showFill = "true";
        /* --- Test Values --- */

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
        dashPen = new Pen(_Grey, 1);
        dashPen.DashCap = DashCap.Flat;
        dashPen.DashPattern = new float[] { 4, 4 };

        graphics.FillRectangle(Brushes.White, 0, 0, width, height);

        drawBackground(graphics, width, height);
        startUp(graphics);

        /* --- End --- */
        Calibri.Dispose();
        graphics.Dispose();
        bitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
        bitmap.Dispose();
        context.Response.AddHeader("Content-Type", "image/png");
        context.Response.End();
    }

    private void drawBackground(Graphics gObj, int w, int h)
    {
        gridWidth = width - 100;
        gridHeight = height - 100;
        if (gridWidth < gridHeight)
        {
            gridHeight = gridWidth;
        }
        else
        {
            gridWidth = gridHeight;
        }
        maxR = gridWidth / 2;
        gridY = (h - gridHeight) / 2;
        gridX = (w - gridWidth) / 2;
        Brush brGradient = new LinearGradientBrush(new Rectangle(3, 3, w - 6, h - 6), Common.HexToColor("FFFFFF", 255), Common.HexToColor("BBDDFF", 255), 90, false);
        Pen border = new Pen(Common.HexToColor("BBDDFF", 255), 1);
        Brush brWhite = new SolidBrush(Common.HexToColor("FFFFFF", 255));
        gObj.FillRectangle(brWhite, 0, 0, w, h);
        gObj.FillRectangle(brGradient, 3, 3, w - 6, h - 6);
        gObj.DrawRectangle(border, 3, 3, w - 6, h - 6);
        brWhite.Dispose();
        border.Dispose();
        brGradient.Dispose();
    }

    private void startUp(Graphics gObj)
    {
        _maxValue = ((maxValue == null) || (maxValue == "")) ? 100 : Convert.ToDouble(maxValue);
        _minValue = ((minValue == null) || (minValue == "")) ? 0 : Convert.ToDouble(minValue);
        _divisionEach = ((divisionEach == null) || (divisionEach == "")) ? 50 : Convert.ToDouble(divisionEach);
        _showScale = ((showScale == null) || (showScale == "")) ? false : Convert.ToBoolean(showScale);
        _showFill = ((showFill == null) || (showFill == "")) ? false : Convert.ToBoolean(showFill);
        if (values == null) values = "0,0,0,0";
        if (colors == null) colors = "";
        if (colors == "") colors = "000000";
        _colors = colors.Split(',');
        if (labels == null) labels = "";
        valuePerPoint = values.Split('@');
        if (labels != "") labelPerPoint = labels.Split(',');
        valuesToDisplay = labelPerPoint.Length;
        if (_divisionEach > _maxValue) _divisionEach = _maxValue;
        gridDivisions = Math.Round(_maxValue / _divisionEach);
        if (gridDivisions == 0) gridDivisions = 1;

        gridCenter = gridWidth / 2;
        gridDiameter = gridWidth;
        gridCircumference = gridDiameter * Math.PI;
        lineRotation = 360 / valuesToDisplay;

        drawBasicInformation(gObj);
    }

    private void drawBasicInformation(Graphics gObj)
    {
        int i = 0;
        int k = 0;
        float _div;
        double mx, my;
        double offset = 2;
        Brush brLightGrey = new SolidBrush(_Grey);
        Brush brDarkGrey = new SolidBrush(_DarkGrey);
        Brush brBlue = new SolidBrush(_Blue);
        Brush brRed = new SolidBrush(_Red);
        Brush brLightBlue = new SolidBrush(_LightBlue);
        Brush brAlphaRed = new SolidBrush(_AlphaRed);
        Brush brAlphaBlue = new SolidBrush(_AlphaBlue);
        Font SmallFont = new System.Drawing.Font("Segoe UI", 8);
        Font LabelFont = new System.Drawing.Font("Segoe UI", 10);

        _div = (float)((_divisionEach * gridDivisions * maxR) / _maxValue);
        double valDiv;
        for (i = 1; i <= gridDivisions; i++)
        {
            _div = (float)((_divisionEach * i * maxR) / _maxValue);
            if (i == gridDivisions)
            {
                gObj.DrawEllipse(greyPen, gridX + (float)gridCenter - _div, gridY + (float)gridCenter - _div, _div * 2, _div * 2);
            }
            else
            {
                gObj.DrawEllipse(dashPen, gridX + (float)gridCenter - _div, gridY + (float)gridCenter - _div, _div * 2, _div * 2);
            }
            mx = gridX + gridCenter + offset;
            my = gridY + gridCenter - _div;
            if (_showScale)
            {
                valDiv = _divisionEach * i;
                gObj.DrawString(valDiv.ToString(), SmallFont, brLightGrey, (float)mx, (float)my);
            }
        }

        for (k = 0; k <= valuePerPoint.Length - 1; k++)
        {
            string[] valuePerColor;
            GraphicsPath shapeCreator = new GraphicsPath();
            Point p0 = new Point(0, 0);
            Point p1 = new Point(0, 0);
            string label;
            valuePerColor = valuePerPoint[k].Split(',');
            for (i = 1; i <= valuePerColor.Length; i++)
            {
                if (labels != "")
                {
                    label = labelPerPoint[i - 1];
                }
                else
                {
                    label = i.ToString();
                }
                drawDiv(gObj, label, (float)(lineRotation * (i - 1)));
                valuePerColor[i - 1] = valuePerColor[i - 1];
                try
                {
                    p1 = addPoint(gObj, (float)Convert.ToDouble(valuePerColor[i - 1]), (float)(lineRotation * (i - 1)), _colors[k]);
                }
                catch { }
                if (i != 1)
                {
                    shapeCreator.AddLine(p0, p1);
                }
                p0 = p1;
            }
            shapeCreator.CloseFigure();
            Pen pPath = new Pen(Common.HexToColor(_colors[k], 255), 2);
            if (_showFill)
            {
                Brush pBrush = new SolidBrush(Common.HexToColor(_colors[k], 40));
                gObj.FillPath(pBrush, shapeCreator);
                pBrush.Dispose();
            }
            gObj.DrawPath(pPath, shapeCreator);
            pPath.Dispose();
            shapeCreator.Reset();
        }

        brAlphaRed.Dispose();
        brAlphaBlue.Dispose();
        brLightGrey.Dispose();
        brDarkGrey.Dispose();
        SmallFont.Dispose();
        LabelFont.Dispose();
        brLightBlue.Dispose();
    }

    private Point addPoint(Graphics gObj, float value, float rot, string color)
    {
        Point p = new Point();
        Brush brush = new SolidBrush(Common.HexToColor(color, 255));
        gObj.TranslateTransform(gridX + (float)gridCenter, gridY + (float)gridCenter);
        gObj.RotateTransform(rot + 180);
        gObj.TranslateTransform(-(gridX + (float)gridCenter), -(gridY + (float)gridCenter));
        gObj.FillEllipse(brush, gridX + (float)gridCenter - 4, gridY + (float)gridCenter + (float)(value / _maxValue * maxR) - 4, 8, 8);
        gObj.ResetTransform();

        p = Rotacion2D(gridX + (float)gridCenter, gridY + (float)gridCenter + (float)(value / _maxValue * maxR), gridX + (float)gridCenter, gridY + (float)gridCenter, -rot + 180);

        brush.Dispose();
        return p;
    }

    private void drawDiv(Graphics gObj, string label, float rot)
    {
        Pen transparentPen = new Pen(Color.FromArgb(0, 0, 0, 0), 2);
        Brush brDarkGrey = new SolidBrush(_DarkGrey);
        Font LabelFont = new System.Drawing.Font("Segoe UI", 10);

        gObj.TranslateTransform(gridX + (float)gridCenter, gridY + (float)gridCenter);
        gObj.RotateTransform(rot);
        gObj.TranslateTransform(-(gridX + (float)gridCenter), -(gridY + (float)gridCenter));
        gObj.DrawLine(greyPen, gridX + (float)gridCenter, gridY + (float)gridCenter - maxR, gridX + (float)gridCenter, gridY + (float)gridCenter);
        gObj.DrawString(label, LabelFont, brDarkGrey, gridX + (float)gridCenter - (5 * label.Length), gridY + (float)gridCenter - maxR - LabelFont.GetHeight() - 3);
        gObj.ResetTransform();

        transparentPen.Dispose();
        brDarkGrey.Dispose();
        LabelFont.Dispose();
    }

    private Point Rotacion2D(double x, double y, double xc, double yc, double angulo)
    {
        double a;
        int NewX, NewY;
        Point p = new Point(0, 0);
        a = angulo * Math.PI / 180;
        NewX = (int)((x - xc) * Math.Cos(a) - (yc - y) * Math.Sin(a) + xc);
        NewY = (int)(yc - (x - xc) * Math.Sin(a) - (yc - y) * Math.Cos(a));
        x = NewX;
        y = NewY;
        p.X = (int)x;
        p.Y = (int)y;
        return p;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
