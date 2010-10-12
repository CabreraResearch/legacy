<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="Popup_ChangeView.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Popup_ChangeView" 
         MasterPageFile="~/PopupLayout.master" 
         Title="Edit"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="Change View" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
    <script language="Javascript">
        checkChangesEnabled = false;
    </script>

    <asp:PlaceHolder runat="server" ID="ph" />
    
</asp:Content>

 

