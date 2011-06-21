<%@ Page Language="C#" AutoEventWireup="true" Inherits="ChemSW.Nbt.WebPages.Statistics" 
         MasterPageFile="~/MainLayout.master" Title="Statistics" Codebehind="Statistics.aspx.cs" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Statistics
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MasterLeftContent" Runat="Server">
    <asp:PlaceHolder ID="leftph" runat="server"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MasterRightContent" Runat="Server">
    <asp:PlaceHolder ID="rightph" runat="server"></asp:PlaceHolder>
</asp:Content>
