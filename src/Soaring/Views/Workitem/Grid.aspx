<%@ Page Title="grid" Language="C#" MasterPageFile="~/main.master" %>

<script runat="server">
    public List<Workitem> ListProposed;
    public List<Workitem> ListWorking;
    public List<Workitem> ListReadyForTest;
    public List<Workitem> ListComplated;
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<style type="text/css">
	.list { list-style-type: none; margin: 0; padding: 0; width: 100%; min-height:700px; }
	.list li { margin: 0 3px 3px 3px; padding: 0.4em; padding-left: 1.5em; font-size: 1.0em; height: auto; }
	.list li span { position: absolute; margin-left: -1.3em; }
	</style>
    <script type="text/javascript">
        $(function () {
            $(".list").sortable({ connectWith: ".list"});
            $(".list").disableSelection();
            //$(".list").scrollable();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div class="content">
<table><tr><th>Proposed</th><th>Working</th><th>ReadyForTest</th><th>Complated</th></tr>
<tr><td style="width:25%;border:solid 1px black;" valign="top">
    <ul id="proposed" class="list">
        <% foreach (var workitem in ListProposed) { %>
    	    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span><%= workitem.Id %> <%= LinkTo.Action("show").Parameters(workitem.Id).Title(workitem.Title) %></li>
        <% } %>
    </ul>
</td><td style="width:25%;border:solid 1px black;" valign="top">
    <ul id="working" class="list">
        <% foreach (var workitem in ListWorking) { %>
    	    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span><%= workitem.Id %> <%= LinkTo.Action("show").Parameters(workitem.Id).Title(workitem.Title) %></li>
        <% } %>
    </ul>
</td><td style="width:25%;border:solid 1px black;" valign="top">
    <ul id="readyfortest" class="list">
        <% foreach (var workitem in ListReadyForTest) { %>
    	    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span><%= workitem.Id %> <%= LinkTo.Action("show").Parameters(workitem.Id).Title(workitem.Title) %></li>
        <% } %>
    </ul>
</td><td style="width:25%;border:solid 1px black;" valign="top">
    <ul id="complated" class="list">
        <% foreach (var workitem in ListComplated) { %>
    	    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span><%= workitem.Id %> <%= LinkTo.Action("show").Parameters(workitem.Id).Title(workitem.Title) %></li>
        <% } %>
    </ul>
</td>
</tr></table>
</div>

</asp:Content>

