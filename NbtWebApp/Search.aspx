<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="Search.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Search" 
         MasterPageFile="~/MainLayout.master" 
         Title="Search"
%>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Search
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    <script language="Javascript">
        checkChangesEnabled = false;
    </script>

    <asp:PlaceHolder runat="server" ID="centerph" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContextSensitiveContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ContextSensitivePlaceHolder" />
</asp:Content>


