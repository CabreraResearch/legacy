<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="Popup_EditProp.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Popup_EditProp" 
         MasterPageFile="~/PopupLayout.master" 
         Title="Edit"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="Edit" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
    <script language="javascript">
        checkChangesEnabled = false;
    </script>
    
    <asp:PlaceHolder runat="server" ID="PropPlaceHolder" />
    
</asp:Content>

 
