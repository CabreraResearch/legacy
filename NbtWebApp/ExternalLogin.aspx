<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ChemSW.Nbt.WebPages.Login" %>

<!DOCTYPE html>
 <html runat="server" id="html">
    <head runat="server">
        <meta http-equiv="X-UA-Compatible" content="chrome=1">
        <title>Login</title>
        <!--#include file="release/ExternalLoginIncludes.html" -->
    </head>
    <body>
        <form id="form1" runat="server">
        <div>
            
            <div id="contentDiv"></div>
            <script>
                //(function () {
                    
                    //drop in the values from the server for accessid, username, password, and logoutpath
                    <asp:PlaceHolder ID="JSPlaceHolder" runat="server"></asp:PlaceHolder>

                    var contentDiv = $('#contentDiv');
                    Csw.layouts.login(Csw.dom(null, contentDiv),
                        {
                            accessid: accessid,
                            username: username,
                            password: password,
                            logoutpath: logoutpath,
                        });
                    

                //})();//
            </script>
             
        </div>
        </form>
    </body>
</html>