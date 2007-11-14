<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="User List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<p style="color: Green"><%= flash["notice"] %></p>

<h1>Listing users</h1>

<table>
<tr><th>Id</th><th>Name</th><th>Age</th></tr>
<% foreach(SysUser u in bag["list"] as IEnumerable) { %>
<tr>
  <td><%= u.Id %></td><td><%= u.Name %></td><td><%= u.Age %></td>
  <td><%= LinkTo("Show", null, "show", u.Id) %></td>
  <td><%= LinkTo("Edit", null, "edit", u.Id) %></td>
  <td><%= LinkTo("Destroy", null, "destroy", u.Id, "onclick=\"if (confirm('Are you sure?')) { var f = document.createElement('form'); f.style.display = 'none'; this.parentNode.appendChild(f); f.method = 'POST'; f.action = this.href;f.submit(); };return false;\"")%></td>
</tr>
<% } %>
</table>

<% int count = (int)(long)bag["list_count"];
   int pagesize = (int)bag["list_pagesize"];
   for (int i = 0, n = 1; i < count; n++, i += pagesize) { %>
      &nbsp;<%= LinkTo(n.ToString(), null, "list", n.ToString()) %>
<% } %>

<br /><br />

<%= LinkTo("New User", null, "new", null) %><br />

</asp:Content>
