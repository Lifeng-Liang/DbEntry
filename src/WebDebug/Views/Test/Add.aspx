<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<p>The answer is : <%= this["obj"] %></p>
<p>The http get 'tt' Parameter is : <%= Request["tt"] %></p>
</asp:Content>
