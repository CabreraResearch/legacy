using System;
using System.Collections.Generic;
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
            public const string User = "User";
        }

        #endregion

        #region ctor

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassContainerLocation( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

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

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _setStatus();
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()

        protected override void afterPopulateProps()
        {
            ContainerScan.SetOnPropChange( OnContainerScanPropChange );
            LocationScan.SetOnPropChange( OnLocationScanPropChange );
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion

        #region Private Methods

        private void _setStatus()
        {
            CswEnumNbtContainerLocationStatusOptions ContLocStatus = CswEnumNbtContainerLocationStatusOptions.Correct;
            if( Type.Value == CswEnumNbtContainerLocationTypeOptions.Scan.ToString() )
            {
                ContLocStatus = CswEnumNbtContainerLocationStatusOptions.ScannedCorrect;
                CswNbtObjClassContainer ContainerNode = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
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
                if( ContainerNode.Missing.Checked == CswEnumTristate.True )
                {
                    ContLocStatus = CswEnumNbtContainerLocationStatusOptions.Missing;
                }
            }
            else if( Type.Value == CswEnumNbtContainerLocationTypeOptions.Missing.ToString() )
            {
                ContLocStatus = CswEnumNbtContainerLocationStatusOptions.NotScanned;
            }
            Status.Value = ContLocStatus.ToString();
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
        private void OnContainerScanPropChange( CswNbtNodeProp Prop )
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
        private void OnLocationScanPropChange( CswNbtNodeProp Prop )
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

        #endregion

    }//CswNbtObjClassContainerLocation

}//namespace ChemSW.Nbt.ObjClasses

