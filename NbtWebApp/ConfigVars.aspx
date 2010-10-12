<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="ConfigVars.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.ConfigVars" 
         MasterPageFile="~/MainLayout.master" 
         Title="Edit Config Vars"
%>
<%@ MasterType VirtualPath="~/MainLayout.master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Edit Config Vars
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ph" />
</asp:Content>

