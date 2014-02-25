using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31192 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31192; }
        }

        private CswNbtMetaDataNodeType GHSClassNT;

        public override void update()
        {
            // Add new GHS Classifications -- Acute Toxicity (generic)
            CswNbtMetaDataObjectClass GHSClassOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClassificationClass );
            GHSClassNT = GHSClassOC.FirstNodeType;

            _addGhsClassNode( "Health", "Acute Toxicity (Category 1)" );
            _addGhsClassNode( "Health", "Acute Toxicity (Category 2)" );
            _addGhsClassNode( "Health", "Acute Toxicity (Category 3)" );
            _addGhsClassNode( "Health", "Acute Toxicity (Category 4)" );
            _addGhsClassNode( "Health", "Acute Toxicity (Category 5)" );

        } // update()

        private void _addGhsClassNode( string Category, string Classification )
        {
            _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( GHSClassNT.NodeTypeId, delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassGHSClassification GHSClassNode = NewNode;
                GHSClassNode.Category.Value = Category;
                GHSClassNode.English.Text = Classification;
            } );

        } // _addGhsClassNode()

    } // class CswUpdateSchema_02K_Case31192
} // namespace