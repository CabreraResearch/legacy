<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="SearchCustom.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.SearchCustom" 
         MasterPageFile="~/MainLayout.master" 
         Title="Custom Search"
%>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Custom Search
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


