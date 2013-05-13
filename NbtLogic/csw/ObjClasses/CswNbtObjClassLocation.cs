using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Schema;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassLocation: CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string ChildLocationType = "Child Location Type";
            public const string LocationTemplate = "Location Template";
            public const string Location = "Location";
            public const string Order = "Order";
            public const string Rows = "Rows";
            public const string Columns = "Columns";
            public const string Barcode = "Barcode";
            public const string Name = "Name";
            public const string InventoryGroup = "Inventory Group";
            public const string LocationCode = "Location Code";
            public const string AllowInventory = "Allow Inventory";
            public const string StorageCompatibility = "Storage Compatibility";
            public const string ControlZone = "Control Zone";
            public const string Containers = "Containers";
            public const string InventoryLevels = "Inventory Levels";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassLocation( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassLocation
        /// </summary>
        public static implicit operator CswNbtObjClassLocation( CswNbtNode Node )
        {
            CswNbtObjClassLocation ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.LocationClass ) )
            {
                ret = (CswNbtObjClassLocation) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) &&
                Location.WasModified &&
                _CswNbtResources.EditMode != CswEnumNbtNodeEditMode.Add )
            {
                CswNbtNodePropWrapper LocationWrapper = Node.Properties[PropertyName.Location];
                string PrevLocationId = LocationWrapper.GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleLocation) _CswNbtResources.MetaData.getFieldTypeRule( LocationWrapper.getFieldTypeValue() ) ).NodeIdSubField.Column );

                CswPrimaryKey PrevLocationPk = null;
                CswPrimaryKey CurrLocationPk = null;
                if( false == String.IsNullOrEmpty( PrevLocationId ) )
                {
                    PrevLocationPk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( PrevLocationId ) );
                }
                if( null != Location.SelectedNodeId )
                {
                    CurrLocationPk = Location.SelectedNodeId;
                }
                if( PrevLocationPk != null && PrevLocationPk != CurrLocationPk )
                {
                    //Case 26849 - Executing even if one of the locations is Top or null so that the other location can still be updated
                    CswNbtBatchOpInventoryLevels BatchOp = new CswNbtBatchOpInventoryLevels( _CswNbtResources );
                    BatchOp.makeBatchOp( PrevLocationPk, CurrLocationPk );
                }
            }

            //Case 27495 - Sites can only be at "Top"
            if( NodeType.NodeTypeName == "Site" )
            {
                Location.SelectedNodeId = null;
                Location.RefreshNodeName();
                Location.SyncGestalt();
            }

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
            // BZ 6744
            // Hide the Child Location Type and Location Template controls
            if( _CswNbtResources.ConfigVbls.getConfigVariableValue( "loc_use_images" ) == "0" )
            {
                this.ChildLocationType.setHidden( value : true, SaveToDb : false );
                this.Rows.setHidden( value : true, SaveToDb : false );
                this.Columns.setHidden( value : true, SaveToDb : false );
                this.LocationTemplate.setHidden( value : true, SaveToDb : false );
            }
            Location.SetOnPropChange( OnLocationPropChange );
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

        #endregion  Inherited Events

        #region Object class specific properties

        public CswNbtNodePropList ChildLocationType { get { return ( _CswNbtNode.Properties[PropertyName.ChildLocationType] ); } }
        public CswNbtNodePropList LocationTemplate { get { return ( _CswNbtNode.Properties[PropertyName.LocationTemplate] ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        private void OnLocationPropChange( CswNbtNodeProp Prop )
        {
            Int32 ParentLevel = Location.CachedFullPath.Split( new string[] { CswNbtNodePropLocation.PathDelimiter }, StringSplitOptions.RemoveEmptyEntries ).Count();
            Int32 MaxLevel = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.loc_max_depth.ToString() ) );
            if( ParentLevel >= MaxLevel && false == String.IsNullOrEmpty( Name.Text ) )
            {
                throw new CswDniException
                (
                    CswEnumErrorType.Warning,
                    "Cannot create \"" + Name.Text + "\" under \"" + Location.CachedFullPath + "\" because it exceeds the maximum allowed Location depth.",
                    Name.Text + " exceeds loc_max_depth"
                );
            }
        }
        public CswNbtNodePropNumber Order { get { return ( _CswNbtNode.Properties[PropertyName.Order] ); } }
        public CswNbtNodePropNumber Rows { get { return ( _CswNbtNode.Properties[PropertyName.Rows] ); } }
        public CswNbtNodePropNumber Columns { get { return ( _CswNbtNode.Properties[PropertyName.Columns] ); } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[PropertyName.Barcode] ); } }
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropRelationship InventoryGroup { get { return ( _CswNbtNode.Properties[PropertyName.InventoryGroup] ); } }
        public CswNbtNodePropText LocationCode { get { return ( _CswNbtNode.Properties[PropertyName.LocationCode] ); } }
        public CswNbtNodePropLogical AllowInventory { get { return ( _CswNbtNode.Properties[PropertyName.AllowInventory] ); } }
        public CswNbtNodePropImageList StorageCompatibility { get { return ( _CswNbtNode.Properties[PropertyName.StorageCompatibility] ); } }
        public CswNbtNodePropRelationship ControlZone { get { return ( _CswNbtNode.Properties[PropertyName.ControlZone] ); } }
        public CswNbtNodePropGrid Containers { get { return ( _CswNbtNode.Properties[PropertyName.Containers] ); } }
        public CswNbtNodePropGrid InventoryLevels { get { return ( _CswNbtNode.Properties[PropertyName.InventoryLevels] ); } }

        #endregion Object class specific properties

        #region Custom Logic

        public static void makeLocationsTreeView( ref CswNbtView LocationsView, CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, Int32 loc_max_depth = Int32.MinValue, CswPrimaryKey NodeIdToFilterOut = null, bool RequireAllowInventory = false, Collection<CswPrimaryKey> InventoryGroupIds = null )
        {
            _makeLocationsTreeView( ref LocationsView, CswNbtSchemaModTrnsctn.MetaData, CswNbtSchemaModTrnsctn.ConfigVbls, loc_max_depth, NodeIdToFilterOut, RequireAllowInventory, InventoryGroupIds );
        }

        public static void makeLocationsTreeView( ref CswNbtView LocationsView, CswNbtResources CswNbtResources, Int32 loc_max_depth = Int32.MinValue, CswPrimaryKey NodeIdToFilterOut = null, bool RequireAllowInventory = false, Collection<CswPrimaryKey> InventoryGroupIds = null )
        {
            _makeLocationsTreeView( ref LocationsView, CswNbtResources.MetaData, CswNbtResources.ConfigVbls, loc_max_depth, NodeIdToFilterOut, RequireAllowInventory, InventoryGroupIds );
        }

        private static void _makeLocationsTreeView( ref CswNbtView LocationsView, CswNbtMetaData MetaData, CswConfigurationVariables ConfigVbls, Int32 loc_max_depth, CswPrimaryKey NodeIdToFilterOut, bool RequireAllowInventory, Collection<CswPrimaryKey> InventoryGroupIds )
        {
            if( null != LocationsView )
            {
                CswNbtMetaDataObjectClass LocationOC = MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClassProp LocationLocationOCP = LocationOC.getObjectClassProp( PropertyName.Location );
                CswNbtMetaDataObjectClassProp LocationOrderOCP = LocationOC.getObjectClassProp( PropertyName.Order );
                CswNbtMetaDataObjectClassProp LocationAllowInventoryOCP = LocationOC.getObjectClassProp( PropertyName.AllowInventory );
                CswNbtMetaDataObjectClassProp LocationInventoryGroupOCP = LocationOC.getObjectClassProp( PropertyName.InventoryGroup );

                if( loc_max_depth == Int32.MinValue )
                {
                    loc_max_depth = CswConvert.ToInt32( ConfigVbls.getConfigVariableValue( "loc_max_depth" ) );
                }
                if( loc_max_depth < 1 )
                {
                    loc_max_depth = 5;
                }

                LocationsView.Root.ChildRelationships.Clear();

                CswNbtViewRelationship LocReln = null;
                for( Int32 i = 1; i <= loc_max_depth; i++ )
                {
                    if( null == LocReln )
                    {
                        // Top level: Only Locations with null parent locations at the root
                        LocReln = LocationsView.AddViewRelationship( LocationOC, true );
                        LocationsView.AddViewPropertyAndFilter( LocReln, LocationLocationOCP,
                                                                Conjunction : CswEnumNbtFilterConjunction.And,
                                                                SubFieldName : CswEnumNbtSubFieldName.NodeID,
                                                                FilterMode : CswEnumNbtFilterMode.Null );
                    }
                    else
                    {
                        LocReln = LocationsView.AddViewRelationship( LocReln, CswEnumNbtViewPropOwnerType.Second, LocationLocationOCP, true );
                    }
                    if( null != NodeIdToFilterOut )
                    {
                        LocReln.NodeIdsToFilterOut.Add( NodeIdToFilterOut );
                    }

                    CswNbtViewProperty InGroupVp = LocationsView.AddViewProperty( LocReln, LocationInventoryGroupOCP );
                    InGroupVp.Width = 100;

                    if( null != InventoryGroupIds )
                    {
                        CswCommaDelimitedString Pks = new CswCommaDelimitedString();
                        foreach( CswPrimaryKey InventoryGroupId in InventoryGroupIds )
                        {
                            Pks.Add( InventoryGroupId.PrimaryKey.ToString() );
                        }

                        LocationsView.AddViewPropertyFilter( InGroupVp,
                                                                Conjunction : CswEnumNbtFilterConjunction.And,
                                                                ResultMode : CswEnumNbtFilterResultMode.Disabled,
                                                                FilterMode : CswEnumNbtFilterMode.In,
                                                                SubFieldName : CswEnumNbtSubFieldName.NodeID,
                                                                Value : Pks.ToString() );
                    }

                    CswNbtViewProperty OrderVPn = LocationsView.AddViewProperty( LocReln, LocationOrderOCP );
                    LocationsView.setSortProperty( OrderVPn, CswEnumNbtViewPropertySortMethod.Ascending, false );

                    if( RequireAllowInventory )
                    {
                        LocationsView.AddViewPropertyAndFilter( LocReln, LocationAllowInventoryOCP,
                                                                Conjunction : CswEnumNbtFilterConjunction.And,
                                                                ResultMode : CswEnumNbtFilterResultMode.Disabled,
                                                                FilterMode : CswEnumNbtFilterMode.Equals,
                                                                Value : CswEnumTristate.True.ToString() );
                    }
                } // for( Int32 i = 1; i <= loc_max_depth; i++ )
            } // if( null != LocationsView )
        } // makeLocationsTreeView()

        #endregion Custom Logic

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
