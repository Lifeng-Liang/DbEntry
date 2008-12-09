<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="User List" %>
<script runat="server">
    public IEnumerable list;
    public long list_count;
    public int list_pagesize;
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<p style="color: Green"><%= Flash["notice"] %></p>

<h1>Listing users</h1>

<table>
<tr><th>Id</th><th>Name</th><th>Age</th></tr>
<% foreach (SysUser u in list) { %>
<tr>
  <td><%= u.Id %></td><td><%= u.Name %></td><td><%= u.Age %></td>
  <td><%= LinkTo(new LTArgs{Title = "Show", Action = "show"}, u.Id) %></td>
  <td><%= LinkTo(new LTArgs{Title = "Edit", Action = "edit"}, u.Id) %></td>
  <td><%= LinkTo(new LTArgs {Title = "Destroy", Action = "destroy", Addon = "onclick=\"if (confirm('Are you sure?')) { var f = document.createElement('form'); f.style.display = 'none'; this.parentNode.appendChild(f); f.method = 'POST'; f.action = this.href;f.submit(); };return false;\""}, u.Id) %></td>
</tr>
<% } %>
</table>

<% for (int i = 0, n = 1; i < list_count; n++, i += list_pagesize) { %>
      &nbsp;<%= LinkTo(new LTArgs{Title = n.ToString(), Action = "list"}, n) %>
<% } %>

<br /><br />

<%= LinkTo(new LTArgs{Title = "New User", Action = "new"}) %><br />

</asp:Content>
