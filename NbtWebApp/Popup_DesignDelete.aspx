<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.Popup_DesignDelete"  
         MasterPageFile="~/PopupLayout.master" 
         Title="Design - Delete" Codebehind="Popup_DesignDelete.aspx.cs" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="Design - Delete" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ph"></asp:PlaceHolder>
</asp:Content>

 
