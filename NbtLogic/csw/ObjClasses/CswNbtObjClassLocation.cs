using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using System;


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
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) &&
                Location.WasModified &&
                _CswNbtResources.EditMode != NodeEditMode.Add )
            {
                CswNbtNodePropWrapper LocationWrapper = Node.Properties[LocationPropertyName];
                string PrevLocationId = LocationWrapper.GetOriginalPropRowValue( ( (CswNbtFieldTypeRuleLocation) _CswNbtResources.MetaData.getFieldTypeRule( LocationWrapper.getFieldType().FieldType ) ).NodeIdSubField.Column );

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
                if( PrevLocationPk != CurrLocationPk )
                {
                    //Case 26849 - Executing even if one of the locations is Top or null so that the other location can still be updated
                    CswNbtBatchOpInventoryLevels BatchOp = new CswNbtBatchOpInventoryLevels( _CswNbtResources );
                    BatchOp.makeBatchOp( PrevLocationPk, CurrLocationPk );
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
                this.ChildLocationType.setHidden( value: true, SaveToDb: false );
                this.Rows.setHidden( value: true, SaveToDb: false );
                this.Columns.setHidden( value: true, SaveToDb: false );
                this.LocationTemplate.setHidden( value: true, SaveToDb: false );
            }

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
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
                return ( _CswNbtNode.Properties[LocationCodePropertyName] );
            }
        }
        public CswNbtNodePropLogical AllowInventory
        {
            get
            {
                return ( _CswNbtNode.Properties[AllowInventoryPropertyName] );
            }
        }
        public CswNbtNodePropImageList StorageCompatability
        {
            get
            {
                return ( _CswNbtNode.Properties[StorageCompatabilityPropertyName] );
            }
        }

        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
