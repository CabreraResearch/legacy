using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28514
    /// </summary>
    public class CswUpdateSchema_01W_Case28514 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28514; }
        }

        public override void update()
        {

            /* Owner property should be above Location property in the receive wizard */
            CswNbtMetaDataNodeType ContainerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( null != ContainerNT )
            {
                CswNbtMetaDataNodeTypeProp LocationNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
                CswNbtMetaDataNodeTypeProp OwnerNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Owner );
                CswNbtMetaDataNodeTypeProp ExpirationDateNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate );

                if( null != LocationNTP )
                {
                    LocationNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, Int32.MinValue, 8, 1 );
                }

                if( null != OwnerNTP )
                {
                    OwnerNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, Int32.MinValue, 7, 1 );
                }

                if( null != ExpirationDateNTP )
                {
                    ExpirationDateNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, Int32.MinValue, 9, 1 );
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28514

}//namespace ChemSW.Nbt.Schema