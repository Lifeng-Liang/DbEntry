<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="User List" %>
<script runat="server">
    public IEnumerable List;
    public long ListCount;
    public int ListPageSize;
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<p style="color: Green"><%= Flash["notice"] %></p>

<h1>Listing users</h1>

<table>
<tr><th>Id</th><th>Name</th><th>Age</th></tr>
<% foreach (SysUser u in List) { %>
<tr>
  <td><%= u.Id %></td><td><%= u.Name %></td><td><%= u.Age %></td>
  <td><%= LinkTo.Title("Show").Action("show").Parameters(u.Id) %></td>
  <td><%= LinkTo.Title("Edit").Action("edit").Parameters(u.Id) %></td>
  <td><%= LinkTo.Title("Destroy").Action("destroy").Addon("onclick=\"if (confirm('Are you sure?')) { var f = document.createElement('form'); f.style.display = 'none'; this.parentNode.appendChild(f); f.method = 'POST'; f.action = this.href;f.submit(); };return false;\"").Parameters(u.Id) %></td>
</tr>
<% } %>
</table>

<% for (int i = 0, n = 1; i < ListCount; n++, i += ListPageSize) { %>
      &nbsp;<%= LinkTo.Title(n.ToString()).Action("list").Parameters(n) %>
<% } %>

<br /><br />

<%= LinkTo.Title("New User").Action("new") %><br />

</asp:Content>
