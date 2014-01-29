using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
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

        public ContainerData getOutstandingActionsCount( ContainerData.ReconciliationRequest Request )
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
                            ContainersTree.goToNthChild( j );
                            if( ContainersTree.getChildNodeCount() > 0 )//ContainerLocation Nodes
                            {
                                CswNbtObjClassContainerLocation ContainerLocationNode = _getMostRelevantContainerLocation();
                                if( null != ContainerLocationNode && 
                                    false == String.IsNullOrEmpty( ContainerLocationNode.Action.Value ) && 
                                    ContainerLocationNode.Action.Value != CswEnumNbtContainerLocationTypeOptions.Ignore.ToString() &&
                                    ContainerLocationNode.ActionApplied.Checked != CswEnumTristate.True )
                                {
                                    Data.OutstandingActionsCount++;
                                }
                            }
                            ContainersTree.goToParentNode();
                        }
                    }
                    ContainersTree.goToParentNode();
                }
            }
            return Data;
        }

        public ContainerData getContainerStatistics( ContainerData.ReconciliationRequest Request )
        {
            for( int i = 0; i < CswEnumNbtContainerLocationStatusOptions._All.Count(); i++ )
            {
                Data.ContainerStatistics.Add( new ContainerData.ReconciliationStatistics() );
                Data.ContainerStatistics[i].ContainerCount = 0;
                Data.ContainerStatistics[i].AmountScanned = 0;
                Data.ContainerStatistics[i].Status = CswEnumNbtContainerLocationStatusOptions._All.ToArray()[i].ToString();
            }
            _setContainersTree( Request );
            if( ContainersTree.getChildNodeCount() > 0 )
            {
                Collection<CswPrimaryKey> AllLocationIds = new Collection<CswPrimaryKey>();
                Collection<CswPrimaryKey> ScannedLocationIds = new Collection<CswPrimaryKey>();
                for( int i = 0; i < ContainersTree.getChildNodeCount(); i++ )//Location Nodes
                {
                    ContainersTree.goToNthChild( i );
                    AllLocationIds.Add( ContainersTree.getNodeIdForCurrentPosition() );
                    if( ContainersTree.getChildNodeCount() > 0 )
                    {
                        for( int j = 0; j < ContainersTree.getChildNodeCount(); j++ )//Container Nodes
                        {
                            ContainersTree.goToNthChild( j );
                            CswNbtTreeNodeProp DisposedProp = ContainersTree.getChildNodePropsOfNode().FirstOrDefault( p => p.ObjectClassPropName == CswNbtObjClassContainer.PropertyName.Disposed );
                            if( ContainersTree.getChildNodeCount() > 0 )//ContainerLocation Nodes
                            {
                                CswNbtObjClassContainerLocation ContainerLocationNode = _getMostRelevantContainerLocation();
                                if( null != ContainerLocationNode && _isTypeEnabled( ContainerLocationNode.Type.Value, Request ) )
                                {
                                    _incrementContainerCount( Data.ContainerStatistics,
                                                             ContainerLocationNode.Status.Value,
                                                             ContainerLocationNode.Type.Value );
                                    if( ContainerLocationNode.Type.Value == CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() )
                                    {
                                        ScannedLocationIds.Add( ContainerLocationNode.Location.SelectedNodeId );
                                    }
                                }
                                else if( null != DisposedProp && false == CswConvert.ToBoolean( DisposedProp.Field1 ) )
                                {
                                    _incrementContainerCount( Data.ContainerStatistics, CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString() );
                                }
                            }
                            else
                            {
                                _incrementContainerCount( Data.ContainerStatistics, CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString() );
                            }
                            ContainersTree.goToParentNode();
                        }
                    }
                    ContainersTree.goToParentNode();
                }
                if( _isTypeEnabled( CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString(), Request ) )
                {
                    foreach( CswPrimaryKey LocationId in AllLocationIds )
                    {
                        if( false == ScannedLocationIds.Contains( LocationId ) )
                        {
                            Data.UnscannedLocations.Add( _makeUnscannedLocation( LocationId ) );
                        }
                    }
                }
            }
            foreach( CswNbtObjClassContainerLocation OutOfScopeScan in _getRelevantScansOutsideLocationScope( Request ).Values )
            {
                _incrementContainerCount( Data.ContainerStatistics, CswEnumNbtContainerLocationStatusOptions.WrongLocation.ToString() );
            }
            foreach( ContainerData.ReconciliationStatistics Stat in Data.ContainerStatistics )
            {
                if( Stat.ContainerCount > 0 )
                {
                    Stat.PercentScanned = CswConvert.ToDouble( Stat.AmountScanned ) / CswConvert.ToDouble( Stat.ContainerCount ) * 100.0;
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
                            bool IncludeContainer = true;
                            ContainerData.ReconciliationStatuses ContainerStatus = new ContainerData.ReconciliationStatuses();
                            ContainersTree.goToNthChild( j );
                            CswNbtNode ContainerNode = ContainersTree.getNodeForCurrentPosition();//In this case, instancing the base node is faster
                            ContainerStatus.ContainerId = ContainerNode.NodeId.ToString();
                            ContainerStatus.ContainerBarcode = ContainerNode.Properties[CswNbtObjClassContainer.PropertyName.Barcode].AsBarcode.Barcode;
                            ContainerStatus.LocationId = LocationId.ToString();
                            ContainerStatus.ExpectedLocation = ContainerNode.Properties[CswNbtObjClassContainer.PropertyName.Location].AsLocation.CachedFullPath;
                            ContainerStatus.ContainerStatus = CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString();
                            CswPrimaryKey ScannedLocationId = null;
                            if( ContainersTree.getChildNodeCount() > 0 )//ContainerLocation Nodes
                            {
                                CswNbtObjClassContainerLocation ContainerLocationNode = _getMostRelevantContainerLocation();
                                if( null != ContainerLocationNode )
                                {
                                    ContainerStatus.ContainerLocationId = ContainerLocationNode.NodeId.ToString();
                                    ScannedLocationId = ContainerLocationNode.Location.SelectedNodeId;
                                    ContainerStatus.ScanDate = ContainerLocationNode.ScanDate.DateTimeValue.Date.ToShortDateString();
                                    ContainerStatus.Action = ContainerLocationNode.Action.Value;
                                    ContainerStatus.Completed = ContainerLocationNode.ActionApplied.Checked.ToString();
                                    if( _isTypeEnabled( ContainerLocationNode.Type.Value, Request ) )
                                    {
                                        ContainerStatus.ContainerStatus = ContainerLocationNode.Status.Value;
                                        if( ContainerLocationNode.Type.Value == CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() )
                                        {
                                            ContainerStatus.ScannedLocation = ContainerLocationNode.Location.CachedFullPath;
                                        }
                                    }
                                    else if( ContainerNode.Properties[CswNbtObjClassContainer.PropertyName.Disposed].AsLogical.Checked == CswEnumTristate.True )
                                    {
                                        IncludeContainer = false;
                                    }
                                }
                            }
                            ContainerStatus.ActionOptions = _getActionOptions( ContainerStatus.ContainerStatus, ScannedLocationId );
                            if( IncludeContainer )
                            {
                                Data.ContainerStatuses.Add( ContainerStatus );
                            }
                            ContainersTree.goToParentNode();
                        }
                    }
                    ContainersTree.goToParentNode();
                }
            }
            foreach( CswNbtObjClassContainerLocation OutOfScopeScan in _getRelevantScansOutsideLocationScope( Request ).Values )
            {
                CswNbtNode ContainerNode = _CswNbtResources.Nodes[OutOfScopeScan.Container.RelatedNodeId];
                Data.ContainerStatuses.Add( new ContainerData.ReconciliationStatuses
                {
                    ContainerId = OutOfScopeScan.Container.RelatedNodeId.ToString(),
                    ContainerBarcode = OutOfScopeScan.ContainerScan.Text,
                    LocationId = OutOfScopeScan.Location.SelectedNodeId.ToString(),
                    ExpectedLocation = ContainerNode.Properties[CswNbtObjClassContainer.PropertyName.Location].AsLocation.CachedFullPath,
                    ScannedLocation = OutOfScopeScan.Location.CachedFullPath,
                    ContainerStatus = CswEnumNbtContainerLocationStatusOptions.WrongLocation.ToString(),
                    ContainerLocationId = OutOfScopeScan.NodeId.ToString(),
                    ScanDate = OutOfScopeScan.ScanDate.DateTimeValue.Date.ToShortDateString(),
                    Action = OutOfScopeScan.Action.Value,
                    Completed = OutOfScopeScan.ActionApplied.Checked.ToString(),
                    ActionOptions = _getActionOptions( CswEnumNbtContainerLocationStatusOptions.WrongLocation.ToString(), OutOfScopeScan.Location.SelectedNodeId )
                } );
            }
            return Data;
        }

        public void saveContainerActions( ContainerData.ReconciliationRequest Request )
        {
            if( null != Request.ContainerActions )
            {
                foreach( ContainerData.ReconciliationActions Action in Request.ContainerActions )
                {
                    if( Action.Action == CswEnumNbtContainerLocationActionOptions.MarkMissing.ToString() )
                    {
                        _createNotScannedContainerLocation( Action, CswEnumNbtContainerLocationTypeOptions.Missing );
                    }
                    else
                    {
                        CswPrimaryKey ContLocNodeId = CswConvert.ToPrimaryKey( Action.ContainerLocationId );
                        if( CswTools.IsPrimaryKey( ContLocNodeId ) )
                        {
                            CswNbtObjClassContainerLocation ContLocNode = _CswNbtResources.Nodes.GetNode( ContLocNodeId );
                            ContLocNode.Action.Value = Action.Action;
                            ContLocNode.ActionByUser.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                            ContLocNode.postChanges( false );
                        }
                        else if( Action.Action == CswEnumNbtContainerLocationActionOptions.Ignore.ToString() )
                        {
                            _createNotScannedContainerLocation( Action, CswEnumNbtContainerLocationTypeOptions.Ignore );
                        }
                    }
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        #region View Creation

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
                    throw new CswDniException( CswEnumErrorType.Error, "Unable to get Reconciliation data.", ErrorMessage, ex );
                }

            }
        }

        private CswNbtView _getReconciliationView( ContainerData.ReconciliationRequest Request )
        {
            Collection<CswPrimaryKey> LocationIds = _getLocationIds( Request );

            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp LocationOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
            CswNbtMetaDataObjectClassProp DateCreatedOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.DateCreated );
            CswNbtMetaDataObjectClassProp DisposedOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Disposed );
            CswNbtMetaDataObjectClass ContainerLocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataObjectClassProp ContainerOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );
            CswNbtMetaDataObjectClassProp ScanDateOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ScanDate );

            CswNbtView ContainersView = new CswNbtView( _CswNbtResources );

            CswNbtViewRelationship LocationVR = ContainersView.AddViewRelationship( LocationOC, false );
            LocationVR.NodeIdsToFilterIn = LocationIds;
            CswNbtViewRelationship ContainerVR = ContainersView.AddViewRelationship( LocationVR, CswEnumNbtViewPropOwnerType.Second, LocationOCP, false );
            CswNbtViewProperty DateCreatedVP = ContainersView.AddViewProperty( ContainerVR, DateCreatedOCP );
            ContainersView.AddViewPropertyFilter( DateCreatedVP, FilterMode: CswEnumNbtFilterMode.LessThanOrEquals, Value: Request.EndDate );
            CswNbtViewProperty DisposedVP = ContainersView.AddViewProperty( ContainerVR, DisposedOCP );
            CswNbtViewRelationship ContainerLocationVR = ContainersView.AddViewRelationship( ContainerVR, CswEnumNbtViewPropOwnerType.Second, ContainerOCP, false );
            CswNbtViewProperty ScanDateVP = ContainersView.AddViewProperty( ContainerLocationVR, ScanDateOCP );
            if( CswConvert.ToDateTime( Request.StartDate ) > CswConvert.ToDateTime( Request.EndDate ) )
            {
                Request.StartDate = Request.EndDate;
            }
            ContainersView.AddViewPropertyFilter( ScanDateVP, FilterMode: CswEnumNbtFilterMode.GreaterThanOrEquals, Value: Request.StartDate );
            ContainersView.AddViewPropertyFilter( ScanDateVP, FilterMode: CswEnumNbtFilterMode.LessThanOrEquals, Value: Request.EndDate );
            ContainersView.setSortProperty( ScanDateVP, CswEnumNbtViewPropertySortMethod.Descending );

            return ContainersView;
        }

        private CswNbtView _getReconciliationScansForOutOfScopeContainersView( ContainerData.ReconciliationRequest Request )
        {
            Collection<CswPrimaryKey> LocationIds = _getLocationIds( Request );

            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp CLocationOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
            CswNbtMetaDataObjectClass ContainerLocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataObjectClassProp ContainerOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Container );
            CswNbtMetaDataObjectClassProp TypeOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Type );
            CswNbtMetaDataObjectClassProp LocationOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.Location );
            CswNbtMetaDataObjectClassProp ScanDateOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ScanDate );

            CswNbtView ContainerLocationsView = new CswNbtView( _CswNbtResources );
            //Filter to selected location (and all child locations if applicable)
            CswNbtViewRelationship LocationVR = ContainerLocationsView.AddViewRelationship( LocationOC, false );
            LocationVR.NodeIdsToFilterIn = LocationIds;
            CswCommaDelimitedString LocIds = new CswCommaDelimitedString();
            foreach( CswPrimaryKey LocId in LocationIds )
            {
                LocIds.Add( LocId.PrimaryKey.ToString() );
            }
            //Filter to Container Location records of type Reconcile Scans in the current time scope 
            CswNbtViewRelationship ContainerLocationVR = ContainerLocationsView.AddViewRelationship( LocationVR, CswEnumNbtViewPropOwnerType.Second, LocationOCP, false );
            CswNbtViewProperty TypeVP = ContainerLocationsView.AddViewProperty( ContainerLocationVR, TypeOCP );
            ContainerLocationsView.AddViewPropertyFilter( TypeVP, FilterMode: CswEnumNbtFilterMode.Equals, Value: CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() );
            CswNbtViewProperty ScanDateVP = ContainerLocationsView.AddViewProperty( ContainerLocationVR, ScanDateOCP );
            if( CswConvert.ToDateTime( Request.StartDate ) > CswConvert.ToDateTime( Request.EndDate ) )
            {
                Request.StartDate = Request.EndDate;
            }
            ContainerLocationsView.AddViewPropertyFilter( ScanDateVP, FilterMode: CswEnumNbtFilterMode.GreaterThanOrEquals, Value: Request.StartDate );
            ContainerLocationsView.AddViewPropertyFilter( ScanDateVP, FilterMode: CswEnumNbtFilterMode.LessThanOrEquals, Value: Request.EndDate );
            ContainerLocationsView.setSortProperty( ScanDateVP, CswEnumNbtViewPropertySortMethod.Descending );
            //Filter to Containers not matching current location scope
            CswNbtViewRelationship ContainerVR = ContainerLocationsView.AddViewRelationship( ContainerLocationVR, CswEnumNbtViewPropOwnerType.First, ContainerOCP, false );
            CswNbtViewProperty CLocVP = ContainerLocationsView.AddViewProperty( ContainerVR, CLocationOCP );
            ContainerLocationsView.AddViewPropertyFilter( CLocVP, CswEnumNbtSubFieldName.NodeID, CswEnumNbtFilterMode.In, LocIds.ToString() );

            return ContainerLocationsView;
        }

        #endregion View Creation

        #region Location Collection

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

        #endregion Location Collection

        //TODO - Looking back, this is pretty evil (and fragile) - we're manipulating the global tree under the assumption that
        //TODO - it's in the proper relationship level.  We should really fix this.
        private CswNbtObjClassContainerLocation _getMostRelevantContainerLocation()
        {
            CswNbtObjClassContainerLocation ContainerLocationNode = null;
            Int32 NumOfContainerLocationRecords = ContainersTree.getChildNodeCount();
            for( int k = 0; k < NumOfContainerLocationRecords; k++ )
            {
                ContainersTree.goToNthChild( k );
                if( null == ContainerLocationNode )
                {
                    ContainerLocationNode = ContainersTree.getNodeForCurrentPosition();
                    if( ContainerLocationNode.Type.Value == CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() )
                    {
                        ContainersTree.goToParentNode();
                        break;
                    }
                }
                else
                {
                    CswNbtObjClassContainerLocation TempContainerLocationNode = ContainersTree.getNodeForCurrentPosition();
                    if( TempContainerLocationNode.Type.Value == CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() &&
                        ContainerLocationNode.Type.Value != CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() )
                    {
                        ContainerLocationNode = TempContainerLocationNode;
                        ContainersTree.goToParentNode();
                        break;
                    }
                }
                ContainersTree.goToParentNode();
            }
            return ContainerLocationNode;
        }

        private Dictionary<CswPrimaryKey, CswNbtObjClassContainerLocation> _getRelevantScansOutsideLocationScope( ContainerData.ReconciliationRequest Request )
        {
            Dictionary<CswPrimaryKey, CswNbtObjClassContainerLocation> OutOfScopeScans = new Dictionary<CswPrimaryKey, CswNbtObjClassContainerLocation>();
            ICswNbtTree OutOfScopeScansTree = _CswNbtResources.Trees.getTreeFromView( _getReconciliationScansForOutOfScopeContainersView( Request ), false, true, false );
            if( OutOfScopeScansTree.getChildNodeCount() > 0 )
            {
                for( int i = 0; i < OutOfScopeScansTree.getChildNodeCount(); i++ )//Location Nodes
                {
                    OutOfScopeScansTree.goToNthChild( i );
                    if( OutOfScopeScansTree.getChildNodeCount() > 0 )
                    {
                        for( int j = 0; j < OutOfScopeScansTree.getChildNodeCount(); j++ )//ContainerLocation Nodes
                        {
                            OutOfScopeScansTree.goToNthChild( j );
                            if( OutOfScopeScansTree.getChildNodeCount() == 0 )//Container Nodes
                            {
                                //No container means it's out of scope - only add the most recent scan record
                                CswNbtObjClassContainerLocation OutOfScopeScan = OutOfScopeScansTree.getNodeForCurrentPosition();
                                if( false == OutOfScopeScans.ContainsKey( OutOfScopeScan.Container.RelatedNodeId ) )
                                {
                                    OutOfScopeScans.Add( OutOfScopeScan.Container.RelatedNodeId, OutOfScopeScan );
                                }
                            }
                            OutOfScopeScansTree.goToParentNode();
                        }
                    }
                    OutOfScopeScansTree.goToParentNode();
                }
            }
            return OutOfScopeScans;
        }

        private bool _isTypeEnabled( String Type, ContainerData.ReconciliationRequest Request )
        {
            bool Enabled = false;
            foreach( ContainerData.ReconciliationTypes ContainerLocationType in Request.ContainerLocationTypes )
            {
                if( Type == ContainerLocationType.Type ||
                    ( ContainerLocationType.Type == CswEnumNbtContainerLocationTypeOptions.Touch.ToString() &&
                      ( Type == CswEnumNbtContainerLocationTypeOptions.Move.ToString() ||
                        Type == CswEnumNbtContainerLocationTypeOptions.Dispense.ToString()  ||
                        Type == CswEnumNbtContainerLocationTypeOptions.Dispose.ToString()  ||
                        Type == CswEnumNbtContainerLocationTypeOptions.Undispose.ToString() ) ) )
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
                    if( Type == CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() )
                    {
                        Stat.AmountScanned += 1;
                    }
                }
            }
        }

        private ContainerData.UnscannedLocation _makeUnscannedLocation( CswPrimaryKey LocationId )
        {
            ContainerData.UnscannedLocation UnscannedLocation = new ContainerData.UnscannedLocation();
            CswNbtObjClassLocation LocationNode = _CswNbtResources.Nodes[LocationId];
            if( null != LocationNode )
            {
                UnscannedLocation.LocationId = LocationId.ToString();
                UnscannedLocation.Name = LocationNode.Name.Text;
                UnscannedLocation.Path = LocationNode.Location.CachedFullPath;
                UnscannedLocation.Link = LocationNode.Node.NodeLink;
                UnscannedLocation.AllowInventory = CswConvert.ToBoolean( LocationNode.AllowInventory.Checked );
            }
            return UnscannedLocation;
        }

        private Collection<String> _getActionOptions( String Status, CswPrimaryKey ScannedLocationId )
        {
            Collection<String> ActionOptions = new Collection<String>();
            ActionOptions.Add( String.Empty );
            if( Status != CswEnumNbtContainerLocationStatusOptions.Correct.ToString() &&
                Status != CswEnumNbtContainerLocationStatusOptions.ScannedCorrect.ToString() )
            {
                ActionOptions.Add( CswEnumNbtContainerLocationActionOptions.Ignore.ToString() );
            }
            if( Status == CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString() )
            {
                ActionOptions.Add( CswEnumNbtContainerLocationActionOptions.MarkMissing.ToString() );
            }
            if( Status == CswEnumNbtContainerLocationStatusOptions.Disposed.ToString() ||
                Status == CswEnumNbtContainerLocationStatusOptions.DisposedAtWrongLocation.ToString() )
            {
                ActionOptions.Add( CswEnumNbtContainerLocationActionOptions.Undispose.ToString() );
            }
            if( Status == CswEnumNbtContainerLocationStatusOptions.WrongLocation.ToString() ||
                Status == CswEnumNbtContainerLocationStatusOptions.DisposedAtWrongLocation.ToString() )
            {
                bool CanMove = true;
                CswNbtObjClassLocation ScannedLocation = _CswNbtResources.Nodes[ScannedLocationId];
                if( null != ScannedLocation && ScannedLocation.AllowInventory.Checked != CswEnumTristate.True)
                {
                    CanMove = false;
                }
                if( CanMove )
                {
                    ActionOptions.Add( CswEnumNbtContainerLocationActionOptions.MoveToLocation.ToString() );
                }
            }
            if( Status == CswEnumNbtContainerLocationStatusOptions.DisposedAtWrongLocation.ToString() )
            {
                ActionOptions.Add( CswEnumNbtContainerLocationActionOptions.UndisposeAndMove.ToString() );
            }
            if( Status == CswEnumNbtContainerLocationStatusOptions.Missing.ToString() )
            {
                ActionOptions.Add( CswEnumNbtContainerLocationActionOptions.UnmarkMissing.ToString() );
            }
            return ActionOptions;
        }

        private void _createNotScannedContainerLocation( ContainerData.ReconciliationActions Action, CswEnumNbtContainerLocationTypeOptions Type )
        {
            CswNbtMetaDataObjectClass ContLocOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataNodeType ContLocNt = ContLocOc.FirstNodeType;
            if( null != ContLocNt )
            {
                _CswNbtResources.Nodes.makeNodeFromNodeTypeId( ContLocNt.NodeTypeId, delegate( CswNbtNode NewNode )
                    {
                        CswNbtObjClassContainerLocation ContLocNode = NewNode;
                        ContLocNode.Container.RelatedNodeId = CswConvert.ToPrimaryKey( Action.ContainerId );
                        ContLocNode.Location.SelectedNodeId = CswConvert.ToPrimaryKey( Action.LocationId );
                        ContLocNode.Type.Value = Type.ToString();
                        ContLocNode.Status.Value = CswEnumNbtContainerLocationStatusOptions.NotScanned.ToString();
                        ContLocNode.ActionApplied.Checked = CswEnumTristate.False;
                        if( Type == CswEnumNbtContainerLocationTypeOptions.Missing )
                        {
                            ContLocNode.Action.Value = CswEnumNbtContainerLocationActionOptions.MarkMissing.ToString();
                        }
                        else if( Type == CswEnumNbtContainerLocationTypeOptions.Ignore )
                        {
                            ContLocNode.Action.Value = CswEnumNbtContainerLocationActionOptions.Ignore.ToString();
                        }
                        ContLocNode.ActionByUser.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId; 
                        ContLocNode.ScanDate.DateTimeValue = DateTime.Now;
                        ContLocNode.User.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                    } );
            }
        }

        #endregion Private Methods
    }
}
