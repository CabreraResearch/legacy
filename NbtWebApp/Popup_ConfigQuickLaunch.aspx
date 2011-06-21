<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.Popup_ConfigQuickLaunch" 
         MasterPageFile="~/PopupLayout.master" 
         Title="Configure Quick Launch"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="Popup_ConfigQuickLaunch.aspx.cs" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Configure Quick Launch
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
    <script language="javascript">
        checkChangesEnabled = false;
    </script>

    <asp:PlaceHolder runat="server" ID="ph"></asp:PlaceHolder>
    
</asp:Content>

 
