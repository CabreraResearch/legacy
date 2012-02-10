using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtUniqueEnforcement
    {
        protected CswNbtResources _CswNbtResources = null;

        /// <summary>
        /// 
        /// </summary>
        public CswNbtUniqueEnforcement( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor()

        public void validatePropValUniqueness( CswNbtNodeProp CswNbtNodeProp )
        {
            /*
            CswNbtMetaDataNodeTypeProp NodeTypeProp = CswNbtNodeProp.NodeTypeProp;

            CswNbtView CswNbtView = new CswNbtView( _CswNbtResources );
            CswNbtView.ViewName = "Other Nodes, for Property Uniqueness";

            CswNbtViewRelationship ViewRel = null;
            if( NodeTypeProp.IsGlobalUnique() )  // BZ 9754
                ViewRel = CswNbtView.AddViewRelationship( _CswNbtResources.MetaData.getObjectClassByNodeTypeId( NodeTypeProp.NodeTypeId ), false );
            else
                ViewRel = CswNbtView.AddViewRelationship( NodeTypeProp.getNodeType(), false );

            if( CswNbtNodeProp.NodeId != null )
                ViewRel.NodeIdsToFilterOut.Add( CswNbtNodeProp.NodeId );

            //bz# 5959
            CswNbtViewProperty UniqueValProperty = CswNbtView.AddViewProperty( ViewRel, NodeTypeProp );

            // BZ 10099
            NodeTypeProp.getFieldTypeRule().AddUniqueFilterToView( CswNbtView, UniqueValProperty, CswNbtNodePropData );

            ICswNbtTree NodeTree = _CswNbtResources.Trees.getTreeFromView( CswNbtView, true, true, false, false );


            if( NodeTree.getChildNodeCount() > 0 )
            {
                NodeTree.goToNthChild( 0 );
                if( !IsCopy || Required )
                {
                    CswNbtNode CswNbtNode = NodeTree.getNodeForCurrentPosition();
                    string EsotericMessage = "Unique constraint violation: The proposed value '" + this.Gestalt + "' ";
                    EsotericMessage += "of property '" + NodeTypeProp.PropName + "' ";
                    EsotericMessage += "for nodeid (" + this.NodeId.ToString() + ") ";
                    EsotericMessage += "of nodetype '" + NodeTypeProp.getNodeType().NodeTypeName + "' ";
                    EsotericMessage += "is invalid because the same value is already set for node '" + CswNbtNode.NodeName + "' (" + CswNbtNode.NodeId.ToString() + ").";
                    string ExotericMessage = "The " + NodeTypeProp.PropName + " property value must be unique";
                    throw ( new CswDniException( ErrorType.Warning, ExotericMessage, EsotericMessage ) );
                }
                else
                {
                    // BZ 9987 - Clear the value
                    this._CswNbtNodePropData.ClearValue();
                    this.clearModifiedFlag();
                }
            }
             */

        }//IsPropValUnique()


    }//CswNbtUniqueEnforcement

}//namespace ChemSW.Nbt.ObjClasses
