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
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30251 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30251";

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
            _upgradeDepartmentToObjectClass();
            _upgradeLQNoToObjectClass();
            _upgradeControlZoneToObjectClass();
            _upgradeVendorNTPs();
            _upgradeFeedbackNTP();
            _upgradeGHSNTP();
        }

        #region ObjectClasses

        private void _upgradeDepartmentToObjectClass()
        {
            CswNbtMetaDataObjectClass DepartmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DepartmentClass );
            if( null == DepartmentOC )
            {
                DepartmentOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DepartmentClass, "folder.png", false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( DepartmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassDepartment.PropertyName.DepartmentName,
                    FieldType = CswEnumNbtFieldType.Text,
                    IsRequired = true,
                    IsUnique = true
                } );
                CswNbtMetaDataNodeType DepartmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Department" );
                if( null != DepartmentNT )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( DepartmentNT, DepartmentOC );
                }
            }
        }

        private void _upgradeLQNoToObjectClass()
        {
            CswNbtMetaDataObjectClass LQNoOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LQNoClass );
            if( null == LQNoOC )
            {
                LQNoOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.LQNoClass, "folder.png", false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( LQNoOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassLQNo.PropertyName.LQNo,
                    FieldType = CswEnumNbtFieldType.Text,
                    IsUnique = true
                } );
                CswNbtMetaDataNodeType WeightNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit_Weight" );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( LQNoOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassLQNo.PropertyName.Limit,
                    FieldType = CswEnumNbtFieldType.Quantity,
                    IsRequired = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(),
                    FkValue = WeightNt != null ? WeightNt.NodeTypeId : Int32.MinValue
                } );
                CswNbtMetaDataNodeType LQNoNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "LQNo" );
                if( null != LQNoNT )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( LQNoNT, LQNoOC );
                }
            }
        }

        private void _upgradeControlZoneToObjectClass()
        {
            CswNbtMetaDataObjectClass ControlZoneOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ControlZoneClass );
            if( null == ControlZoneOC )
            {
                ControlZoneOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.ControlZoneClass, "folder.png", false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ControlZoneOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassControlZone.PropertyName.Name,
                    FieldType = CswEnumNbtFieldType.Text,
                    IsRequired = true,
                    IsUnique = true
                } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ControlZoneOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassControlZone.PropertyName.MAQOffset,
                    FieldType = CswEnumNbtFieldType.Number,
                    NumberMinValue = 0,
                    NumberMaxValue = 100
                } );
                CswNbtMetaDataObjectClass FCEASOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountSetClass );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ControlZoneOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassControlZone.PropertyName.FireClassSetName,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsRequired = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = FCEASOC.ObjectClassId
                } );
                CswNbtMetaDataObjectClassProp LocationsOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ControlZoneOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassControlZone.PropertyName.Locations,
                    FieldType = CswEnumNbtFieldType.Grid
                } );
                CswNbtMetaDataNodeType ControlZoneNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Control Zone" );
                if( null != ControlZoneNT )
                {
                    CswNbtMetaDataNodeTypeProp CZLocationsNTP = ControlZoneNT.getNodeTypeProp( "Locations" );
                    if( null != CZLocationsNTP )
                    {
                        CswNbtView CZLocationsView = _CswNbtSchemaModTrnsctn.restoreView( CZLocationsNTP.ViewId );
                        if( null != CZLocationsView )
                        {
                            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocationsOCP, CswEnumNbtObjectClassPropAttributes.viewxml, CZLocationsView.ToString() );
                        }
                    }
                    _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( ControlZoneNT, ControlZoneOC );
                }
            }
        }

        #endregion ObjectClasses

        #region ObjectClassProps

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

        private void _upgradeFeedbackNTP()
        {
            CswNbtMetaDataObjectClass FeedbackOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.FeedbackClass );
            _addOCP( FeedbackOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassFeedback.PropertyName.Document,
                FieldType = CswEnumNbtFieldType.File
            } );
        }

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

        private void _addOCP( CswNbtMetaDataObjectClass OC, CswNbtWcfMetaDataModel.ObjectClassProp PropDef )
        {
            CswNbtMetaDataObjectClassProp OCP = OC.getObjectClassProp( PropDef.PropName );
            if( OCP == null )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( OC, PropDef );
            }
        }

        #endregion ObjectClassProps

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30251
}//namespace ChemSW.Nbt.Schema


