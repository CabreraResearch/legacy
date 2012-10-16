using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_01T_Case27865_part2
    /// </summary>
    public class CswUpdateSchema_01T_Case27865_part2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass mepOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ManufacturerEquivalentPartClass );

            //create the MEP NT
            CswNbtMetaDataNodeType mepNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Manufacturing Equivalent Part" );
            if( null == mepNT )
            {
                mepNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( mepOC.ObjectClassId, "Manufacturing Equivalent Part", "MLM" );

                string mepNameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Manufacturer );
                mepNameTemplate += CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassManufacturerEquivalentPart.PropertyName.EnterprisePart );
                mepNameTemplate += CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Material );
                mepNT.setNameTemplateText( mepNameTemplate );

                CswNbtMetaDataNodeTypeProp materialNTP = mepNT.getNodeTypePropByObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Material );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, mepNT.NodeTypeId, materialNTP.PropId, true );

                CswNbtMetaDataNodeTypeProp epNTP = mepNT.getNodeTypePropByObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.EnterprisePart );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, mepNT.NodeTypeId, epNTP.PropId, true );

                CswNbtMetaDataNodeTypeProp manufactufacturerNTP = mepNT.getNodeTypePropByObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Manufacturer );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, mepNT.NodeTypeId, manufactufacturerNTP.PropId, true );

                //create demo MEP node
                CswNbtObjClassManufacturerEquivalentPart mepNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( mepNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );

                //get the default EP node and set it o the demo MEP nodes EP prop
                CswNbtMetaDataNodeType epNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Enterprise Part" );
                if( null != epNT )
                {
                    foreach( CswNbtObjClassEnterprisePart epNode in epNT.getNodes( false, false ) )
                    {
                        if( epNode.GCAS.Text.Equals( "EP123" ) ) //the default EP node had its GCAS set to this in CswUpdateSchema_01T_Case27865_part1
                        {
                            mepNode.EnterprisePart.RelatedNodeId = epNode.NodeId;
                        }
                    }
                }
                mepNode.IsDemo = true;
                mepNode.postChanges( false );
            }

            //create the MEPs view
            CswNbtView mepsView = _CswNbtSchemaModTrnsctn.restoreView( "Manufacturing Equivalent Parts" );
            if( null == mepsView )
            {
                mepsView = _CswNbtSchemaModTrnsctn.makeNewView( "Manufacturing Equivalent Parts", NbtViewVisibility.Global );
                mepsView.Category = "MLM (demo)";
                mepsView.ViewMode = NbtViewRenderingMode.Tree;
                mepsView.AddViewRelationship( mepOC, true );
                mepsView.IsDemo = true;
                mepsView.save();
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

    }//class CswUpdateSchema_01T_Case27865_part2

}//namespace ChemSW.Nbt.Schema