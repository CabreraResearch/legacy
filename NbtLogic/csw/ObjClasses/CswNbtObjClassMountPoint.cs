using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMountPoint : CswNbtObjClass, ICswNbtPropertySetInspectionParent
    {
        public static string LastInspectionDatePropertyName { get { return "Last Inspection Date"; } }
        public static string StatusPropertyName { get { return "Status"; } }
        public static string LocationPropertyName { get { return "Location"; } }
        public static string DescriptionPropertyName { get { return "Description"; } }
        public static string TypePropertyName { get { return "Type"; } }
        public static string BarcodePropertyName { get { return "Barcode"; } }
        public static string MountPointGroupPropertyName { get { return "Mount Point Group"; } }

        //ICswNbtPropertySetInspectionParent
        public string InspectionParentStatusPropertyName { get { return StatusPropertyName; } }
        public string InspectionParentLastInspectionDatePropertyName { get { return LastInspectionDatePropertyName; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassMountPoint( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public CswNbtObjClassMountPoint( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MountPointClass ); }
        }

        #region Inherited Events

        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtNode MountPointGroupNode = _CswNbtResources.Nodes.GetNode( this.MountPointGroup.RelatedNodeId );

            if( null != MountPointGroupNode )
            {
                CswNbtView SchedulesView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship GeneratorRelationship = SchedulesView.AddViewRelationship( GeneratorOC, false );
                CswNbtViewProperty OwnerProperty = SchedulesView.AddViewProperty( GeneratorRelationship, GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.OwnerPropertyName ) );
                CswNbtViewPropertyFilter OwnerPropFilter = SchedulesView.AddViewPropertyFilter( OwnerProperty, 
                                                                                                CswNbtSubField.SubFieldName.NodeID, 
                                                                                                CswNbtPropFilterSql.PropertyFilterMode.Equals, 
                                                                                                MountPointGroupNode.NodeId.PrimaryKey.ToString(), 
                                                                                                false );
                ICswNbtTree SchedulesTree = _CswNbtResources.Trees.getTreeFromView( SchedulesView, true, true, false, false );
                SchedulesTree.goToRoot();

                
                //CswDelimitedString NodeTypeIds = new CswDelimitedString(',');

                //For each generator with this Mount Point's MPG
                for( Int32 i = 0; i < SchedulesTree.getChildNodeCount(); i++ )
                {
                    SchedulesTree.goToNthChild( i );
                    CswNbtNode ScheduleNode = SchedulesTree.getNodeForCurrentPosition();
                    CswNbtObjClassGenerator ScheduleOC = CswNbtNodeCaster.AsGenerator( ScheduleNode );

                    CswCommaDelimitedString NodeTypeIds = ScheduleOC.TargetType.SelectedNodeTypeIds;
                    //For each target node type on the generator
                    foreach( String NtId in NodeTypeIds )
                    {
                        CswNbtMetaDataNodeType InspectionNT = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NtId ) );
                        if( null != InspectionNT )
                        {
                            //For the past interval. Scheduler will handle current interval.
                            CswNbtNode PastInspectionNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( InspectionNT.LatestVersionNodeType.NodeTypeId,
                                                                                                          CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                            if( null != PastInspectionNode )
                            {
                                CswNbtObjClassInspectionDesign InspectionOC = CswNbtNodeCaster.AsInspectionDesign( PastInspectionNode );
                                InspectionOC.Owner.RelatedNodeId = this.NodeId;
                                InspectionOC.Generator.RelatedNodeId = ScheduleNode.NodeId;
                                CswRateInterval ScheduleInterval = ScheduleOC.DueDateInterval.RateInterval;
                                InspectionOC.Date.DateValue = ScheduleInterval.getPrevious( ScheduleOC.NextDueDate.DateValue );
                                PastInspectionNode.postChanges( true );
                            }

                        }
                    }// for( Int32 n = 0; n < NodeTypeIds.Count; n++ )

                    SchedulesTree.goToParentNode();

                } // for( Int32 i = 0; i < SchedulesTree.getChildNodeCount(); i++ )
            } // if( null != MountPointGroupNode )

            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            _CswNbtObjClassDefault.beforeWriteNode();
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

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

        #endregion

        #region Object class specific properties

        /// <summary>
        /// Date of last Inspection
        /// </summary>
        public CswNbtNodePropDate LastInspectionDate
        {
            get
            {
                return ( _CswNbtNode.Properties[LastInspectionDatePropertyName].AsDate );
            }
        }

        /// <summary>
        /// Mount Point Inspection Status (OK, OOC)
        /// </summary>
        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[StatusPropertyName].AsList );
            }
        }

        /// <summary>
        /// Location of Mount Point
        /// </summary>
        public CswNbtNodePropLocation Location
        {
            get
            {
                return ( _CswNbtNode.Properties[LocationPropertyName].AsLocation );
            }
        }

        public CswNbtNodePropList Type
        {
            get
            {
                return ( _CswNbtNode.Properties[TypePropertyName].AsList );
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

        public CswNbtNodePropRelationship MountPointGroup
        {
            get
            {
                return ( _CswNbtNode.Properties[MountPointGroupPropertyName].AsRelationship );
            }
        }
        
        #endregion



    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
