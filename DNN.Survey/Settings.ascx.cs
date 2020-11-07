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
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey
{
   public partial class Settings : ModuleSettingsBase
   {
      #region Module Settings
      protected SurveyType SurveyType
      {
         get
         {
            object surveyType = ModuleSettings["SurveyType"];
            if (surveyType == null)
               return SurveyType.Survey;
            else
               return (SurveyType)Convert.ToInt32(surveyType);
         }
         set
         {
            UpdateIntegerSetting("SurveyType", Convert.ToInt32(value));
         }
      }

      protected DateTime SurveyClosingDate
      {
         get
         {
            object surveyClosingDate = ModuleSettings["SurveyClosingDate"];
            if (surveyClosingDate == null)
               return DateTime.MinValue;
            else
               return Convert.ToDateTime(surveyClosingDate);
         }
         set
         {
            if (value == DateTime.MinValue)
               UpdateTextSetting("SurveyClosingDate", String.Empty);
            else
               UpdateTextSetting("SurveyClosingDate", Globals.DateToString(value));
         }
      }

      protected bool PrivacyConfirmation
      {
         get
         {
            object privacyConfirmation = ModuleSettings["PrivacyConfirmation"];
            if (privacyConfirmation == null)
               return false;
            else
               return Convert.ToBoolean(privacyConfirmation);
         }
         set
         {
            UpdateBooleanSetting("PrivacyConfirmation", value);
         }
      }

      protected UseCaptcha UseCaptcha
      {
         get
         {
            object useCaptcha = ModuleSettings["UseCaptcha"];
            if (useCaptcha == null)
               return UseCaptcha.Never;
            else
               return (UseCaptcha)Convert.ToInt32(useCaptcha);
         }
         set
         {
            UpdateIntegerSetting("UseCaptcha", Convert.ToInt32(value));
         }
      }

      protected bool ShowClosingDateMessage
      {
         get
         {
            object showClosingDateMessage = ModuleSettings["ShowClosingDateMessage"];
            if (showClosingDateMessage == null)
               return false;
            else
               return Convert.ToBoolean(showClosingDateMessage);
         }
         set
         {
            UpdateBooleanSetting("ShowClosingDateMessage", value);
         }
      }
      protected Separator Separator
      {
         get
         {
            object separator = ModuleSettings["Separator"];
            if (separator == null)
            {
               return Separator.SemiColon;
            }
            else
            {
               return (Separator)Convert.ToInt32(separator);
            }
         }
         set
         {
            UpdateIntegerSetting("Separator", Convert.ToInt32(value));
         }
      }
      protected TextQualifier TextQualifier
      {
         get
         {
            object textQualifier = ModuleSettings["TextQualifier"];
            if (textQualifier == null)
            {
               return TextQualifier.None;
            }
            else
            {
               return (TextQualifier)Convert.ToInt32(textQualifier);
            }
         }
         set
         {
            UpdateIntegerSetting("TextQualifier", Convert.ToInt32(value));
         }
      }
      #endregion

      #region Page Events
      protected override void OnPreRender(EventArgs e)
      {
         // Add DatePicker (pikaday) to ClosingDate textbox
         if (!(Page.ClientScript.IsStartupScriptRegistered("SurveySettings")))
            Page.ClientScript.RegisterStartupScript(GetType(), "SurveySettings", String.Format("$('#{0}').pikaday({{\"minDate\":new Date('1900-01-01 00:00:00'),\"maxDate\":new Date('9999-12-31 23:59:59'),\"format\":\"YYYY-MM-DD HH:mm:ss\",\"showTime\":true,\"use24hour\":true,\"autoClose\":true}});", SurveyClosingDateTextBox.ClientID), true);

         base.OnPreRender(e);
      }
      #endregion

      #region Settings Events
      public override void LoadSettings()
      {
         foreach (SurveyType surveyType in (SurveyType[]) Enum.GetValues(typeof(SurveyType)))
         {
            SurveyTypeRadioButtonList.Items.Add(new ListItem(Localization.GetString(string.Format("SurveyType.{0}.Text", Enum.GetName(typeof(SurveyType), surveyType)), LocalResourceFile), Convert.ToInt32(surveyType).ToString()));
         }
         SurveyTypeRadioButtonList.SelectedValue = Convert.ToInt32(SurveyType).ToString();

         if (SurveyClosingDate == DateTime.MinValue)
            SurveyClosingDateTextBox.Text = String.Empty;
         else
            SurveyClosingDateTextBox.Text = String.Format("{0:yyyy-MM-dd HH:mm:ss}", SurveyClosingDate);

         PrivacyConfirmationCheckBox.Checked = PrivacyConfirmation;
         ShowClosingDateMessageCheckBox.Checked = ShowClosingDateMessage;

         foreach (UseCaptcha useCaptcha in (UseCaptcha[]) Enum.GetValues(typeof(UseCaptcha)))
         {
            UseCaptchaRadioButtonList.Items.Add(new ListItem(Localization.GetString(string.Format("UseCaptcha.{0}.Text", Enum.GetName(typeof(UseCaptcha), useCaptcha)), LocalResourceFile), Convert.ToInt32(useCaptcha).ToString()));
         }
         UseCaptchaRadioButtonList.SelectedValue = Convert.ToInt32(UseCaptcha).ToString();

         foreach (Separator separator in (Separator[]) Enum.GetValues(typeof(Separator)))
         {
            CSVSeparatorDropDownlist.Items.Add(new ListItem(Localization.GetString(string.Format("Separator.{0}.Text", Enum.GetName(typeof(Separator), separator)), LocalResourceFile), Convert.ToInt32(separator).ToString()));
         }
         CSVSeparatorDropDownlist.SelectedValue = ((int)Separator).ToString();

         foreach (TextQualifier textQualifier in (TextQualifier[]) Enum.GetValues(typeof(TextQualifier)))
         {
            CSVTextQualifierDropDownList.Items.Add(new ListItem(Localization.GetString(string.Format("TextQualifier.{0}.Text", Enum.GetName(typeof(TextQualifier), textQualifier)), LocalResourceFile), Convert.ToInt32(textQualifier).ToString()));
         }
         CSVTextQualifierDropDownList.SelectedValue = ((int)TextQualifier).ToString();

         base.LoadSettings();
      }

      public override void UpdateSettings()
      {
         SurveyType = (SurveyType)Convert.ToInt32(SurveyTypeRadioButtonList.SelectedValue);
         if (String.IsNullOrEmpty(SurveyClosingDateTextBox.Text))
            SurveyClosingDate = DateTime.MinValue;
         else
         {
            if (DateTime.TryParse(SurveyClosingDateTextBox.Text, out DateTime result))
               SurveyClosingDate = result;
            else
               SurveyClosingDate = DateTime.MinValue;
         }

         PrivacyConfirmation = PrivacyConfirmationCheckBox.Checked;
         ShowClosingDateMessage = ShowClosingDateMessageCheckBox.Checked;
         UseCaptcha = (UseCaptcha)Convert.ToInt32(UseCaptchaRadioButtonList.SelectedValue);
         TextQualifier = (TextQualifier)Convert.ToInt32(CSVTextQualifierDropDownList.SelectedValue);
         Separator = (Separator)Convert.ToInt32(CSVSeparatorDropDownlist.SelectedValue);
         base.UpdateSettings();
      }
      #endregion

      #region Helper functions
      private void UpdateBooleanSetting(string settingName, bool settingValue)
      {
         UpdateBooleanSetting(settingName, settingValue, false);
      }

      private void UpdateBooleanSetting(string settingName, bool settingValue, bool isTabModuleSetting)
      {
         if (isTabModuleSetting)
         {
            if (settingValue)
            {
               ModuleController.Instance.UpdateTabModuleSetting(TabModuleId, settingName, settingValue.ToString());
            }
            else
            {
               ModuleController.Instance.DeleteTabModuleSetting(TabModuleId, settingName);
            }
         }
         else
         {
            if (settingValue)
            {
               ModuleController.Instance.UpdateModuleSetting(ModuleId, settingName, settingValue.ToString());
            }
            else
            {
               ModuleController.Instance.DeleteModuleSetting(ModuleId, settingName);
            }
         }
      }

      private void UpdateTextSetting(string settingName, string settingValue)
      {
         UpdateTextSetting(settingName, settingValue, false);
      }

      private void UpdateTextSetting(string settingName, string settingValue, bool isTabModuleSetting)
      {
         if (isTabModuleSetting)
         {
            if (!(string.IsNullOrEmpty(settingValue)))
            {
               ModuleController.Instance.UpdateTabModuleSetting(TabModuleId, settingName, settingValue);
            }
            else
            {
               ModuleController.Instance.DeleteTabModuleSetting(TabModuleId, settingName);
            }
         }
         else
         {
            if (!(string.IsNullOrEmpty(settingValue)))
            {
               ModuleController.Instance.UpdateModuleSetting(ModuleId, settingName, settingValue);
            }
            else
            {
               ModuleController.Instance.DeleteModuleSetting(ModuleId, settingName);
            }
         }
      }

      private void UpdateIntegerSetting(string settingName, int? settingValue)
      {
         UpdateIntegerSetting(settingName, settingValue, false);
      }

      private void UpdateIntegerSetting(string settingName, int? settingValue, bool isTabModuleSetting)
      {
         if (isTabModuleSetting)
         {
            if (settingValue.HasValue)
            {
               if (settingValue.Value == 0)
               {
                  ModuleController.Instance.DeleteTabModuleSetting(TabModuleId, settingName);
               }
               else
               {
                  ModuleController.Instance.UpdateTabModuleSetting(TabModuleId, settingName, settingValue.ToString());
               }
            }
            else
            {
               ModuleController.Instance.DeleteTabModuleSetting(TabModuleId, settingName);
            }
         }
         else
         {
            if (settingValue.HasValue)
            {
               if (settingValue.Value == 0)
               {
                  ModuleController.Instance.DeleteModuleSetting(ModuleId, settingName);
               }
               else
               {
                  ModuleController.Instance.UpdateModuleSetting(ModuleId, settingName, settingValue.ToString());
               }
            }
            else
            {
               ModuleController.Instance.DeleteModuleSetting(ModuleId, settingName);
            }
         }
      }
      #endregion
   }
}