using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActReconciliation
    {
        private CswNbtResources _CswNbtResources;
        private ContainerData Data;
        private ICswNbtTree ContainersTree;

        public CswNbtActReconciliation( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Data = new ContainerData();
        }

        public ContainerData getReconciliationData( ContainerData.ReconciliationRequest Request )
        {
            getContainerStatistics( Request );
            getContainerStatuses( Request );
            return Data;
        }

        public ContainerData getContainerStatistics( ContainerData.ReconciliationRequest Request )
        {
            for( int i = 0; i < CswNbtObjClassContainerLocation.StatusOptions._All.Count(); i++ )
            {
                Data.ContainerStatistics.Add( new ContainerData.ReconciliationStatistics() );
                Data.ContainerStatistics[i].ContainerCount = 0;
                Data.ContainerStatistics[i].AmountScanned = 0;
                Data.ContainerStatistics[i].Status = CswNbtObjClassContainerLocation.StatusOptions._All.ToArray()[i].ToString();
            }
            _setContainersTree( Request );
            if( ContainersTree.getChildNodeCount() > 0 )
            {
                for( int i = 0; i < ContainersTree.getChildNodeCount(); i++ )//Location Nodes
                {
                    ContainersTree.goToNthChild( i );
                    if( ContainersTree.getChildNodeCount() > 0 )
                    {
                        for( int j = 0; j < ContainersTree.getChildNodeCount(); j++ )//Container Nodes
                        {
                            ContainersTree.goToNthChild( j );
                            if( ContainersTree.getChildNodeCount() > 0 )//ContainerLocation Nodes
                            {
                                ContainersTree.goToNthChild( 0 );
                                CswNbtObjClassContainerLocation ContainerLocationNode = ContainersTree.getNodeForCurrentPosition();
                                _incrementContainerCount( Data.ContainerStatistics, ContainerLocationNode.Status.Value, ContainerLocationNode.ContainerScan.Text );
                                ContainersTree.goToParentNode();
                            }
                            else
                            {
                                _incrementContainerCount( Data.ContainerStatistics, CswNbtObjClassContainerLocation.StatusOptions.NotScanned.ToString() );
                            }
                            ContainersTree.goToParentNode();
                        }
                    }
                    ContainersTree.goToParentNode();
                }
            }
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.ContainerCount > 0 )
                {
                    Stat.PercentScanned = CswConvert.ToDouble(Stat.AmountScanned) / CswConvert.ToDouble(Stat.ContainerCount) * 100.0;
                }
                else
                {
                    Stat.PercentScanned = 0.0;
                }
            }
            return Data;
        }

        public ContainerData getContainerStatuses( ContainerData.ReconciliationRequest Request )
        {
            _setContainersTree( Request );
            if( ContainersTree.getChildNodeCount() > 0 )
            {
                for( int i = 0; i < ContainersTree.getChildNodeCount(); i++ )//Location Nodes
                {
                    ContainersTree.goToNthChild( i );
                    if( ContainersTree.getChildNodeCount() > 0 )
                    {
                        for( int j = 0; j < ContainersTree.getChildNodeCount(); j++ )//Container Nodes
                        {
                            ContainerData.ReconciliationStatuses ContainerStatus = new ContainerData.ReconciliationStatuses();
                            ContainersTree.goToNthChild( j );
                            CswNbtObjClassContainer ContainerNode = ContainersTree.getNodeForCurrentPosition();
                            ContainerStatus.ContainerId = ContainerNode.NodeId.ToString();
                            ContainerStatus.ContainerBarcode = ContainerNode.Barcode.Barcode;
                            if( ContainersTree.getChildNodeCount() > 0 )//ContainerLocation Nodes
                            {
                                ContainersTree.goToNthChild( 0 );
                                CswNbtObjClassContainerLocation ContainerLocationNode = ContainersTree.getNodeForCurrentPosition();
                                ContainerStatus.ContainerStatus = ContainerLocationNode.Status.Value;
                                ContainerStatus.Action = ContainerLocationNode.Action.Value;
                                ContainerStatus.ActionApplied = ContainerLocationNode.ActionApplied.Checked.ToString();
                                ContainersTree.goToParentNode();
                            }
                            else
                            {
                                ContainerStatus.ContainerStatus = CswNbtObjClassContainerLocation.StatusOptions.NotScanned.ToString();
                            }
                            Data.ContainerStatuses.Add( ContainerStatus );
                            ContainersTree.goToParentNode();
                        }
                    }
                    ContainersTree.goToParentNode();
                }
            }
            return Data;
        }

        private void _setContainersTree( ContainerData.ReconciliationRequest Request )
        {
            if( null == ContainersTree )
            {
                try//TODO - remove try/catch block when Case 28194 is resolved
                {
                    ContainersTree = _CswNbtResources.Trees.getTreeFromView( _getReconciliationView( Request ), false, true, false );
                }
                catch( Exception ex )
                {
                    String ErrorMessage = "Treeloader error occured.";
                    if( ex.StackTrace.Contains( "ORA-01795" ) )
                    {
                        ErrorMessage += " Too many child locations.";
                    }
                    throw new CswDniException( ErrorType.Error, "Unable to get Reconciliation data.", ErrorMessage, ex );
                }
                
            }
        }

        private CswNbtView _getReconciliationView( ContainerData.ReconciliationRequest Request )
        {
            Collection<CswPrimaryKey> LocationIds = _getLocationIds( Request );

            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp LocationOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
            CswNbtMetaDataObjectClass ContainerLocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataObjectClassProp ContainerOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );
            CswNbtMetaDataObjectClassProp ScanDateOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ScanDate );

            CswNbtView ContainersView = new CswNbtView( _CswNbtResources );

            CswNbtViewRelationship LocationVR = ContainersView.AddViewRelationship( LocationOC, false );
            LocationVR.NodeIdsToFilterIn = LocationIds;
            CswNbtViewRelationship ContainerVR = ContainersView.AddViewRelationship( LocationVR, NbtViewPropOwnerType.Second, LocationOCP, false );
            CswNbtViewRelationship ContainerLocationVR = ContainersView.AddViewRelationship( ContainerVR, NbtViewPropOwnerType.Second, ContainerOCP, false );
            CswNbtViewProperty ScanDateVP = ContainersView.AddViewProperty( ContainerLocationVR, ScanDateOCP );
            ContainersView.AddViewPropertyFilter( ScanDateVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.GreaterThan, Value: Request.StartDate );
            ContainersView.AddViewPropertyFilter( ScanDateVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThan, Value: Request.EndDate );
            ContainersView.setSortProperty( ScanDateVP, NbtViewPropertySortMethod.Descending );

            return ContainersView;
        }

        private Collection<CswPrimaryKey> _getLocationIds( ContainerData.ReconciliationRequest Request )
        {
            Collection<CswPrimaryKey> LocationIds = new Collection<CswPrimaryKey>();
            CswPrimaryKey RootLocationId = CswConvert.ToPrimaryKey( Request.LocationId );
            if( null != RootLocationId )
            {
                if( Request.IncludeChildLocations )
                {
                    CswNbtView LocationTreeView = CswNbtNodePropLocation.LocationPropertyView( _CswNbtResources, null );
                    ICswNbtTree LocationTree = _CswNbtResources.Trees.getTreeFromView( LocationTreeView, false, true, false );
                    _addChildLocationIds( RootLocationId, LocationTree, LocationIds );
                }
                else
                {
                    LocationIds.Add( RootLocationId );
                }
            }
            return LocationIds;
        }

        private void _addChildLocationIds( CswPrimaryKey LocationId, ICswNbtTree LocationTree, Collection<CswPrimaryKey> LocationIds )
        {
            LocationIds.Add( LocationId );
            LocationTree.makeNodeCurrent( LocationId );
            if( LocationTree.getChildNodeCount() > 0 )
            {
                for( int i = 0; i < LocationTree.getChildNodeCount(); i++ )
                {
                    LocationTree.goToNthChild( i );
                    _addChildLocationIds( LocationTree.getNodeIdForCurrentPosition(), LocationTree, LocationIds );
                    LocationTree.goToParentNode();
                }
            }
        }

        private void _incrementContainerCount( IEnumerable<ContainerData.ReconciliationStatistics> Stats, String Status, String Scan = null )
        {
            foreach( ContainerData.ReconciliationStatistics Stat in Stats )
            {
                if( Stat.Status == Status )
                {
                    Stat.ContainerCount += 1;
                    if( false == String.IsNullOrEmpty( Scan ) )
                    {
                        Stat.AmountScanned += 1;
                    }
                }
            }
        }
    }
}
