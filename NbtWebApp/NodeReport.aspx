<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.NodeReport" 
         MasterPageFile="~/PrintableLayout.master" 
         Title="Report"
 Codebehind="NodeReport.aspx.cs" %>

<%@ MasterType VirtualPath="~/PrintableLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Report
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="MasterContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="centerph" />
</asp:Content>
