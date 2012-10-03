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
                CswNbtMetaDataNodeTypeProp LQNoNTP = _createNewProp( LQNoNt, "LQNo", CswNbtMetaDataFieldType.NbtFieldType.Text );
                LQNoNTP.setIsUnique( true );
                CswNbtMetaDataNodeTypeProp LimitNtp = _createNewProp( LQNoNt, "Limit", CswNbtMetaDataFieldType.NbtFieldType.Quantity );
                LimitNtp.IsRequired = true;
                CswNbtMetaDataNodeType WeightNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Weight)" );
                if( WeightNt != null )
                {
                    LimitNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), WeightNt.NodeTypeId );
                }
                LQNoNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "LQNo" ) );

                //UNCode NodeType
                CswNbtMetaDataNodeType UNCodeNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOc.ObjectClassId, "UN Code", "MLM" );
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, UNCodeNt.NodeTypeId );
                CswNbtMetaDataNodeTypeProp UNCodeNtp = _createNewProp( UNCodeNt, "UN Code", CswNbtMetaDataFieldType.NbtFieldType.Text );
                UNCodeNtp.setIsUnique( true );
                CswNbtMetaDataNodeTypeProp LQNoNtp = _createNewProp( UNCodeNt, "LQNo", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false );
                LQNoNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), LQNoNt.NodeTypeId );
                UNCodeNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "UN Code" ) );

                //Create Demo Data
                if( WeightNt != null )
                {
                    CswPrimaryKey kgNodeId = null;
                    foreach( CswNbtObjClassUnitOfMeasure WeightNode in WeightNt.getNodes( false, false ) )
                    {
                        if( WeightNode.Name.Text == "kg" )
                        {
                            kgNodeId = WeightNode.NodeId;
                        }
                    }
                    CswNbtNode LQNoNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( LQNoNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    LQNoNode.Properties[LQNoNTP].AsText.Text = "1 Metric Ton";
                    LQNoNode.Properties[LimitNtp].AsQuantity.Quantity = 1000;
                    LQNoNode.Properties[LimitNtp].AsQuantity.UnitId = kgNodeId;
                    LQNoNode.IsDemo = true;
                    LQNoNode.postChanges( false );

                    CswNbtNode UNCodeNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UNCodeNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    UNCodeNode.Properties[LQNoNtp].AsRelationship.RelatedNodeId = LQNoNode.NodeId;
                    UNCodeNode.Properties[UNCodeNtp].AsText.Text = "US ITH";
                    UNCodeNode.IsDemo = true;
                    UNCodeNode.postChanges( false );
                }

                //Create new Views
                CswNbtView UNCodeView = _CswNbtSchemaModTrnsctn.makeNewView( "UN Codes", NbtViewVisibility.Global );
                UNCodeView.Category = "MLM";
                UNCodeView.ViewMode = NbtViewRenderingMode.Tree;
                UNCodeView.AddViewRelationship( UNCodeNt, true );
                UNCodeView.save();

                CswNbtView LQNoView = _CswNbtSchemaModTrnsctn.makeNewView( "UN Codes by LQNo", NbtViewVisibility.Global );
                LQNoView.Category = "MLM";
                LQNoView.ViewMode = NbtViewRenderingMode.Tree;
                CswNbtViewRelationship LQNoRelationship = LQNoView.AddViewRelationship( LQNoNt, true );
                LQNoView.AddViewRelationship( LQNoRelationship, NbtViewPropOwnerType.Second, LQNoNtp, false );
                LQNoView.save();

                //Update Chemical to include UN Code
                CswNbtMetaDataNodeType ChemicalNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
                if( ChemicalNt != null )
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