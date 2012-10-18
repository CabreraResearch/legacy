<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalLogin.aspx.cs" Inherits="ChemSW.Nbt.WebPages.ExternalLogin" %>

<!doctype html xmlns="http://www.w3.org/1999/xhtml">
 <html>
    <head runat="server">
        <meta http-equiv="X-UA-Compatible" content="chrome=1">
        <title>Login</title>
        <!--#include file="MainIncludes.html" -->
        <!--#include file="MainCswIncludes.html" -->
    </head>
    <body>
        <form id="form1" runat="server">
        <div>
            Please Wait...

            <script language="javascript">
                <asp:PlaceHolder ID="JSPlaceHolder" runat="server"></asp:PlaceHolder>
            </script>

        </div>
        </form>
    </body>
</html>