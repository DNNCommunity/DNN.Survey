<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrivacyConfirmationCheckBox.ascx.cs" Inherits="DNN.Modules.Survey.Controls.PrivacyConfirmationCheckBox" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnnSurvey" Namespace="DNN.Modules.Survey.Components.UI.WebControls.Validators" Assembly="DNN.Modules.Survey" %>

<div class="dnnFormItem SurveyFormItem">
   <dnn:Label ID="PrivacyConfirmationLabel" runat="server" Text="&nbsp;" />
   <asp:CheckBox ID="PrivacyConfirmation" runat="server"
      CssClass="dnnFormRadioButtons" />
   <dnnSurvey:CheckBoxValidator ID="PrivacyConfirmationValidator" runat="server"
      ControlToValidate="PrivacyConfirmation"
      CssClass="dnnFormMessage dnnFormError" />
</div>
