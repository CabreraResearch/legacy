<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Popup_About.aspx.cs" Inherits="ChemSW.Nbt.WebPages.Popup_About"  MasterPageFile="~/PopupLayout.master" Title="About" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="About NBT" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
    <div runat="server" id="aboutcontentdiv" class="DialogText" align="center">
        <asp:Label runat="server" ID="AboutText"></asp:Label>
        <asp:Button runat="server" ID="AboutClosebutton" Text="Close" CssClass="Button" OnClientClick="Popup_Cancel_Clicked();" />
    </div>
</asp:Content>

 
