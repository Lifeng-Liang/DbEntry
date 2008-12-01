<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Edit" %>
<script runat="server">
    public SysUser item;
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<h1>User Edit</h1>
<form action="<%= UrlTo(new UTArgs{Action = "update"}, item.Id) %>" method="post">
<p><label for="sysuser_name">Name</label><br /><input id="sysuser_name" name="sysuser[name]" size="30" type="text" value="<%= item.Name %>" /></p>
<p><label for="sysuser_age">Age</label><br /><input id="sysuser_age" name="sysuser[age]" size="30" type="text" value="<%= item.Age %>" /></p>
<p><label for="sysuser_birthday">Birthday</label><br /><input id="sysuser_birthday" name="sysuser[birthday]" size="30" type="text" value="<%= item.Birthday %>" /></p>
<p><label for="sysuser_ismale">IsMale</label><br /><input id="sysuser_ismale" name="sysuser[ismale]" size="30" type="text" value="<%= item.IsMale %>" /></p>
<input name="commit" type="submit" value="Update" />
</form>

<%= LinkTo(new LTArgs{Title = "Show", Action = "show"}, item.Id) %>
<%= LinkTo(new LTArgs{Title = "Back", Action = "list"}) %>

</asp:Content>
