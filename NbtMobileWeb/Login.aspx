<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="Login.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Login" 
         MasterPageFile="~/Standard.master" 
%>

<%@ MasterType VirtualPath="~/Standard.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" Runat="Server">
    <title>Login</title>  
</asp:Content>

<asp:Content ID="AllContent" ContentPlaceHolderID="StandardContent" runat="server">
    <script language="Javascript">
//        if(screen.width < 800)
//        {
//            window.location="LowRes_Login.aspx";            
//        }
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
