<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="LowRes_Props.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.LowRes_Props" 
         MasterPageFile="~/LowResLayout.master" 
%>

<%@ MasterType VirtualPath="~/LowResLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" Runat="Server">
    Properties
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ph" />
</asp:Content>
