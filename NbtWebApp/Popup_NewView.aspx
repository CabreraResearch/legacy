<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.Popup_NewView" 
         MasterPageFile="~/PopupLayout.master" 
         Title="New View"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="Popup_NewView.aspx.cs" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="New View" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">

    <asp:PlaceHolder runat="server" ID="ph"></asp:PlaceHolder>
    
</asp:Content>

 
