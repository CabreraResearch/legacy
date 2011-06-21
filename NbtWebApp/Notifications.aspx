<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.Notifications" 
         MasterPageFile="~/MainLayout.master" 
         Title="Notifications"
 Codebehind="Notifications.aspx.cs" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Notifications
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ph" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContextSensitiveContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ContextSensitivePlaceHolder" />
</asp:Content>


