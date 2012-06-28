using System;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassLocation : CswNbtObjClass
    {
        public const string ChildLocationTypePropertyName = "Child Location Type";
        public const string LocationTemplatePropertyName = "Location Template";
        public const string LocationPropertyName = "Location";
        public const string OrderPropertyName = "Order";
        public const string RowsPropertyName = "Rows";
        public const string ColumnsPropertyName = "Columns";
        public const string BarcodePropertyName = "Barcode";
        public const string NamePropertyName = "Name";
        public const string InventoryGroupPropertyName = "Inventory Group";
        public const string LocationCodePropertyName = "Location Code";
        public const string AllowInventoryPropertyName = "Allow Inventory";
        public const string StorageCompatabilityPropertyName = "Storage Compatability";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassLocation( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassLocation
        /// </summary>
        public static implicit operator CswNbtObjClassLocation( CswNbtNode Node )
        {
            CswNbtObjClassLocation ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass ) )
            {
                ret = (CswNbtObjClassLocation) Node.ObjClass;
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
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) &&
                Location.WasModified &&
                _CswNbtResources.EditMode != NodeEditMode.Add )
            {
                CswNbtNodePropWrapper LocationWrapper = Node.Properties[LocationPropertyName];
                string PrevLocationName = LocationWrapper.GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleLocation) _CswNbtResources.MetaData.getFieldTypeRule( LocationWrapper.getFieldType().FieldType ) ).NameSubField.Column );
                string PrevLocationId = LocationWrapper.GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleLocation) _CswNbtResources.MetaData.getFieldTypeRule( LocationWrapper.getFieldType().FieldType ) ).NodeIdSubField.Column );
                if( false == string.IsNullOrEmpty( PrevLocationId ) &&
                    PrevLocationName != CswNbtNodePropLocation.TopLevelName )
                {
                    CswPrimaryKey PrevLocationPk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( PrevLocationId ) );
                    if( Int32.MinValue != PrevLocationPk.PrimaryKey &&
                        PrevLocationPk != Location.SelectedNodeId )
                    {
                        //We should only be here if the change can actually affect inventory levels: not adding a new location node, not changing a location's location from Top, not setting a previous unset location's location value
                        CswNbtBatchOpInventoryLevels BatchOp = new CswNbtBatchOpInventoryLevels( _CswNbtResources );
                        BatchOp.makeBatchOp( PrevLocationPk, Location.SelectedNodeId );
                    }
                }
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

        public override void afterPopulateProps()
        {
            // BZ 6744
            // Hide the Child Location Type and Location Template controls
            if( _CswNbtResources.ConfigVbls.getConfigVariableValue( "loc_use_images" ) == "0" )
            {
                this.ChildLocationType.Hidden = true;
                this.Rows.Hidden = true;
                this.Columns.Hidden = true;
                this.LocationTemplate.Hidden = true;
            }

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties


        public CswNbtNodePropList ChildLocationType
        {
            get
            {
                return ( _CswNbtNode.Properties[ChildLocationTypePropertyName] );
            }
        }

        public CswNbtNodePropList LocationTemplate
        {
            get
            {
                return ( _CswNbtNode.Properties[LocationTemplatePropertyName] );
            }
        }

        public CswNbtNodePropLocation Location
        {
            get
            {
                return ( _CswNbtNode.Properties[LocationPropertyName] );
            }
        }

        public CswNbtNodePropNumber Order
        {
            get
            {
                return ( _CswNbtNode.Properties[OrderPropertyName] );
            }
        }

        public CswNbtNodePropNumber Rows
        {
            get
            {
                return ( _CswNbtNode.Properties[RowsPropertyName] );
            }
        }

        public CswNbtNodePropNumber Columns
        {
            get
            {
                return ( _CswNbtNode.Properties[ColumnsPropertyName] );
            }
        }

        public CswNbtNodePropBarcode Barcode
        {
            get
            {
                return ( _CswNbtNode.Properties[BarcodePropertyName] );
            }
        }
        public CswNbtNodePropText Name
        {
            get
            {
                return ( _CswNbtNode.Properties[NamePropertyName] );
            }
        }
        public CswNbtNodePropRelationship InventoryGroup
        {
            get
            {
                return ( _CswNbtNode.Properties[InventoryGroupPropertyName] );
            }
        }
        public CswNbtNodePropText LocationCode
        {
            get
            {
                return ( _CswNbtNode.Properties[InventoryGroupPropertyName] );
            }
        }
        public CswNbtNodePropLogical AllowInventory
        {
            get
            {
                return ( _CswNbtNode.Properties[InventoryGroupPropertyName] );
            }
        }
        public CswNbtNodePropImageList StorageCompatability
        {
            get
            {
                return ( _CswNbtNode.Properties[InventoryGroupPropertyName] );
            }
        }

        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
