using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02J_Case31164 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31164; }
        }

        public override string Title
        {
            get { return "Add Requesting Module"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RequestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            if( Int32.MinValue == _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.Requesting ) )
            {
                _CswNbtSchemaModTrnsctn.createModule( "Requesting", CswEnumNbtModuleName.Requesting, true );
                _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.Containers, CswEnumNbtModuleName.Requesting );
                _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswEnumNbtModuleName.Requesting, CswEnumNbtActionName.Submit_Request );

                Int32 ModuleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.Containers );
                Int32 ActionId = _CswNbtSchemaModTrnsctn.getActionId( CswEnumNbtActionName.Submit_Request );
                CswTableUpdate JctModulesATable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "SchemaModTrnsctn_ModuleJunctionUpdate", "jct_modules_actions" );
                DataTable JctModulesADataTable = JctModulesATable.getTable("where actionid = " + ActionId + " and moduleid = "+ ModuleId );
                if( JctModulesADataTable.Rows.Count > 0 )
                {
                    JctModulesADataTable.Rows[0].Delete();
                    JctModulesATable.update( JctModulesADataTable );
                }

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.Requesting, RequestOC.ObjectClassId );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.Requesting, RequestItemOC.ObjectClassId );
            }
            
            foreach( CswNbtMetaDataNodeType RequestITemNT in RequestItemOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp InventoryGroupNTP = RequestITemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
                CswNbtView InventoryGroupView = _CswNbtSchemaModTrnsctn.restoreView( InventoryGroupNTP.ViewId );
                InventoryGroupView.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
                CswNbtMetaDataObjectClassProp CentralOCP = InventoryGroupOC.getObjectClassProp( CswNbtObjClassInventoryGroup.PropertyName.Central );
                CswNbtViewRelationship IGVR = InventoryGroupView.AddViewRelationship( InventoryGroupOC, true );
                InventoryGroupView.AddViewPropertyAndFilter( IGVR, CentralOCP, FilterMode: CswEnumNbtFilterMode.Equals, Value: CswEnumTristate.True );
                InventoryGroupView.save();
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema