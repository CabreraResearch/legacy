using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29499
    /// </summary>
    public class CswUpdateSchema_02D_Case29499: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29499; }
        }

        public override void update()
        {

            //Last Modified By and Last Modified On are only on Edit layouts
            CswNbtMetaDataPropertySet DocumentPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.DocumentSet );
            foreach( CswNbtMetaDataObjectClass DocumentOC in DocumentPS.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType DocumentNT in DocumentOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeTab firstTab = DocumentNT.getFirstNodeTypeTab();

                    CswNbtMetaDataNodeTypeProp LastModifiedOnNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.LastModifiedOn );
                    LastModifiedOnNTP.removeFromAllLayouts();
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, DocumentNT.NodeTypeId, LastModifiedOnNTP, false, firstTab.TabId );

                    CswNbtMetaDataNodeTypeProp LastModifiedByNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.LastModifiedBy );
                    LastModifiedByNTP.removeFromAllLayouts();
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, DocumentNT.NodeTypeId, LastModifiedByNTP, false, firstTab.TabId );
                }
            }

        } // update()

    }//class CswUpdateSchema_02C_Case29499

}//namespace ChemSW.Nbt.Schema