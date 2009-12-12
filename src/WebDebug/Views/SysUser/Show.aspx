<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Show User" %>
<script runat="server">
    public SysUser Item;
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<p style="color: Green"><%= Flash["notice"] %></p>

<p><b>Id:</b><%= Item.Id %></p>
<p><b>Name:</b><%= Item.Name %></p>
<p><b>Age:</b><%= Item.Age %></p>

<%= LinkTo.Title("Edit").Action("edit").Parameters(Item.Id) %>
<%= LinkTo.Title("Back").Action("list") %>

</asp:Content>
