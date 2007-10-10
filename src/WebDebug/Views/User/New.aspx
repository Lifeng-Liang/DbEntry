<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="New User" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<h1>New User</h1>
<form action="/WebDebug/user/create" method="post">
<p><label for="user_name">Name</label><br /><input id="user_name" name="user[name]" size="30" type="text" value="" /></p>
<p><label for="user_age">Age</label><br /><input id="user_age" name="user[age]" size="30" type="text" value="" /></p>
<p><label for="user_birthday">Birthday</label><br /><input id="user_birthday" name="user[birthday]" size="30" type="text" value="" /></p>
<p><label for="user_ismale">IsMale</label><br /><input id="user_ismale" name="user[ismale]" size="30" type="text" value="" /></p>
<input name="commit" type="submit" value="Create" />
</form>

<%= LinkTo("Back", null, "list", null) %>

</asp:Content>
