<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyView.ascx.cs" Inherits="DNN.Modules.Survey.SurveyView" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm">
   <asp:Label ID="SurveyMessageLabel" runat="server"
      EnableViewState="false"
      Visible="false" />

   <div class="dnnForm">
      <fieldset>
         <div class="dnnCheckBoxFormItemForFaxQuestion">
            <dnn:Label ID="ContactByFaxOnly" runat="server"
               ControlName="ContactByFaxOnlyCheckBox"
               ResourceKey="ContactByFaxOnly"
               Suffix=":"
               TabIndex="-1" />
            <asp:CheckBox ID="ContactByFaxOnlyCheckBox" runat="server"
               AutoPostBack="true"
               Checked="false"
               OnCheckedChanged="ContactByFaxOnlyCheckBox_CheckedChanged"
               TabIndex="-1" />
         </div>
         <asp:PlaceHolder ID="SurveyPlaceHolder" runat="server" />
      </fieldset>
   </div>

   <ul class="dnnActions dnnClear">
      <li>
         <asp:LinkButton ID="SubmitSurveyButton" runat="server"
            CausesValidation="true"
            CssClass="dnnPrimaryAction"
            OnClick="SubmitSurveyButton_Click"
            OnLoad="SubmitSurveyButton_Load"
            ResourceKey="SubmitSurvey.Action" />
      </li>
      <li>
         <asp:LinkButton ID="ViewResultsButton" runat="server"
            CssClass="dnnSecondaryAction"
            OnClick="ViewResultsButton_Click"
            ResourceKey="ViewResults.Action" />
      </li>
      <li>
         <asp:LinkButton ID="ExportToCsvButton" runat="server"
            CssClass="dnnSecondaryAction"
            OnClick="ExportToCsvButton_Click"
            ResourceKey="ExportToCsv.Action"
            Visible="false" />
      </li>
   </ul>
</div>
