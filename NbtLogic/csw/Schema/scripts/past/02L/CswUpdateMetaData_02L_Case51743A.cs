using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case51743A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 51743; }
        }

        public override string Title
        {
            get { return "Add sequence number property to batch operations"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass DesignSequenceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass );
            CswNbtMetaDataNodeType DesignSequenceNT = DesignSequenceOC.getNodeTypes().FirstOrDefault();
            if( null == DesignSequenceNT )
            {
                throw new CswDniException( CswEnumErrorType.Error, "No design sequence nodetype exists", "Cannot create a Sequence property for batch operations when no Design Sequence NT exists" );
            }
            Collection<CswNbtNode> Sequences = _getNodesByPropertyValue( DesignSequenceOC, CswNbtObjClassDesignSequence.PropertyName.Name, "BatchOpId" );
            CswNbtObjClassDesignSequence SequenceNode = null;
            if( Sequences.Count > 0 )
            {
                SequenceNode = Sequences[0];
            }
            else
            {
                SequenceNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( DesignSequenceNT.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassDesignSequence AsSequence = NewNode;
                        AsSequence.Name.Text = "BatchOpId";
                        AsSequence.Pre.Text = "Bat";
                        AsSequence.Pad.Value = 6;
                    } );
            }

            CswNbtMetaDataFieldType SequenceFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Sequence );
            CswNbtMetaDataObjectClass BatchOpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BatchOpClass );
            foreach( CswNbtMetaDataNodeType BatchOpNT in BatchOpOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp BatchOpIdNTP = BatchOpNT.getNodeTypeProp( "Batch Op Id" );
                if( null == BatchOpIdNTP )
                {
                    BatchOpIdNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( BatchOpNT, SequenceFT, "Batch Op Id" ) );
                    CswNbtMetaDataNodeTypeProp DesignSequenceNTP = BatchOpIdNTP.DesignNode.NodeType.getNodeTypeProp( "Sequence" );
                    if( null == DesignSequenceNTP )
                    {
                        throw new CswDniException(CswEnumErrorType.Error, "Cannot add sequence to Batch Operation when no Sequence relationship exists on the Sequence NTP Node", "The design node for Sequences does not have a Sequence NTP");
                    }
                    BatchOpIdNTP.DesignNode.Node.Properties[DesignSequenceNTP].AsRelationship.RelatedNodeId = SequenceNode.NodeId;
                    BatchOpIdNTP.DesignNode.postChanges( false );
                }
            }

        } // update()

        private Collection<CswNbtNode> _getNodesByPropertyValue( CswNbtMetaDataObjectClass ObjClass, string ObjClassPropName, string value )
        {
            CswNbtMetaDataObjectClassProp prop = ObjClass.getObjectClassProp( ObjClassPropName );

            CswNbtView view = _CswNbtSchemaModTrnsctn.makeNewView( "GetBatchOpsByName", CswEnumNbtViewVisibility.Hidden );
            CswNbtViewRelationship parent = view.AddViewRelationship( ObjClass, false );
            view.AddViewPropertyAndFilter( parent, prop, Value : value );

            Collection<CswNbtNode> ret = new Collection<CswNbtNode>();

            ICswNbtTree tree = _CswNbtSchemaModTrnsctn.getTreeFromView( view, false );
            for( int i = 0; i < tree.getChildNodeCount(); i++ )
            {
                tree.goToNthChild( i );
                ret.Add( tree.getNodeForCurrentPosition() );
                tree.goToParentNode();
            }

            return ret;
        }

    }

}//namespace ChemSW.Nbt.Schema