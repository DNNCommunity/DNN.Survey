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

using DNN.Modules.Survey.Components;
using DNN.Modules.Survey.Components.Controllers;
using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Controls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Client.ClientResourceManagement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey
{
   public partial class SurveyResults : PortalModuleBase
   {
      #region "Private Properties
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

      protected bool HasEditPermission
      {
         get
         {
            return ModulePermissionController.HasModulePermission(ModulePermissionCollection, ModuleSecurity.EDIT_PERMISSION);
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
            {
               return DateTime.MinValue;
            }
            else
            {
               return Convert.ToDateTime(surveyClosingDate);
            }
         }
      }

      protected int ResultsVersion
      {
         // This is: If the results are cleared, this number increases - therefore the cookie changes and users can vote again.
         get
         {
            object resultsVersion = Settings["ResultsVersion"];
            if (resultsVersion == null)
            {
               return 0;
            }
            else
            {
               return Convert.ToInt32(resultsVersion);
            }
         }
      }

      protected SurveyType SurveyType
      {
         get
         {
            object surveyType = Settings["SurveyType"];
            if (surveyType == null)
            {
               return SurveyType.Survey;
            }
            else
            {
               return (SurveyType)Convert.ToInt32(surveyType);
            }
         }
      }
      #endregion

      #region Control Events
      protected override void OnInit(EventArgs e)
      {
         JavaScript.RequestRegistration(CommonJs.DnnPlugins);
         JavaScript.RequestRegistration(CommonJs.jQuery);
         ClientResourceManager.RegisterScript(this.Page, "DesktopModules/Survey/js/Chart.min.js", DotNetNuke.Web.Client.FileOrder.Js.DefaultPriority, DotNetNuke.Web.Client.Providers.DnnFormBottomProvider.DefaultName);
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
            ModuleHelpPanel.Visible = HasEditPermission;
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

            switch (survey.ChartType)
            {
               case ChartType.List:
                  labelBuilder.Append(string.Format("<h3>{0}</h3>", survey.Question));
                  ChartPlaceHolder.Controls.Add(new Label() { Text = labelBuilder.ToString() });
                  dataBuilder.Append("<ul>");
                  foreach (SurveyResultInfo r in result)
                  {
                     dataBuilder.Append(string.Format("<li>{0}{1} ({2})</li>", r.IsCorrect ? string.Format("{0}: ", Localization.GetString("CorrectAnswer.Text", LocalResourceFile)) : string.Empty, ClientAPI.GetSafeJSString(r.OptionName), r.Votes));
                  }
                  dataBuilder.Append("</ul>");
                  ChartPlaceHolder.Controls.Add(new Label() { Text = dataBuilder.ToString() });
                  break;
               case ChartType.Table:
                  labelBuilder.Append(string.Format("<h3>{0}</h3>", survey.Question));
                  ChartPlaceHolder.Controls.Add(new Label() { Text = labelBuilder.ToString() });
                  dataBuilder.Append("<table class=\"dnnGrid surveyResultsTable\"><tbody>");
                  dataBuilder.Append(string.Format("<tr class=\"dnnGridHeader surveyResultsGridHeader\">"));
                  dataBuilder.Append(string.Format("<td>{0}</td><td>{1}</td>", Localization.GetString("Question", LocalResourceFile), Localization.GetString("Answer", LocalResourceFile)));
                  dataBuilder.Append("</tr>");
                  int lineNumber = 1;
                  foreach (SurveyResultInfo r in result)
                  {
                     dataBuilder.Append(string.Format("<tr class=\"{0}\">", lineNumber % 2 == 1 ? "dnnGridItem surveyGridItem" : "dnnGridAltItem surveyGridAltItem"));
                     dataBuilder.Append(string.Format("<td class=\"surveyQuestionsRow\">{0}</td><td class=\"surveyVotesRow\">{1}</td>", r.OptionName, r.Votes));
                     dataBuilder.Append("</tr>");
                     lineNumber++;
                  }
                  dataBuilder.Append("<tbody></table>");
                  ChartPlaceHolder.Controls.Add(new Label() { Text = dataBuilder.ToString() });
                  break;
               default:
                  StringBuilder bgColorsBuilder = new StringBuilder();
                  StringBuilder bColorsBuilder = new StringBuilder();
                  foreach (SurveyResultInfo r in result)
                  {
                     labelBuilder.Append(string.Format("\"{0}\"", String.Format("{0}{1}", ClientAPI.GetSafeJSString(r.OptionName), r.IsCorrect ? string.Format("<span class=\"surveyCorrectAnswer\">{0}<span>", Localization.GetString("CorrectAnswer.Text", LocalResourceFile)) : string.Empty)));
                     dataBuilder.Append(r.Votes);
                     bgColorsBuilder.Append(string.Format("\"{0}\"", Base.GetColor(result.IndexOf(r), (!(r.IsCorrect)))));
                     bColorsBuilder.Append(string.Format("\"{0}\"", r.IsCorrect ? "rgba(0,170,0,1)" : Base.GetColor(result.IndexOf(r), false)));
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
                  break;
            }
         }

         base.OnPreRender(e);
      }

      protected void ViewSurveyButton_Click(object sender, EventArgs e)
      {
         Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), false);
      }
      #endregion
   }
}