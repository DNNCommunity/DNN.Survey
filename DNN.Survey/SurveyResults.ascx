<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyResults.ascx.cs" Inherits="DNN.Modules.Survey.SurveyResults" %>

<div class="dnnForm">
   <asp:Label ID="SurveyMessageLabel" runat="server"
      EnableViewState="false"
      Visible="false" />

   <asp:Panel ID="ResultsPanel" runat="server">
      <asp:PlaceHolder ID="ChartPlaceHolder" runat="server" />
   </asp:Panel>

   <ul class="dnnActions dnnClear">
      <li>
         <asp:LinkButton ID="ViewSurveyButton" runat="server"
            CssClass="dnnPrimaryAction"
            OnClick="ViewSurveyButton_Click"
            ResourceKey="ViewSurvey.Action" />
      </li>
   </ul>
</div>