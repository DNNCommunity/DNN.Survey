using DNN.Modules.Survey.Components;
using DNN.Modules.Survey.Components.Controllers;
using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Controls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey
{
   public partial class SurveyView : PortalModuleBase, IActionable
   {
      #region Settings
      protected TrackingMethod SurveyTracking
      {
         get
         {
            object surveyTracking = Settings["SurveyTracking"];
            if (surveyTracking == null)
               return TrackingMethod.Cookie;
            else
               return (TrackingMethod)Convert.ToInt32(surveyTracking);
         }
      }
      #endregion

      #region TabModuleSettings
      protected bool ShowClosingDateMessage
      {
         get
         {
            object showClosingDateMessage = ModuleController.Instance.GetTabModule(TabModuleId).TabModuleSettings["ShowClosingDateMessage"];
            if (showClosingDateMessage == null)
               return false;
            else
               return Convert.ToBoolean(showClosingDateMessage);
         }
      }
      #endregion

      #region Private Properties
      private ModulePermissionCollection _modulePermissionCollection = null;
      private SurveysController _surveysController = null;
      private SurveyOptionsController _surveyOptionsController = null;
      private SurveyResultsController _surveyResultsController = null;

      protected ModulePermissionCollection ModulePermissionCollection
      {
         get
         {
            if (_modulePermissionCollection == null)
               _modulePermissionCollection = ModulePermissionController.GetModulePermissions(ModuleId, TabId);
            return _modulePermissionCollection;
         }
      }

      protected SurveysController SurveysController
      {
         get
         {
            if (_surveysController == null)
               _surveysController = new SurveysController();
            return _surveysController;
         }
      }

      protected SurveyOptionsController SurveyOptionsController
      {
         get
         {
            if (_surveyOptionsController == null)
               _surveyOptionsController = new SurveyOptionsController();
            return _surveyOptionsController;
         }
      }

      protected SurveyResultsController SurveyResultsController
      {
         get
         {
            if (_surveyResultsController == null)
               _surveyResultsController = new SurveyResultsController();
            return _surveyResultsController;
         }
      }
      #endregion

      #region Settings
      protected DateTime SurveyClosingDate
      {
         get
         {
            object surveyClosingDate = Settings["SurveyClosingDate"];
            if (surveyClosingDate == null)
               return DateTime.MinValue;
            else
               return Convert.ToDateTime(surveyClosingDate);
         }
      }
      #endregion

      #region IActionable
      public ModuleActionCollection ModuleActions
      {
         get
         {
            ModuleActionCollection actions = new ModuleActionCollection();
            // Add Question
            actions.Add(GetNextActionID(), Localization.GetString("AddQuestion.Action", LocalResourceFile), ModuleActionType.AddContent, string.Empty, string.Empty, EditUrl(), false, SecurityAccessLevel.Edit, true, false);
            if (ModulePermissionController.HasModulePermission(ModulePermissionCollection, ModuleSecurity.VIEW_RESULTS_PERMISSION))
            {
               // View Results
               actions.Add(GetNextActionID(), Localization.GetString("ViewResults.Action", LocalResourceFile), ModuleActionType.AddContent, string.Empty, string.Empty, EditUrl("SurveyResults"), false, SecurityAccessLevel.View, true, false);
            }
            return actions;
         }
      }
      #endregion

      protected override void OnLoad(EventArgs e)
      {
         if (SurveyClosingDate == DateTime.MinValue)
         {
            SurveyMessageLabel.Visible = false;
         }
         else
         {
            if (SurveyClosingDate >= DateTime.Now)
            {
               if (ShowClosingDateMessage)
               {
                  SurveyMessageLabel.Text = String.Format(Localization.GetString("SurveyWillClose.Text", LocalResourceFile), SurveyClosingDate);
                  SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormInfo";
               }
               else
               {
                  SurveyMessageLabel.Visible = false;
               }
            }
            else
            {
               SurveyMessageLabel.Text = String.Format(Localization.GetString("SurveyClosed.Text", LocalResourceFile), SurveyClosingDate);
               SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormWarning";
            }
         }

         if (ModulePermissionController.HasModulePermission(ModulePermissionCollection, ModuleSecurity.VIEW_RESULTS_PERMISSION))
            ViewResultsButton.Visible = true;
         else
            ViewResultsButton.Visible = false;

         List<SurveysInfo> surveys = SurveysController.GetAll(ModuleId);

         foreach (SurveysInfo survey in surveys)
         {
            List<SurveyOptionsInfo> surveyOptions = SurveyOptionsController.GetAll(survey.SurveyID);
            switch (survey.OptionType)
            {
               case QuestionType.RadioButtons:
                  SurveyRadioButtons surveyRadioButtons = (SurveyRadioButtons)LoadControl(String.Format("{0}Controls/SurveyRadioButtons.ascx", ControlPath));
                  surveyRadioButtons.ID = String.Format("SurveyRadiobutton-{0}", survey.SurveyID);
                  surveyRadioButtons.Label = survey.Question;
                  surveyRadioButtons.RepeatDirection = (survey.RepeatDirection.HasValue ? survey.RepeatDirection.Value : RepeatDirection.Horizontal);
                  surveyRadioButtons.RepeatColumns = (((survey.RepeatColumns == null) || (survey.RepeatColumns <= 1)) ? 1 : survey.RepeatColumns.Value);
                  surveyRadioButtons.EditUrl = EditUrl("SurveyID", survey.SurveyID.ToString());
                  surveyRadioButtons.IsEditable = IsEditable;
                  surveyRadioButtons.DataSource = surveyOptions;
                  surveyRadioButtons.DataTextField = "OptionName";
                  surveyRadioButtons.DataValueField = "SurveyOptionID";
                  surveyRadioButtons.DataBind();
                  SurveyPlaceHolder.Controls.Add(surveyRadioButtons);
                  break;
               case QuestionType.CheckBoxes:
                  SurveyCheckBoxes surveyCheckBoxes = (SurveyCheckBoxes)LoadControl(String.Format("{0}Controls/SurveyCheckBoxes.ascx", ControlPath));
                  surveyCheckBoxes.ID = String.Format("SurveyCheckbox-{0}", survey.SurveyID);
                  surveyCheckBoxes.Label = survey.Question;
                  surveyCheckBoxes.RepeatDirection = (survey.RepeatDirection.HasValue ? survey.RepeatDirection.Value : RepeatDirection.Horizontal);
                  surveyCheckBoxes.RepeatColumns = (((survey.RepeatColumns == null) || (survey.RepeatColumns <= 1)) ? 1 : survey.RepeatColumns.Value);
                  surveyCheckBoxes.EditUrl = EditUrl("SurveyID", survey.SurveyID.ToString());
                  surveyCheckBoxes.IsEditable = IsEditable;
                  surveyCheckBoxes.DataSource = surveyOptions;
                  surveyCheckBoxes.DataTextField = "OptionName";
                  surveyCheckBoxes.DataValueField = "SurveyOptionID";
                  surveyCheckBoxes.DataBind();
                  SurveyPlaceHolder.Controls.Add(surveyCheckBoxes);
                  break;
               case QuestionType.Text:
                  SurveyText surveyTextBox = (SurveyText)LoadControl(String.Format("{0}Controls/SurveyText.ascx", ControlPath));
                  surveyTextBox.ID = String.Format("SurveyTextBox-{0}", survey.SurveyID);
                  surveyTextBox.Label = survey.Question;
                  surveyTextBox.EditUrl = EditUrl("SurveyID", survey.SurveyID.ToString());
                  surveyTextBox.IsEditable = IsEditable;
                  surveyTextBox.SurveyOptionID = surveyOptions[0].SurveyOptionID;
                  SurveyPlaceHolder.Controls.Add(surveyTextBox);
                  break;
               default:
                  break;
            }
         }

         base.OnLoad(e);
      }

      protected void ViewResultsButton_Click(object sender, EventArgs e)
      {
         Response.Redirect(EditUrl("SurveyResults"), false);
      }

      protected void SubmitSurveyButton_Click(object sender, EventArgs e)
      {
         List<SurveysInfo> surveys = SurveysController.GetAll(ModuleId);
         List<SurveyResultsInfo> surveyResults = new List<SurveyResultsInfo>();

         foreach (SurveysInfo survey in surveys)
         {
            SurveyResultsInfo surveyResult;
            switch (survey.OptionType)
            {
               case QuestionType.RadioButtons:
                  SurveyRadioButtons surveyRadioButtons = (SurveyRadioButtons)FindControl(String.Format("SurveyRadiobutton-{0}", survey.SurveyID));
                  surveyResult = new SurveyResultsInfo();
                  surveyResult.SurveyOptionID = Convert.ToInt32(surveyRadioButtons.SelectedValue);
                  surveyResult.UserID = UserId;
                  surveyResult.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                  surveyResults.Add(surveyResult);
                  break;
               case QuestionType.CheckBoxes:
                  SurveyCheckBoxes surveyCheckBoxes = (SurveyCheckBoxes)FindControl(String.Format("SurveyCheckbox-{0}", survey.SurveyID));
                  foreach (int surveyOptionID in surveyCheckBoxes.SelectedItems)
                  {
                     surveyResult = new SurveyResultsInfo();
                     surveyResult.SurveyOptionID = surveyOptionID;
                     surveyResult.UserID = UserId;
                     surveyResult.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                     surveyResults.Add(surveyResult);
                  }
                  break;
               case QuestionType.Text:
                  SurveyText surveyTextBox = (SurveyText)FindControl(String.Format("SurveyTextBox-{0}", survey.SurveyID));
                  surveyResult = new SurveyResultsInfo();
                  surveyResult.SurveyOptionID = surveyTextBox.SurveyOptionID;
                  surveyResult.UserID = UserId;
                  surveyResult.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                  surveyResult.TextAnswer = surveyTextBox.Text;
                  surveyResults.Add(surveyResult);
                  break;
               default:
                  break;
            }
         }
         SurveyResultsController.Add(surveyResults, SurveyTracking);
      }
   }
}