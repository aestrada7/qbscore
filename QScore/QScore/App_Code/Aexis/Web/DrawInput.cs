using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Aexis.Web
{
    /// <summary>
    /// The DrawInput Class provides a wrapper for various different form inputs in a webpage.
    /// </summary>
    public static class DrawInput
    {
        /// <summary>
        /// Draws a Password Field
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="value">The current value for the field</param>
        /// <param name="maxLength">The maximum number of characters a field can have</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onChange">The JavaScript action it will fire when changed</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputPasswordField(string id, string value, string maxLength, string cssClass, string onChange, string styles, string extraAttributes)
        {
            return "<input type=\"password\" name=\"" + id + "\" id=\"" + id + "\" value=\"" + value + "\" maxlength=\"" + maxLength + "\" onChange=\"" + onChange + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" onMouseOut=\"inOut(this);\" onMouseOver=\"inOver(this);\" onBlur=\"inBlur(this);\" onFocus=\"inFocus(this);\" " + extraAttributes + " />";
        }

        /// <summary>
        /// Draws a Text Field
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="value">The current value for the field</param>
        /// <param name="maxLength">The maximum number of characters a field can have</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onChange">The JavaScript action it will fire when changed</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputTextField(string id, string value, string maxLength, string cssClass, string onChange, string styles, string extraAttributes)
        {
            return "<input type=\"text\" name=\"" + id + "\" id=\"" + id + "\" value=\"" + value + "\" maxlength=\"" + maxLength + "\" onChange=\"" + onChange + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" onMouseOut=\"inOut(this);\" onMouseOver=\"inOver(this);\" onBlur=\"inBlur(this);\" onFocus=\"inFocus(this);\" " + extraAttributes + " />";
        }

        /// <summary>
        /// Draws a File Field
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="value">The current value for the field</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onChange">The JavaScript action it will fire when changed</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputFileField(string id, string value, string cssClass, string onChange, string styles, string extraAttributes)
        {
            return "<input type=\"file\" name=\"" + id + "\" id=\"" + id + "\" value=\"" + value + "\" onChange=\"" + onChange + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" onMouseOut=\"inOut(this);\" onMouseOver=\"inOver(this);\" onBlur=\"inBlur(this);\" onFocus=\"inFocus(this);\" " + extraAttributes + " />";
        }

        /// <summary>
        /// Draws a Text Area
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="value">The current value for the field</param>
        /// <param name="rows">The number of lines</param>
        /// <param name="cols">The visible width</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onChange">The JavaScript action it will fire when changed</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputTextArea(string id, string value, string rows, string cols, string cssClass, string onChange, string styles, string extraAttributes)
        {
            return "<textarea type=\"text\" name=\"" + id + "\" id=\"" + id + "\" cols=\"" + cols + "\" rows=\"" + rows + "\" onChange=\"" + onChange + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" onMouseOut=\"inOut(this);\" onMouseOver=\"inOver(this);\" onBlur=\"inBlur(this);\" onFocus=\"inFocus(this);\" " + extraAttributes + ">" + value + "</textarea>";
        }

        /// <summary>
        /// Draws a Hidden Field
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="value">The current value for the field</param>
        /// <param name="onChange">The JavaScript action it will fire when changed</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputHiddenField(string id, string value, string onChange, string extraAttributes)
        {
            return "<input type=\"hidden\" name=\"" + id + "\" id=\"" + id + "\" value=\"" + value + "\" onChange=\"" + onChange + "\" " + extraAttributes + " />";
        }

        /// <summary>
        /// Draws a Button
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="value">The text of the button</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onClick">The JavaScript action it will fire when clicked</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputButton(string id, string value, string cssClass, string onClick, string styles, string extraAttributes)
        {
            return "<input type='button' name=\"" + id + "\" id=\"" + id + "\" value=\"" + value + "\" onClick=\"" + onClick + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" " + extraAttributes + "></input>";
        }

        /// <summary>
        /// Draws a Submit Button. The difference between InputButton and InputSubmit is that Submit works when pressing enter.
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="value">The text of the button</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onClick">The JavaScript action it will fire when clicked</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputSubmit(string id, string value, string cssClass, string onClick, string styles, string extraAttributes)
        {
            return "<input type='submit' name=\"" + id + "\" id=\"" + id + "\" value=\"" + value + "\" onClick=\"" + onClick + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" " + extraAttributes + "></input>";
        }

        /// <summary>
        /// Draws a Checkbox
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="value">The text of the button</param>
        /// <param name="isChecked">Boolean value, is the checkbox checked or not</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onClick">The JavaScript action it will fire when clicked</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputCheckbox(string id, string value, bool isChecked, string cssClass, string onClick, string styles, string extraAttributes)
        {
            return "<input type='checkbox' name=\"" + id + "\" id=\"" + id + "\" " + (isChecked ? "checked='checked'" : "") + " value=\"" + value + "\" onClick=\"" + onClick + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" " + extraAttributes + "></input>";
        }

        /// <summary>
        /// Draws a Radio Button
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="value">The value of the radio</param>
        /// <param name="isChecked">Boolean value, is the radio checked or not</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onClick">The JavaScript action it will fire when clicked</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputRadio(string id, string value, bool isChecked, string cssClass, string onClick, string styles, string extraAttributes)
        {
            return "<input type='radio' name=\"" + id + "\" id=\"" + id + "\" " + (isChecked ? "checked='checked'" : "") + " value=\"" + value + "\" onClick=\"" + onClick + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" " + extraAttributes + "></input>";
        }

        /// <summary>
        /// Draws a Selection Combobox from an array of values
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="selectedValue">The currently selected value, must be present in the values Array</param>
        /// <param name="values">An array of strings of all the possible values</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onChange">The JavaScript action it will fire when changed</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputSelect(string id, string selectedValue, string[] values, string cssClass, string onChange, string styles, string extraAttributes)
        {
            string output = "";
            output += "<select name=\"" + id + "\" id=\"" + id + "\" onChange=\"" + onChange + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" onMouseOut=\"inOut(this);\" onMouseOver=\"inOver(this);\" onBlur=\"inBlur(this);\" onFocus=\"inFocus(this);\" " + extraAttributes + ">";
            foreach(string value in values)
            {
                output += "<option value=\"" + value + "\"" + (selectedValue == value ? "selected" : "") + ">" + value + "</option>";
            }
            output += "</select>";
            return output;
        }

        /// <summary>
        /// Draws a Selection Combobox from an array of values and an array of friendly values
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="selectedValue">The currently selected value, must be present in the values Array</param>
        /// <param name="values">An array of strings of all the possible values</param>
        /// <param name="friendlyText">An array of strings of all the possible values (in friendly form)</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onChange">The JavaScript action it will fire when changed</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputSelect(string id, string selectedValue, string[] values, string[] friendlyText, string cssClass, string onChange, string styles, string extraAttributes)
        {
            string output = "";
            int i;
            output += "<select name=\"" + id + "\" id=\"" + id + "\" onChange=\"" + onChange + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" onMouseOut=\"inOut(this);\" onMouseOver=\"inOver(this);\" onBlur=\"inBlur(this);\" onFocus=\"inFocus(this);\" " + extraAttributes + ">";
            for(i = 0; i < values.Length; i++)
            {
                output += "<option value=\"" + values[i] + "\"" + (selectedValue == values[i] ? "selected" : "") + ">" + friendlyText[i] + "</option>";
            }
            output += "</select>";
            return output;
        }

        /// <summary>
        /// Draws a Selection Combobox from a CSV of values and friendly values
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="selectedValue">The currently selected value, must be present in the values Array</param>
        /// <param name="valuesCSV">A CSV of strings of all the possible values</param>
        /// <param name="friendlyTextCSV">A CSV of strings of all the possible values (in friendly form)</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onChange">The JavaScript action it will fire when changed</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputSelect(string id, string selectedValue, string valuesCSV, string friendlyTextCSV, string cssClass, string onChange, string styles, string extraAttributes)
        {
            return InputSelect(id, selectedValue, Common.CSVToArray(valuesCSV), Common.CSVToArray(friendlyTextCSV), cssClass, onChange, styles, extraAttributes);
        }

        /// <summary>
        /// Draws a Selection Combobox from a table in the database
        /// </summary>
        /// <param name="id">The Id and Name for the field</param>
        /// <param name="selectedValue">The currently selected value, must be present in the values Array (could also be a CSV of multiple values)</param>
        /// <param name="tableName">The name of the table</param>
        /// <param name="idField">The field that holds the Id</param>
        /// <param name="valueField">The field that holds the name</param>
        /// <param name="condition">An SQL condition</param>
        /// <param name="cssClass">The name of the CSS class for the field</param>
        /// <param name="onChange">The JavaScript action it will fire when changed</param>
        /// <param name="styles">Additional styles for the field</param>
        /// <param name="extraAttributes">Additional attributes for the field</param>
        /// <returns>A string with the HTML Markup</returns>
        public static string InputSelect(string id, string selectedValue, string tableName, string idField, string valueField, string condition, string cssClass, string onChange, string styles, string extraAttributes)
        {
            string[] selectedValues = Common.CSVToArray(selectedValue);
            string output = "";
            output += "<select name=\"" + id + "\" id=\"" + id + "\" onChange=\"" + onChange + "\" style=\"" + styles + "\" class=\"" + cssClass + "\" onMouseOut=\"inOut(this);\" onMouseOver=\"inOver(this);\" onBlur=\"inBlur(this);\" onFocus=\"inFocus(this);\" " + extraAttributes + ">";
            string sql = "SELECT * FROM " + tableName;
            sql = Common.StrAdd(sql, " WHERE ", condition);
            List<Dictionary<string, string>> result = Common.GetRS(sql);
            foreach (Dictionary<string, string> records in result)
            {
                bool isSelected = false;
                foreach(string sVal in selectedValues)
                {
                    if (sVal == records[idField])
                    {
                        isSelected = true;
                        break;
                    }
                }
                output += "<option value=\"" + records[idField] + "\"" + (isSelected ? "selected" : "") + ">" + records[valueField] + "</option>";
            }
            output += "</select>";
            return output;
        }
    }
}