using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30251B : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30251B";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30251; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            _upgradeVendorNTPs();
            _upgradeFeedbackNTP();
            _upgradeGHSNTP();
            _upgradeTaskNTPs();
            _upgradeSizeNTPs();
            _upgradeEquipmentTypeNTP();
            _upgradeProblemNTPs();
            _upgradeRequestNTP();
            _upgradeRegulatoryListNTP();
            _upgradePrinterNTP();
            _upgradeLocationNTP();
            _upgradeContainerNTPs();
            _upgradeInventoryGroupNTPs();
            _upgradeChemicalNTP();
            _upgradeMaterialSetOCNTPs();
            _upgradeNonChemicalNTP();
        }

        #region Vendor

        private void _upgradeVendorNTPs()
        {
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.AccountNo,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.DeptBillCode,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.ContactName,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.Street1,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.Street2,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.City,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.State,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.Zip,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.Phone,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( VendorOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassVendor.PropertyName.Fax,
                FieldType = CswEnumNbtFieldType.Text
            } );
        }

        #endregion Vendor

        #region Feedback

        private void _upgradeFeedbackNTP()
        {
            CswNbtMetaDataObjectClass FeedbackOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.FeedbackClass );
            _addOCP( FeedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassFeedback.PropertyName.Document,
                FieldType = CswEnumNbtFieldType.File
            } );
        }

        #endregion Feedback

        #region GHS

        private void _upgradeGHSNTP()
        {
            CswNbtMetaDataObjectClass GHSOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            CswCommaDelimitedString ClassificationOptions = new CswCommaDelimitedString
            {
                "Category 1", 
                "Category 2", 
                "Category 3", 
                "Category 4", 
                "Category 1 A/1 B/1 C", 
                "Category 2 (skin)/2A (eye)",
                "Category 2A",
                "Category 2B",
                "Category 1/ 1A / 1B",
                "Category 1A or Category 1B",
                "Type A",
                "Type B",
                "Type C&D",
                "Type E&F",
                "Compressed Gas",
                "Liquidfied Gas",
                "Dissolved Gas",
                "Refridgerated Liquidified Gas"
            };
            _addOCP( GHSOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassGHS.PropertyName.Classification,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = ClassificationOptions.ToString()
            } );
        }

        #endregion GHS

        #region Task

        private void _upgradeTaskNTPs()
        {
            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.TaskClass );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.CalibrationDate,
                FieldType = CswEnumNbtFieldType.DateTime
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.CalibrationResult,
                FieldType = CswEnumNbtFieldType.Number
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.CompletionDescription,
                FieldType = CswEnumNbtFieldType.Memo
            } );
            CswNbtMetaDataObjectClass DepartmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DepartmentClass );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.Department,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = DepartmentOC.ObjectClassId
            } );
            CswCommaDelimitedString EventTypeOptions = new CswCommaDelimitedString
            {
                "Calibration", 
                "Inspection",
                "Maintenance"
            };
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.EventType,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = EventTypeOptions.ToString(),
                SetValOnAdd = true
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.FileLink,
                FieldType = CswEnumNbtFieldType.Link
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.IgnoreCalibrationResult,
                FieldType = CswEnumNbtFieldType.Logical
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.IsCriticalTest,
                FieldType = CswEnumNbtFieldType.Logical
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.LaborCost,
                FieldType = CswEnumNbtFieldType.Text
            } );
            //Can't do Location because the related NodeTypeProp is different for each Task NodeType
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.LowerLimit,
                FieldType = CswEnumNbtFieldType.Number
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.OtherCost,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.OtherCostName,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.PartsCost,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.SOPRef,
                FieldType = CswEnumNbtFieldType.Text
            } );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp TechnicianOCP = _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.Technician,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = UserOC.ObjectClassId
            } );
            CswNbtMetaDataObjectClassProp UserPhoneOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Phone );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.TechnicianPhone,
                FieldType = CswEnumNbtFieldType.PropertyReference,
                IsFk = true,
                FkType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                FkValue = TechnicianOCP.PropId,
                ValuePropId = UserPhoneOCP.PropId,
                ValuePropType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString()
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.TravelCost,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( TaskOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassTask.PropertyName.UpperLimit,
                FieldType = CswEnumNbtFieldType.Number
            } );
        }

        #endregion Task

        #region Size

        private void _upgradeSizeNTPs()
        {
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
            CswCommaDelimitedString ContainerTypeOptions = new CswCommaDelimitedString
            {
                "Aboveground Tank [A]",
                "Bag [J]",
                "Belowground Tank [B]",
                "Box [K]",
                "Can [F]",
                "Carboy [G]",
                "Cylinder [L]",
                "Fiberdrum [I]",
                "Glass Bottle or Jug [M]",
                "Plastic [N]",
                "Plastic or Non-Metal Drum [E]",
                "Steel Drum [D]",
                "Tank Inside Building [C]",
                "Tank Wagon [P]",
                "Tote Bin [O]"
            };
            _addOCP( SizeOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassSize.PropertyName.ContainerType,
                FieldType = CswEnumNbtFieldType.List,
                ListOptions = ContainerTypeOptions.ToString()
            } );
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp SupplierOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.Supplier );
            CswNbtMetaDataObjectClassProp MaterialOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
            _addOCP( SizeOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassSize.PropertyName.Supplier,
                FieldType = CswEnumNbtFieldType.PropertyReference,
                IsFk = true,
                FkType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                FkValue = MaterialOCP.PropId,
                ValuePropId = SupplierOCP.PropId,
                ValuePropType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString()
            } );
        }

        #endregion Size

        #region EquipmentType

        private void _upgradeEquipmentTypeNTP()
        {
            CswNbtMetaDataObjectClass EquipmentTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentTypeClass );
            _addOCP( EquipmentTypeOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassEquipmentType.PropertyName.TypeName,
                FieldType = CswEnumNbtFieldType.Text,
                IsRequired = true
            } );
        }

        #endregion EquipmentType

        #region Problem

        private void _upgradeProblemNTPs()
        {
            CswNbtMetaDataObjectClass ProblemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ProblemClass );
            CswNbtMetaDataObjectClass DepartmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DepartmentClass );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.Department,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = DepartmentOC.ObjectClassId
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.LaborCost,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.OtherCost,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.OtherCostName,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.PartsCost,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.Problem,
                FieldType = CswEnumNbtFieldType.Memo
            } );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp UserPhoneOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Phone );
            CswNbtMetaDataObjectClassProp ReportedByOCP = ProblemOC.getObjectClassProp( CswNbtObjClassProblem.PropertyName.ReportedBy );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.ReporterPhone,
                FieldType = CswEnumNbtFieldType.PropertyReference,
                IsFk = true,
                FkType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                FkValue = ReportedByOCP.PropId,
                ValuePropId = UserPhoneOCP.PropId,
                ValuePropType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString()
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.Resolution,
                FieldType = CswEnumNbtFieldType.Memo
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.StartDate,
                FieldType = CswEnumNbtFieldType.DateTime
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.Summary,
                FieldType = CswEnumNbtFieldType.Text
            } );            
            CswNbtMetaDataObjectClassProp TechnicianOCP = _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.Technician,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = UserOC.ObjectClassId
            } );            
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.TechnicianPhone,
                FieldType = CswEnumNbtFieldType.PropertyReference,
                IsFk = true,
                FkType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                FkValue = TechnicianOCP.PropId,
                ValuePropId = UserPhoneOCP.PropId,
                ValuePropType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString()
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.TravelCost,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.UnderWarranty,
                FieldType = CswEnumNbtFieldType.Logical
            } );
            _addOCP( ProblemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassProblem.PropertyName.WorkOrderPrinted,
                FieldType = CswEnumNbtFieldType.Logical
            } );
        }

        #endregion Problem

        #region Request

        private void _upgradeRequestNTP()
        {
            CswNbtMetaDataObjectClass RequestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
            _createGridOCPFromNTP( RequestOC, CswNbtObjClassRequest.PropertyName.RequestItems );
        }

        #endregion Request

        #region RegulatoryList

        private void _upgradeRegulatoryListNTP()
        {
            CswNbtMetaDataObjectClass RegulatoryListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            _createGridOCPFromNTP( RegulatoryListOC, CswNbtObjClassRegulatoryList.PropertyName.Chemicals );
        }

        #endregion RegulatoryList

        #region Printer

        private void _upgradePrinterNTP()
        {
            CswNbtMetaDataObjectClass PrinterOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrinterClass );
            _createGridOCPFromNTP( PrinterOC, CswNbtObjClassPrinter.PropertyName.Jobs );
        }

        #endregion Printer

        #region Location

        private void _upgradeLocationNTP()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            _createGridOCPFromNTP( LocationOC, CswNbtObjClassLocation.PropertyName.InventoryLevels );
        }

        #endregion Location

        #region Container

        private void _upgradeContainerNTPs()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            _createGridOCPFromNTP( ContainerOC, CswNbtObjClassContainer.PropertyName.ContainerDispenseTransactions );
            _createGridOCPFromNTP( ContainerOC, CswNbtObjClassContainer.PropertyName.Documents );
            _createGridOCPFromNTP( ContainerOC, CswNbtObjClassContainer.PropertyName.SubmittedRequests );
        }

        #endregion Container

        #region InventoryGroup

        private void _upgradeInventoryGroupNTPs()
        {
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            _addOCP( InventoryGroupOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassInventoryGroup.PropertyName.Description,
                FieldType = CswEnumNbtFieldType.Memo
            } );
            _createGridOCPFromNTP( InventoryGroupOC, CswNbtObjClassInventoryGroup.PropertyName.Locations );
            _createGridOCPFromNTP( InventoryGroupOC, CswNbtObjClassInventoryGroup.PropertyName.Permissions );
        }

        #endregion InventoryGroup

        #region Chemical

        private void _upgradeChemicalNTP()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClass LQNoOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LQNoClass );
            _addOCP( ChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassChemical.PropertyName.LQNo,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = LQNoOC.ObjectClassId
            } );
        }

        #endregion Chemical

        #region NonChemical

        private void _upgradeNonChemicalNTP()
        {
            CswNbtMetaDataObjectClass NonChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.NonChemicalClass );
            _addOCP( NonChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassNonChemical.PropertyName.Picture,
                FieldType = CswEnumNbtFieldType.Image
            } );
        }

        #endregion NonChemical

        #region Material

        private void _upgradeMaterialSetOCNTPs()
        {
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialPS.getObjectClasses() )
            {
                _createGridOCPFromNTP( MaterialOC, CswNbtPropertySetMaterial.PropertyName.Documents );
                _createGridOCPFromNTP( MaterialOC, CswNbtPropertySetMaterial.PropertyName.Synonyms );
            }
        }

        #endregion Material

        #region Private

        private CswNbtMetaDataObjectClassProp _addOCP( CswNbtMetaDataObjectClass OC, CswNbtWcfMetaDataModel.ObjectClassProp PropDef )
        {
            CswNbtMetaDataObjectClassProp OCP = OC.getObjectClassProp( PropDef.PropName );
            if( OCP == null )
            {
                OCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( OC, PropDef );
            }
            return OCP;
        }

        private void _createGridOCPFromNTP( CswNbtMetaDataObjectClass OC, String GridPropName )
        {
            CswNbtMetaDataObjectClassProp GridOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( OC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = GridPropName,
                FieldType = CswEnumNbtFieldType.Grid
            } );
            CswNbtMetaDataNodeType NT = OC.FirstNodeType;
            if( null != NT )
            {
                CswNbtMetaDataNodeTypeProp GridNTP = NT.getNodeTypeProp( GridPropName );
                if( null != GridNTP )
                {
                    CswNbtView GridView = _CswNbtSchemaModTrnsctn.restoreView( GridNTP.ViewId );
                    if( null != GridView )
                    {
                        _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GridOCP, CswEnumNbtObjectClassPropAttributes.viewxml, GridView.ToString() );
                    }
                }
            }
        }

        #endregion Private

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30251B
}//namespace ChemSW.Nbt.Schema


