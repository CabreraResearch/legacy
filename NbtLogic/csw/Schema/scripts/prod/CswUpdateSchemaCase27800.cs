using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27800
    /// </summary>
    public class CswUpdateSchemaCase27800 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            //upgrade RequestItem Requestor prop from NTP to OCP
            CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataNodeType requestItemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Item" );
            if( null != requestItemNT && null == requestItemOC.getObjectClassProp( "Requestor" ) )
            {
                CswNbtMetaDataNodeTypeProp doomedRequestorNTP = requestItemNT.getNodeTypeProp( "Requestor" );
                if( null != doomedRequestorNTP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( doomedRequestorNTP );

                    CswNbtMetaDataObjectClassProp reqItemrequestorOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( requestItemOC )
                    {
                        PropName = "Requestor",
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.PropertyReference
                    } );

                    CswNbtMetaDataNodeTypeProp reqItemRequestorNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( requestItemNT.NodeTypeId, reqItemrequestorOCP.PropId );

                    CswNbtMetaDataObjectClass requestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
                    CswNbtMetaDataObjectClassProp requestorOCP = requestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor );
                    CswNbtMetaDataObjectClassProp requestOCP = requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );
                    reqItemRequestorNTP.SetFK( NbtViewPropIdType.ObjectClassPropId.ToString(), requestOCP.PropId, NbtViewPropIdType.ObjectClassPropId.ToString(), requestorOCP.PropId );
                    reqItemRequestorNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                }
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema