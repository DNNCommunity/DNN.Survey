<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyView.ascx.cs" Inherits="DNN.Modules.Survey.SurveyView" %>

<div class="dnnForm">
   <asp:Label ID="SurveyMessageLabel" runat="server" />

   <div class="dnnForm">
      <fieldset>
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
   </ul>
</div>
