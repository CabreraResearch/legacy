<%@ Page Language="C#" AutoEventWireup="true" Inherits="ChemSW.Nbt.WebPages.DisplayLog"
    MasterPageFile="~/MainLayout.master" Title="Display Log" Codebehind="DisplayLog.aspx.cs" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Display Log 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterCenterContent" runat="Server">

    <script language="javascript" type="text/javascript">
    
    </script>

    <asp:PlaceHolder runat="server" id="theGrid" />
</asp:Content>
