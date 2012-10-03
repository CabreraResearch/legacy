using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27873
    /// </summary>
    public class CswUpdateSchema_01T_Case27873 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass JurisdictionOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.JurisdictionClass );
            if( null != JurisdictionOc )
            {
                CswNbtMetaDataNodeType JurisdictionNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Jurisdiction" );
                if( null == JurisdictionNt )
                {
                    //Create new NodeType
                    JurisdictionNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( JurisdictionOc.ObjectClassId, "Jurisdiction", "MLM" );
                    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, JurisdictionNt.NodeTypeId );
                    JurisdictionNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassJurisdiction.PropertyName.Name ) );

                    //Create Demo Data
                    CswNbtObjClassJurisdiction JurisdictionNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( JurisdictionNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    JurisdictionNode.Name.Text = "Default Jurisdiction";
                    JurisdictionNode.IsDemo = true;
                    JurisdictionNode.postChanges( false );
                }

                CswNbtView JurisdictionView = _CswNbtSchemaModTrnsctn.restoreView( "Jurisdictions" );
                if( null == JurisdictionView )
                {
                    //Create new View
                    JurisdictionView = _CswNbtSchemaModTrnsctn.makeNewView( "Jurisdictions", NbtViewVisibility.Global );
                    JurisdictionView.Category = "MLM";
                    JurisdictionView.ViewMode = NbtViewRenderingMode.Tree;
                    JurisdictionView.AddViewRelationship( JurisdictionOc, true );
                    JurisdictionView.save();
                }
            }
        } //Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27873; }
        }

    }//class CswUpdateSchema_01T_Case27873

}//namespace ChemSW.Nbt.Schema