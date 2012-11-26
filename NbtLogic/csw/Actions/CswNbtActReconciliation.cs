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
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;
        private ContainerData Data;
        private ICswNbtTree ContainersTree;

        public CswNbtActReconciliation( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Data = new ContainerData();
        }

        #endregion Properties and ctor

        #region Public Methods

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
                            if( ContainersTree.getChildNodeCount() > 0 )
                            {
                                CswNbtObjClassContainerLocation ContainerLocationNode = null;
                                for( int k = 0; k < ContainersTree.getChildNodeCount(); k++ )//ContainerLocation Nodes
                                {
                                    ContainersTree.goToNthChild(k);
                                    if( null == ContainerLocationNode )
                                    {
                                        ContainerLocationNode = ContainersTree.getNodeForCurrentPosition();
                                        if( ContainerLocationNode.Type.Value == CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() )
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        CswNbtObjClassContainerLocation TempContainerLocationNode = ContainersTree.getNodeForCurrentPosition();
                                        if( TempContainerLocationNode.Type.Value == CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() &&
                                            ContainerLocationNode.Type.Value != CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() )
                                        {
                                            ContainerLocationNode = TempContainerLocationNode;
                                            break;
                                        }
                                    }
                                }
                                if( null != ContainerLocationNode && _isTypeEnabled( ContainerLocationNode.Type.Value, Request ) )
                                {
                                    _incrementContainerCount( Data.ContainerStatistics,
                                                             ContainerLocationNode.Status.Value,
                                                             ContainerLocationNode.Type.Value );
                                }
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
                    CswPrimaryKey LocationId = ContainersTree.getNodeIdForCurrentPosition();
                    if( ContainersTree.getChildNodeCount() > 0 )
                    {
                        for( int j = 0; j < ContainersTree.getChildNodeCount(); j++ )//Container Nodes
                        {
                            ContainerData.ReconciliationStatuses ContainerStatus = new ContainerData.ReconciliationStatuses();
                            ContainersTree.goToNthChild( j );
                            CswNbtNode ContainerNode = ContainersTree.getNodeForCurrentPosition();//TODO - revert to CswObjClassContainer when Case 27520 is resolved
                            ContainerStatus.ContainerId = ContainerNode.NodeId.ToString();
                            ContainerStatus.ContainerBarcode = ContainerNode.Properties[CswNbtObjClassContainer.PropertyName.Barcode].AsBarcode.Barcode;
                            ContainerStatus.LocationId = LocationId.ToString();
                            bool isEnabled = false;
                            if( ContainersTree.getChildNodeCount() > 0 )
                            {
                                CswNbtObjClassContainerLocation ContainerLocationNode = null;
                                for( int k = 0; k < ContainersTree.getChildNodeCount(); k++ )//ContainerLocation Nodes
                                {
                                    ContainersTree.goToNthChild( k );
                                    if( null == ContainerLocationNode )
                                    {
                                        ContainerLocationNode = ContainersTree.getNodeForCurrentPosition();
                                        if( ContainerLocationNode.Type.Value == CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() )
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        CswNbtObjClassContainerLocation TempContainerLocationNode = ContainersTree.getNodeForCurrentPosition();
                                        if( TempContainerLocationNode.Type.Value == CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() &&
                                            ContainerLocationNode.Type.Value != CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() )
                                        {
                                            ContainerLocationNode = TempContainerLocationNode;
                                            break;
                                        }
                                    }
                                }
                                if( null != ContainerLocationNode && _isTypeEnabled( ContainerLocationNode.Type.Value, Request ) )
                                {
                                    ContainerStatus.ContainerLocationId = ContainerLocationNode.NodeId.ToString();
                                    ContainerStatus.ContainerStatus = ContainerLocationNode.Status.Value;
                                    ContainerStatus.ScanDate = ContainerLocationNode.ScanDate.DateTimeValue.Date.ToShortDateString();
                                    ContainerStatus.Action = ContainerLocationNode.Action.Value;
                                    ContainerStatus.ActionApplied = ContainerLocationNode.ActionApplied.Checked.ToString();
                                    isEnabled = true;
                                }
                                ContainersTree.goToParentNode();
                            }
                            else
                            {
                                ContainerStatus.ContainerStatus = CswNbtObjClassContainerLocation.StatusOptions.NotScanned.ToString();
                                isEnabled = true;
                            }
                            ContainerStatus.ActionOptions = _getActionOptions( ContainerStatus.ContainerStatus );
                            if( isEnabled )
                            {
                                Data.ContainerStatuses.Add(ContainerStatus);
                            }
                            ContainersTree.goToParentNode();
                        }
                    }
                    ContainersTree.goToParentNode();
                }
            }
            return Data;
        }

        public void saveContainerActions( ContainerData.ReconciliationRequest Request )
        {
            if( null != Request.ContainerActions )
            {
                foreach (ContainerData.ReconciliationActions Action in Request.ContainerActions)
                {
                    if (Action.Action == CswNbtObjClassContainerLocation.ActionOptions.MarkMissing.ToString())
                    {
                        _createMissingContainerLocation( Action );
                    }
                    else
                    {
                        CswPrimaryKey ContLocNodeId = CswConvert.ToPrimaryKey(Action.ContainerLocationId);
                        if( null != ContLocNodeId )
                        {
                            CswNbtObjClassContainerLocation ContLocNode = _CswNbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( Action.ContainerLocationId ) );
                            ContLocNode.Action.Value = Action.Action;
                            ContLocNode.postChanges( false );
                        }
                    }
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

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
            CswNbtMetaDataObjectClassProp DateCreatedOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.DateCreated );
            CswNbtMetaDataObjectClass ContainerLocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataObjectClassProp ContainerOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );
            CswNbtMetaDataObjectClassProp ScanDateOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ScanDate );

            CswNbtView ContainersView = new CswNbtView( _CswNbtResources );

            CswNbtViewRelationship LocationVR = ContainersView.AddViewRelationship( LocationOC, false );
            LocationVR.NodeIdsToFilterIn = LocationIds;
            CswNbtViewRelationship ContainerVR = ContainersView.AddViewRelationship( LocationVR, NbtViewPropOwnerType.Second, LocationOCP, false );
            CswNbtViewProperty DateCreatedVP = ContainersView.AddViewProperty( ContainerVR, DateCreatedOCP );
            ContainersView.AddViewPropertyFilter( DateCreatedVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, Value: Request.EndDate );
            CswNbtViewRelationship ContainerLocationVR = ContainersView.AddViewRelationship( ContainerVR, NbtViewPropOwnerType.Second, ContainerOCP, false );
            CswNbtViewProperty ScanDateVP = ContainersView.AddViewProperty( ContainerLocationVR, ScanDateOCP );
            ContainersView.AddViewPropertyFilter( ScanDateVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals, Value: Request.StartDate );
            ContainersView.AddViewPropertyFilter( ScanDateVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, Value: Request.EndDate );
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

        private bool _isTypeEnabled( String Type, ContainerData.ReconciliationRequest Request )
        {
            bool Enabled = false;
            foreach( ContainerData.ReconciliationTypes ContainerLocationType in Request.ContainerLocationTypes )
            {
                if(Type == ContainerLocationType.Type)
                {
                    Enabled = ContainerLocationType.Enabled;
                }
            }
            return Enabled;
        }

        private void _incrementContainerCount( IEnumerable<ContainerData.ReconciliationStatistics> Stats, String Status, String Type = null )
        {
            foreach( ContainerData.ReconciliationStatistics Stat in Stats )
            {
                if( Stat.Status == Status )
                {
                    Stat.ContainerCount += 1;
                    if( Type == CswNbtObjClassContainerLocation.TypeOptions.Scan.ToString() )
                    {
                        Stat.AmountScanned += 1;
                    }
                }
            }
        }

        private Collection<String> _getActionOptions( String Status )
        {
            Collection<String> ActionOptions = new Collection<String>();
            ActionOptions.Add( String.Empty );
            if( Status != CswNbtObjClassContainerLocation.StatusOptions.Correct.ToString() )
            {
                ActionOptions.Add( CswNbtObjClassContainerLocation.ActionOptions.NoAction.ToString() );
            }
            if( Status == CswNbtObjClassContainerLocation.StatusOptions.NotScanned.ToString() )
            {
                ActionOptions.Add( CswNbtObjClassContainerLocation.ActionOptions.MarkMissing.ToString() );
            }
            if( Status == CswNbtObjClassContainerLocation.StatusOptions.Disposed.ToString() ||
                Status == CswNbtObjClassContainerLocation.StatusOptions.DisposedAtWrongLocation.ToString() )
            {
                ActionOptions.Add( CswNbtObjClassContainerLocation.ActionOptions.Undispose.ToString() );
            }
            if( Status == CswNbtObjClassContainerLocation.StatusOptions.WrongLocation.ToString() ||
                Status == CswNbtObjClassContainerLocation.StatusOptions.DisposedAtWrongLocation.ToString() )
            {
                ActionOptions.Add( CswNbtObjClassContainerLocation.ActionOptions.MoveToLocation.ToString() );
            }
            if( Status == CswNbtObjClassContainerLocation.StatusOptions.DisposedAtWrongLocation.ToString() )
            {
                ActionOptions.Add( CswNbtObjClassContainerLocation.ActionOptions.UndisposeAndMove.ToString() );
            }
            return ActionOptions;
        }

        private void _createMissingContainerLocation( ContainerData.ReconciliationActions Action )
        {
            CswNbtMetaDataObjectClass ContLocOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataNodeType ContLocNt = ContLocOc.FirstNodeType;
            if( null != ContLocNt )
            {
                CswNbtObjClassContainerLocation ContLocNode = 
                    _CswNbtResources.Nodes.makeNodeFromNodeTypeId( 
                        ContLocNt.NodeTypeId,
                        CswNbtNodeCollection.MakeNodeOperation.DoNothing 
                    );
                ContLocNode.Container.RelatedNodeId = CswConvert.ToPrimaryKey( Action.ContainerId );
                ContLocNode.Location.SelectedNodeId = CswConvert.ToPrimaryKey( Action.LocationId );
                ContLocNode.Type.Value = CswNbtObjClassContainerLocation.TypeOptions.Missing.ToString();
                ContLocNode.Status.Value = CswNbtObjClassContainerLocation.StatusOptions.Missing.ToString();
                ContLocNode.Action.Value = CswNbtObjClassContainerLocation.ActionOptions.MarkMissing.ToString();
                ContLocNode.ActionApplied.Checked = Tristate.False;
                ContLocNode.ScanDate.DateTimeValue = DateTime.Now;
                ContLocNode.User.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                ContLocNode.postChanges( false );
            }
        }

        #endregion Private Methods
    }
}
