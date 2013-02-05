using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28725
    /// </summary>
    public class CswUpdateSchema_01W_Case28725 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28725; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
            {
                if( MaterialNT.NodeTypeName == "Biological" )
                {
                    CswNbtMetaDataNodeTypeProp PictureNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( MaterialNT.NodeTypeId, "Picture" );
                    if( null != PictureNTP )
                    {
                        PictureNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
                    }
                    CswNbtMetaDataNodeTypeProp RefNoNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( MaterialNT.NodeTypeId, "Reference Number" );
                    if( null != RefNoNTP )
                    {
                        RefNoNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
                    }
                    CswNbtMetaDataNodeTypeProp TypeNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( MaterialNT.NodeTypeId, "Type" );
                    if( null != TypeNTP )
                    {
                        TypeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
                    }
                }
                else if( MaterialNT.NodeTypeName == "Chemical" )
                {
                    CswNbtMetaDataNodeTypeProp CasNoNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( MaterialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.CasNo );
                    CasNoNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
                }
            }
        }//Update()
    }//class CswUpdateSchemaCase_01W_28725
}//namespace ChemSW.Nbt.Schema