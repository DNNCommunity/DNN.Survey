<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SurveyEdit.ascx.cs" Inherits="DNN.Modules.Survey.SurveyEdit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>

<div class="dnnForm" id="SurveyEditPanel">
   <div class="dnnFormItem dnnFormHelp dnnClear">
      <p><asp:Label runat="server" ResourceKey="RequiredFields" CssClass="dnnFormRequired" /></p>
   </div>
   <fieldset>
      <div class="dnnFormItem">
         <dnn:Label ID="QuestionLabel" runat="server"
            ControlName="QuestionTextBox"
            CssClass="dnnFormRequired"
            ResourceKey="Question"
            Suffix=":" />
         <asp:TextBox ID="QuestionTextBox" runat="server"
            Rows="3"
            TextMode="MultiLine" />
         <asp:RequiredFieldValidator ID="QuestionValidator" runat="server"
            ControlToValidate="QuestionTextBox"
            CssClass="dnnFormMessage dnnFormError"
            ResourceKey="Question.Required"
            ValidationGroup="QuestionValidation" />
      </div>
      <div class="dnnFormItem">
         <dnn:Label ID="QuestionTypeLabel" runat="server"
            ControlName="QuestionTypeDropDownList"
            ResourceKey="QuestionType"
            Suffix =":" />
         <asp:DropDownList ID="QuestionTypeDropDownList" runat="server"
            AutoPostBack="true"
            OnSelectedIndexChanged="QuestionTypeDropDownList_SelectedIndexChanged" />
      </div>
      <asp:Panel ID="RepeatDirectionPanel" runat="server">
         <div class="dnnFormItem">
            <dnn:Label ID="RepeatDirectionLabel" runat="server"
               ControlName="RepeatDirectionDropdownList"
               ResourceKey="RepeatDirection"
               Suffix=":" />
            <asp:DropDownList ID="RepeatDirectionDropDownList" runat="server" />
         </div>
         <div class="dnnFormItem">
            <dnn:Label ID="RepeatColumnsLabel" runat="server"
               ControlName="RepeatColumnsTextBox"
               ResourceKey="RepeatColumns"
               Suffix=":" />
            <asp:TextBox ID="RepeatColumnsTextBox" runat="server" />
         </div>
      </asp:Panel>
   </fieldset>

   <asp:Panel ID="AnswersPanel" runat="server">
      <h2 class="dnnFormSectionHead"><asp:Label ID="AnswersHeaderLabel" runat="server" ResourceKey="Answers" /></h2>
      <asp:DataGrid ID="AnswersGrid" runat="server"
         AutoGenerateColumns ="false"
         BorderStyle="None"
         CssClass="dnnGrid"
         EnableViewState="true"
         GridLines="None"
         OnInit="AnswersGrid_Init"
         Width="98%">
         <HeaderStyle CssClass="dnnGridHeader" VerticalAlign="Top"/>
         <ItemStyle CssClass="dnnGridItem" HorizontalAlign="Left" />
         <AlternatingItemStyle CssClass="dnnGridAltItem" />
         <EditItemStyle CssClass="dnnFormInput" />
         <SelectedItemStyle CssClass="dnnFormError" />
         <FooterStyle CssClass="dnnGridFooter" />
         <PagerStyle CssClass="dnnGridPager" />
         <Columns>
            <asp:TemplateColumn>
               <ItemTemplate>
                  <dnn:DnnImage ID="UpIcon" runat="server" IconKey="Up" ResourceKey="DragDrop" />
                  <dnn:DnnImage ID="DnIcon" runat="server" IconKey="Dn" ResourceKey="DragDrop" />
               </ItemTemplate>
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="OptionName" HeaderText="OptionName" />
            <asp:BoundColumn DataField="Votes" HeaderText="Votes">
               <ItemStyle HorizontalAlign="Right" />
            </asp:BoundColumn>
            <asp:TemplateColumn HeaderText="IsCorrect">
               <ItemTemplate>
                  <dnn:DnnImage ID="IsCorrectImage" runat="server" IconKey="Checked" Visible='<%# (bool)DataBinder.Eval(Container.DataItem,"IsCorrect") %>' />
                  <dnn:DnnImage ID="IsNotCorrectImage" runat="server" IconKey="Unchecked" Visible='<%# !(bool)DataBinder.Eval(Container.DataItem,"IsCorrect") %>' />
               </ItemTemplate>
               <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="">
               <ItemTemplate>
                  <dnn:DnnImageButton ID="EditImage" runat="server"
                     CommandArgument='<%# Eval("SurveyOptionID") %>'
                     IconKey="Edit"
                     OnClick="EditImage_Click"
                     Visible='<%# (bool)(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "SurveyOptionID")) > 0) %>' />
                  <dnn:DnnImageButton ID="DeleteImage" runat="server"
                     CommandArgument='<%# Eval("SurveyOptionID") %>'
                     IconKey="Delete"
                     OnClick="DeleteImage_Click"
                     Visible='<%# (bool)(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "SurveyOptionID")) > 0) %>' />
                  <input type="hidden" name="SurveyOptionID" value='<%# Eval("SurveyOptionID") %>' />
               </ItemTemplate>
               <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateColumn>
         </Columns>
      </asp:DataGrid>

      <asp:Panel ID="AddEditAnswerPanel" runat="server">
         <div class="dnnForm">
            <fieldset>
               <div class="dnnFormItem">
                  <dnn:Label ID="AnswerLabel" runat="Server"
                     ControlName="AnswerTextBox"
                     CssClass="dnnFormRequired"
                     ResourceKey="Answer"
                     Suffix=":" />
                  <asp:TextBox ID="AnswerTextBox" runat="server"
                     Rows="3"
                     TextMode="MultiLine" />
                  <asp:RequiredFieldValidator ID="AnswerValidator" runat="server"
                     ControlToValidate="AnswerTextBox"
                     CssClass="dnnFormMessage dnnFormError"
                     ResourceKey="Answer.Required"
                     ValidationGroup="AnswerValidation" />
               </div>
               <div class="dnnFormItem">
                  <dnn:Label ID="CorrectAnswerLabel" runat="server"
                     ControlName="CorrectAnswerCheckBox"
                     ResourceKey="CorrectAnswer" />
                  <asp:CheckBox ID="CorrectAnswerCheckBox" runat="server" />
               </div>
            </fieldset>
            <ul class="dnnActions dnnClear">
               <li>
                  <asp:LinkButton ID="AddAnswerButton" runat="server"
                     CausesValidation="true"
                     CssClass="dnnPrimaryAction"
                     OnClick="UpdateAnswerButton_Click"
                     ResourceKey="AddAnswer"
                     ValidationGroup="AnswerValidation" />
               </li>
               <li>
                  <asp:LinkButton ID="UpdateAnswerButton" runat="server"
                     CausesValidation="true"
                     CssClass="dnnPrimaryAction"
                     OnClick="UpdateAnswerButton_Click"
                     ResourceKey="UpdateAnswer"
                     ValidationGroup="AnswerValidation"
                     Visible="false" />
               </li>
            </ul>
         </div>
      </asp:Panel>
   </asp:Panel>

   <ul class="dnnActions dnnClear">
      <li>
         <asp:LinkButton ID="UpdateButton" runat="server"
            CausesValidation="true"
            CssClass="dnnPrimaryAction"
            OnClick="UpdateButton_Click"
            ResourceKey="cmdUpdate"
            ValidationGroup="QuestionValidation" />
      </li>
      <li>
         <asp:LinkButton ID="CancelButton" runat="server"
            CausesValidation="false"
            CssClass="dnnSecondaryAction"
            OnClick="CancelButton_Click"
            ResourceKey="cmdCancel" />
      </li>
   </ul>
</div>

<div class="dnnFormMessage dnnFormInfo" id="TestMessage" runat="server">
   <asp:Label ID="TestMessageLabel" runat="server" />
</div>