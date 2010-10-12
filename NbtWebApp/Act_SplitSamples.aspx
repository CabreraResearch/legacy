<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Act_SplitSamples.aspx.cs"
    Inherits="ChemSW.Nbt.WebPages.Act_SplitSamples" MasterPageFile="~/MainLayout.master"%>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="Server">
    Split Samples
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MasterCenterContent" runat="Server">

    <Csw:CswWizard runat="server" ID="SplitSamplesWizard" WizardTitle="Split Samples">
        <WizardSteps>
            <Csw:CswWizardStep runat="server" ID="SplitSamplesWizard_Step1" Step="1" Title="Scan Sample">
                <asp:placeholder id="Step1PH" runat="Server"/>
            </Csw:CswWizardStep>
            <Csw:CswWizardStep runat="server" ID="SplitSamplesWizard_Step2" Step="2" Title="Define Children">
                <asp:placeholder id="Step2PH" runat="Server"/>
            </Csw:CswWizardStep>
        </WizardSteps>
    </Csw:CswWizard>

</asp:Content>
