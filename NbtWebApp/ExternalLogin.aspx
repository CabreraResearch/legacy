<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalLogin.aspx.cs" Inherits="ChemSW.Nbt.WebPages.ExternalLogin" %>

<!doctype html xmlns="http://www.w3.org/1999/xhtml">
 <html>
    <head runat="server">
        <meta http-equiv="X-UA-Compatible" content="IE=9">
        <title>Login</title>
        <!--#include file="MainIncludes.html" -->
        <script type="text/javascript" src="CswCommon.min.js"></script>
        <script type="text/javascript" src="CswNbt.min.js"></script>
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