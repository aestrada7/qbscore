<%@ WebHandler Language="C#" Class="Slider" %>

using System;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aexis;

public class Slider : IHttpHandler
{
    private Font Segoe = new System.Drawing.Font("Segoe UI", 12);

    /* --- Parameters --- */
    public string color;
    public string color2;
    public string _minValue;
    public string _maxValue;
    public string _value;
    public string _value2;
    public string _divisions;
    public string w;
    public string _labelsOverride;
	
	/* --- Variables --- */
	public int minValue = 0;
    public int maxValue = 100;
    public double value = 0;
    public double value2 = 0;
    public int divisions = 10;
    public int width = 500;
    public int height = 60;
    public string[] labels;
  
	public void ProcessRequest(HttpContext context)
    {
        context.Response.Clear();
        
        color = context.Request["color"];
        color2 = context.Request["color2"];
        _minValue = context.Request["min"];
        _maxValue = context.Request["max"];
        _value = context.Request["value"];
        _value2 = context.Request["value2"];
        _divisions = context.Request["divisions"];
        w = context.Request["width"];
        _labelsOverride = context.Request["labelsOverride"];
        
        try
        {
            minValue = String.IsNullOrEmpty(_minValue) ? 0 : Convert.ToInt32(_minValue);
            maxValue = String.IsNullOrEmpty(_maxValue) ? 100 : Convert.ToInt32(_maxValue);
            value = String.IsNullOrEmpty(_value) ? 0 : Convert.ToDouble(_value);
            value2 = String.IsNullOrEmpty(_value2) ? 0 : Convert.ToDouble(_value2);
            divisions = String.IsNullOrEmpty(_divisions) ? 10 : Convert.ToInt32(_divisions);
            width = String.IsNullOrEmpty(w) ? 500 : Convert.ToInt32(w);
            if(String.IsNullOrEmpty(color)) color = "000000";
            if(!String.IsNullOrEmpty(_labelsOverride))
            {
                labels = _labelsOverride.Split(',');
            }
        }
        catch(Exception ex) {}

        try
        {
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            drawBackground(graphics, width, height);
            startUp(graphics);

            graphics.Dispose();
            bitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
            bitmap.Dispose();
            context.Response.AddHeader("Content-Type", "image/png");
            context.Response.End();
        }
        catch (ArgumentException ex) { }
	}

	private void startUp(Graphics gObj)
    {
        Pen greyPen = new Pen(Common.HexToColor("333366", 255), 1);
        Brush brDarkGrey = new SolidBrush(Common.HexToColor("333366", 255));
        gObj.DrawLine(greyPen, 20, 10, 20, height - 25);
        string txtToShow = minValue.ToString();
        try
        {
            if(!String.IsNullOrEmpty(labels[0])) txtToShow = labels[0];
        }
        catch(Exception ex) {}
        gObj.DrawString(txtToShow, Segoe, brDarkGrey, 10, height - 25);
        gObj.DrawLine(greyPen, 20, height - 30, width - 20, height - 30);
        gObj.DrawLine(greyPen, width - 20, 10, width - 20, height - 25);
        txtToShow = maxValue.ToString();
        try
        {
            if(!String.IsNullOrEmpty(labels[1])) txtToShow = labels[1];
        }
        catch(Exception ex) {}
        gObj.DrawString(txtToShow, Segoe, brDarkGrey, width - 40, height - 25);
        int minXValue = 20;
        int maxXValue = width - 20;
        SolidBrush brush;
        Rectangle rectangle;
        if(value2 > 0)
        {
            float sliderValueProfile = (float)(value2 * (maxXValue - minXValue) / maxValue) + minXValue;
            brush = new SolidBrush(Common.HexToColor(color2, 255));
            rectangle = new Rectangle(minXValue, 23, (int)sliderValueProfile - minXValue, 5);
            gObj.FillRectangle(brush, rectangle);
            gObj.DrawLine(greyPen, (int)sliderValueProfile, 10, (int)sliderValueProfile, height - 25);
            gObj.DrawString(value2.ToString(), Segoe, brDarkGrey, (int)sliderValueProfile + 3, height - 42);
        }
        float sliderValue = (float)(value * (maxXValue - minXValue) / maxValue) + minXValue;
        brush = new SolidBrush(Common.HexToColor(color, 255));
        rectangle = new Rectangle(minXValue, 15, (int)sliderValue - minXValue, value2 > 0 ? 5 : 10);
        gObj.FillRectangle(brush, rectangle);
        gObj.DrawLine(greyPen, (int)sliderValue, 10, (int)sliderValue, height - 25);
        gObj.DrawString(value.ToString(), Segoe, brDarkGrey, (int)sliderValue + 3, value2 > 0 ? height - 57 : height - 52);
        brush.Dispose();
        brDarkGrey.Dispose();
        greyPen.Dispose();
    }
  
	private void drawBackground(Graphics gObj, int w, int h)
    {
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
	
	public bool IsReusable
    {
        get
        {
            return false;
        }
	}

}