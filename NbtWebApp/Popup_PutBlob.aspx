<%@ Page Language="C#" 
         AutoEventWireup="true"  
         CodeFile="Popup_PutBlob.aspx.cs" 
         Inherits="ChemSW.Nbt.WebPages.Popup_PutBlob" 
         MasterPageFile="~/PopupLayout.master" 
         Title="Upload"
%>

<%@ MasterType VirtualPath="~/PopupLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" Runat="Server">
    Upload
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MasterContent" Runat="Server">
<%--
    // TODO: This should live in Main.js
--%>   
    <script language="Javascript"> 

//    var changed = 0;

//    function setPutBlobChanged() {
//        changed = 1;
//    }
//    function unsetPutBlobChanged() {
//        changed = 0;
//    }

//    function checkChanges() {
//        if (changed == 1) {
//            return 'If you continue, you will lose any changes made on this page.  Be sure to save your changes before continuing.';
//        }
//    }

//    function initCheckChanges()
//    {
//        // Assign the checkchanges event to happen onbeforeunload
//        if ((window.onbeforeunload !== null) && (window.onbeforeunload !== undefined)) {
//            window.onbeforeunload = new Function('var f=' + window.onbeforeunload + '; var ret = f(); if(ret) { return checkChanges(); } else { return false; }');
//        } else {
//            window.onbeforeunload = function() { return checkChanges(); };
//        }

//        // Assign the Save Button to ignore changes (since clicking Save will save changes)
//        var sb = document.getElementById('<%=Submit.ClientID %>');
//        if(sb != null)
//        {   
//            if((sb.onclick !== null) && (sb.onclick !== undefined)) {
//                sb.onclick = new Function('unsetPutBlobChanged(); var f=' + sb.onclick + '; return f(); ');
//            } else {
//                sb.onclick = function() { unsetPutBlobChanged(); return true; };
//            }
//        }

//        // IE6 has this annoying habit of throwing unspecified errors if we prevent
//        // the navigation with onbeforeunload after clicking a button.
//        // So we're going to trap this error and prevent it from being shown.
//        window.onerror = function(strError,uri,line) {
//          if (strError.toLowerCase().indexOf('unspecified error') >= 0) {
//            window.event.returnValue = true;
//          } else {
//            window.event.returnValue = false;
//          }
//        }
//    }
//    if ((window.onload !== null) && (window.onload !== undefined)) {
//        window.onload = new Function('initCheckChanges(); var f=' + window.onload + '; return f();');
//    } else {
//        window.onload = function() { initCheckChanges(); };
//    }

     </script>            

    <asp:PlaceHolder ID="ContentHolder" runat="server"></asp:PlaceHolder>
    <br /><br />
    <asp:Button ID="Submit" runat="server" CssClass="Button" OnClick="Submit_Click" Text="Save" />
    <asp:Button runat="server" ID="Cancel" CssClass="Button" Text="Cancel" OnClientClick="Popup_Cancel_Clicked();" />

</asp:Content>
