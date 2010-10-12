<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Act_FutureScheduling.aspx.cs"
    Inherits="ChemSW.Nbt.WebPages.Act_FutureScheduling" MasterPageFile="~/MainLayout.master"%>

<%@ MasterType VirtualPath="~/MainLayout.master" %>



<asp:Content ID="Content4" ContentPlaceHolderID="TitleContent" runat="Server">
    Generate Future Schedule Content
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="MasterCenterContent" runat="Server">
    <script language="Javascript">
        checkChangesEnabled = false;
    </script>
    <Csw:CswWizard runat="server" ID="FutureSchedulingWizard" WizardTitle="Generate Future Schedule Content">
        <WizardSteps>
            <Csw:CswWizardStep runat="server" ID="FutureSchedulingWizard_Step_1" Step="1" Title="Specify Schedules">
                <asp:placeholder id="StepOnePlaceHolder" runat="Server"/>
            </Csw:CswWizardStep>
            <Csw:CswWizardStep runat="server" ID="FutureSchedulingWizard_Step_2" Step="2" Title="Review">
                <asp:placeholder id="StepTwoPlaceHolder" runat="Server" />
            </Csw:CswWizardStep>
        </WizardSteps>
    </Csw:CswWizard>
</asp:Content>
