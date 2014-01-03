using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30008
    /// </summary>
    public class CswUpdateSchema_02D_Case30008 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30008; }
        }

        public override void update()
        {
            // Add nodetypes for the new object class
            CswNbtMetaDataObjectClass RegListListCodeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
            CswNbtMetaDataObjectClassProp RegListListCodeRegListOCP = RegListListCodeOC.getObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.RegulatoryList );

            CswNbtMetaDataNodeType RegListListCodeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Regulatory List List Code" );
            if( null == RegListListCodeNT )
            {
                RegListListCodeNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeTypeDeprecated( new CswNbtWcfMetaDataModel.NodeType( RegListListCodeOC )
                    {
                        NodeTypeName = "Regulatory List List Code",
                        Category = "Materials",
                        SearchDeferObjectClassPropId = RegListListCodeRegListOCP.ObjectClassPropId
                    } );
            }

            // Set the Name Template
            RegListListCodeNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListListCode.PropertyName.RegulatoryList )
                + "-"
                + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListName ) );

            // Hide the LOLI List Code Property from Users
            CswNbtMetaDataNodeTypeProp RegListListCodeListCodeNTP = RegListListCodeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListCode );
            RegListListCodeListCodeNTP.removeFromAllLayouts();

        } // update()

    }//class CswUpdateSchema_02D_Case30008

}//namespace ChemSW.Nbt.Schema