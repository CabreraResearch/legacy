<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="ChangePassword.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.ChangePassword" 
         MasterPageFile="~/MainLayout.master" 
         Title="Change Password"
%>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Change Password
</asp:Content>

<asp:Content ID="AllContent" ContentPlaceHolderID="MasterCenterContent" runat="server">
    <script language="javascript">
        checkChangesEnabled = false;
    </script>
    <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
</asp:Content>
