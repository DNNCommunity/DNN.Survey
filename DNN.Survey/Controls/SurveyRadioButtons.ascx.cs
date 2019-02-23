using DotNetNuke.Entities.Icons;
using DotNetNuke.Services.Localization;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey.Controls
{
   public partial class SurveyRadioButtons : UserControl
   {
      public int SurveyID { get; set; }
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