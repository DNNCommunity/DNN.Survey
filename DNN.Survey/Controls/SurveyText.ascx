<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyText.ascx.cs" Inherits="DNN.Modules.Survey.Controls.SurveyText" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem SurveyFormItem">
   <dnn:Label ID="SurveyTextBoxLabel" runat="server"
      CssClass="SurveyLabel"
      ControlName="SurveyTextBox" />
   <asp:TextBox ID="SurveyTextBox" runat="server" />
   <asp:RequiredFieldValidator ID="SurveyTextBoxValidator" runat="server"
      ControlToValidate="SurveyTextBox"
      CssClass="dnnFormMessage dnnFormError" />
</div>