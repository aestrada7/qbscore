using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aexis.Web
{
    public static class Charts
    {
        public static string ChartLocation = "./charts/";

        /// <summary>
        /// Builds a Slider Chart.
        /// </summary>
        /// <param name="hexColor">The color for the bar. Default: 000000.</param>
        /// <param name="value">The value for the bar. Default: 0.</param>
        /// <param name="width">The width in pixels for the chart. Default: 400.</param>
        /// <returns>An HTML string with the code for the chart.</returns>
        public static string Slider(string hexColor, string value, string width)
        {
            return Slider(hexColor, value, width, "0", "100");
        }

        /// <summary>
        /// Builds a Slider Chart.
        /// </summary>
        /// <param name="hexColor">The color for the bar. Default: 000000.</param>
        /// <param name="value">The value for the bar. Default: 0.</param>
        /// <param name="width">The width in pixels for the chart. Default: 400.</param>
        /// <param name="minValue">The minimum value the chart can display. Default: 0.</param>
        /// <param name="maxValue">The maximum value the chart can display. Default: 100.</param>
        /// <returns>An HTML string with the code for the chart.</returns>
        public static string Slider(string hexColor, string value, string width, string minValue, string maxValue)
        {
            return Slider(hexColor, value, width, minValue, maxValue, "", "", "");
        }

        /// <summary>
        /// Builds a Slider Chart.
        /// </summary>
        /// <param name="hexColor">The color for the bar. Default: 000000.</param>
        /// <param name="value">The value for the bar. Default: 0.</param>
        /// <param name="width">The width in pixels for the chart. Default: 400.</param>
        /// <param name="minValue">The minimum value the chart can display. Default: 0.</param>
        /// <param name="maxValue">The maximum value the chart can display. Default: 100.</param>
        /// <param name="hexColor2">The color for a second bar. Default: none.</param>
        /// <param name="value2">The value for the second bar. Default: none.</param>
        /// <param name="labelsOverride">A CSV String of two labels that will replace the minValue and maxValue properties.</param>
        /// <returns>An HTML string with the code for the chart.</returns>
        public static string Slider(string hexColor, string value, string width, string minValue, string maxValue, string hexColor2, string value2, string labelsOverride)
        {
            return "<img src='" + ChartLocation + "Slider.ashx?color=" + hexColor + "&color2=" + hexColor2 + "&min=" + minValue + "&max=" + maxValue + "&value=" + value + "&value2=" + value2 + "&width=" + width + "&labelsOverride=" + labelsOverride + "' />";
        }

        /// <summary>
        /// Builds a Radial Chart.
        /// </summary>
        /// <param name="hexColors">A CSV String of the n colors that will be used.</param>
        /// <param name="values">A dually separated string of the values that will be given. If it's a single series, it's a CSV (10,20,30), 
        /// if there are two or more series, that one is separated by @ symbols (10,20,30@20,30,40).</param>
        /// <param name="width">The width in pixels for the chart. Default: 550.</param>
        /// <param name="height">The height in pixels for the chart. Default: 400.</param>
        /// <param name="minValue">The minimum value the chart can display. Default: 0.</param>
        /// <param name="maxValue">The maximum value the chart can display. Default: 100.</param>
        /// <param name="divEach">Creates a division each "x" value. Default: 50. (Meaning, two divisions for the default values)</param>
        /// <param name="showScale">If true, shows the scale in use.</param>
        /// <param name="labels">A CSV String of the n labels that will be used.</param>
        /// <param name="showFill">If true, fills the radial chart.</param>
        /// <returns></returns>
        public static string Radial(string hexColors, string values, string width, string height, string minValue, string maxValue, string divEach, bool showScale, string labels, bool showFill)
        {
            return "<img src='" + ChartLocation + "Radial.ashx?colors=" + hexColors + "&values=" + values + "&labels=" + labels + "&min=" + minValue + "&max=" + maxValue + "&div=" + divEach + "&showScale=" + showScale + "&showFill=" + showFill + "&width=" + width + "&height=" + height + "' />";
        }
    }
}