<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.ConfigVars" 
         MasterPageFile="~/MainLayout.master" 
         Title="Edit Config Vars"
 Codebehind="ConfigVars.aspx.cs" %>
<%@ MasterType VirtualPath="~/MainLayout.master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Edit Config Vars
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ph" />
</asp:Content>

