<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="Subscriptions.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Subscriptions" 
         MasterPageFile="~/MainLayout.master" 
         Title="Subscriptions"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Subscriptions
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    <script language="javascript">
        checkChangesEnabled = false;
    </script>

    <asp:PlaceHolder runat="server" ID="ph"></asp:PlaceHolder>
    
</asp:Content>

 
