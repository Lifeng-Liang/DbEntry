<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="New User" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<h1>New User</h1>
<form action="<%= UrlTo(new UTArgs{Action = "create"}) %>" method="post">
<p><label for="sysuser_name">Name</label><br /><input id="sysuser_name" name="sysuser[name]" size="30" type="text" value="" /></p>
<p><label for="sysuser_age">Age</label><br /><input id="sysuser_age" name="sysuser[age]" size="30" type="text" value="" /></p>
<p><label for="sysuser_birthday">Birthday</label><br /><input id="sysuser_birthday" name="sysuser[birthday]" size="30" type="text" value="" /></p>
<p><label for="sysuser_ismale">IsMale</label><br /><input id="sysuser_ismale" name="sysuser[ismale]" size="30" type="text" value="" /></p>
<input name="commit" type="submit" value="Create" />
</form>

<%= LinkTo(new LTArgs{Title = "Back", Action = "list"}) %>

</asp:Content>
