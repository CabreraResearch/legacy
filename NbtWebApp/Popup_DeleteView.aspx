<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="Popup_DeleteView.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Popup_DeleteView" 
         MasterPageFile="~/PopupLayout.master" 
         Title="Delete View"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="Delete View" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
    <asp:Literal runat="server" ID="Literal1" Text="Are you sure you want to delete view "/>
    <asp:Label runat="server" ID="DeleteViewNameLiteral" CssClass="emphasize" />
    <asp:Literal runat="server" ID="Literal2" Text="?"/>
    <br /><br />
    <asp:Button runat="server" ID="DeleteButton" Text="Delete" CssClass="Button" UseSubmitBehavior="false" OnClientClick="Popup_OK_Clicked();" />
    <asp:Button runat="server" ID="CancelButton" Text="Cancel" CssClass="Button" UseSubmitBehavior="false" OnClientClick="Popup_Cancel_Clicked();"/>
    <br /><br />
</asp:Content>

 
