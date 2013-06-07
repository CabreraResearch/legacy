using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29807
    /// </summary>
    public class CswUpdateSchema_02B_Case29807: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29807; }
        }

        public override void update()
        {

            CswNbtMetaDataPropertySet materialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass objectClass in materialPS.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType nodeType in objectClass.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeTab documentsTab = nodeType.getNodeTypeTab( "Documents" );
                    CswNbtMetaDataNodeTypeProp documentsNT = nodeType.getNodeTypeProp( "Documents" );
                    if( null != documentsNT && null != documentsTab )
                    {
                        _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, nodeType.NodeTypeId, documentsNT, true, documentsTab.TabId );
                    }
                }
            }

        } // update()

    }//class CswUpdateSchema_02B_Case29807

}//namespace ChemSW.Nbt.Schema