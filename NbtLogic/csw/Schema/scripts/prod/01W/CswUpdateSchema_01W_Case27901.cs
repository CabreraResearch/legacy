using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27901
    /// </summary>
    public class CswUpdateSchema_01W_Case27901 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 27901; }
        }

        public override void update()
        {
            // Fix existing Relationship views to be List
            foreach( CswNbtMetaDataNodeTypeProp RelationshipNTP in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.Relationship ) )
            {
                CswNbtView RelationshipView = _CswNbtSchemaModTrnsctn.restoreView( RelationshipNTP.ViewId );
                if( RelationshipView.ViewMode == NbtViewRenderingMode.List )
                {
                    RelationshipView.ViewMode = NbtViewRenderingMode.Tree;
                    RelationshipView.save();
                }
            }
        } //Update()

    }//class CswUpdateSchema_01V_Case27901

}//namespace ChemSW.Nbt.Schema