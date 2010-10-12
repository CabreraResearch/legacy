<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="Popup_PrintLabel.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Popup_PrintLabel" 
         MasterPageFile="~/PopupLayout.master" 
         Title="Print Label"
         validateRequest="false" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    <asp:Literal runat="server" ID="TitleContentLiteral" Text="Print Label" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">

    <asp:PlaceHolder runat="server" ID="ph"></asp:PlaceHolder>
    
</asp:Content>

