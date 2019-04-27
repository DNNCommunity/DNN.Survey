<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyRadioButtons.ascx.cs" Inherits="DNN.Modules.Survey.Controls.SurveyRadioButtons" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem SurveyFormItem">
   <dnn:Label ID="SurveyRadioButtonsLabel" runat="server"
      CssClass="SurveyLabel"
      ControlName="SurveyRadioButtonList" />
   <asp:RadioButtonList ID="SurveyRadioButtonList" runat="server"
      CssClass="dnnFormRadioButtons" />
   <asp:RequiredFieldValidator ID="SurveyRadioButtonListValidator" runat="server"
      ControlToValidate="SurveyRadioButtonList"
      CssClass="dnnFormMessage dnnFormError" />
</div>
