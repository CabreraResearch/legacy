<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="ChemSW.Nbt.WebPages.Act_AssignInspection" MasterPageFile="~/MainLayout.master" Codebehind="Act_AssignInspection.aspx.cs" %>

<%@ MasterType VirtualPath="~/MainLayout.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="Server">
    Assign Inspection
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MasterCenterContent" runat="Server">
    <Csw:CswWizard runat="server" ID="AssignInspectionWizard" WizardTitle="Assign Inspection">
        <WizardSteps>
            <Csw:CswWizardStep runat="server" ID="AssignInspectionWizard_Step1" Step="1" Title="Select Inspection">
                <asp:placeholder id="Step1PH" runat="Server"/>
            </Csw:CswWizardStep>
            <Csw:CswWizardStep runat="server" ID="AssignInspectionWizard_Step2" Step="2" Title="Choose View of Targets">
                <asp:placeholder id="Step2PH" runat="Server"/>
            </Csw:CswWizardStep>
            <Csw:CswWizardStep runat="server" ID="AssignInspectionWizard_Step3" Step="3" Title="Choose Targets">
                <asp:placeholder id="Step3PH" runat="Server"/>
            </Csw:CswWizardStep>
            <Csw:CswWizardStep runat="server" ID="AssignInspectionWizard_Step4" Step="4" Title="Set Schedule">
                <asp:placeholder id="Step4PH" runat="Server" />
            </Csw:CswWizardStep>
        </WizardSteps>
    </Csw:CswWizard>
</asp:Content>
