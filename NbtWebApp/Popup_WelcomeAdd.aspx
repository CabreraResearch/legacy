<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.Popup_WelcomeAdd" 
         MasterPageFile="~/PopupLayout.master" 
         Title="Welcome - Add"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="Popup_WelcomeAdd.aspx.cs" %>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Welcome - Add
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">

    <asp:PlaceHolder runat="server" ID="ph" />
    
</asp:Content>

 

