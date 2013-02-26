using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01Y_Case28671 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 28671; }
        }

        public override void update()
        {

            string UNCodeNodeTypeName = "UN Code";
            string ChemicalNodeTypeName = "Chemical";
            string LQNoNodeTypeName = "LQNo";
            string HazardsTabName = "Hazards";

            CswNbtMetaDataNodeType UNCodeNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( UNCodeNodeTypeName );
            if( null != UNCodeNodeType )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( UNCodeNodeType );
            }

            CswNbtMetaDataNodeType ChemicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( ChemicalNodeTypeName );

            if( null != ChemicalNodeType )
            {

                CswNbtMetaDataNodeTypeProp UnCodeProp = ChemicalNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.UNCode );

                if( null != UnCodeProp )
                {
                    CswNbtMetaDataNodeTypeTab HazardsTab = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeTab( ChemicalNodeType.NodeTypeId, HazardsTabName );

                    if( null != HazardsTab )
                    {
                        UnCodeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId );
                    }//if we have a hazards tab

                    UnCodeProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                }//if we have a uncode 

                //Add the LQNo prop on Chemical
                CswNbtMetaDataNodeType LQNoNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( LQNoNodeTypeName );
                if( null != LQNoNodeType )
                {
                    CswNbtMetaDataNodeTypeProp LQNoProp = ChemicalNodeType.getNodeTypeProp( LQNoNodeType.NodeTypeName );

                    if( null == LQNoProp )
                    {
                        LQNoProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Relationship, LQNoNodeType.NodeTypeName, HazardsTabName );
                    } 

                    LQNoProp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), LQNoNodeType.NodeTypeId );

                    LQNoProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                }

            }//if we have a chemcial node type




            ///_CswNbtSchemaModTrnsctn.MetaData.remove
        } //Update()

    }//class CswUpdateSchema_01Y_Case28671

}//namespace ChemSW.Nbt.Schema