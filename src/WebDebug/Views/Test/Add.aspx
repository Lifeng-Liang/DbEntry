<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<p>The answer is : <%= bag["obj"] %></p>
<p>The http get 'tt' paramter is : <%= Request["tt"] %></p>
</asp:Content>
