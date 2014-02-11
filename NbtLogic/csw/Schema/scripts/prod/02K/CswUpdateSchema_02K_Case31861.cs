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
    public class CswUpdateSchema_02K_Case31861: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31861; }
        }

        public override string Title
        {
            get { return "Create Sequence for Item Number property on Request Items"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            CswNbtMetaDataObjectClassProp ItemNoOCP = RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.ItemNumber );

            //First verify there is no sequence for any ItemNo NTPs
            Collection<CswNbtMetaDataNodeTypeProp> PropsWithNoSequences = new Collection<CswNbtMetaDataNodeTypeProp>();
            foreach( CswNbtMetaDataNodeTypeProp ItemNoNTP in ItemNoOCP.getNodeTypeProps() )
            {
                if( null == ItemNoNTP.Sequence )
                {
                    PropsWithNoSequences.Add( ItemNoNTP );
                }
            }

            if( PropsWithNoSequences.Any() )
            {
                CswNbtMetaDataObjectClass DesignSequenceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass );
                CswNbtMetaDataNodeType DesignSequenceNT = DesignSequenceOC.getNodeTypes().FirstOrDefault();
                if( null == DesignSequenceNT )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Error creating new Sequence for Item Number property", "Cannot create a new sequence because there is no DesignSequence NodeType" );
                }

                CswNbtObjClassDesignSequence ItemNoSequence = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( DesignSequenceNT.NodeTypeId, OnAfterMakeNode : delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassDesignSequence AsSequence = NewNode;
                        AsSequence.Name.Text = "RequestItem_ItemNumber";
                        AsSequence.Pre.Text = "RI";
                        AsSequence.Pad.Value = 6;
                    } );

                foreach( CswNbtMetaDataNodeTypeProp SequenceslessProp in PropsWithNoSequences )
                {
                    //Update the NTP to have a sequence
                    CswNbtObjClassDesignNodeTypeProp DesignNodeProp = SequenceslessProp.DesignNode;
                    CswNbtMetaDataNodeTypeProp SequenceNTP = DesignNodeProp.NodeType.getNodeTypeProp( "Sequence" );
                    if( null == SequenceNTP )
                    {
                        throw new CswDniException(CswEnumErrorType.Error, "Error assigning sequence to Item Number property", "Cannot assign a sequence to the Item Number property because there is no Sequence relationship property to assign a value to");
                    }
                    DesignNodeProp.Node.Properties[SequenceNTP].AsRelationship.RelatedNodeId = ItemNoSequence.NodeId;
                    DesignNodeProp.postChanges( false );

                    //Update existing Request Items Item No prop to have a value
                    _updateRequestItems(SequenceslessProp.getNodeType(), SequenceslessProp);
                }
            }

        } // update()

        private void _updateRequestItems( CswNbtMetaDataNodeType RequestItemNT, CswNbtMetaDataNodeTypeProp ItemNumberNTP )
        {
            CswNbtView RequestItems = _CswNbtSchemaModTrnsctn.makeNewView( "RequestItemsNoItemNo31861", CswEnumNbtViewVisibility.Hidden );
            CswNbtViewRelationship Parent = RequestItems.AddViewRelationship( RequestItemNT, false );
            RequestItems.AddViewPropertyAndFilter( Parent, ItemNumberNTP, FilterMode : CswEnumNbtFilterMode.Null );

            ICswNbtTree RequestItemsTree = _CswNbtSchemaModTrnsctn.getTreeFromView( RequestItems, false );
            for( int i = 0; i < RequestItemsTree.getChildNodeCount(); i++ )
            {
                RequestItemsTree.goToNthChild( i );
                CswNbtObjClassRequestItem RequestItem = RequestItemsTree.getNodeForCurrentPosition();
                RequestItem.ItemNumber.setSequenceValue();
                RequestItem.postChanges( false );
                RequestItemsTree.goToParentNode();
            }
        }

    }

}//namespace ChemSW.Nbt.Schema