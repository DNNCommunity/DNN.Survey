using DNN.Modules.Survey.Components.UI.WebControls.Validators;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey.Controls
{
   public partial class SurveyCheckBoxes : UserControl
   {
      public string EditUrl { get; set; }
      public bool IsEditable { get; set; }

      public string Label
      {
         get
         {
            return SurveyCheckBoxesLabel.Text;
         }
         set
         {
            SurveyCheckBoxesLabel.Text = value;
         }
      }

      public ListItemCollection Items
      {
         get
         {
            return SurveyCheckBoxList.Items;
         }
      }

      public object DataSource
      {
         get
         {
            return SurveyCheckBoxList.DataSource;
         }
         set
         {
            SurveyCheckBoxList.DataSource = value;
         }
      }

      public string DataTextField
      {
         get
         {
            return SurveyCheckBoxList.DataTextField;
         }
         set
         {
            SurveyCheckBoxList.DataTextField = value;
         }
      }

      public string DataValueField
      {
         get
         {
            return SurveyCheckBoxList.DataValueField;
         }
         set
         {
            SurveyCheckBoxList.DataValueField = value;
         }
      }

      public List<int> SelectedItems
      {
         get
         {
            List<int> selectedItems = new List<int>();
            foreach (ListItem li in SurveyCheckBoxList.Items)
            {
               if (li.Selected)
               {
                  selectedItems.Add(Convert.ToInt32(li.Value));
               }
            }
            return selectedItems;
         }
      }

      public RepeatDirection RepeatDirection
      {
         get
         {
            return SurveyCheckBoxList.RepeatDirection;
         }
         set
         {
            SurveyCheckBoxList.RepeatDirection = value;
         }
      }

      public int RepeatColumns
      {
         get
         {
            return SurveyCheckBoxList.RepeatColumns;
         }
         set
         {
            SurveyCheckBoxList.RepeatColumns = value;
         }
      }

      public override void DataBind()
      {
         SurveyCheckBoxList.DataBind();
      }

      public string ValidationGroup
      {
         get
         {
            return SurveyCheckBoxListValidator.ValidationGroup;
         }
         set
         {
            SurveyCheckBoxListValidator.ValidationGroup = value;
         }
      }

      public string ErrorMessage
      {
         get
         {
            return SurveyCheckBoxListValidator.ErrorMessage;
         }
         set
         {
            SurveyCheckBoxListValidator.ErrorMessage = value;
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