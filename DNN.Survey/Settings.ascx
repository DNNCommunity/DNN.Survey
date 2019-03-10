<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DNN.Modules.Survey.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="DotNetNuke.Web.Client" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" TagPrefix="dnn" %>

<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/Survey/css/Settings.css" />

<div class="dnnForm" id="SurveySettings">
   <div class="dnnFormMessage dnnFormInfo surveyTabModuleMessage">
      <asp:Label ID="SurveyResultsTypeMovedLabel" runat="server"
         ResourceKey="SurveyResultsTypeMovedMessage" />
   </div>
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
            <asp:ListItem Value="0" />
            <asp:ListItem Value="1" />
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
            <asp:ListItem Value="0" />
            <asp:ListItem Value="1" />
            <asp:ListItem Value="2" />
         </asp:RadioButtonList>
      </div>
      <div class="dnnClear"></div>
      <div class="dnnFormMessage dnnFormInfo surveyTabModuleMessage">
         <asp:Label ID="TabModuleSettingsMessageLabel" runat="server"
            ResourceKey="TabModuleSettingsMessage" />
      </div>
      <div class="dnnFormItem">
         <dnn:Label ID="ShowClosingDateMessageLabel" runat="server"
            ControlName="ShowClosingDateMessageCheckBox"
            ResourceKey="ShowClosingDateMessage" />
         <asp:CheckBox ID="ShowClosingDateMessageCheckBox" runat="server" />
      </div>
      <div class="dnnFormItem">
         <dnn:Label ID="SurveyGraphWidthLabel" runat="server"
            ControlName="SurveyGraphWidthTextBox"
            ResourceKey="SurveyGraphWidth"
            Suffix=":" />
         <asp:TextBox ID="SurveyGraphWidthTextBox" runat="server" />
      </div>
   </fieldset>
</div>