<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.Location" 
         MasterPageFile="~/MainLayout.master" 
         Title="Location"
 Codebehind="Location.aspx.cs" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    View By Location
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    
    <div runat="server" id="MenuDiv" class="MenuDiv">
        <asp:PlaceHolder runat="server" ID="MainMenuPlaceHolder" />
    </div>
    <div class="LocationDiv">
        <asp:PlaceHolder runat="server" ID="LocationNavigatorPlaceHolder" />
    </div>

    
    <div runat="server" id="MainContentDiv" class="BottomPanelDiv">
        <asp:PlaceHolder runat="server" ID="PropGridPlaceHolder" />
    </div>
    
</asp:Content>
