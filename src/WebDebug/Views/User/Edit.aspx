<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<h1>New User</h1>
<form action="/WebDebug/user/update/<%= ((DebugLib.Models.User)bag["item"]).Id %>" method="post">
<p><label for="user_name">Name</label><br /><input id="user_name" name="user[name]" size="30" type="text" value="<%= ((DebugLib.Models.User)bag["item"]).Name %>" /></p>
<p><label for="user_age">Age</label><br /><input id="user_age" name="user[age]" size="30" type="text" value="<%= ((DebugLib.Models.User)bag["item"]).Age %>" /></p>
<p><label for="user_birthday">Birthday</label><br /><input id="user_birthday" name="user[birthday]" size="30" type="text" value="<%= ((DebugLib.Models.User)bag["item"]).Birthday %>" /></p>
<p><label for="user_ismale">IsMale</label><br /><input id="user_ismale" name="user[ismale]" size="30" type="text" value="<%= ((DebugLib.Models.User)bag["item"]).IsMale %>" /></p>
<input name="commit" type="submit" value="Update" />
</form>

<%= LinkTo("Show", null, "show", ((DebugLib.Models.User)bag["item"]).Id) %>
<%= LinkTo("Back", null, "list", null) %>

</asp:Content>
