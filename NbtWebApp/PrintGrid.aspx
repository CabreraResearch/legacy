<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.PrintGrid" 
         MasterPageFile="~/PrintableLayout.master" 
         Title="Print Grid"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="PrintGrid.aspx.cs" %>

<%@ MasterType VirtualPath="~/PrintableLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Print Grid
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">

    <asp:PlaceHolder runat="server" ID="ph"></asp:PlaceHolder>
    
</asp:Content>

