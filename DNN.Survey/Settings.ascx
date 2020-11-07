<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DNN.Modules.Survey.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="DotNetNuke.Web.Client" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" TagPrefix="dnn" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/Survey/css/Settings.css" />

<div class="dnnForm" id="SurveySettings">
   <h2 id="GeneralSettings" class="dnnFormSectionHead"><a href="#"><asp:Label ID="GeneralSettingsLabel" runat="server" ResourceKey="GeneralSettings" /></a></h2>
   <fieldset>
      <div class="dnnFormItem">
         <dnn:Label ID="SurveyTypeLabel" runat="server"
            ControlName="SurveyTypeRadioButtonList"
            ResourceKey="SurveyType"
            Suffix=":" />
         <asp:RadioButtonList ID="SurveyTypeRadioButtonList" runat="server"
            CssClass="dnnFormRadioButtons"
            RepeatDirection="Horizontal"
            RepeatColumns="2">
         </asp:RadioButtonList>
      </div>
      <div class="dnnFormItem">
         <dnn:Label ID="SurveyClosingDateLabel" runat="server"
            ControlName="SurveyClosingDateTextBox"
            ResourceKey="SurveyClosingDate"
            Suffix=":" />
         <asp:TextBox ID="SurveyClosingDateTextBox" runat="server"
            CssClass="surveyDatePicker" />
      </div>
   </fieldset>
   <h2 id="AppearanceSecurityOptions" class="dnnFormSectionHead"><a href="#"><asp:Label ID="AppearanceSecurityOptionsLabel" runat="server" ResourceKey="AppearanceSecurityOptions" /></a></h2>
   <fieldset>
      <div class="dnnFormItem">
         <dnn:Label ID="ShowClosingDateMessageLabel" runat="server"
            ControlName="ShowClosingDateMessageCheckBox"
            ResourceKey="ShowClosingDateMessage" />
         <asp:CheckBox ID="ShowClosingDateMessageCheckBox" runat="server" />
      </div>
      <div class="dnnFormItem">
         <dnn:Label ID="PrivacyConfirmationLabel" runat="server"
            ControlName="PrivacyConfirmationCheckBox"
            ResourceKey="PrivacyConfirmation" />
         <asp:CheckBox ID="PrivacyConfirmationCheckBox" runat="server" />
      </div>
      <div class="dnnFormItem">
         <dnn:Label ID="UseCaptchaLabel" runat="server"
            ControlName="UseCaptchaRadioButtonList"
            ResourceKey="UseCaptcha" />
         <asp:RadioButtonList ID="UseCaptchaRadioButtonList" runat="server"
            CssClass="dnnFormRadioButtons"
            RepeatDirection="Horizontal"
            RepeatColumns="3">
         </asp:RadioButtonList>
      </div>
   </fieldset>
   <h2 id="CSVExportOptions" class="dnnFormSectionHead"><a href="#"><asp:Label ID="CSVExportOptionsLabel" runat="server" ResourceKey="CSVExportOptions" /></a></h2>
   <fieldset>
      <div class="dnnFormItem">
         <dnn:Label ID="CSVSeparatorLabel" runat="server"
            ControlName="CSVSeparatorDropDownlist"
            ResourceKey="Separator" />
         <asp:DropDownList ID="CSVSeparatorDropDownlist" runat="server" />
      </div>
      <div class="dnnFormItem">
         <dnn:Label ID="CSVTextQualifierLabel" runat="server"
            ControlName="CSVTextQualifierDropDownList"
            ResourceKey="TextQualifier" />
         <asp:DropDownList ID="CSVTextQualifierDropDownList" runat="server" />
      </div>
   </fieldset>
</div>