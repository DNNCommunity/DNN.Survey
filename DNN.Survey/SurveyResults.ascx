<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyResults.ascx.cs" Inherits="DNN.Modules.Survey.SurveyResults" %>

<asp:Panel ID="ResultsNotPublicPanel" runat="server"
   CssClass="dnnFormMessage dnnFormValidationSummary">
   <asp:Label ID="ResultsNotPublicMessage" runat="server"
      ResourceKey="ResultsNotPublicMessage" />
</asp:Panel>

<asp:Panel ID="ResultsPanel" runat="server">
   RESULTS!
</asp:Panel>

<div class="dnnForm">
   <ul class="dnnActions dnnClear">
      <li>
         <asp:LinkButton ID="ViewResultsButton" runat="server"
            CssClass="dnnPrimaryAction"
            OnClick="ViewResultsButton_Click"
            ResourceKey="ViewSurvey.Action" />
      </li>
   </ul>
</div>