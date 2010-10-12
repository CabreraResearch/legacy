<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="ChemSW.Nbt.WebPages.Main"
    MasterPageFile="~/MainLayout.master" Title="Main" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="Server">
    Main
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MasterLeftContent" runat="Server">
    <asp:PlaceHolder runat="server" ID="leftph" />
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="MasterRightContent" runat="Server">
    <asp:PlaceHolder runat="server" ID="rightph" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MasterCenterContent" runat="Server">
    <asp:PlaceHolder runat="server" ID="centerph" />
    
    <script language="javascript">
        //BZ 9936 
        unsetChanged();
    </script>
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContextSensitiveContent" runat="Server">
    <asp:PlaceHolder runat="server" ID="ContextSensitivePlaceHolder" />
</asp:Content>
