<%@ Page Language="C#" 
         AutoEventWireup="true" 
         CodeFile="Popup_CopyNode.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Popup_CopyNode"  
         MasterPageFile="~/PopupLayout.master" 
         Title="Copy" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="Copy" />
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
    
    <asp:PlaceHolder runat="server" ID="ph"></asp:PlaceHolder>
    
</asp:Content>

 
