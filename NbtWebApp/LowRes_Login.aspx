<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="LowRes_Login.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.LowRes_Login" 
         MasterPageFile="~/LowResLayout.master" 
         Title="Login"
%>

<%@ MasterType VirtualPath="~/LowResLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Login
</asp:Content>

<asp:Content ID="AllContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="Login">
        <table>
            <tr>
                <td align="center"><Csw:CswLogin ID="Login1" runat="server" /></td>
            </tr>
        </table>
    </div>
</asp:Content>
