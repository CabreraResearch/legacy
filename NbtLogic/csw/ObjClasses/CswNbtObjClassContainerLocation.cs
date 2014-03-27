﻿using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Container Dispense Transaction Object Class
    /// </summary>
    public class CswNbtObjClassContainerLocation : CswNbtObjClass
    {
        #region Static Properties

        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Container = "Container";
            public const string Location = "Location";
            public const string Type = "Type";
            public const string ContainerScan = "Container Scan";
            public const string LocationScan = "Location Scan";
            public const string ScanDate = "Scan Date";
            public const string Status = "Status";
            public const string Action = "Action";
            public const string ActionApplied = "Action Applied";
            public const string ActionByUser = "Action By User";
            public const string User = "User";
        }

        #endregion

        #region ctor

        public CswNbtObjClassContainerLocation( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainerLocation
        /// </summary>
        public static implicit operator CswNbtObjClassContainerLocation( CswNbtNode Node )
        {
            CswNbtObjClassContainerLocation ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ContainerLocationClass ) )
            {
                ret = (CswNbtObjClassContainerLocation) Node.ObjClass;
            }
            return ret;
        }

        #endregion

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            _setStatus();
        }//beforeWriteNode()

        protected override void afterPopulateProps()
        {
            ContainerScan.SetOnPropChange( OnContainerScanPropChange );
            LocationScan.SetOnPropChange( OnLocationScanPropChange );
        }//afterPopulateProps()

        #endregion

        #region Private Methods

        private void _setStatus()
        {
            bool doUpdate = true;
            CswEnumNbtContainerLocationStatusOptions ContLocStatus = CswEnumNbtContainerLocationStatusOptions.Correct;
            if( Type.Value == CswEnumNbtContainerLocationTypeOptions.ReconcileScans.ToString() )
            {
                ContLocStatus = CswEnumNbtContainerLocationStatusOptions.ScannedCorrect;
                CswNbtObjClassContainer ContainerNode = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                if( null != ContainerNode )
                {
                    if( ContainerNode.Missing.Checked == CswEnumTristate.True )
                    {
                        ContLocStatus = CswEnumNbtContainerLocationStatusOptions.Missing;
                    }
                    if( ContainerNode.Disposed.Checked == CswEnumTristate.True )
                    {
                        ContLocStatus = CswEnumNbtContainerLocationStatusOptions.Disposed;
                    }
                    if( ContainerNode.Location.SelectedNodeId != Location.SelectedNodeId )
                    {
                        ContLocStatus = ContLocStatus == CswEnumNbtContainerLocationStatusOptions.Disposed
                                            ? CswEnumNbtContainerLocationStatusOptions.DisposedAtWrongLocation
                                            : CswEnumNbtContainerLocationStatusOptions.WrongLocation;
                    }
                }
                else//If we're here, it's because we can no longer find the container that was scanned
                {
                    doUpdate = false;
                }
            }
            else if( Type.Value == CswEnumNbtContainerLocationTypeOptions.Missing.ToString() ||
                     Type.Value == CswEnumNbtContainerLocationTypeOptions.Ignore.ToString() )
            {
                ContLocStatus = CswEnumNbtContainerLocationStatusOptions.NotScanned;
            }
            if( doUpdate )
            {
                Status.Value = ContLocStatus.ToString();
            }
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Container
        {
            get { return _CswNbtNode.Properties[PropertyName.Container]; }
        }
        public CswNbtNodePropLocation Location
        {
            get { return _CswNbtNode.Properties[PropertyName.Location]; }
        }
        public CswNbtNodePropList Type
        {
            get { return _CswNbtNode.Properties[PropertyName.Type]; }
        }
        public CswNbtNodePropText ContainerScan
        {
            get { return _CswNbtNode.Properties[PropertyName.ContainerScan]; }
        }
        private void OnContainerScanPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( null != ContainerScan.Text )
            {
                CswNbtMetaDataObjectClass ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                CswNbtView ContainerView = new CswNbtView( _CswNbtResources );

                CswNbtViewRelationship ContainerRel = ContainerView.AddViewRelationship( ContainerOc, false );
                CswNbtMetaDataObjectClassProp BarcodeOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );

                ContainerView.AddViewPropertyAndFilter( ContainerRel, BarcodeOcp, ContainerScan.Text );
                ICswNbtTree ContainerTree = _CswNbtResources.Trees.getTreeFromView( ContainerView, false, false, true );
                if( ContainerTree.getChildNodeCount() > 0 )
                {
                    ContainerTree.goToNthChild( 0 );
                    CswNbtObjClassContainer ContainerNode = ContainerTree.getNodeForCurrentPosition();
                    Container.RelatedNodeId = ContainerNode.NodeId;
                }
                else
                {
                    Container.RelatedNodeId = null;
                }
            }
            _setStatus();
        }
        public CswNbtNodePropText LocationScan
        {
            get { return _CswNbtNode.Properties[PropertyName.LocationScan]; }
        }
        private void OnLocationScanPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( null != LocationScan.Text )
            {
                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                CswNbtView LocationView = new CswNbtView( _CswNbtResources );

                CswNbtViewRelationship LocationRel = LocationView.AddViewRelationship( LocationOc, false );
                CswNbtMetaDataObjectClassProp BarcodeOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Barcode );

                LocationView.AddViewPropertyAndFilter( LocationRel, BarcodeOcp, LocationScan.Text );
                ICswNbtTree LocationTree = _CswNbtResources.Trees.getTreeFromView( LocationView, false, false, true );
                if( LocationTree.getChildNodeCount() > 0 )
                {
                    LocationTree.goToNthChild( 0 );
                    CswNbtObjClassLocation LocationNode = LocationTree.getNodeForCurrentPosition();
                    Location.SelectedNodeId = LocationNode.NodeId;
                    Location.RefreshNodeName();
                }
                else
                {
                    Location.SelectedNodeId = null;
                    Location.CachedNodeName = String.Empty;
                    Location.CachedPath = String.Empty;
                    Location.CachedBarcode = String.Empty;
                }
            }
            _setStatus();
        }
        public CswNbtNodePropDateTime ScanDate
        {
            get { return _CswNbtNode.Properties[PropertyName.ScanDate]; }
        }
        public CswNbtNodePropList Status
        {
            get { return _CswNbtNode.Properties[PropertyName.Status]; }
        }
        public CswNbtNodePropList Action
        {
            get { return _CswNbtNode.Properties[PropertyName.Action]; }
        }
        public CswNbtNodePropLogical ActionApplied
        {
            get { return _CswNbtNode.Properties[PropertyName.ActionApplied]; }
        }
        public CswNbtNodePropRelationship User
        {
            get { return _CswNbtNode.Properties[PropertyName.User]; }
        }
        public CswNbtNodePropRelationship ActionByUser
        {
            get { return _CswNbtNode.Properties[PropertyName.ActionByUser]; }
        }

        #endregion

    }//CswNbtObjClassContainerLocation

}//namespace ChemSW.Nbt.ObjClasses