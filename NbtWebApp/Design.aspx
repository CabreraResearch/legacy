<%@ Page Language="C#" AutoEventWireup="true" Inherits="ChemSW.Nbt.WebPages.Design"  MasterPageFile="~/MainLayout.master" Title="Design" Codebehind="Design.aspx.cs" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Design
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MasterLeftContent" runat="Server">
    <script language="Javascript">
        checkChangesEnabled = false;
    </script>
    <asp:PlaceHolder runat="server" ID="leftph" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MasterRightContent" runat="Server">
    <asp:PlaceHolder runat="server" ID="rightph" />
</asp:Content>
