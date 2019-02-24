using DNN.Modules.Survey.Components;
using DNN.Modules.Survey.Components.Controllers;
using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Components.Providers;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
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
      #endregion

      #region Events
      protected override void OnInit(EventArgs e)
      {
         JavaScript.RequestRegistration(CommonJs.DnnPlugins);
         JavaScript.RequestRegistration(CommonJs.jQuery);
         JavaScript.RequestRegistration(CommonJs.jQueryUI);

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

            if (SurveyID > 0)
            {
               Survey = SurveysController.Get(SurveyID);
               Answers = SurveyOptionsController.GetAll(SurveyID);
               QuestionTextBox.Text = Survey.Question;
               QuestionTypeDropDownList.SelectedValue = Convert.ToInt32(Survey.OptionType).ToString();
               if (Survey.OptionType == QuestionType.RadioButtons || Survey.OptionType == QuestionType.CheckBoxes)
               {
                  RepeatDirectionDropDownList.SelectedValue = Convert.ToInt32(Survey.RepeatDirection).ToString();
                  RepeatColumnsTextBox.Text = Survey.RepeatColumns.ToString();
               }
               else
               {
                  NumberOfRowsTextBox.Text = Survey.NumberOfRows.ToString();
                  RepeatDirectionPanel.Visible = false;
                  AnswersPanel.Visible = false;
                  TextAnswerPanel.Visible = true;
               }
               AnswersGrid.DataSource = Answers;
               AnswersGrid.DataBind();
               MaxViewOrder = Answers.Count;
            }
         }
      }

      protected override void OnPreRender(EventArgs e)
      {
         StringBuilder scriptBuilder = new StringBuilder();
         scriptBuilder.Append("$(function() {\r\n");
         scriptBuilder.Append("   $(\"#" + AnswersGrid.ClientID + "\").sortable({\r\n");
         scriptBuilder.Append("      items: 'tr:not(tr:first-child)',\r\n");
         scriptBuilder.Append("      cursor: 'pointer',\r\n");
         scriptBuilder.Append("      axis: 'y',\r\n");
         scriptBuilder.Append("      dropOnEmpty: false,\r\n");
         scriptBuilder.Append("      start: function(e, ui) {\r\n");
         scriptBuilder.Append("         ui.item.addClass(\"dnnFormError\");\r\n");
         scriptBuilder.Append("      },\r\n");
         scriptBuilder.Append("      stop: function(e, ui) {\r\n");
         scriptBuilder.Append("         ui.item.removeClass(\"dnnFormError\");\r\n");
         scriptBuilder.Append("      },\r\n");
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
               if (survey.OptionType == QuestionType.Text)
               {
                  int surveyOptionID = 0;
                  if (SurveyID > 0)
                     surveyOptionID = SurveyOptionsController.GetAll(SurveyID)[0].SurveyOptionID;
                  Answers = DummyData.LoadDummyForTextAnswer(SurveyID, surveyOptionID, UserId);
               }
               else
               {
                  if (SurveyID > 0)
                  {
                     int[] surveyOptionIDs = (from p in Request.Form["SurveyOptionID"].Split(',')
                                              select int.Parse(p)).ToArray();
                     int viewOrder = 1;
                     List<SurveyOptionsInfo> answers = Answers;
                     foreach (int surveyOptionID in surveyOptionIDs)
                     {
                        answers.Find(x => x.SurveyOptionID == surveyOptionID).ViewOrder = viewOrder;
                        viewOrder++;
                     }
                     Answers = answers;
                  }
               }
               survey.RepeatDirection = (RepeatDirection)Convert.ToInt32(RepeatDirectionDropDownList.SelectedValue);
               survey.RepeatColumns = (String.IsNullOrEmpty(RepeatColumnsTextBox.Text) ? (int?)null : Convert.ToInt32(RepeatColumnsTextBox.Text));
               survey.NumberOfRows = (((String.IsNullOrEmpty(NumberOfRowsTextBox.Text)) || (NumberOfRowsTextBox.Text == "1")) ? (int?)null : Convert.ToInt32(NumberOfRowsTextBox.Text));
               SurveysController.AddOrChange(survey, XmlDataProvider.SurveyOptionsToXml(Answers), UserId);
               Response.Redirect(Globals.NavigateURL(), false);
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
            List<SurveyOptionsInfo> answers = Answers;
            if (answers == null) answers = new List<SurveyOptionsInfo>();
            if (SurveyOptionID == 0)
            {
               MaxViewOrder = MaxViewOrder + 1;
               SurveyOptionsInfo surveyOption = new SurveyOptionsInfo();
               surveyOption.ViewOrder = MaxViewOrder;
               surveyOption.OptionName = AnswerTextBox.Text;
               surveyOption.Votes = 0;
               surveyOption.IsCorrect = CorrectAnswerCheckBox.Checked;
               surveyOption.CreatedByUserID = UserId;
               surveyOption.CreatedDate = DateTime.Now;
               answers.Add(surveyOption);
            }
            else
            {
               SurveyOptionsInfo surveyOption = SurveyOption;
               surveyOption.OptionName = AnswerTextBox.Text;
               surveyOption.IsCorrect = CorrectAnswerCheckBox.Checked;
               answers.Remove(answers.Find(so => so.SurveyOptionID == SurveyOptionID));
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
         }
         else
         {
            RepeatDirectionPanel.Visible = false;
            AnswersPanel.Visible = false;
            TextAnswerPanel.Visible = true;
         }
      }
      #endregion
   }
}