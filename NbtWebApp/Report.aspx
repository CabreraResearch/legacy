<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report.aspx.cs" Inherits="ChemSW.Nbt.WebPages.Report"  MasterPageFile="~/MainLayout.master" Title="Design" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Report
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MasterCenterContent" runat="Server">
    <asp:PlaceHolder runat="server" ID="ph" />
    
    <asp:Button runat="server" ID="LoadReportButton" />
    
    <script>
    LoadReport();
    </script>

</asp:Content>
