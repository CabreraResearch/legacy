<%@ Page Language="C#" 
         AutoEventWireup="true" 
         Inherits="ChemSW.Nbt.WebPages.License" 
         MasterPageFile="~/MainLayout.master" 
         Title="License"
         validateRequest="false"
 Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="License.aspx.cs" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    License
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MasterCenterContent" Runat="Server">
    <script language="Javascript">
        checkChangesEnabled = false;
    </script>

    <div style="text-align: center">
        Service Level Agreement
        <br /><br />
        <asp:TextBox ID="LicenseMemo" runat="server" TextMode="MultiLine" Columns="60" Rows="15" ReadOnly="true"></asp:TextBox>
        <br /><br />
        <asp:Button ID="AcceptButton" runat="server" CssClass="Button" Text="I Accept" />
        <asp:Button ID="DeclineButton" runat="server" CssClass="Button" Text="I Decline" />
    </div>
</asp:Content>

