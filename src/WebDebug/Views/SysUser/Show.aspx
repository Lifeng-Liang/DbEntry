<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Show User" %>
<script runat="server">
    public SysUser item;
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<p style="color: Green"><%= Flash["notice"] %></p>

<p><b>Id:</b><%= item.Id %></p>
<p><b>Name:</b><%= item.Name %></p>
<p><b>Age:</b><%= item.Age %></p>

<%= LinkTo(new LTArgs{Title = "Edit", Action = "edit"}, item.Id) %>
<%= LinkTo(new LTArgs{Title = "Back", Action = "list"}) %>

</asp:Content>
