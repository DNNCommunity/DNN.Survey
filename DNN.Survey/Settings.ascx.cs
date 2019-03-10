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
      #endregion

      #region TabModule Settings
      protected bool ShowClosingDateMessage
      {
         get
         {
            object showClosingDateMessage = TabModuleSettings["ShowClosingDateMessage"];
            if (showClosingDateMessage == null)
               return false;
            else
               return Convert.ToBoolean(showClosingDateMessage);
         }
         set
         {
            UpdateBooleanSetting("ShowClosingDateMessage", value, true);
         }
      }

      protected int SurveyGraphWidth
      {
         get
         {
            object surveyGraphWidth = TabModuleSettings["SurveyGraphWidth"];
            if (surveyGraphWidth == null)
               return 100;
            else
               return Convert.ToInt32(surveyGraphWidth);
         }
         set
         {
            if (value == 100)
               UpdateIntegerSetting("SurveyGraphWidth", null, true);
            else
               UpdateIntegerSetting("SurveyGraphWidth", value, true);
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
         foreach (ListItem li in SurveyTypeRadioButtonList.Items)
         {
            li.Text = Localization.GetString(string.Format("SurveyType.{0}.Text", Enum.GetName(typeof(SurveyType), Convert.ToInt32(li.Value))), LocalResourceFile);
         }
         SurveyTypeRadioButtonList.SelectedValue = Convert.ToInt32(SurveyType).ToString();

         if (SurveyClosingDate == DateTime.MinValue)
            SurveyClosingDateTextBox.Text = String.Empty;
         else
            SurveyClosingDateTextBox.Text = String.Format("{0:yyyy-MM-dd HH:mm:ss}", SurveyClosingDate);

         PrivacyConfirmationCheckBox.Checked = PrivacyConfirmation;
         ShowClosingDateMessageCheckBox.Checked = ShowClosingDateMessage;

         foreach (ListItem li in UseCaptchaRadioButtonList.Items)
         {
            li.Text = Localization.GetString(string.Format("UseCaptcha.{0}.Text", Enum.GetName(typeof(UseCaptcha), Convert.ToInt32(li.Value))), LocalResourceFile);
         }
         UseCaptchaRadioButtonList.SelectedValue = Convert.ToInt32(UseCaptcha).ToString();

         if (SurveyGraphWidth == 0)
            SurveyGraphWidthTextBox.Text = String.Empty;
         else
            SurveyGraphWidthTextBox.Text = SurveyGraphWidth.ToString();

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

         if (String.IsNullOrEmpty(SurveyGraphWidthTextBox.Text))
            SurveyGraphWidth = 0;
         else
         {
            if (Int32.TryParse(SurveyGraphWidthTextBox.Text, out Int32 result))
               SurveyGraphWidth = result;
            else
               SurveyGraphWidth = 0;
         }

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
               ModuleController.Instance.DeleteModuleSetting(TabModuleId, settingName);
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
            if (settingValue != null)
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
            if (settingValue != null)
            {
               ModuleController.Instance.UpdateModuleSetting(ModuleId, settingName, settingValue.ToString());
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