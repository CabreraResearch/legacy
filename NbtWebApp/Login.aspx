<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.Login" 
         MasterPageFile="~/MainLayout.master" 
         Title="Login"
 Codebehind="Login.aspx.cs" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Login
</asp:Content>

<asp:Content ID="AllContent" ContentPlaceHolderID="MasterCenterContent" runat="server">
    <script language="Javascript">
        if(screen.width < 800 && location.search != "?redir=n")
        {
            //window.location = "LowRes_Login.aspx";
            window.location = "Mobile.html";            
        }
    </script>

    <div class="Login">
        <table align="center">
            <tr>
                <td align="center"><Csw:CswLogin ID="Login1" runat="server" /></td>
            </tr>
        </table>
       <br /><br />
        <div style="text-align: right">
            <asp:Label runat="server" ID="AssemblyLabel" Text="Unknown Assembly"></asp:Label>
        </div>
    </div>
    
    <script language="javascript">
        <asp:PlaceHolder runat="server" ID="JavascriptPlaceHolder"></asp:PlaceHolder>
    </script>
</asp:Content>
