using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27648
    /// </summary>
    public class CswUpdateSchema_01S_Case27648 : CswUpdateSchemaTo
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
                if( null != feedbackNTT )
                {
                    CswNbtMetaDataNodeTypeProp documentNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( feedbackNT, CswNbtMetaDataFieldType.NbtFieldType.File, "Document", feedbackNTT.TabId );

                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, feedbackNT.NodeTypeId, documentNTP.PropId, true, feedbackNTT.TabId );
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, feedbackNT.NodeTypeId, documentNTP.PropId, true, feedbackNTT.TabId );
                }
            }

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        //Update()

    }

}//namespace ChemSW.Nbt.Schema