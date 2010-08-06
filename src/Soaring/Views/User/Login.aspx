<%@ Page Title="" Language="C#" MasterPageFile="~/main.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
.title
{
	text-align: right;
	font-weight: bold;
}
.inputbox
{
	text-align: left;
}
.ErrMsg
{
    color: red;
	font-weight: bold;
}
#center
{
    height:450px;
    width:auto;
    text-align:center;
    background-color:#f0f0f0;
    padding-top:100px;
    margin-left:20px;
    margin-right:20px;
}
#mainPanel
{
    margin:0 auto;
	background: url(../images/bg.jpg) no-repeat;
    text-align: center;
    width:480px;
    height: 272px;
    background-color:#f2f8ff;
    border:solid 1px #ccccff;
}
    </style>
    <script type="text/javascript">
        $(function () {
            $("email").focus().select();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <form id="form1" action="<%= UrlTo.Controller("user").Action("login") %>" method="post">
        <div id="center">
        <div id="mainPanel">
            <table border="0" style="margin-left:130px; margin-right:auto; margin-top:140px">
                <tr><td class="title">Email:</td><td class="inputbox"><input id="email" name="email" size="20" type="text" maxlength="128" /></td></tr>
                <tr><td class="title">Password:</td><td class="inputbox"><input id="password" name="password" size="20" type="password" maxlength="99" /></td></tr>
                <tr><td class="title"></td><td class="inputbox"><input id="rememberme" name="rememberme" type="checkbox" /><label for="rememberme">Remember Me</label></td></tr>
                <tr><td colspan="2" align="center"><input name="commit" type="submit" value=" 登录 " /></td></tr>
            </table>
        </div>
        <p style="color: Red"><%= Flash.Warning %></p><p style="color: Green"><%= Flash.Notice %></p>
        </div>
    </form>
</asp:Content>
