<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="User List" Inherits="Lephone.Web.PageBase, Lephone.Web" %>
<%@ Import Namespace="Lephone.Web.Common" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<h1>Listing users</h1>

<table>
<tr><th>Id</th><th>Name</th><th>Age</th></tr>
<% foreach(DebugLib.Models.User u in bag["list"] as IEnumerable) { %>
<tr>
  <td><%= u.Id %></td><td><%= u.Name %></td><td><%= u.Age %></td>
  <td><%= LinkTo("Show", null, "show", u.Id) %></td>
  <td><%= LinkTo("Edit", null, "edit", u.Id) %></td>
  <td><%= LinkTo("Destroy", null, "destroy", u.Id) %></td>
</tr>
<% } %>
</table>

<% int count = (int)(long)bag["list_count"];
   for (int i = 0, n = 1; i < count; n++, i += WebSettings.DefaultPageSize) { %>
      &nbsp;<%= LinkTo(n.ToString(), null, "list", n.ToString()) %>
<% } %>

<br /><br />

<%= LinkTo("New User", null, "new", null) %><br />

</asp:Content>
