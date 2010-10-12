<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="UserList.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.UserList" 
         MasterPageFile="~/MainLayout.master" 
         Title="User List"
%>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    User List
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
</asp:Content>
