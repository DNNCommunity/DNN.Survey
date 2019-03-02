﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DNN.Modules.Survey.Settings" %>
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
         <dnn:Label ID="SurveyClosingDateLabel" runat="server"
            ControlName="SurveyClosingDateTextBox"
            ResourceKey="SurveyClosingDate"
            Suffix=":" />
         <asp:TextBox ID="SurveyClosingDateTextBox" runat="server"
            CssClass="surveyDatePicker" />
      </div>
      <div class="dnnFormItem">
         <dnn:Label ID="SurveyTrackingLabel" runat="server"
            ControlName="SurveyTrackingRadioButtons"
            ResourceKey="SurveyTracking"
            Suffix=":" />
         <asp:RadioButtonList ID="SurveyTrackingRadioButtons" runat="server"
            CssClass="dnnFormRadioButtons">
            <asp:ListItem Value="0" />
            <asp:ListItem Value="1" />
         </asp:RadioButtonList>
      </div>
      <div class="dnnFormItem">
         <dnn:Label ID="SurveyResultTemplateLabel" runat="server"
            ControlName="SurveyResultTemplateTextBox"
            ResourceKey="SurveyResultTemplate"
            Suffix=":" />
         <asp:TextBox ID="SurveyResultTemplateTextBox" runat="server"
            TextMode="MultiLine"
            Rows="8" />
      </div>
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