using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52799 : CswUpdateNbtMasterSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 52799; }
        }

        public override string Title
        {
            get { return "Permissions: the Reckoning"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void doUpdate()
        {
            //getting NTs by name is acceptable, because this is a master only script
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                _CswNbtSchemaModTrnsctn.ConfigVbls.setConfigVariableValue( "license_type", "1" );

                CswNbtMetaDataNodeType AssemblyDocNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly Document" );
                CswNbtMetaDataNodeType AssemblyScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly Schedule" );
                CswNbtMetaDataNodeType BatchOpNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Batch Operation" );
                CswNbtMetaDataNodeType BiologicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Biological" );
                CswNbtMetaDataNodeType BoxNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Box" );
                CswNbtMetaDataNodeType BuildingNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Building" );
                CswNbtMetaDataNodeType CofANT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "C of A Document" );
                CswNbtMetaDataNodeType CabinetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Cabinet" );
                CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
                CswNbtMetaDataNodeType ConstituentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Constituent" );
                CswNbtMetaDataNodeType ContainerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
                CswNbtMetaDataNodeType ContainerDispenseNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Dispense Transaction" );
                CswNbtMetaDataNodeType ContainerDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Document" );
                CswNbtMetaDataNodeType ContainerGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Group" );
                CswNbtMetaDataNodeType ContainerLocationNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Location" );
                CswNbtMetaDataNodeType ControlZoneNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Control Zone" );
                CswNbtMetaDataNodeType DepartmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Department" );
                CswNbtMetaDataNodeType DSDPhraseNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "DSD Phrase" );
                CswNbtMetaDataNodeType EquipmentDocNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Document" );
                CswNbtMetaDataNodeType EquipmentScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Schedule" );
                CswNbtMetaDataNodeType FeedbackNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Feedback" );
                CswNbtMetaDataNodeType FireClassNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Fire Class Exempt Amount" );
                CswNbtMetaDataNodeType FireClassSetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Fire Class Exempt Amount Set" );
                CswNbtMetaDataNodeType FloorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Floor" );
                CswNbtMetaDataNodeType GHSNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "GHS" );
                CswNbtMetaDataNodeType GHSClassificationNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "GHS Classification" );
                CswNbtMetaDataNodeType GHSPhraseNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "GHS Phrase" );
                CswNbtMetaDataNodeType GHSSignalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "GHS Signal Word" );
                CswNbtMetaDataNodeType IMCSReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "IMCS Report" );
                CswNbtMetaDataNodeType InspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inspection Schedule" );
                CswNbtMetaDataNodeType InventoryLevelNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Level" );
                CswNbtMetaDataNodeType InventoryGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Group" );
                CswNbtMetaDataNodeType InventoryGroupPermissionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Group Permission" );
                CswNbtMetaDataNodeType JurisdictionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Jurisdiction" );
                CswNbtMetaDataNodeType LQNoNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "LQNo" );
                CswNbtMetaDataNodeType MailReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Mail Report" );
                CswNbtMetaDataNodeType MailReportGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Mail Report Group" );
                CswNbtMetaDataNodeType MailReportGroupPermissionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Mail Report Group Permission" );
                CswNbtMetaDataNodeType MaterialComponentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Component" );
                CswNbtMetaDataNodeType MaterialDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
                CswNbtMetaDataNodeType MaterialSynonymNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Synonym" );
                CswNbtMetaDataNodeType PrintJobNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Print Job" );
                CswNbtMetaDataNodeType PrintLabelNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Print Label" );
                CswNbtMetaDataNodeType PrinterNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Printer" );
                CswNbtMetaDataNodeType ReceiptLotNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Receipt Lot" );
                CswNbtMetaDataNodeType RegulatoryListNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Regulatory List" );
                CswNbtMetaDataNodeType RegulatoryListCASNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Regulatory List CAS" );
                CswNbtMetaDataNodeType RegulatoryListListCodeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Regulatory List List Code" );
                CswNbtMetaDataNodeType RegulatoryListMemberNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Regulatory List Member" );
                CswNbtMetaDataNodeType ReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );
                CswNbtMetaDataNodeType ReportGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report Group" );
                CswNbtMetaDataNodeType ReportGroupPermissionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report Group Permission" );
                CswNbtMetaDataNodeType RequestNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request" );
                CswNbtMetaDataNodeType RequestItemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Item" );
                CswNbtMetaDataNodeType RoleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Role" );
                CswNbtMetaDataNodeType RoomNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
                CswNbtMetaDataNodeType SDSNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SDS Document" );
                CswNbtMetaDataNodeType ShelfNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Shelf" );
                CswNbtMetaDataNodeType SIReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SI Report" );
                CswNbtMetaDataNodeType SiteNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Site" );
                CswNbtMetaDataNodeType SizeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
                CswNbtMetaDataNodeType SupplyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Supply" );
                CswNbtMetaDataNodeType UnitRadiationNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit Radiation" );
                CswNbtMetaDataNodeType UnitTimeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit Time" );
                CswNbtMetaDataNodeType UnitEachNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit_Each" );
                CswNbtMetaDataNodeType UnitWeightNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit_Volume" );
                CswNbtMetaDataNodeType UnitVolumeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit_Weight" );
                CswNbtMetaDataNodeType UserNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "User" );
                CswNbtMetaDataNodeTypeTab ProfileTab = UserNT.getNodeTypeTab( "Profile" );
                CswNbtMetaDataNodeType VendorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Vendor" );
                CswNbtMetaDataNodeType WorkUnitNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Work Unit" );

                {
                    //Cispro_Admin
                    CswNbtObjClassRole AdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, AssemblyDocNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyDocNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, AssemblyDocNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, AssemblyDocNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, AssemblyScheduleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyScheduleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, AssemblyScheduleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, AssemblyScheduleNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, CofANT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, CofANT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, CofANT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, CofANT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, ContainerNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ContainerDispenseNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ContainerGroupNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerGroupNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ContainerGroupNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, ContainerGroupNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerLocationNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ControlZoneNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ControlZoneNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ControlZoneNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, ControlZoneNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, DSDPhraseNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, DSDPhraseNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, DSDPhraseNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, EquipmentDocNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentDocNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, EquipmentDocNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, EquipmentDocNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, EquipmentScheduleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentScheduleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, EquipmentScheduleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, EquipmentScheduleNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, FeedbackNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, FeedbackNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, FeedbackNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, FeedbackNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, FireClassNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, FireClassNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, FireClassNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, FireClassSetNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, FireClassSetNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, FireClassSetNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, FloorNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, FloorNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, FloorNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, GHSClassificationNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, GHSPhraseNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, GHSPhraseNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, GHSPhraseNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, GHSSignalNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, IMCSReportNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, IMCSReportNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, IMCSReportNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, IMCSReportNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, InspectionScheduleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InspectionScheduleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, InspectionScheduleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, InspectionScheduleNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, JurisdictionNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, JurisdictionNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, JurisdictionNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, JurisdictionNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, LQNoNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, LQNoNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, LQNoNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, PrintJobNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrintJobNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, PrintJobNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, PrintJobNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, PrinterNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrinterNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, PrinterNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ReceiptLotNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, ReceiptLotNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReceiptLotNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ReceiptLotNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RequestNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RequestNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, RequestNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RequestItemNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestItemNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RequestItemNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, RequestItemNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RoleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RoleNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, RoleNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RequestNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RequestNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, RequestNT, AdminRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, UnitRadiationNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, UnitRadiationNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, UnitRadiationNT, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, UnitRadiationNT, AdminRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, UserNT, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, UserNT, AdminRole, false );


                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Login_Data, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Manage_Locations, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Tier_II_Reporting, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Upload_Legacy_Mobile_Data, AdminRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.ChemWatch, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Merge, AdminRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Submit_Request, AdminRole, false );

                    AdminRole.Administrator.Checked = CswEnumTristate.False;
                    AdminRole.postChanges( false );

                    //also make 'admin' a CISPro_Admin
                    CswNbtObjClassUser AdminUser = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( "admin" );
                    AdminUser.Role.RelatedNodeId = AdminRole.NodeId;
                    AdminUser.postChanges( true );

                    //then delete the old admin role
                    CswNbtObjClassRole OldAdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
                    OldAdminRole.Node.delete();
                }

                {
                    //CISPro_Receiver
                    CswNbtObjClassRole ReceiverRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Receiver" );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyDocNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyScheduleNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, BatchOpNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, BatchOpNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, BiologicalNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, CofANT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, CofANT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, ChemicalNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, ConstituentNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ContainerDispenseNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ContainerDocumentNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ContainerGroupNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerGroupNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ContainerGroupNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerLocationNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, DepartmentNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentDocNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentScheduleNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, FeedbackNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, FeedbackNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, GHSNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, GHSNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, GHSClassificationNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, GHSSignalNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, IMCSReportNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InspectionScheduleNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InventoryGroupPermissionNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, InventoryLevelNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, InventoryLevelNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, JurisdictionNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, JurisdictionNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, JurisdictionNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, LQNoNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, LQNoNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, LQNoNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportGroupNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportGroupPermissionNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, MaterialComponentNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, MaterialDocumentNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, MaterialSynonymNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, MaterialSynonymNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, PrintJobNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrintJobNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrinterNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ReceiptLotNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ReceiptLotNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReceiptLotNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListCASNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListListCodeNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RegulatoryListMemberNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListMemberNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportGroupNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportGroupPermissionNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RequestNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RequestNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, RequestNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RequestItemNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestItemNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RequestItemNT, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, RequestItemNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, SDSNT, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, SDSNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, SIReportNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, SizeNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, SupplyNT, ReceiverRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, UnitRadiationNT, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ProfileTab, ReceiverRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, VendorNT, ReceiverRole, false );


                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.DispenseContainer, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.DisposeContainer, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Kiosk_Mode, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Upload_Legacy_Mobile_Data, ReceiverRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Edit_View, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Multi_Edit, ReceiverRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Submit_Request, ReceiverRole, false );

                    ReceiverRole.Timeout.Value = 15;
                    ReceiverRole.Administrator.Checked = CswEnumTristate.False;
                    ReceiverRole.postChanges( false );
                }

                {
                    //CISPro_General
                    CswNbtObjClassRole GeneralRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_General" );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyDocNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyScheduleNT, GeneralRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, CofANT, GeneralRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ConstituentNT, GeneralRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ContainerDispenseNT, GeneralRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ContainerDocumentNT, GeneralRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ContainerDocumentNT, GeneralRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerGroupNT, GeneralRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerLocationNT, GeneralRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ControlZoneNT, GeneralRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentDocNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentScheduleNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, FeedbackNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, GHSPhraseNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, IMCSReportNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InspectionScheduleNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InventoryGroupPermissionNT, GeneralRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportGroupNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportGroupPermissionNT, GeneralRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, MaterialComponentNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, MaterialComponentNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, MaterialComponentNT, GeneralRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, PrintJobNT, GeneralRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrintJobNT, GeneralRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrinterNT, GeneralRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReceiptLotNT, GeneralRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListMemberNT, GeneralRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportGroupNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportGroupPermissionNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestNT, GeneralRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestItemNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RequestItemNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RequestItemNT, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, RequestItemNT, GeneralRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, SIReportNT, GeneralRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, SDSNT, GeneralRole, true );



                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Kiosk_Mode, GeneralRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Edit_View, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Multi_Edit, GeneralRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Submit_Request, GeneralRole, false );

                    GeneralRole.Timeout.Value = 10;
                    GeneralRole.postChanges( false );
                }

                {
                    //CISPro_View_Only
                    CswNbtObjClassRole ViewRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_View_Only" );


                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyDocNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyScheduleNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, BatchOpNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, CofANT, ViewRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ConstituentNT, ViewRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerGroupNT, ViewRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerLocationNT, ViewRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ControlZoneNT, ViewRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentDocNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentScheduleNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, FeedbackNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, GHSPhraseNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, IMCSReportNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InspectionScheduleNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InventoryGroupPermissionNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportGroupNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportGroupPermissionNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrintLabelNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReceiptLotNT, ViewRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListMemberNT, ViewRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportGroupNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportGroupPermissionNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestNT, ViewRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, SDSNT, ViewRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, UnitTimeNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, UnitEachNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, UnitWeightNT, ViewRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, UnitVolumeNT, ViewRole, false );

                    ViewRole.Timeout.Value = 8;
                    ViewRole.postChanges( false );
                }

                {
                    //CISPro_Request_Filler
                    CswNbtObjClassRole FulfillerRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Request_Fulfiller" );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyDocNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, AssemblyScheduleNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, BatchOpNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, BiologicalNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, BoxNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, BuildingNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, CofANT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, CabinetNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ChemicalNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ConstituentNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, ContainerNT, FulfillerRole, false );


                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerDispenseNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ContainerDispenseNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerDocumentNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ContainerDocumentNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ContainerDocumentNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerGroupNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ContainerGroupNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ContainerGroupNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ContainerLocationNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, DepartmentNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentDocNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, EquipmentScheduleNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, FeedbackNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, FloorNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, GHSNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, GHSPhraseNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, IMCSReportNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InspectionScheduleNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InventoryGroupNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InventoryGroupPermissionNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InventoryLevelNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, MaterialComponentNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, MaterialComponentNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, MaterialComponentNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportGroupNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MailReportGroupPermissionNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MaterialDocumentNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, MaterialDocumentNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, MaterialDocumentNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, MaterialSynonymNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrintJobNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, PrintJobNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrinterNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, PrintLabelNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReceiptLotNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, ReceiptLotNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListCASNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListListCodeNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RegulatoryListMemberNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RegulatoryListMemberNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportGroupNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ReportGroupPermissionNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, InventoryGroupNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestItemNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RequestItemNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RequestItemNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, RequestItemNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RoomNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, SDSNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, SDSNT, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, SDSNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, ShelfNT, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, SIReportNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, SIReportNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, SIReportNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, SIReportNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, SiteNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, SiteNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, SiteNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, SizeNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, SizeNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, SupplyNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, UnitRadiationNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, UnitTimeNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, UnitEachNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, UnitVolumeNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, UnitWeightNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, UnitRadiationNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, UnitTimeNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, UnitEachNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, UnitVolumeNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, UnitWeightNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, UnitRadiationNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, UnitTimeNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, UnitEachNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, UnitVolumeNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, UnitWeightNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, UserNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, UserNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, ProfileTab, FulfillerRole, true );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, VendorNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, VendorNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, VendorNT, FulfillerRole, false );

                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, WorkUnitNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, WorkUnitNT, FulfillerRole, false );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, WorkUnitNT, FulfillerRole, false );


                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.DispenseContainer, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.DisposeContainer, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Kiosk_Mode, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Multi_Edit, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Submit_Request, FulfillerRole, true );
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Subscriptions, FulfillerRole, true );
                }
            }
        }//update()


    }
}