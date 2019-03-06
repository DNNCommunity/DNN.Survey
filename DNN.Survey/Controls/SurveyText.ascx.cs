using DotNetNuke.Entities.Icons;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey.Controls
{
   public partial class SurveyText : UserControl
   {
      public string EditUrl { get; set; }
      public bool IsEditable { get; set; }
      public int SurveyOptionID { get; set; }

      public string Label
      {
         get
         {
            return SurveyTextBoxLabel.Text;
         }
         set
         {
            SurveyTextBoxLabel.Text = value;
         }
      }

      public string Text
      {
         get
         {
            return SurveyTextBox.Text;
         }
      }

      public int NumberOfRows
      {
         get
         {
            return SurveyTextBox.Rows;
         }
         set
         {
            if (value > 1)
            {
               SurveyTextBox.TextMode = TextBoxMode.MultiLine;
               SurveyTextBox.Rows = value;
            }
            else
            {
               SurveyTextBox.TextMode = TextBoxMode.SingleLine;
            }
         }
      }

      public string ValidationGroup
      {
         get
         {
            return SurveyTextBoxValidator.ValidationGroup;
         }
         set
         {
            SurveyTextBoxValidator.ValidationGroup = value;
         }
      }

      public string ErrorMessage
      {
         get
         {
            return SurveyTextBoxValidator.ErrorMessage;
         }
         set
         {
            SurveyTextBoxValidator.ErrorMessage = value;
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