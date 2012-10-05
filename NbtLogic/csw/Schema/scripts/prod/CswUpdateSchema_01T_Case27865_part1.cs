using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_01T_Case27865_part1
    /// </summary>
    public class CswUpdateSchema_01T_Case27865_part1 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass enterprisePartOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EnterprisePartClass );


            //create the EP nodetype
            CswNbtMetaDataNodeType enterprisePartNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Enterprise Part" );
            if( null == enterprisePartNT )
            {
                enterprisePartNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( enterprisePartOC.ObjectClassId, "Enterprise Part", "MLM" );

                string epnameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassEnterprisePart.PropertyName.GCAS );
                enterprisePartNT.setNameTemplateText( epnameTemplate );

                CswNbtMetaDataNodeTypeProp gcasNTP = enterprisePartNT.getNodeTypePropByObjectClassProp( CswNbtObjClassEnterprisePart.PropertyName.GCAS );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, enterprisePartNT.NodeTypeId, gcasNTP.PropId, true );

                //create the demo EP node
                CswNbtObjClassEnterprisePart epNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( enterprisePartNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                epNode.GCAS.Text = "EP123";
                epNode.IsDemo = true;
                epNode.postChanges( false );
            }

            //create the EPs view
            CswNbtView enterprisePartsView = _CswNbtSchemaModTrnsctn.restoreView( "Enterprise Parts" );
            if( null == enterprisePartsView )
            {
                enterprisePartsView = _CswNbtSchemaModTrnsctn.makeNewView( "Enterprise Parts", NbtViewVisibility.Global );
                enterprisePartsView.Category = "MLM (demo)";
                enterprisePartsView.ViewMode = NbtViewRenderingMode.Tree;
                enterprisePartsView.AddViewRelationship( enterprisePartOC, true );
                enterprisePartsView.IsDemo = true;
                enterprisePartsView.save();
            }

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27865; }
        }

        //Update()

    }//class CswUpdateSchemaCaseCswUpdateSchema_01T_Case27865_part1

}//namespace ChemSW.Nbt.Schema