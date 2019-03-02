<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyResults.ascx.cs" Inherits="DNN.Modules.Survey.SurveyResults" %>

<asp:Panel ID="ResultsNotPublicPanel" runat="server"
   CssClass="dnnFormMessage dnnFormValidationSummary">
   <asp:Label ID="ResultsNotPublicMessage" runat="server"
      ResourceKey="ResultsNotPublicMessage" />
</asp:Panel>

<asp:Panel ID="ResultsPanel" runat="server">
   <asp:PlaceHolder ID="ChartPlaceHolder" runat="server" />
</asp:Panel>

<div class="dnnForm">
   <ul class="dnnActions dnnClear">
      <li>
         <asp:LinkButton ID="ViewSurveyButton" runat="server"
            CssClass="dnnPrimaryAction"
            OnClick="ViewSurveyButton_Click"
            ResourceKey="ViewSurvey.Action" />
      </li>
   </ul>
</div>