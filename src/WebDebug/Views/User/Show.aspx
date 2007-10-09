<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Show User" Inherits="Lephone.Web.Rails.PageBase, Lephone.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<p style="color: Green"><%= flash["notice"] %></p>

<p><b>Id:</b><%= ((DebugLib.Models.User)bag["item"]).Id %></p>
<p><b>Name:</b><%= ((DebugLib.Models.User)bag["item"]).Name %></p>
<p><b>Age:</b><%= ((DebugLib.Models.User)bag["item"]).Age %></p>

<%= LinkTo("Edit", null, "edit", ((DebugLib.Models.User)bag["item"]).Id) %>
<%= LinkTo("Back", null, "list", null) %>

</asp:Content>
