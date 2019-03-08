using DNN.Modules.Survey.Components;
using DNN.Modules.Survey.Components.Controllers;
using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Controls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Permissions;
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

      #region Control Events
      protected override void OnInit(EventArgs e)
      {
         JavaScript.RequestRegistration(CommonJs.DnnPlugins);
         JavaScript.RequestRegistration(CommonJs.jQuery);
         JavaScript.RequestRegistration("chartjs");
         base.OnInit(e);
      }

      protected override void OnLoad(EventArgs e)
      {
         if (ModulePermissionController.HasModulePermission(ModulePermissionCollection, ModuleSecurity.VIEW_RESULTS_PERMISSION))
         {
            ResultsNotPublicPanel.Visible = false;
            ResultsPanel.Visible = true;
         }
         else
         {
            ResultsNotPublicPanel.Visible = true;
            ResultsPanel.Visible = false;
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