<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Act_AssignTests.aspx.cs"
    Inherits="ChemSW.Nbt.WebPages.Act_AssignTests" MasterPageFile="~/MainLayout.master"%>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="Server">
    Assign Tests
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MasterCenterContent" runat="Server">
    <Csw:CswWizard runat="server" ID="AssignTestsWizard" WizardTitle="Assign Tests">
        <WizardSteps>
            <Csw:CswWizardStep runat="server" ID="AssignTestsWizard_Step1" Step="1" Title="Select Samples View">
                <asp:placeholder id="Step1PH" runat="Server"/>
            </Csw:CswWizardStep>
            <Csw:CswWizardStep runat="server" ID="AssignTestsWizard_Step2" Step="2" Title="Choose Samples">
                <asp:placeholder id="Step2PH" runat="Server"/>
            </Csw:CswWizardStep>
            <Csw:CswWizardStep runat="server" ID="AssignTestsWizard_Step3" Step="3" Title="Select Tests View">
                <asp:placeholder id="Step3PH" runat="Server" />
            </Csw:CswWizardStep>
            <Csw:CswWizardStep runat="server" ID="AssignTestsWizard_Step4" Step="4" Title="Choose Tests">
                <asp:placeholder id="Step4PH" runat="Server" />
            </Csw:CswWizardStep>
        </WizardSteps>
    </Csw:CswWizard>
</asp:Content>
