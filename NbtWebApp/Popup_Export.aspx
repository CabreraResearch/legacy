<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.Popup_Export" 
         MasterPageFile="~/PrintableLayout.master" 
         Title="Export"
         validateRequest="false"
 Culture="auto" UICulture="auto" Codebehind="Popup_Export.aspx.cs" %>

<%@ MasterType VirtualPath="~/PrintableLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Export
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
    <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
</asp:Content>

