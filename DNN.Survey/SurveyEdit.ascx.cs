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
using DotNetNuke.Framework.JavaScriptLibraries;
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
   public partial class SurveyEdit : PortalModuleBase
   {
      #region Private Properties
      private SurveysController _surveysController = null;
      private SurveyOptionsController _surveyOptionsController = null;

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

      protected int SurveyID
      {
         get
         {
            int surveyID;
            if (ViewState["SurveyID"] == null)
            {
               if (Request.Params["SurveyID"] == null)
               {
                  surveyID = 0;
               }
               else
               {
                  if (!(Int32.TryParse(Request.Params["SurveyID"], out surveyID)))
                  {
                     surveyID = 0;
                  }
               }
               ViewState["SurveyID"] = surveyID;
            }
            else
            {
               if(!(Int32.TryParse(ViewState["SurveyID"].ToString(), out surveyID)))
               {
                  surveyID = 0;
               }
            }
            return surveyID;
         }
      }

      protected int SurveyOptionID
      {
         get
         {
            int surveyOptionID;
            if (ViewState["SurveyOptionID"] == null)
            {
               surveyOptionID = 0;
            }
            else
            {
               if (!(Int32.TryParse(ViewState["SurveyOptionID"].ToString(), out surveyOptionID)))
               {
                  surveyOptionID = 0;
               }
            }
            return surveyOptionID;
         }
         set
         {
            ViewState["SurveyOptionID"] = value;
         }
      }

      protected int MaxViewOrder
      {
         get
         {
            int maxViewOrder;
            if (ViewState["MaxViewOrder"] == null)
            {
               maxViewOrder = 0;
            }
            else
            {
               if (!(Int32.TryParse(ViewState["MaxViewOrder"].ToString(), out maxViewOrder)))
               {
                  maxViewOrder = 0;
               }
            }
            return maxViewOrder;
         }
         set
         {
            ViewState["MaxViewOrder"] = value;
         }
      }

      protected SurveysInfo Survey
      {
         get
         {
            string surveyXml;
            if (ViewState["SurveyXml"] == null)
            {
               surveyXml = String.Empty;
            }
            else
            {
               surveyXml = ViewState["SurveyXml"].ToString();
            }
            return XmlDataProvider.SurveyFromXml(surveyXml);
         }
         set
         {
            ViewState["SurveyXml"] = XmlDataProvider.SurveyToXml(value);
         }
      }

      protected SurveyOptionsInfo SurveyOption
      {
         get
         {
            string surveyOptionXml;
            if (ViewState["SurveyOptionXml"] == null)
            {
               surveyOptionXml = String.Empty;
            }
            else
            {
               surveyOptionXml = ViewState["SurveyOptionXml"].ToString();
            }
            return XmlDataProvider.SurveyOptionFromXml(surveyOptionXml);
         }
         set
         {
            ViewState["SurveyOptionXml"] = XmlDataProvider.SurveyOptionToXml(value);
         }
      }

      protected List<SurveyOptionsInfo> Answers
      {
         get
         {
            string answersXml;
            if (ViewState["AnswersXml"] == null)
            {
               answersXml = String.Empty;
            }
            else
            {
               answersXml = ViewState["AnswersXml"].ToString();
            }
            return XmlDataProvider.SurveyOptionsFromXml(answersXml);
         }
         set
         {
            value.Sort(delegate (SurveyOptionsInfo a1, SurveyOptionsInfo a2)
            {
               return a1.ViewOrder.CompareTo(a2.ViewOrder);
            });
            ViewState["AnswersXml"] = XmlDataProvider.SurveyOptionsToXml(value);
         }
      }

      protected bool ChartTypeChanged
      {
         get
         {
            bool chartTypeChanged;
            if (ViewState["ChartTypeChanged"] == null)
               chartTypeChanged = false;
            else
               chartTypeChanged = Convert.ToBoolean(ViewState["ChartTypeChanged"]);
            return chartTypeChanged;
         }
         set
         {
            ViewState["ChartTypeChanged"] = value;
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
      #endregion

      #region Events
      protected override void OnInit(EventArgs e)
      {
         JavaScript.RequestRegistration(CommonJs.DnnPlugins);
         JavaScript.RequestRegistration(CommonJs.jQuery);
         JavaScript.RequestRegistration(CommonJs.jQueryUI);
         ClientResourceManager.RegisterStyleSheet(Page, string.Format("{0}Module.css", ControlPath));

         ScriptManager.GetCurrent(Page).RegisterPostBackControl(UpdateButton);
         ScriptManager.GetCurrent(Page).RegisterPostBackControl(CancelButton);
         ScriptManager.GetCurrent(Page).RegisterPostBackControl(AddAnswerButton);
         ScriptManager.GetCurrent(Page).RegisterPostBackControl(UpdateAnswerButton);
         base.OnInit(e);
      }

      protected override void OnLoad(EventArgs e)
      {
         if(!(Page.IsPostBack))
         {
            CreateQuestionTypeEntries();
            CreateRepeatDirectionEntries();
            CreateChartTypeEntries();

            CorrectAnswerPanel.Visible = (SurveyType == SurveyType.Quiz);

            if (SurveyID > 0)
            {
               Survey = SurveysController.Get(SurveyID);
               Answers = SurveyOptionsController.GetAll(SurveyID);
               QuestionTextBox.Text = Survey.Question;
               QuestionTypeDropDownList.SelectedValue = Convert.ToInt32(Survey.OptionType).ToString();
               ChartTypeDropDownList.SelectedValue = Convert.ToInt32(Survey.ChartType).ToString();
               IsStatisticalCheckBox.Checked = (Survey.IsStatistical.HasValue ? Survey.IsStatistical.Value : false);
               if (Survey.OptionType == QuestionType.RadioButtons || Survey.OptionType == QuestionType.CheckBoxes)
               {
                  RepeatDirectionDropDownList.SelectedValue = Convert.ToInt32(Survey.RepeatDirection).ToString();
                  RepeatColumnsTextBox.Text = Survey.RepeatColumns.ToString();
               }
               else
               {
                  NumberOfRowsTextBox.Text = Survey.NumberOfRows.ToString();
                  IsStatisticalCheckBox.Enabled = false;
                  RepeatDirectionPanel.Visible = false;
                  AnswersPanel.Visible = false;
                  TextAnswerPanel.Visible = true;
               }
               AnswersGrid.DataSource = Answers;
               AnswersGrid.DataBind();
               MaxViewOrder = Answers.Count;
               ChartTypeChanged = true;
            }
            if (SurveyType == SurveyType.Quiz)
            {
               IsStatisticalPanel.Visible = true;
            }
         }
      }

      protected override void OnPreRender(EventArgs e)
      {
         StringBuilder scriptBuilder = new StringBuilder();
         scriptBuilder.Append("$(function() {\r\n");
         scriptBuilder.Append("   $(\"#" + AnswersGrid.ClientID + "\").sortable({\r\n");
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

         if (!(Page.ClientScript.IsStartupScriptRegistered("AnswersGridRowDragging")))
            Page.ClientScript.RegisterStartupScript(GetType(), "AnswersGridRowDragging", scriptBuilder.ToString(), true);
      }

      protected void CancelButton_Click(object sender, EventArgs e)
      {
         Response.Redirect(Globals.NavigateURL(), false);
      }

      private void CreateQuestionTypeEntries()
      {
         string[] questionTypeNames = Enum.GetNames(typeof(QuestionType));
         Dictionary<int, string> questionTypeEntries = new Dictionary<int, string>();
         for (int i = 0; i < questionTypeNames.Length; i++)
            questionTypeEntries.Add(Convert.ToInt32(Enum.GetValues(typeof(QuestionType)).GetValue(i)), Localization.GetString(String.Format("QuestionType.{0}.Text", questionTypeNames[i]), LocalResourceFile));
         QuestionTypeDropDownList.DataSource = questionTypeEntries;
         QuestionTypeDropDownList.DataTextField = "Value";
         QuestionTypeDropDownList.DataValueField = "Key";
         QuestionTypeDropDownList.DataBind();
      }

      private void CreateRepeatDirectionEntries()
      {
         string[] repeatDirectionNames = Enum.GetNames(typeof(RepeatDirection));
         Dictionary<int, string> repeatDirectionEntries = new Dictionary<int, string>();
         for (int i = 0; i < repeatDirectionNames.Length; i++)
            repeatDirectionEntries.Add(Convert.ToInt32(Enum.GetValues(typeof(RepeatDirection)).GetValue(i)), Localization.GetString(String.Format("RepeatDirection.{0}.Text", repeatDirectionNames[i]), LocalResourceFile));
         RepeatDirectionDropDownList.DataSource = repeatDirectionEntries;
         RepeatDirectionDropDownList.DataTextField = "Value";
         RepeatDirectionDropDownList.DataValueField = "Key";
         RepeatDirectionDropDownList.DataBind();
      }

      private void CreateChartTypeEntries()
      {
         string[] chartTypeNames = Enum.GetNames(typeof(ChartType));
         Dictionary<int, string> chartTypeEntries = new Dictionary<int, string>();
         for (int i = 0; i < chartTypeNames.Length; i++)
            chartTypeEntries.Add(Convert.ToInt32(Enum.GetValues(typeof(ChartType)).GetValue(i)), Localization.GetString(String.Format("ChartType.{0}.Text", chartTypeNames[i]), LocalResourceFile));
         ChartTypeDropDownList.DataSource = chartTypeEntries;
         ChartTypeDropDownList.DataTextField = "Value";
         ChartTypeDropDownList.DataValueField = "Key";
         ChartTypeDropDownList.DataBind();
      }

      protected void UpdateButton_Click(object sender, EventArgs e)
      {
         if (Page.IsValid)
         {
            try
            {
               SurveysInfo survey;
               if (SurveyID == 0)
               {
                  survey = new SurveysInfo();
               }
               else
               {
                  survey = Survey;
               }
               survey.SurveyID = SurveyID;
               survey.ModuleID = ModuleId;
               survey.Question = QuestionTextBox.Text;
               survey.OptionType = (QuestionType)Convert.ToInt32(QuestionTypeDropDownList.SelectedValue);
               survey.IsStatistical = (SurveyType == SurveyType.Quiz ? IsStatisticalCheckBox.Checked : (bool?)null);
               if (survey.OptionType == QuestionType.Text)
               {
                  int surveyOptionID = 0;
                  if (SurveyID > 0)
                     surveyOptionID = SurveyOptionsController.GetAll(SurveyID)[0].SurveyOptionID;
                  Answers = DummyData.LoadDummyForTextAnswer(SurveyID, surveyOptionID, UserId);
               }
               else
               {
                  if (Request.Form["SurveyOptionID"] == null)
                  {
                     // You can't add a single or multiple choice question with no answers...
                     ErrorMessagePanel.Visible = true;
                     ErrorMessageLabel.Text = Localization.GetString("NoAnswersProvided.Text", LocalResourceFile);
                  }
                  else
                  {
                     //if (SurveyID > 0)
                     //{
                        List<SurveyOptionsInfo> answers = Answers;
                        int[] surveyOptionIDs = (from p in Request.Form["SurveyOptionID"].Split(',')
                                                 select int.Parse(p)).ToArray();
                        int viewOrder = 1;
                        foreach (int surveyOptionID in surveyOptionIDs)
                        {
                           SurveyOptionsInfo answer = answers.Find(x => x.SurveyOptionID == surveyOptionID);
                           answer.ViewOrder = viewOrder;
                           viewOrder++;
                        }
                        Answers = answers;
                     //}
                     int correctAnswers = Answers.Where(a => a.IsCorrect).Count();
                     if ((SurveyType == SurveyType.Quiz) && (!(IsStatisticalCheckBox.Checked)) && (correctAnswers == 0))
                     {
                        ErrorMessagePanel.Visible = true;
                        ErrorMessageLabel.Text = Localization.GetString("NoCorrectAnswersProvided.Text", LocalResourceFile);
                     }
                     if ((SurveyType == SurveyType.Quiz) && (!(IsStatisticalCheckBox.Checked)) && (survey.OptionType == QuestionType.RadioButtons) && (correctAnswers > 1))
                     {
                        ErrorMessagePanel.Visible = true;
                        ErrorMessageLabel.Text = Localization.GetString("OnlyOneCorrectAnswerAllowed.Text", LocalResourceFile);
                     }
                  }
               }
               if (!(ErrorMessagePanel.Visible))
               {
                  survey.RepeatDirection = (RepeatDirection)Convert.ToInt32(RepeatDirectionDropDownList.SelectedValue);
                  survey.RepeatColumns = (String.IsNullOrEmpty(RepeatColumnsTextBox.Text) ? (int?)null : Convert.ToInt32(RepeatColumnsTextBox.Text));
                  survey.NumberOfRows = (((String.IsNullOrEmpty(NumberOfRowsTextBox.Text)) || (NumberOfRowsTextBox.Text == "1")) ? (int?)null : Convert.ToInt32(NumberOfRowsTextBox.Text));

                  survey.ChartType = (ChartType)Convert.ToInt32(ChartTypeDropDownList.SelectedValue);

                  SurveysController.AddOrChange(survey, XmlDataProvider.SurveyOptionsToXml(Answers), UserId);
                  Response.Redirect(Globals.NavigateURL(), false);
               }
            }
            catch (Exception ex)
            {
               Exceptions.ProcessModuleLoadException(this, ex);
            }
         }
      }

      protected void UpdateAnswerButton_Click(object sender, EventArgs e)
      {
         Page.Validate("AnswerValidation");
         if (Page.IsValid)
         {
            LinkButton updateAnswerButton = (LinkButton)sender;
            List<SurveyOptionsInfo> answers = Answers;
            if (answers == null) answers = new List<SurveyOptionsInfo>();

            if (Request.Form["SurveyOptionID"] != null)
            {
               int[] surveyOptionIDs = (from p in Request.Form["SurveyOptionID"].Split(',')
                                        select int.Parse(p)).ToArray();
               int viewOrder = 1;
               foreach (int surveyOptionID in surveyOptionIDs)
               {
                  SurveyOptionsInfo answer = answers.Find(x => x.SurveyOptionID == surveyOptionID);
                  answer.ViewOrder = viewOrder;
                  viewOrder++;
               }
            }

            if (SurveyOptionID == 0)
            {
               MaxViewOrder = MaxViewOrder + 1;
               SurveyOptionsInfo surveyOption = new SurveyOptionsInfo();
               surveyOption.OptionName = AnswerTextBox.Text;
               surveyOption.IsCorrect = CorrectAnswerCheckBox.Checked;
               surveyOption.SurveyOptionID = MaxViewOrder * (-1);
               surveyOption.ViewOrder = MaxViewOrder;
               surveyOption.Votes = 0;
               surveyOption.CreatedByUserID = UserId;
               surveyOption.CreatedDate = DateTime.Now;
               answers.Add(surveyOption);
               answers.OrderBy(so => so.ViewOrder);
            }
            else
            {
               SurveyOptionsInfo surveyOption = SurveyOption;
               answers.Remove(answers.Find(so => so.SurveyOptionID == SurveyOptionID));
               surveyOption.OptionName = AnswerTextBox.Text;
               surveyOption.IsCorrect = CorrectAnswerCheckBox.Checked;
               answers.Add(surveyOption);
               answers.OrderBy(so => so.ViewOrder);
               SurveyOptionID = 0;
               SurveyOption = null;
               AddAnswerButton.Visible = true;
               UpdateAnswerButton.Visible = false;
            }
            Answers = answers;
            AnswersGrid.DataSource = Answers;
            AnswersGrid.DataBind();
            AnswerTextBox.Text = String.Empty;
            CorrectAnswerCheckBox.Checked = false;
         }
      }

      protected void EditImage_Click(object sender, EventArgs e)
      {
         DnnImageButton editImage = (DnnImageButton)sender;
         SurveyOptionsInfo surveyOption = SurveyOptionsController.Get(Convert.ToInt32(editImage.CommandArgument));

         if (surveyOption != null)
         {
            SurveyOptionID = surveyOption.SurveyOptionID;
            SurveyOption = surveyOption;

            AnswerTextBox.Text = surveyOption.OptionName;
            CorrectAnswerCheckBox.Checked = surveyOption.IsCorrect;

            AddAnswerButton.Visible = false;
            UpdateAnswerButton.Visible = true;
         }
      }

      protected void DeleteImage_Click(object sender, EventArgs e)
      {
         DnnImageButton deleteImage = (DnnImageButton)sender;
         int surveyOptionID = Convert.ToInt32(deleteImage.CommandArgument);

         List<SurveyOptionsInfo> answers = Answers;
         answers.Remove(answers.Find(so => so.SurveyOptionID == surveyOptionID));
         Answers = answers;
         AnswersGrid.DataSource = Answers;
         AnswersGrid.DataBind();
      }

      protected void AnswersGrid_Init(object sender, EventArgs e)
      {
         Localization.LocalizeDataGrid(ref AnswersGrid, LocalResourceFile);
      }

      protected void QuestionTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
      {
         DropDownList questionTypeDropDownList = (DropDownList)sender;
         QuestionType selectedQuestionType = (QuestionType)Convert.ToInt32(questionTypeDropDownList.SelectedValue);
         if ((selectedQuestionType == QuestionType.RadioButtons) || (selectedQuestionType == QuestionType.CheckBoxes))
         {
            RepeatDirectionPanel.Visible = true;
            AnswersPanel.Visible = true;
            TextAnswerPanel.Visible = false;
            if (!(ChartTypeChanged))
            {
               ChartTypeDropDownList.SelectedValue = Convert.ToInt32(ChartType.Bar).ToString();
               ChartTypeChanged = false;
            }
            if (SurveyType == SurveyType.Quiz)
            {
               IsStatisticalCheckBox.Visible = true;
               IsStatisticalCheckBox.Enabled = true;
               CorrectAnswerPanel.Visible = true;
            }
         }
         else
         {
            RepeatDirectionPanel.Visible = false;
            AnswersPanel.Visible = false;
            TextAnswerPanel.Visible = true;
            if (!(ChartTypeChanged))
            {
               ChartTypeDropDownList.SelectedValue = Convert.ToInt32(ChartType.HorizontalBar).ToString();
               ChartTypeChanged = false;
            }
            if (SurveyType == SurveyType.Quiz)
            {
               IsStatisticalCheckBox.Visible = true;
               IsStatisticalCheckBox.Checked = true;
               IsStatisticalCheckBox.Enabled = false;
               CorrectAnswerPanel.Visible = false;
            }
         }
      }

      protected void ChartTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
      {
         DropDownList chartTypeDropList = (DropDownList)sender;
         ChartTypeChanged = true;
      }
      #endregion

      protected void IsStatisticalCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         CheckBox isStatisticalCheckBox = (CheckBox)sender;
         CorrectAnswerPanel.Visible = (!((isStatisticalCheckBox.Checked) || ((QuestionType)Convert.ToInt32(QuestionTypeDropDownList.SelectedValue) == QuestionType.Text)));
      }
   }
}