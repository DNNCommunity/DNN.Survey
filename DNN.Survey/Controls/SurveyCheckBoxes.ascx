<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyCheckBoxes.ascx.cs" Inherits="DNN.Modules.Survey.Controls.SurveyCheckBoxes" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnnSurvey" Namespace="DNN.Modules.Survey.Components.UI.WebControls.Validators" Assembly="DNN.Modules.Survey" %>

<div class="dnnFormItem SurveyFormItem">
   <dnn:Label ID="SurveyCheckBoxesLabel" runat="server"
      CssClass="SurveyLabel" />
   <asp:CheckBoxList ID="SurveyCheckBoxList" runat="server"
      CssClass="dnnFormRadioButtons" />
   <dnnSurvey:CheckBoxListValidator ID="SurveyCheckBoxListValidator" runat="server"
      ControlToValidate="SurveyCheckBoxList"
      CssClass="dnnFormMessage dnnFormError"
      MinimumNumberOfSelectedCheckBoxes="1" />
</div>
