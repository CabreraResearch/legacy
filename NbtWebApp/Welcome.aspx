<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="Welcome.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Welcome" 
         MasterPageFile="~/MainLayout.master" 
         Title="Welcome"
%>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Welcome
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MasterLeftContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="leftph" />
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="MasterRightContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="rightph" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="centerph" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContextSensitiveContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ContextSensitivePlaceHolder" />
</asp:Content>


