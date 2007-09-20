<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="User List" Inherits="Lephone.Web.PageBase, Lephone.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<%= LinkTo("New", null, "new", null) %><br />

<table>
<tr style="font-weight:bold"><td>Id</td><td>Name</td><td>Age</td></tr>
<% foreach(DebugLib.Models.User u in bag["list"] as IEnumerable) { %>
<tr><td><%= u.Id %></td><td><%= u.Name %></td><td><%= u.Age %></td></tr>
<% } %>
</table>

</asp:Content>
