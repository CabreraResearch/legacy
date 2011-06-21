<%@ Page Language="C#" AutoEventWireup="true" Inherits="TimeIntervalTest" Codebehind="TimeIntervalTest.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Time Interval Test</title>
    <script src="Main.js"></script>
    <script language="Javascript">
        checkChangesEnabled = false;
    </script>

    <% if( Request.UserAgent.Contains( "MSIE 6.0" ) )
       { %>
    <link rel="stylesheet" href="ie6style.css" />
    <% } %>
    <link rel="stylesheet" href="style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="sm"></asp:ScriptManager>
        <div>
            <asp:PlaceHolder runat="server" ID="ph"></asp:PlaceHolder>    
        </div>
    </form>
</body>
</html>
