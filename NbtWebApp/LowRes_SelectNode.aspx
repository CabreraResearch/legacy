<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="LowRes_SelectNode.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.LowRes_SelectNode" 
         MasterPageFile="~/LowResLayout.master" 
%>

<%@ MasterType VirtualPath="~/LowResLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Select Node
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ph" />
</asp:Content>
