using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27800
    /// </summary>
    public class CswUpdateSchema_01S_Case27800 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // moved from RunBeforeEveryExecutionOfUpdater_01b
            CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
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

    }//class CswUpdateSchema_01S_Case27800

}//namespace ChemSW.Nbt.Schema