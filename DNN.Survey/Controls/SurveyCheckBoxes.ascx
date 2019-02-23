<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyCheckBoxes.ascx.cs" Inherits="DNN.Modules.Survey.Controls.SurveyCheckBoxes" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem SurveyFormItem">
   <dnn:Label ID="SurveyCheckBoxesLabel" runat="server"
      CssClass="SurveyLabel"
      ControlName="SurveyCheckBoxList" />
   <asp:CheckBoxList ID="SurveyCheckBoxList" runat="server"
      CssClass="dnnFormRadioButtons" />
</div>
