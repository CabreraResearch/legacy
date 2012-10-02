using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27691
    /// </summary>
    public class CswUpdateSchema_01S_Case27691 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            /* Kindly note that PART 1 of this case has been spun off into it's own case (27763) */

            /* Kindly note that PART 2 of this case is a non-issue */

            /* PART 3 - add the Requestor of the Request NT as a property reference to the RequestItem NT */
            CswNbtMetaDataNodeType requestItemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Item" );
            //if( null != requestItemNT )
            //{
            //    CswNbtMetaDataNodeTypeTab requestItemNTT = requestItemNT.getFirstNodeTypeTab();
            //    if( null != requestItemNTT )
            //    {
            //        CswNbtMetaDataObjectClass requestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            //        CswNbtMetaDataObjectClassProp requestorOCP = requestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor );

            //        CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            //        CswNbtMetaDataObjectClassProp requestOCP = requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );

            //        CswNbtMetaDataNodeTypeProp reqItemRequestorNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp(
            //            NodeType: requestItemNT,
            //            FieldType: CswNbtMetaDataFieldType.NbtFieldType.PropertyReference,
            //            PropName: "Requestor",
            //            TabId: requestItemNTT.TabId );

            //        reqItemRequestorNTP.SetFK( NbtViewPropIdType.ObjectClassPropId.ToString(), requestOCP.PropId, NbtViewPropIdType.ObjectClassPropId.ToString(), requestorOCP.PropId );
            //        reqItemRequestorNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            //    }
            //}

            //NOTE -  the above is commented out due to Case 27800

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        //Update()
    }

}//namespace ChemSW.Nbt.Schema