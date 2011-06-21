<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.Popup_DeleteNode" 
         MasterPageFile="~/PopupLayout.master" 
         Title="Delete"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="Popup_DeleteNode.aspx.cs" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="Delete" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
    <asp:Literal runat="server" ID="Literal1" Text="Are you sure you want to delete: "/>
    <asp:Label runat="server" ID="DeleteNodeNameLiteral" CssClass="emphasize" />
    <asp:Literal runat="server" ID="Literal2" Text="?"/>
    <asp:Button runat="server" ID="ShowButton" Text="Show Me" CssClass="Button" UseSubmitBehavior="false" Visible="false"/>
    <br /><br />
    <asp:Button runat="server" ID="DeleteButton" Text="Delete" CssClass="Button" UseSubmitBehavior="true" OnClick="DeleteButton_Click" />
    <asp:Button runat="server" ID="CancelButton" Text="Cancel" CssClass="Button" UseSubmitBehavior="false" OnClientClick="Popup_Cancel_Clicked();"/>
    <br /><br />
</asp:Content>

 
