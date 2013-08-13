using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02E_Case29700 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 29700";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29700; }
        }

        #endregion Blame Logic

        public override string ScriptName
        {
            get { return "02E_Case29700"; }
        }

        public override bool AlwaysRun
        {
            get { return false; }
        }

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            //Put Manufacturer on add layout
            CswNbtMetaDataObjectClassProp ManufacturerOCP = ReceiptLotOC.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.Manufacturer );
            foreach( CswNbtMetaDataNodeType ReceiptLotNT in ReceiptLotOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ManufacturerNTP = ReceiptLotNT.getNodeTypePropByObjectClassProp( ManufacturerOCP );
                ManufacturerNTP.updateLayout( CswEnumNbtLayoutType.Add, false );
            }
            //Add new props to ReceiptLot
            CswNbtMetaDataObjectClassProp ManufacturerLotNoOCP = ReceiptLotOC.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.ManufacturerLotNo );
            if( null == ManufacturerLotNoOCP )
            {
                ManufacturerLotNoOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ReceiptLotOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.ManufacturerLotNo,
                    FieldType = CswEnumNbtFieldType.Text,
                    SetValOnAdd = true
                } );
            }
            CswNbtMetaDataObjectClassProp ManufacturedDateOCP = ReceiptLotOC.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.ManufacturedDate );
            if( null == ManufacturedDateOCP )
            {
                ManufacturedDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ReceiptLotOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.ManufacturedDate,
                    FieldType = CswEnumNbtFieldType.DateTime,
                    SetValOnAdd = true
                } );
            }
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
            //Upgrade AssignedCofA to OCP
            CswNbtMetaDataObjectClassProp AssignedCofAOCP = ReceiptLotOC.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.AssignedCofA );
            if( null == AssignedCofAOCP )
            {
                AssignedCofAOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ReceiptLotOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.AssignedCofA,
                    FieldType = CswEnumNbtFieldType.Grid
                } );
                CswNbtView AssignedCofAView = _CswNbtSchemaModTrnsctn.restoreView( "Assigned C of A" );
                if( null != AssignedCofAView )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AssignedCofAOCP, CswEnumNbtObjectClassPropAttributes.viewxml, AssignedCofAView.ToString() );
                }
            }
            //Remove the ManufacturerLotNo property off Container
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ManufacturerLotNoNTP = ContainerNT.getNodeTypeProp( "Manufacturer Lot Number" );
                if( null != ManufacturerLotNoNTP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( ManufacturerLotNoNTP );
                }
            }
            //Rename "C of A" module to "Manufacturer Lot Info"
            CswTableUpdate ModulesTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "ModulesSelect_29700", "modules" );
            DataTable ModulesTable = ModulesTableUpdate.getTable( "where name = 'C of A'" );
            if( ModulesTable.Rows.Count > 0 )
            {
                ModulesTable.Rows[0]["name"] = CswEnumNbtModuleName.ManufacturerLotInfo;
                ModulesTable.Rows[0]["description"] = "Manufacturer Lot Info";
                ModulesTableUpdate.update( ModulesTable );
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02E_Case29700
}//namespace ChemSW.Nbt.Schema


