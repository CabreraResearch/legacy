using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Properties;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27648
    /// </summary>
    public class CswUpdateSchemaCase27648 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataNodeType feedbackNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Feedback" );
            if( null != feedbackNT )
            {
                CswNbtMetaDataNodeTypeTab feedbackNTT = feedbackNT.getFirstNodeTypeTab();
                CswNbtMetaDataNodeTypeProp documentNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( feedbackNT, CswNbtMetaDataFieldType.NbtFieldType.File, "Document", feedbackNTT.TabId );

                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, feedbackNT.NodeTypeId, documentNTP.PropId, true, feedbackNTT.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, feedbackNT.NodeTypeId, documentNTP.PropId, true, feedbackNTT.TabId );
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema