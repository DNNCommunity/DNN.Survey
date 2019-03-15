using DNN.Modules.Survey.Components;
using DNN.Modules.Survey.Components.Controllers;
using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Controls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey
{
   public partial class SurveyView : PortalModuleBase, IActionable
   {
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
      private SurveyBusinessController _surveyBusinessController = null;

      private string _cookie;

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

      protected SurveyBusinessController SurveyBusinessController
      {
         get
         {
            if (_surveyBusinessController == null)
               _surveyBusinessController = new SurveyBusinessController();
            return _surveyBusinessController;
         }
      }

      protected bool IsSurveyExpired
      {
         get
         {
            return ((SurveyClosingDate != DateTime.MinValue) && (SurveyClosingDate < DateTime.Now));
         }
      }

      protected bool HasVoted
      {
         get
         {
            bool hasVoted;
            if (PortalSecurity.IsInRole("Administrators"))
            {
               // Administrators may always see the survey (and may vote more than once)
               hasVoted = false;
            }
            else
            {
               if ((AuthorizedUsersOnly) || (UserId > 0))
               {
                  hasVoted = (SurveysController.HasVoted(ModuleId, UserId));
               }
               else
               {
                  hasVoted = (Request.Cookies[_cookie] != null);
               }
            }
            return hasVoted;
         }
      }

      protected bool HasViewResultsPermission
      {
         get
         {
            return ModulePermissionController.HasModulePermission(ModulePermissionCollection, ModuleSecurity.VIEW_RESULTS_PERMISSION);
         }
      }

      protected bool HasEditPermission
      {
         get
         {
            return ModulePermissionController.HasModulePermission(ModulePermissionCollection, ModuleSecurity.EDIT_PERMISSION);
         }
      }

      protected int DeleteResultsActionID { get; set; }
      protected int Export2CsvActionID { get; set; }

      protected bool AuthorizedUsersOnly
      {
         get
         {
            bool authorizedUsersOnly = true;
            ModulePermissionCollection permissions = ModulePermissionController.GetModulePermissions(ModuleId, TabId);
            foreach (ModulePermissionInfo permission in permissions)
            {
               if ((permission.PermissionCode == ModuleSecurity.PERMISSION_CODE) && (permission.PermissionKey == ModuleSecurity.PARTICIPATE_PERMISSION) && ((permission.RoleID == -1) || (permission.RoleID == -3)))
               {
                  authorizedUsersOnly = false;
                  break;
               }
            }
            return (authorizedUsersOnly);
         }
      }
      #endregion

      #region Settings
      protected SurveyType SurveyType
      {
         get
         {
            object surveyType = Settings["SurveyType"];
            if (surveyType == null)
               return SurveyType.Survey;
            else
               return (SurveyType)Convert.ToInt32(surveyType);
         }
      }

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

      protected bool PrivacyConfirmation
      {
         get
         {
            object privacyConfirmation = Settings["PrivacyConfirmation"];
            if (privacyConfirmation == null)
               return false;
            else
               return Convert.ToBoolean(privacyConfirmation);
         }
      }

      protected UseCaptcha UseCaptcha
      {
         get
         {
            object useCaptcha = Settings["UseCaptcha"];
            if (useCaptcha == null)
               return UseCaptcha.Never;
            else
               return (UseCaptcha)Convert.ToInt32(useCaptcha);
         }
      }

      protected int ResultsVersion
      {
         // This is: If the results are cleared, this number increases - therefore the cookie changes and users can vote again.
         get
         {
            object resultsVersion = Settings["ResultsVersion"];
            if (resultsVersion == null)
               return 0;
            else
               return Convert.ToInt32(resultsVersion);
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
            actions.Add(GetNextActionID(), Localization.GetString("OrganizeQuestions.Action", LocalResourceFile), ModuleActionType.AddContent, string.Empty, IconController.IconURL("ViewStats"), EditUrl("Organize"), false, SecurityAccessLevel.Edit, true, false);
            if (HasViewResultsPermission)
            {
               // View Results
               actions.Add(GetNextActionID(), Localization.GetString("ViewResults.Action", LocalResourceFile), ModuleActionType.AddContent, string.Empty, IconController.IconURL("View"), EditUrl("SurveyResults"), false, SecurityAccessLevel.View, true, false);
            }
            DeleteResultsActionID = GetNextActionID();
            actions.Add(DeleteResultsActionID, Localization.GetString("DeleteResults.Action", LocalResourceFile), "DeleteResults", "Delete", IconController.IconURL("Delete"), string.Empty, true, SecurityAccessLevel.Edit, true, false);
            //Export2CsvActionID = GetNextActionID();
            //actions.Add(Export2CsvActionID, Localization.GetString("ExportToCsv.Action", LocalResourceFile), "ExportToCsv");
            return actions;
         }
      }
      #endregion

      #region Events
      protected override void OnInit(EventArgs e)
      {
         JavaScript.RequestRegistration(CommonJs.DnnPlugins);
         _cookie = string.Format("_Module_{0}_Survey_{1}_", ModuleId, ResultsVersion);
         AddActionHandler(SurveyActions_Click);
         base.OnInit(e);
      }

      protected override void OnLoad(EventArgs e)
      {
         try
         {
            SubmitSurveyButton.ValidationGroup = string.Format("Survey-{0}-ValidationGroup", ModuleId);
            if (IsSurveyExpired)
            {
               if ((HasViewResultsPermission) && (!(PortalSecurity.IsInRole("Administrators"))))
               {
                  Response.Redirect(EditUrl("SurveyResults"), false);
               }
               else
               {
                  SurveyMessageLabel.Text = string.Format(Localization.GetString("SurveyClosed.Text", LocalResourceFile), SurveyClosingDate);
                  SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormWarning";
                  SubmitSurveyButton.Visible = false;
               }
            }
            else
            {
               if (ShowClosingDateMessage)
               {
                  SurveyMessageLabel.Text = string.Format(Localization.GetString("SurveyWillClose.Text", LocalResourceFile), SurveyClosingDate);
                  SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormInfo";
                  SurveyMessageLabel.Visible = true;
               }

               if ((AuthorizedUsersOnly) && (UserId < 1))
               {
                  SurveyMessageLabel.Text = Localization.GetString("MustBeSignedIn.Text", LocalResourceFile);
                  SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormValidationSummary";
                  SurveyMessageLabel.Visible = true;
                  SubmitSurveyButton.Visible = false;
               }
               else
               {
                  if (HasVoted)
                  {
                     if ((HasViewResultsPermission) && (!(PortalSecurity.IsInRole("Administrators"))))
                     {
                        Response.Redirect(EditUrl("SurveyResults"), false);
                     }
                     else
                     {
                        SurveyMessageLabel.Text = string.Format(Localization.GetString("HasVoted.Text", LocalResourceFile), SurveyClosingDate);
                        SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormSuccess";
                        SurveyMessageLabel.Visible = true;
                        SubmitSurveyButton.Visible = false;
                     }
                  }
                  else
                  {
                     CreateSurveyItems(SurveysController.GetAll(ModuleId));
                  }
               }
            }

            if (HasViewResultsPermission)
               ViewResultsButton.Visible = true;
            else
               ViewResultsButton.Visible = false;

            if ((!(IsEditable)) && (HasEditPermission))
            {
               ExportToCsvButton.Visible = true;
            }
            base.OnLoad(e);
         }
         catch (Exception ex)
         {
            Exceptions.ProcessModuleLoadException(this, ex);
         }
      }

      protected override void OnPreRender(EventArgs e)
      {
         if (IsEditable)
         {
            StringBuilder confirmDeleteScriptBuilder = new StringBuilder();
            confirmDeleteScriptBuilder.Append("$(document).ready(function() {\r\n");
            confirmDeleteScriptBuilder.Append(string.Format("   var deleteLink = $(\"a[href='javascript: __doPostBack(\\\\\\\'dnn$ctr{0}$ModuleActions$actionButton\\\\\\\', \\\\\\\'{1}\\\\\\\')']\");\r\n", ModuleId, DeleteResultsActionID));
            confirmDeleteScriptBuilder.Append("   deleteLink.dnnConfirm({\r\n");
            confirmDeleteScriptBuilder.Append(string.Format("      text: \"{0}\",\r\n", Localization.GetString("ConfirmResultsDelete.Text", LocalResourceFile)));
            confirmDeleteScriptBuilder.Append(string.Format("      yesText: \"{0}\",\r\n", Localization.GetString("Yes.Text")));
            confirmDeleteScriptBuilder.Append(string.Format("      noText: \"{0}\",\r\n", Localization.GetString("No.Text")));
            confirmDeleteScriptBuilder.Append(string.Format("      title: \"{0}\"\r\n", Localization.GetString("DeleteResults.Action", LocalResourceFile)));
            confirmDeleteScriptBuilder.Append("   });\r\n");
            confirmDeleteScriptBuilder.Append("});\r\n");

            if (!(Page.ClientScript.IsStartupScriptRegistered("ConfirmDeleteScript")))
               Page.ClientScript.RegisterStartupScript(GetType(), "ConfirmDeleteScript", confirmDeleteScriptBuilder.ToString(), true);
         }
         base.OnPreRender(e);
      }

      protected void ViewResultsButton_Click(object sender, EventArgs e)
      {
         Response.Redirect(EditUrl("SurveyResults"), false);
      }

      protected void SubmitSurveyButton_Click(object sender, EventArgs e)
      {
         // First, check CAPTCHA
         CaptchaControl captcha = (CaptchaControl)FindControl(string.Format("Captcha_{0}", ModuleId));
         if (((captcha != null) && (captcha.IsValid)) || (captcha == null))
         {
            // Then validate page...
            Page.Validate(string.Format("Survey_{0}_ValidationGroup", ModuleId));
            if (Page.IsValid)
            {
               if (ContactByFaxOnlyCheckBox.Checked)
               {
                  // if someone activates this checkbox send him home :-)
                  Response.Redirect("http://localhost/");
               }
               List<SurveysInfo> surveys = SurveysController.GetAll(ModuleId);
               List<SurveyResultsInfo> surveyResults = new List<SurveyResultsInfo>();

               Guid resultUserID = Guid.NewGuid();

               foreach (SurveysInfo survey in surveys)
               {
                  SurveyResultsInfo surveyResult;
                  switch (survey.OptionType)
                  {
                     case QuestionType.RadioButtons:
                        SurveyRadioButtons surveyRadioButtons = (SurveyRadioButtons)FindControl(string.Format("SurveyRadiobutton_{0}", survey.SurveyID));
                        surveyResult = new SurveyResultsInfo();
                        surveyResult.SurveyOptionID = Convert.ToInt32(surveyRadioButtons.SelectedValue);
                        surveyResult.UserID = (UserId < 1 ? (int?)null : UserId);
                        surveyResult.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                        surveyResult.IsCorrect = SurveyOptionsController.GetAll(survey.SurveyID).Find(x => x.SurveyOptionID == surveyResult.SurveyOptionID).IsCorrect;
                        surveyResult.ResultUserID = resultUserID;
                        surveyResults.Add(surveyResult);
                        break;
                     case QuestionType.CheckBoxes:
                        SurveyCheckBoxes surveyCheckBoxes = (SurveyCheckBoxes)FindControl(string.Format("SurveyCheckbox_{0}", survey.SurveyID));
                        foreach (int surveyOptionID in surveyCheckBoxes.SelectedItems)
                        {
                           surveyResult = new SurveyResultsInfo();
                           surveyResult.SurveyOptionID = surveyOptionID;
                           surveyResult.UserID = (UserId < 1 ? (int?)null : UserId);
                           surveyResult.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                           surveyResult.IsCorrect = SurveyOptionsController.GetAll(survey.SurveyID).Find(x => x.SurveyOptionID == surveyResult.SurveyOptionID).IsCorrect;
                           surveyResult.ResultUserID = resultUserID;
                           surveyResults.Add(surveyResult);
                        }
                        break;
                     case QuestionType.Text:
                        SurveyText surveyTextBox = (SurveyText)FindControl(string.Format("SurveyTextBox_{0}", survey.SurveyID));
                        surveyResult = new SurveyResultsInfo();
                        surveyResult.SurveyOptionID = surveyTextBox.SurveyOptionID;
                        surveyResult.UserID = (UserId < 1 ? (int?)null : UserId);
                        surveyResult.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                        surveyResult.TextAnswer = surveyTextBox.Text;
                        surveyResult.IsCorrect = true;
                        surveyResult.ResultUserID = resultUserID;
                        surveyResults.Add(surveyResult);
                        break;
                     default:
                        break;
                  }
               }
               if (PortalSecurity.IsInRole("Administrators"))
               {
                  // This is just to force the SQL Script SurveyResults_Add to add the result if the user is an administrator
                  SurveyResultsController.Add(surveyResults, false);
               }
               else
               {
                  SurveyResultsController.Add(surveyResults, AuthorizedUsersOnly);
               }
               HttpCookie cookie = new HttpCookie(_cookie);
               cookie.Value = "True";
               cookie.Expires = (SurveyClosingDate == DateTime.MinValue ? DateTime.MaxValue : SurveyClosingDate.AddDays(1));
               Response.AppendCookie(cookie);
               SubmitSurveyButton.Visible = false;
               if (SurveyType == SurveyType.Survey)
               {
                  SurveyPlaceHolder.Visible = false;
                  if (HasViewResultsPermission)
                  {
                     Response.Redirect(EditUrl("SurveyResults"), false);
                  }
                  else
                  {
                     SurveyMessageLabel.Text = Localization.GetString("HasVoted.Text", LocalResourceFile);
                     SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormSuccess";
                     SurveyMessageLabel.Visible = true;
                  }
               }
               else
               {
                  SurveyMessageLabel.Text = Localization.GetString("QuizResults.Text", LocalResourceFile);
                  SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormSuccess";
                  SurveyMessageLabel.Visible = true;
                  SurveyPlaceHolder.Controls.Clear();
                  DisplayQuizResults(surveys, surveyResults);
               }
            }
         }
      }

      protected void SubmitSurveyButton_Load(object sender, EventArgs e)
      {
         LinkButton submitSurveyButton = (LinkButton)sender;
         submitSurveyButton.ValidationGroup = string.Format("Survey_{0}_ValidationGroup", ModuleId);
      }

      protected void ContactByFaxOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         // if someone activates this checkbox send him home :-)
         Response.Redirect("http://localhost/", true);
      }
      #endregion

      #region Private Methods
      private void CreateSurveyItems(List<SurveysInfo> surveys)
      {
         foreach (SurveysInfo survey in surveys)
         {
            List<SurveyOptionsInfo> surveyOptions = SurveyOptionsController.GetAll(survey.SurveyID);
            switch (survey.OptionType)
            {
               case QuestionType.RadioButtons:
                  SurveyRadioButtons surveyRadioButtons = (SurveyRadioButtons)LoadControl(string.Format("{0}Controls/SurveyRadioButtons.ascx", ControlPath));
                  surveyRadioButtons.ID = string.Format("SurveyRadiobutton_{0}", survey.SurveyID);
                  surveyRadioButtons.Label = survey.Question;
                  surveyRadioButtons.RepeatDirection = (survey.RepeatDirection.HasValue ? survey.RepeatDirection.Value : RepeatDirection.Horizontal);
                  surveyRadioButtons.RepeatColumns = (((survey.RepeatColumns == null) || (survey.RepeatColumns <= 1)) ? 1 : survey.RepeatColumns.Value);
                  surveyRadioButtons.EditUrl = EditUrl("SurveyID", survey.SurveyID.ToString());
                  surveyRadioButtons.IsEditable = IsEditable;
                  surveyRadioButtons.ErrorMessage = string.Format(Localization.GetString("RadioButtonRequired.ErrorMessage", LocalResourceFile), survey.Question);
                  surveyRadioButtons.ValidationGroup = string.Format("Survey_{0}_ValidationGroup", ModuleId);
                  surveyRadioButtons.DataSource = surveyOptions;
                  surveyRadioButtons.DataTextField = "OptionName";
                  surveyRadioButtons.DataValueField = "SurveyOptionID";
                  surveyRadioButtons.DataBind();
                  SurveyPlaceHolder.Controls.Add(surveyRadioButtons);
                  break;
               case QuestionType.CheckBoxes:
                  SurveyCheckBoxes surveyCheckBoxes = (SurveyCheckBoxes)LoadControl(string.Format("{0}Controls/SurveyCheckBoxes.ascx", ControlPath));
                  surveyCheckBoxes.ID = string.Format("SurveyCheckbox_{0}", survey.SurveyID);
                  surveyCheckBoxes.Label = survey.Question;
                  surveyCheckBoxes.RepeatDirection = (survey.RepeatDirection.HasValue ? survey.RepeatDirection.Value : RepeatDirection.Horizontal);
                  surveyCheckBoxes.RepeatColumns = (((survey.RepeatColumns == null) || (survey.RepeatColumns <= 1)) ? 1 : survey.RepeatColumns.Value);
                  surveyCheckBoxes.EditUrl = EditUrl("SurveyID", survey.SurveyID.ToString());
                  surveyCheckBoxes.IsEditable = IsEditable;
                  surveyCheckBoxes.ErrorMessage = string.Format(Localization.GetString("CheckBoxRequired.ErrorMessage", LocalResourceFile), survey.Question);
                  surveyCheckBoxes.ValidationGroup = string.Format("Survey_{0}_ValidationGroup", ModuleId);
                  surveyCheckBoxes.DataSource = surveyOptions;
                  surveyCheckBoxes.DataTextField = "OptionName";
                  surveyCheckBoxes.DataValueField = "SurveyOptionID";
                  surveyCheckBoxes.DataBind();
                  SurveyPlaceHolder.Controls.Add(surveyCheckBoxes);
                  break;
               case QuestionType.Text:
                  SurveyText surveyTextBox = (SurveyText)LoadControl(string.Format("{0}Controls/SurveyText.ascx", ControlPath));
                  surveyTextBox.ID = string.Format("SurveyTextBox_{0}", survey.SurveyID);
                  surveyTextBox.Label = survey.Question;
                  surveyTextBox.NumberOfRows = (((survey.NumberOfRows.HasValue) && (survey.NumberOfRows.Value > 1)) ? survey.NumberOfRows.Value : 1);
                  surveyTextBox.EditUrl = EditUrl("SurveyID", survey.SurveyID.ToString());
                  surveyTextBox.IsEditable = IsEditable;
                  surveyTextBox.ErrorMessage = string.Format(Localization.GetString("TextBoxRequired.ErrorMessage", LocalResourceFile), survey.Question);
                  surveyTextBox.ValidationGroup = string.Format("Survey_{0}_ValidationGroup", ModuleId);
                  surveyTextBox.SurveyOptionID = surveyOptions[0].SurveyOptionID;
                  SurveyPlaceHolder.Controls.Add(surveyTextBox);
                  break;
               default:
                  break;
            }
         }

         if (PrivacyConfirmation)
         {
            // This is DNN 9.2.2 code...
            string privacyUrl = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Privacy");
            string termsUrl = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Terms");
            // For DNN 9.3.0 use this code then...
            //string privacyUrl = (PortalSettings.PrivacyTabId == Null.NullInteger ? Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Privacy") : Globals.NavigateURL(PortalSettings.PrivacyTabId));
            //string termsUrl = (PortalSettings.TermsTabId == Null.NullInteger ? Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Terms") : Globals.NavigateURL(PortalSettings.TermsTabId));

            PrivacyConfirmationCheckBox privacyConfirmation = (PrivacyConfirmationCheckBox)LoadControl(string.Format("{0}Controls/PrivacyConfirmationCheckBox.ascx", ControlPath));
            privacyConfirmation.ID = string.Format("PrivacyConfirmationCheckBox_{0}", ModuleId);
            privacyConfirmation.Label = string.Format(Localization.GetString("PrivacyConfirmation.Text", LocalResourceFile), privacyUrl, termsUrl);
            privacyConfirmation.ErrorMessage = Localization.GetString("PrivacyConfirmation.ErrorMessage", LocalResourceFile);
            privacyConfirmation.ValidationGroup = string.Format("Survey_{0}_ValidationGroup", ModuleId);
            SurveyPlaceHolder.Controls.Add(privacyConfirmation);
         }

         if ((UseCaptcha == UseCaptcha.Always) || ((UseCaptcha == UseCaptcha.UnauthorizedUsersOnly) && (UserId < 1)))
         {
            CaptchaControl captcha = new CaptchaControl();
            captcha.ID = string.Format("Captcha_{0}", ModuleId);
            captcha.Text = Localization.GetString("Captcha.Text", LocalResourceFile);
            captcha.CaptchaLength = 8;
            captcha.ErrorMessage = Localization.GetString("Captcha.ErrorMessage", LocalResourceFile);
            captcha.CaptchaChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
            captcha.ErrorStyle.CssClass = "dnnFormMessage dnnFormError";
            SurveyPlaceHolder.Controls.Add(captcha);
         }
      }

      private void DisplayQuizResults(List<SurveysInfo> surveys, List<SurveyResultsInfo> surveyResults)
      {
         int score = 0;
         List<SurveysInfo> quizQuestions = surveys.FindAll(s => (!(s.IsStatistical.Value)));
         List<SurveysInfo> statisticalQuestions = surveys.FindAll(s => s.IsStatistical.Value);

         foreach (SurveysInfo survey in quizQuestions)
         {
            SurveyPlaceHolder.Controls.Add(new LiteralControl(string.Format("<h2>{0}</h2>", survey.Question)));
            List<SurveyOptionsInfo> answers = SurveyOptionsController.GetAll(survey.SurveyID);
            int correctAnswersCount = answers.FindAll(a => a.IsCorrect).Count;
            StringBuilder yourAnswers = new StringBuilder();
            StringBuilder correctAnswers = new StringBuilder();
            int yourAnswersCount = 0;
            int yourCorrectAnswers = 0;
            List<SurveyResultsInfo> yourAnswersList = new List<SurveyResultsInfo>();
            foreach (SurveyOptionsInfo answer in answers)
            {
               yourAnswersList = surveyResults.FindAll(r => r.SurveyOptionID == answer.SurveyOptionID);
               yourAnswersCount += yourAnswersList.Count;
               foreach (SurveyResultsInfo yourAnswer in yourAnswersList)
               {
                  if (yourAnswer.IsCorrect)
                  {
                     yourCorrectAnswers++;
                     yourAnswers.Append(string.Format("<img src=\"{0}images/correct.png\" class=\"surveyImageLeft\" />{1}<br />", ControlPath, answer.OptionName));
                  }
                  else
                  {
                     yourAnswers.Append(string.Format("<img src=\"{0}images/not-correct.png\" class=\"surveyImageLeft\" />{1}<br />", ControlPath, answer.OptionName));
                  }
               }
               if (answer.IsCorrect)
               {
                  correctAnswers.Append(string.Format("<img src=\"{0}images/correct.png\" class=\"surveyImageLeft\" />{1}<br />", ControlPath, answer.OptionName));
               }
            }
            string answerClass = (yourCorrectAnswers == correctAnswersCount ? "dnnFormSuccess" : "dnnFormValidationSummary");
            SurveyPlaceHolder.Controls.Add(new LiteralControl(string.Format("<h3>{0}:</h3><div class=\"dnnFormMessage {1}\">{2}</div>", Localization.GetString((yourAnswersCount == 1 ? "YourAnswer.Text" : "YourAnswers.Text"), LocalResourceFile), answerClass, yourAnswers.Remove(yourAnswers.Length - 6, 6).ToString())));
            if ((yourCorrectAnswers < correctAnswersCount))
            {
               SurveyPlaceHolder.Controls.Add(new LiteralControl(string.Format("<h4>{0}:</h4><div class=\"dnnFormMessage dnnFrormSuccess\">{1}</div>", Localization.GetString((correctAnswersCount == 1 ? "CorrectAnswer.Text" : "CorrectAnswers.Text"), LocalResourceFile), correctAnswers.Remove(correctAnswers.Length - 6, 6).ToString())));
            }
            else
            {
               score++;
            }
         }
         string scoreClass = (score == surveys.Count ? "dnnFormSuccess" : (score == 0 ? "dnnFormValidationSummary" : "dnnFormWarning"));
         SurveyPlaceHolder.Controls.Add(new LiteralControl(string.Format("<div class=\"dnnFormMessage {0} surveyQuizResult\">{1}: {2}/{3} - {4:0.00}%</div>", scoreClass, Localization.GetString("YourResult.Text", LocalResourceFile), score, quizQuestions.Count, Convert.ToDouble(score) / Convert.ToDouble(quizQuestions.Count) * 100.00)));

         if (statisticalQuestions.Count > 0)
         {
            StringBuilder yourStatisticalAnswers = new StringBuilder();
            yourStatisticalAnswers.Append(string.Format("<h2>{0}</h2>", Localization.GetString("StatisticalAnswers.Text", LocalResourceFile)));
            foreach (SurveysInfo survey in statisticalQuestions)
            {
               yourStatisticalAnswers.Append(string.Format("<div class=\"dnnFormMessage dnnFormInfo\"><h3>{0}</h3>", survey.Question));
               if ((survey.OptionType == QuestionType.RadioButtons) || (survey.OptionType == QuestionType.CheckBoxes))
               {
                  List<SurveyOptionsInfo> answers = SurveyOptionsController.GetAll(survey.SurveyID);
                  List<SurveyResultsInfo> yourAnswersList = new List<SurveyResultsInfo>();
                  foreach (SurveyOptionsInfo answer in answers)
                  {
                     yourAnswersList = surveyResults.FindAll(r => r.SurveyOptionID == answer.SurveyOptionID);
                     foreach (SurveyResultsInfo yourAnswer in yourAnswersList)
                     {
                        yourStatisticalAnswers.Append(string.Format("<img src =\"{0}images/info.png\" class=\"surveyImageLeft\" />{1}<br />", ControlPath, answer.OptionName));
                     }
                  }
               }
               else
               {
                  yourStatisticalAnswers.Append(string.Format("<img src =\"{0}images/info.png\" class=\"surveyImageLeft\" />{1}<br />", ControlPath, surveyResults[0].TextAnswer));
               }
               yourStatisticalAnswers.Append("</div>");
            }
            SurveyPlaceHolder.Controls.Add(new LiteralControl(yourStatisticalAnswers.ToString()));
         }
      }

      private void SurveyActions_Click(object sender, ActionEventArgs e)
      {
         switch (e.Action.CommandName)
         {
            case "DeleteResults":
               if (e.Action.CommandArgument == "Delete")
               {
                  SurveyResultsController.DropAll(ModuleId);
                  ModuleController.Instance.UpdateModuleSetting(ModuleId, "ResultsVersion", (ResultsVersion + 1).ToString());
               }
               break;
            default:
               break;
         }
      }
      #endregion

      protected void ExportToCsvButton_Click(object sender, EventArgs e)
      {
         string csv = SurveyBusinessController.CSVExport(ModuleId, Localization.SharedResourceFile);
         Response.Clear();
         Response.Charset = string.Empty;
         Response.AddHeader("content-disposition", string.Format("attachment; filename=Survey_Results_{0}_{1:yyyy-MM-dd-hhmmss}.csv", ModuleId, DateTime.Now));
         Response.ContentType = "application/octet-stream";
         Response.Cache.SetCacheability(HttpCacheability.NoCache);
         StringWriter stringWriter = new StringWriter();
         HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter);
         stringWriter.Write(csv);
         Response.Write(stringWriter.ToString());
         Response.End();
      }
   }
}