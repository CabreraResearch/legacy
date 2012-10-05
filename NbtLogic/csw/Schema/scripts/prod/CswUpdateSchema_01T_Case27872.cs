using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27872
    /// </summary>
    public class CswUpdateSchema_01T_Case27872 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass GenericOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass );
            if( null != GenericOc )
            {
                //LQNo NodeType
                CswNbtMetaDataNodeType LQNoNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOc.ObjectClassId, "LQNo", "MLM" );
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, LQNoNt.NodeTypeId );
                CswNbtMetaDataNodeTypeProp LQNoLQNoNtp = _createNewProp( LQNoNt, "LQNo", CswNbtMetaDataFieldType.NbtFieldType.Text );
                LQNoLQNoNtp.setIsUnique( true );
                CswNbtMetaDataNodeTypeProp LQNoLimitNtp = _createNewProp( LQNoNt, "Limit", CswNbtMetaDataFieldType.NbtFieldType.Quantity );
                LQNoLimitNtp.IsRequired = true;
                CswNbtMetaDataNodeType WeightNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Weight)" );
                if( null != WeightNt )
                {
                    LQNoLimitNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), WeightNt.NodeTypeId );
                }
                LQNoNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "LQNo" ) );

                //UNCode NodeType
                CswNbtMetaDataNodeType UNCodeNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOc.ObjectClassId, "UN Code", "MLM" );
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, UNCodeNt.NodeTypeId );
                CswNbtMetaDataNodeTypeProp UNCodeUNCodeNtp = _createNewProp( UNCodeNt, "UN Code", CswNbtMetaDataFieldType.NbtFieldType.Text );
                UNCodeUNCodeNtp.setIsUnique( true );
                CswNbtMetaDataNodeTypeProp UNCodeLQNoNtp = _createNewProp( UNCodeNt, "LQNo", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false );
                UNCodeLQNoNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), LQNoNt.NodeTypeId );
                UNCodeNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "UN Code" ) );

                //Create Demo Data
                if( null != WeightNt )
                {
                    CswPrimaryKey kgNodeId = null;
                    foreach( CswNbtObjClassUnitOfMeasure WeightNode in WeightNt.getNodes( false, false ) )
                    {
                        if( "kg" == WeightNode.Name.Text )
                        {
                            kgNodeId = WeightNode.NodeId;
                        }
                    }
                    CswNbtNode LQNoNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( LQNoNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    LQNoNode.Properties[LQNoLQNoNtp].AsText.Text = "1 Metric Ton";
                    LQNoNode.Properties[LQNoLimitNtp].AsQuantity.Quantity = 1000;
                    LQNoNode.Properties[LQNoLimitNtp].AsQuantity.UnitId = kgNodeId;
                    LQNoNode.IsDemo = true;
                    LQNoNode.postChanges( false );

                    CswNbtNode UNCodeNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UNCodeNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    UNCodeNode.Properties[UNCodeLQNoNtp].AsRelationship.RelatedNodeId = LQNoNode.NodeId;
                    UNCodeNode.Properties[UNCodeUNCodeNtp].AsText.Text = "US ITH";
                    UNCodeNode.IsDemo = true;
                    UNCodeNode.postChanges( false );
                }

                //Create demo Views
                CswNbtView UNCodeView = _CswNbtSchemaModTrnsctn.makeNewView( "UN Codes", NbtViewVisibility.Global );
                UNCodeView.Category = "MLM (demo)";
                UNCodeView.IsDemo = true;
                UNCodeView.ViewMode = NbtViewRenderingMode.Tree;
                UNCodeView.AddViewRelationship( UNCodeNt, true );
                UNCodeView.save();

                CswNbtView LQNoView = _CswNbtSchemaModTrnsctn.makeNewView( "UN Codes by LQNo", NbtViewVisibility.Global );
                LQNoView.Category = "MLM (demo)";
                LQNoView.IsDemo = true;
                LQNoView.ViewMode = NbtViewRenderingMode.Tree;
                CswNbtViewRelationship LQNoRelationship = LQNoView.AddViewRelationship( LQNoNt, true );
                LQNoView.AddViewRelationship( LQNoRelationship, NbtViewPropOwnerType.Second, UNCodeLQNoNtp, false );
                LQNoView.save();

                //Update Chemical to include UN Code
                CswNbtMetaDataNodeType ChemicalNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
                if( null != ChemicalNt )
                {
                    CswNbtMetaDataNodeTypeProp ChemUNCodeNtp = _createNewProp( ChemicalNt, "UN Code", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false );
                    ChemUNCodeNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), UNCodeNt.NodeTypeId );
                }
            }
        } //Update()

        private CswNbtMetaDataNodeTypeProp _createNewProp( CswNbtMetaDataNodeType Nodetype, string PropName, CswNbtMetaDataFieldType.NbtFieldType PropType, bool SetValOnAdd = true )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( Nodetype, PropType, PropName, Nodetype.getFirstNodeTypeTab().TabId );
            if( SetValOnAdd )
            {
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add,
                    Nodetype.NodeTypeId,
                    Prop.PropId,
                    true,
                    Nodetype.getFirstNodeTypeTab().TabId
                    );
            }
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                Nodetype.NodeTypeId,
                Prop.PropId,
                true,
                Nodetype.getFirstNodeTypeTab().TabId
                );

            return Prop;
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27872; }
        }

    }//class CswUpdateSchema_01T_Case27872

}//namespace ChemSW.Nbt.Schema