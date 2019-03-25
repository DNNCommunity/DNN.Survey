/*
 * DNN® - https://www.dnnsoftware.com
 * Copyright (c) 2002-2019
 * by DNN Corp.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions
 * of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

using DotNetNuke.Entities.Icons;
using DotNetNuke.Services.Localization;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey.Controls
{
   public partial class SurveyRadioButtons : UserControl
   {
      public string EditUrl { get; set; }
      public bool IsEditable { get; set; }

      public string Label
      {
         get
         {
            return SurveyRadioButtonsLabel.Text;
         }
         set
         {
            SurveyRadioButtonsLabel.Text = value;
         }
      }

      public ListItemCollection Items
      {
         get
         {
            return SurveyRadioButtonList.Items;
         }
      }

      public object DataSource
      {
         get
         {
            return SurveyRadioButtonList.DataSource;
         }
         set
         {
            SurveyRadioButtonList.DataSource = value;
         }
      }

      public string DataTextField
      {
         get
         {
            return SurveyRadioButtonList.DataTextField;
         }
         set
         {
            SurveyRadioButtonList.DataTextField = value;
         }
      }

      public string DataValueField
      {
         get
         {
            return SurveyRadioButtonList.DataValueField;
         }
         set
         {
            SurveyRadioButtonList.DataValueField = value;
         }
      }

      public int SelectedValue
      {
         get
         {
            return Convert.ToInt32(SurveyRadioButtonList.SelectedValue);
         }
      }

      public RepeatDirection RepeatDirection
      {
         get
         {
            return SurveyRadioButtonList.RepeatDirection;
         }
         set
         {
            SurveyRadioButtonList.RepeatDirection = value;
         }
      }

      public int RepeatColumns
      {
         get
         {
            return SurveyRadioButtonList.RepeatColumns;
         }
         set
         {
            SurveyRadioButtonList.RepeatColumns = value;
         }
      }

      public override void DataBind()
      {
         SurveyRadioButtonList.DataBind();
      }

      public string ValidationGroup
      {
         get
         {
            return SurveyRadioButtonListValidator.ValidationGroup;
         }
         set
         {
            SurveyRadioButtonListValidator.ValidationGroup = value;
         }
      }

      public string ErrorMessage
      {
         get
         {
            return SurveyRadioButtonListValidator.ErrorMessage;
         }
         set
         {
            SurveyRadioButtonListValidator.ErrorMessage = value;
         }
      }

      protected void Page_Load(object sender, EventArgs e)
      {
         if (IsEditable)
         {
            string pencil = string.Format("<a href=\"{0}\" title=\"{1}\"><img src=\"{2}\" style=\"border: 0px none;\"></a>",
               EditUrl,
               Localization.GetString("cmdEdit.Text"),
               IconController.IconURL("Edit"));
            Label = String.Format("{0}&nbsp;{1}", pencil, Label);
         }
      }
   }
}