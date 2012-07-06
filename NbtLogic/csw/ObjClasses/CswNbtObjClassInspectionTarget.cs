using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionTarget : CswNbtObjClass, ICswNbtPropertySetInspectionParent
    {
        //public static string LastInspectionDatePropertyName { get { return "Last Inspection Date"; } }
        public static string StatusPropertyName { get { return "Status"; } }
        public static string LocationPropertyName { get { return "Location"; } }
        public static string DescriptionPropertyName { get { return "Description"; } }
        public static string BarcodePropertyName { get { return "Barcode"; } }
        public static string InspectionTargetGroupPropertyName { get { return "Inspection Target Group"; } }

        //ICswNbtPropertySetInspectionParent
        public string InspectionParentStatusPropertyName { get { return StatusPropertyName; } }
        //public string InspectionParentLastInspectionDatePropertyName { get { return LastInspectionDatePropertyName; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInspectionTarget( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionTarget
        /// </summary>
        public static implicit operator CswNbtObjClassInspectionTarget( CswNbtNode Node )
        {
            CswNbtObjClassInspectionTarget ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass ) )
            {
                ret = (CswNbtObjClassInspectionTarget) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            // Case 20947: We don't want to create past inspections automatically (for now). Maybe this should be configurable later?

            //CswNbtMetaDataObjectClass GeneratorOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            //CswNbtNode InspectionTargetGroupNode = _CswNbtResources.Nodes.GetNode( this.InspectionTargetGroup.RelatedNodeId );

            //if( null != InspectionTargetGroupNode )
            //{
            //    CswNbtView SchedulesView = new CswNbtView( _CswNbtResources );
            //    CswNbtViewRelationship GeneratorRelationship = SchedulesView.AddViewRelationship( GeneratorOC, false );
            //    CswNbtViewProperty OwnerProperty = SchedulesView.AddViewProperty( GeneratorRelationship, GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.OwnerPropertyName ) );
            //    CswNbtViewPropertyFilter OwnerPropFilter = SchedulesView.AddViewPropertyFilter( OwnerProperty, 
            //                                                                                    CswNbtSubField.SubFieldName.NodeID, 
            //                                                                                    CswNbtPropFilterSql.PropertyFilterMode.Equals, 
            //                                                                                    InspectionTargetGroupNode.NodeId.PrimaryKey.ToString(), 
            //                                                                                    false );
            //    ICswNbtTree SchedulesTree = _CswNbtResources.Trees.getTreeFromView( SchedulesView, true, true, false, false );
            //    SchedulesTree.goToRoot();


            //    //CswDelimitedString NodeTypeIds = new CswDelimitedString(',');

            //    //For each generator with this Inspection Target's MPG
            //    for( Int32 i = 0; i < SchedulesTree.getChildNodeCount(); i++ )
            //    {
            //        SchedulesTree.goToNthChild( i );
            //        CswNbtNode ScheduleNode = SchedulesTree.getNodeForCurrentPosition();
            //        CswNbtObjClassGenerator ScheduleOC = (CswNbtObjClassGenerator) ScheduleNode;

            //        CswCommaDelimitedString NodeTypeIds = ScheduleOC.TargetType.SelectedNodeTypeIds;
            //        //For each target node type on the generator
            //        foreach( String NtId in NodeTypeIds )
            //        {
            //            CswNbtMetaDataNodeType InspectionNT = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NtId ) );
            //            if( null != InspectionNT )
            //            {
            //                //For the past interval. Scheduler will handle current interval.
            //                CswNbtNode PastInspectionNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( InspectionNT.LatestVersionNodeType.NodeTypeId,
            //                                                                                              CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            //                if( null != PastInspectionNode )
            //                {
            //                    CswNbtObjClassInspectionDesign InspectionOC = (CswNbtObjClassInspectionDesign) PastInspectionNode;
            //                    InspectionOC.Owner.RelatedNodeId = this.NodeId;
            //                    InspectionOC.Generator.RelatedNodeId = ScheduleNode.NodeId;
            //                    CswRateInterval ScheduleInterval = ScheduleOC.DueDateInterval.RateInterval;
            //                    InspectionOC.Date.DateValue = ScheduleInterval.getPrevious( ScheduleOC.NextDueDate.DateValue );
            //                    PastInspectionNode.postChanges( true );
            //                }

            //            }
            //        }// for( Int32 n = 0; n < NodeTypeIds.Count; n++ )

            //        SchedulesTree.goToParentNode();

            //    } // for( Int32 i = 0; i < SchedulesTree.getChildNodeCount(); i++ )
            //} // if( null != InspectionTargetGroupNode )

            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode(bool DeleteAllRequiredRelatedNodes = false)
        {
            _CswNbtObjClassDefault.beforeDeleteNode(DeleteAllRequiredRelatedNodes);
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out JObject ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = new JObject();
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties
/*
        /// <summary>
        /// Date of last Inspection
        /// </summary>
        public CswNbtNodePropDateTime LastInspectionDate
        {
            get
            {
                return ( _CswNbtNode.Properties[LastInspectionDatePropertyName].AsDateTime );
            }
        }
        */

        /// <summary>
        /// Inspection Target Inspection Status (OK, Deficient)
        /// </summary>
        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[StatusPropertyName].AsList );
            }
        }

        /// <summary>
        /// Location of Inspection Target
        /// </summary>
        public CswNbtNodePropLocation Location
        {
            get
            {
                return ( _CswNbtNode.Properties[LocationPropertyName].AsLocation );
            }
        }

        public CswNbtNodePropText Description
        {
            get
            {
                return ( _CswNbtNode.Properties[DescriptionPropertyName].AsText );
            }
        }

        public CswNbtNodePropBarcode Barcode
        {
            get
            {
                return ( _CswNbtNode.Properties[BarcodePropertyName].AsBarcode );
            }
        }

        public CswNbtNodePropRelationship InspectionTargetGroup
        {
            get
            {
                return ( _CswNbtNode.Properties[InspectionTargetGroupPropertyName].AsRelationship );
            }
        }

        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
