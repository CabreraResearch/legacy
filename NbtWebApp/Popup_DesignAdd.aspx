<%@ Page Language="C#" 
         AutoEventWireup="true" 
         CodeFile="Popup_DesignAdd.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Popup_DesignAdd"  
         MasterPageFile="~/PopupLayout.master" 
         Title="Design - Add" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="Design - Add" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">

    <asp:PlaceHolder runat="server" ID="ph" />
    
</asp:Content>

 
