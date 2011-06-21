<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.EditView" 
         MasterPageFile="~/MainLayout.master" 
         Title="Edit View"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="EditView.aspx.cs" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    View Editor
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    <script language="Javascript">
        checkChangesEnabled = false;
    </script>

    <asp:placeholder id="ViewEditorManagerPlaceholder" runat="Server" />
</asp:Content>

