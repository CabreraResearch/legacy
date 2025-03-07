﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28902
    /// </summary>
    public class CswUpdateSchema_01Y_Case28902 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28902; }
        }

        public override void update()
        {

            //Tie the Containers, Size and Inventory Level, Inventory Group, Inventory Group Permission and Work Unit OCs to the Containers Module and remove them from CISPro
            int containerOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.ContainerClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, containerOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, containerOC_Id );

            int sizeOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.SizeClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, sizeOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, sizeOC_Id );

            int invLvlOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.InventoryLevelClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, invLvlOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, invLvlOC_Id );

            int workUnitOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.WorkUnitClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, workUnitOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, workUnitOC_Id );

            int invGrpOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.InventoryGroupClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, invGrpOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, invGrpOC_Id );

            int invGrpPermOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.InventoryGroupPermissionClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, invGrpPermOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, invGrpPermOC_Id );

            int requestMatDispenseOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.RequestMaterialDispenseClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, requestMatDispenseOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, requestMatDispenseOC_Id );

            int requestContainerUpdateOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.RequestContainerUpdateClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, requestContainerUpdateOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, requestContainerUpdateOC_Id );

            int requestContaineDispenseOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.RequestContainerDispenseClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, requestContaineDispenseOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, requestContaineDispenseOC_Id );

            int requestMaterialCreateOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.RequestMaterialCreateClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, requestMaterialCreateOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, requestMaterialCreateOC_Id );

            int containerDispenseTransOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.ContainerDispenseTransactionClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, containerDispenseTransOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, containerDispenseTransOC_Id );

            int containerGroupOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.ContainerGroupClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, containerGroupOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, containerGroupOC_Id );

            int containerLocationOC_Id = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.ContainerLocationClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.Containers, containerLocationOC_Id );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswNbtModuleName.CISPro, containerLocationOC_Id );

            CswNbtMetaDataNodeType containerDocNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Document" );
            if( null != containerDocNT )
            {
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.Containers, containerDocNT.NodeTypeId );
                _CswNbtSchemaModTrnsctn.deleteModuleNodeTypeJunction( CswNbtModuleName.CISPro, containerDocNT.NodeTypeId );
            }

            //Tie Receive, Reconcile and Submit_Request and Legacy_Mobile_Data actions to the module (intentionally leave Kiosk Mode out)
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.Containers, CswNbtActionName.Receiving );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.Containers, CswNbtActionName.Reconciliation );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.Containers, CswNbtActionName.Submit_Request );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.Containers, CswNbtActionName.Upload_Legacy_Mobile_Data );

            //Un-tie the actions from CISPro - they belong to Containers now
            _CswNbtSchemaModTrnsctn.deleteModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.Kiosk_Mode );
            _CswNbtSchemaModTrnsctn.deleteModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.Receiving );
            _CswNbtSchemaModTrnsctn.deleteModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.Reconciliation );
            _CswNbtSchemaModTrnsctn.deleteModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.Submit_Request );
            _CswNbtSchemaModTrnsctn.deleteModuleActionJunction( CswNbtModuleName.CISPro, CswNbtActionName.Upload_Legacy_Mobile_Data );


        } //Update()

    }//class CswUpdateSchema_01Y_Case28902

}//namespace ChemSW.Nbt.Schema