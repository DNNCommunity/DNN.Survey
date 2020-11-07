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
using DNN.Modules.Survey.Components.Providers;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey
{
   public partial class SurveyOrganize : PortalModuleBase
   {
      private SurveysController _surveysController = null;

      protected SurveysController SurveysController
      {
         get
         {
            if (_surveysController == null)
               _surveysController = new SurveysController();
            return _surveysController;
         }
      }

      protected List<SurveysInfo> Surveys
      {
         get
         {
            string surveysXml;
            if (ViewState["SurveysXml"] == null)
            {
               surveysXml = String.Empty;
            }
            else
            {
               surveysXml = ViewState["SurveysXml"].ToString();
            }
            return XmlDataProvider.SurveysFromXml(surveysXml);
         }
         set
         {
            ViewState["SurveysXml"] = XmlDataProvider.SurveysToXml(value);
         }
      }

      protected override void OnInit(EventArgs e)
      {
         ClientResourceManager.RegisterStyleSheet(Page, string.Format("{0}Module.css", ControlPath));
         ScriptManager.GetCurrent(Page).RegisterPostBackControl(UpdateButton);
         ScriptManager.GetCurrent(Page).RegisterPostBackControl(CancelButton);
         base.OnInit(e);
      }

      protected override void OnLoad(EventArgs e)
      {
         if (!Page.IsPostBack)
         {
            Surveys = SurveysController.GetAll(ModuleId);
            QuestionsGrid.DataSource = Surveys;
            QuestionsGrid.DataBind();
         }
         base.OnLoad(e);
      }

      protected override void OnPreRender(EventArgs e)
      {
         StringBuilder scriptBuilder = new StringBuilder();
         scriptBuilder.Append("$(function() {\r\n");
         scriptBuilder.Append("   $(\"#" + QuestionsGrid.ClientID + "\").sortable({\r\n");
         scriptBuilder.Append("      items: \"tr:not(tr:first-child)\",\r\n");
         scriptBuilder.Append("      cursor: \"move\",\r\n");
         scriptBuilder.Append("      axis: \"y\",\r\n");
         scriptBuilder.Append("      dropOnEmpty: false,\r\n");
         scriptBuilder.Append("      handle: \".SurveyHandle\",\r\n");
         scriptBuilder.Append("      update: function(e, ui){\r\n");
         scriptBuilder.Append("         $(this).find(\"tr\").each(function(index) {\r\n");
         scriptBuilder.Append("            if (index > 0){\r\n");
         scriptBuilder.Append("               if (index % 2 == 1) {\r\n");
         scriptBuilder.Append("                  $(this).removeClass(\"dnnGridItem\");\r\n");
         scriptBuilder.Append("                  $(this).removeClass(\"dnnGridAltItem\");\r\n");
         scriptBuilder.Append("                  $(this).addClass(\"dnnGridItem\");\r\n");
         scriptBuilder.Append("               } else {\r\n");
         scriptBuilder.Append("                  $(this).removeClass(\"dnnGridItem\");\r\n");
         scriptBuilder.Append("                  $(this).removeClass(\"dnnGridAltItem\");\r\n");
         scriptBuilder.Append("                  $(this).addClass(\"dnnGridAltItem\");\r\n");
         scriptBuilder.Append("               }\r\n");
         scriptBuilder.Append("            }\r\n");
         scriptBuilder.Append("         });\r\n");
         scriptBuilder.Append("      },\r\n");
         scriptBuilder.Append("      receive: function(e, ui) {\r\n");
         scriptBuilder.Append("         $(this).find(\"tbody\").append(ui.item);\r\n");
         scriptBuilder.Append("      }\r\n");
         scriptBuilder.Append("   });\r\n");
         scriptBuilder.Append("});\r\n");

         if (!(Page.ClientScript.IsStartupScriptRegistered("QuestionsGridRowDragging")))
            Page.ClientScript.RegisterStartupScript(GetType(), "QuestionsGridRowDragging", scriptBuilder.ToString(), true);

         base.OnPreRender(e);
      }

      protected void QuestionsGrid_Init(object sender, EventArgs e)
      {
         Localization.LocalizeDataGrid(ref QuestionsGrid, LocalResourceFile);
      }

      protected void QuestionsGrid_ItemCreated(object sender, DataGridItemEventArgs e)
      {
         DataGrid questionGrid = (DataGrid)sender;
         if (!(Page.IsPostBack))
         {
            SurveysInfo si = (SurveysInfo)e.Item.DataItem;
            switch (e.Item.ItemType)
            {
               case ListItemType.Item:
               case ListItemType.AlternatingItem:
                  Label questionTypeLabel = (Label)e.Item.FindControl("QuestionTypeLabel");
                  questionTypeLabel.Text = Localization.GetString(String.Format("QuestionType.{0}.Text", Enum.GetName(typeof(QuestionType), si.OptionType)), LocalResourceFile);
                  break;
               default:
                  break;
            }
         }
      }

      protected void DeleteImage_Click(object sender, EventArgs e)
      {
         DnnImageButton deleteImage = (DnnImageButton)sender;
         int surveyID = Convert.ToInt32(deleteImage.CommandArgument);

         int[] surveyIDs = (from p in Request.Form["SurveyID"].Split(',')
                            select int.Parse(p)).ToArray();
         int viewOrder = 1;
         List<SurveysInfo> surveys = Surveys;
         SurveysInfo survey;

         foreach (int sID in surveyIDs)
         {
            survey = surveys.Find(x => x.SurveyID == sID);
            if (sID == surveyID)
            {
               surveys.Remove(survey);
            }
            else
            {
               survey.ViewOrder = viewOrder;
               survey.LastModifiedByUserID = UserId;
               viewOrder++;
            }
         }
         Surveys = surveys;
         QuestionsGrid.DataSource = Surveys;
         QuestionsGrid.DataBind();
      }

      protected void CancelButton_Click(object sender, EventArgs e)
      {
         Response.Redirect(Globals.NavigateURL(), false);
      }

      protected void UpdateButton_Click(object sender, EventArgs e)
      {
         SurveysInfo survey;
         try
         {
            List<SurveysInfo> surveys;
            if (Request.Form["SurveyID"] == null)
            {
               surveys = new List<SurveysInfo>();
            }
            else
            {
               surveys = Surveys;
               int[] surveyIDs = (from p in Request.Form["SurveyID"].Split(',')
                                  select int.Parse(p)).ToArray();
               int viewOrder = 1;
               foreach (int surveyID in surveyIDs)
               {
                  survey = surveys.Find(x => x.SurveyID == surveyID);
                  survey.ViewOrder = viewOrder;
                  survey.LastModifiedByUserID = UserId;
                  viewOrder++;
               }
            }
            SurveysController.Sort(surveys);
            Response.Redirect(Globals.NavigateURL(), false);
         }
         catch (Exception ex)
         {
            Exceptions.ProcessModuleLoadException(this, ex);
         }
      }
   }
}