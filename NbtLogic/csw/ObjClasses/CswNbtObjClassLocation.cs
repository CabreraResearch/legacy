using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassLocation : CswNbtObjClass
    {
        public static string ChildLocationTypePropertyName { get { return "Child Location Type"; } }
        public static string LocationTemplatePropertyName { get { return "Location Template"; } }
        public static string LocationPropertyName { get { return "Location"; } }
        public static string OrderPropertyName { get { return "Order"; } }
        public static string RowsPropertyName { get { return "Rows"; } }
        public static string ColumnsPropertyName { get { return "Columns"; } }
        public static string BarcodePropertyName { get { return "Barcode"; } }
        public static string NamePropertyName { get { return "Name"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassLocation( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public CswNbtObjClassLocation( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
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
            // BZ 6744
            // Hide the Child Location Type and Location Template controls
            if( _CswNbtResources.getConfigVariableValue( "loc_use_images" ) == "0" )
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

        #endregion

        #region Object class specific properties


        public CswNbtNodePropList ChildLocationType
        {
            get
            {
                return ( _CswNbtNode.Properties[ChildLocationTypePropertyName].AsList );
            }
        }

        public CswNbtNodePropList LocationTemplate
        {
            get
            {
                return ( _CswNbtNode.Properties[LocationTemplatePropertyName].AsList );
            }
        }

        public CswNbtNodePropLocation Location
        {
            get
            {
                return ( _CswNbtNode.Properties[LocationPropertyName].AsLocation );
            }
        }

        public CswNbtNodePropNumber Order
        {
            get
            {
                return ( _CswNbtNode.Properties[OrderPropertyName].AsNumber );
            }
        }

        public CswNbtNodePropNumber Rows
        {
            get
            {
                return ( _CswNbtNode.Properties[RowsPropertyName].AsNumber );
            }
        }

        public CswNbtNodePropNumber Columns
        {
            get
            {
                return ( _CswNbtNode.Properties[ColumnsPropertyName].AsNumber );
            }
        }

        public CswNbtNodePropBarcode Barcode
        {
            get
            {
                return ( _CswNbtNode.Properties[BarcodePropertyName].AsBarcode );
            }
        }
        public CswNbtNodePropText Name
        {
            get
            {
                return ( _CswNbtNode.Properties[NamePropertyName].AsText );
            }
        }

        #endregion



    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
