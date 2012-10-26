using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27800
    /// </summary>
    public class CswUpdateSchema_01S_Case27800 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // moved from RunBeforeEveryExecutionOfUpdater_01OC
            CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestItemClass );
            CswNbtMetaDataNodeType requestItemNT = requestItemOC.FirstNodeType;
            if( null != requestItemNT )
            {
                CswNbtMetaDataNodeTypeProp reqItemRequestorNTP = requestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Requestor );
                if( null != reqItemRequestorNTP )
                {
                    reqItemRequestorNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                }
            }
        }//Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 27800; }
        }

    }//class CswUpdateSchemaCase27800

}//namespace ChemSW.Nbt.Schema