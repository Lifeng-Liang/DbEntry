<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Show User" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<p style="color: Green"><%= flash["notice"] %></p>

<p><b>Id:</b><%= ((SysUser)bag["item"]).Id %></p>
<p><b>Name:</b><%= ((SysUser)bag["item"]).Name %></p>
<p><b>Age:</b><%= ((SysUser)bag["item"]).Age %></p>

<%= LinkTo("Edit", null, "edit", null, ((SysUser)bag["item"]).Id) %>
<%= LinkTo("Back", null, "list", null) %>

</asp:Content>
