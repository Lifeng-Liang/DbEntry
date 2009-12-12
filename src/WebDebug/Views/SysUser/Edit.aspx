<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Edit" %>
<script runat="server">
    public SysUser Item;
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<h1>User Edit</h1>

<form action="<%= UrlTo.Action("update").Parameters(Item.Id) %>" method="post">
  <p><label for="sysuser_name">Name</label><br /><input id="sysuser_name" name="sysuser[name]" size="30" type="text" value="<%= Item.Name %>" /></p>
  <p><label for="sysuser_age">Age</label><br /><input id="sysuser_age" name="sysuser[age]" size="30" type="text" value="<%= Item.Age %>" /></p>
  <p><label for="sysuser_birthday">Birthday</label><br /><input id="sysuser_birthday" name="sysuser[birthday]" size="30" type="text" value="<%= Item.Birthday %>" /></p>
  <p><label for="sysuser_ismale">IsMale</label><br /><input id="sysuser_ismale" name="sysuser[ismale]" size="30" type="text" value="<%= Item.IsMale %>" /></p>
  <input name="commit" type="submit" value="Update" />
</form>

<%= LinkTo.Title("Show").Action("show").Parameters(Item.Id) %>
<%= LinkTo.Title("Back").Action("list") %>

</asp:Content>
