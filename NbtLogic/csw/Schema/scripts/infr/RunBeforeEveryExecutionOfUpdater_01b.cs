using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01b : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region SEBASTIAN

            // case 27703 - change containers dispose/dispense buttons to say "Dispose this Container" and "Dispense this Container"
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );

            CswNbtMetaDataObjectClassProp dispenseOCP = containerOC.getObjectClassProp( "Dispense" );
            if( null != dispenseOCP ) //have to null check because property might have already been updated
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( dispenseOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, "Dispense this Container" );
            }

            CswNbtMetaDataObjectClassProp disposeOCP = containerOC.getObjectClassProp( "Dispose" );
            if( null != disposeOCP ) //have to null check here because property might have been updated
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( disposeOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, "Dispose this Container" );
            }

            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp ControlTypeOcp = PrintLabelOc.getObjectClassProp( "Control Type" );
            if( null != ControlTypeOcp )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( ControlTypeOcp, DeleteNodeTypeProps: true );
            }

            //upgrade RequestItem Requestor prop from NTP to OCP
            CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataNodeType requestItemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Item" );
            if( null != requestItemNT && null == requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Requestor ) )
            {

                CswNbtMetaDataObjectClass requestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
                CswNbtMetaDataObjectClassProp requestorOCP = requestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor );
                CswNbtMetaDataObjectClassProp requestOCP = requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );

                CswNbtMetaDataObjectClassProp reqItemrequestorOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( requestItemOC )
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Requestor,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.PropertyReference,
                    IsFk = true,
                    FkType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                    FkValue = requestOCP.PropId,
                    ValuePropType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                    ValuePropId = requestorOCP.PropId
                } );

                CswNbtMetaDataNodeTypeProp reqItemRequestorNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( requestItemNT.NodeTypeId, reqItemrequestorOCP.PropId );

                reqItemRequestorNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }


            #endregion SEBASTIAN

            #region TITANIA

            #region Case 27869 - Method Object Class

            CswNbtMetaDataObjectClass MethodOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MethodClass );
            if( null == MethodOc )
            {
                //Create new ObjectClass
                MethodOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MethodClass, "newicons/16/barchart.png", true, false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MethodOc )
                    {
                        PropName = CswNbtObjClassMethod.PropertyName.MethodNo,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                        IsRequired = true,
                        IsUnique = true,
                        SetValOnAdd = true
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MethodOc )
                    {
                        PropName = CswNbtObjClassMethod.PropertyName.MethodDescription,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                        SetValOnAdd = true
                    } );

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, MethodOc.ObjectClassId );
            }

            CswNbtMetaDataNodeType MethodNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Method" );
            if( null == MethodNt )
            {
                //Create new NodeType
                MethodNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( MethodOc.ObjectClassId, "Method", "MLM" );
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, MethodNt.NodeTypeId );

                string NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMethod.PropertyName.MethodNo );
                MethodNt.setNameTemplateText( NameTemplate );

                //Create Demo Data
                CswNbtObjClassMethod MethodNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MethodNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                MethodNode.MethodNo.Text = "000001";
                MethodNode.MethodDescription.Text = "Demo Method";
                MethodNode.IsDemo = true;
                MethodNode.postChanges( false );
            }

            CswNbtView MethodView = _CswNbtSchemaModTrnsctn.restoreView( "Methods" );
            if( null == MethodView )
            {
                //Create new View
                MethodView = _CswNbtSchemaModTrnsctn.makeNewView( "Methods", NbtViewVisibility.Global );
                MethodView.Category = "MLM";
                MethodView.ViewMode = NbtViewRenderingMode.Tree;
                MethodView.AddViewRelationship( MethodOc, true );
                MethodView.save();
            }

            #endregion Case 27869 - Method Object Class

            #endregion TITANIA


        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01b

}//namespace ChemSW.Nbt.Schema


