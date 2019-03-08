using DNN.Modules.Survey.Components;
using DNN.Modules.Survey.Components.Controllers;
using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Controls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey
{
   public partial class SurveyResults : PortalModuleBase
   {
      private ModulePermissionCollection _modulePermissionCollection = null;
      private SurveysController _surveysController = null;
      private SurveyResultsController _surveyResultsController = null;
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

      protected SurveyResultsController SurveyResultsController
      {
         get
         {
            if (_surveyResultsController == null)
               _surveyResultsController = new SurveyResultsController();
            return _surveyResultsController;
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
      protected bool HasViewResultsPermission
      {
         get
         {
            return ModulePermissionController.HasModulePermission(ModulePermissionCollection, ModuleSecurity.VIEW_RESULTS_PERMISSION);
         }
      }

      protected bool HasParticipatePermission
      {
         get
         {
            return ModulePermissionController.HasModulePermission(ModulePermissionCollection, ModuleSecurity.PARTICIPATE_PERMISSION);
         }
      }

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

      #region Control Events
      protected override void OnInit(EventArgs e)
      {
         JavaScript.RequestRegistration(CommonJs.DnnPlugins);
         JavaScript.RequestRegistration(CommonJs.jQuery);
         JavaScript.RequestRegistration("chartjs");
         _cookie = string.Format("_Module_{0}_Survey_{1}_", ModuleId, ResultsVersion);
         base.OnInit(e);
      }

      protected override void OnLoad(EventArgs e)
      {
         if (HasViewResultsPermission)
         {
            if (IsSurveyExpired)
            {
               SurveyMessageLabel.Text = String.Format(Localization.GetString("SurveyClosed.Text", LocalResourceFile), SurveyClosingDate);
               SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormWarning";
               SurveyMessageLabel.Visible = true;
            }
         }
         else
         {
            SurveyMessageLabel.Text = Localization.GetString("ResultsNotPublicMessage", LocalResourceFile);
            SurveyMessageLabel.CssClass = "dnnFormMessage dnnFormValidationSummary";
            SurveyMessageLabel.Visible = true;
            ResultsPanel.Visible = false;
         }

         if (((!(HasParticipatePermission)) || (IsSurveyExpired) || (HasVoted)) && (!(PortalSecurity.IsInRole("Administrators"))))
         {
            ViewSurveyButton.Visible = false;
         }

         base.OnLoad(e);
      }

      protected override void OnPreRender(EventArgs e)
      {
         List<SurveysInfo> surveys = SurveysController.GetAll(ModuleId);

         foreach (SurveysInfo survey in surveys)
         {
            List<SurveyResultInfo> result = SurveyResultsController.Get(survey.SurveyID);
            StringBuilder labelBuilder = new StringBuilder();
            StringBuilder dataBuilder = new StringBuilder();
            StringBuilder bgColorsBuilder = new StringBuilder();
            StringBuilder bColorsBuilder = new StringBuilder();
            foreach (SurveyResultInfo r in result)
            {
               labelBuilder.Append(string.Format("\"{0}\"", r.OptionName));
               dataBuilder.Append(r.Votes);
               bgColorsBuilder.Append(string.Format("\"{0}\"", Base.GetColor(result.IndexOf(r), true)));
               bColorsBuilder.Append(string.Format("\"{0}\"", Base.GetColor(result.IndexOf(r), false)));
               if (result.IndexOf(r) < result.Count - 1)
               {
                  labelBuilder.Append(",");
                  dataBuilder.Append(",");
                  bgColorsBuilder.Append(",");
                  bColorsBuilder.Append(",");
               }
            }

            CanvasControl canvas = (CanvasControl)LoadControl("Controls/CanvasControl.ascx");
            canvas.Header = survey.Question;
            canvas.Labels = labelBuilder.ToString();
            canvas.Data = dataBuilder.ToString();
            canvas.BackgroundColors = bgColorsBuilder.ToString();
            canvas.BorderColors = bColorsBuilder.ToString();
            canvas.ChartType = survey.ChartType;
            ChartPlaceHolder.Controls.Add(canvas);
         }

         base.OnPreRender(e);
      }

      protected void ViewSurveyButton_Click(object sender, EventArgs e)
      {
         Response.Redirect(Globals.NavigateURL(), false);
      }
      #endregion
   }
}