using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDesignNodeTypeTab : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string IncludeInReport = "Include In Report";
            public const string NodeTypeValue = "NodeType";
            public const string Order = "Order";
            public const string TabName = "Tab Name";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignNodeTypeTab( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDesignNodeTypeTab
        /// </summary>
        public static implicit operator CswNbtObjClassDesignNodeTypeTab( CswNbtNode Node )
        {
            CswNbtObjClassDesignNodeTypeTab ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DesignNodeTypeTabClass ) )
            {
                ret = (CswNbtObjClassDesignNodeTypeTab) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( Int32.MinValue == Order.Value )
            {
                Order.Value = NodeType.getNextTabOrder();
            }
        }

        public override void afterCreateNode()
        {
            // ------------------------------------------------------------
            // This logic from makeNewTab in CswNbtMetaData.cs
            // ------------------------------------------------------------
            if( CswTools.IsPrimaryKey( RelationalId ) )
            {
                Int32 TabId = RelationalId.PrimaryKey;

                CswTableUpdate TabsUpdate = _CswNbtResources.makeCswTableUpdate( "DesignNodeTypeTab_afterCreateNode_TabsUpdate", "nodetype_tabset" );
                DataTable TabsTable = TabsUpdate.getTable( "nodetypetabsetid", TabId );
                if( TabsTable.Rows.Count > 0 )
                {
                    // Version, if necessary
                    //NodeType = CheckVersioning( NodeType );

                    DataRow Row = TabsTable.Rows[0];
                    Row["firsttabversionid"] = CswConvert.ToDbVal( TabId );
                    TabsUpdate.update( TabsTable );

                    CswNbtMetaDataNodeTypeProp SaveNtp = NodeType.getNodeTypeProp( CswNbtObjClass.PropertyName.Save );
                    if( null != SaveNtp ) //Case 29181 - Save prop on new tabs
                    {
                        //Note - when first creating a new NodeType and creating its first tab this will be null, which is expected
                        SaveNtp.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId: TabId, DisplayColumn: 1, DisplayRow: Int32.MaxValue );
                    }
                } // if( TabsTable.Rows.Count > 0 )
            } // if( CswTools.IsPrimaryKey( RelationalId ) )
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
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
            _CswNbtResources.MetaData.DeleteNodeTypeTab( _CswNbtResources.MetaData.getNodeTypeTab( this.RelationalId.PrimaryKey ) );
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
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

        #region Object class specific properties


        public CswNbtNodePropLogical IncludeInReport { get { return ( _CswNbtNode.Properties[PropertyName.IncludeInReport] ); } }
        public CswNbtNodePropRelationship NodeTypeValue { get { return ( _CswNbtNode.Properties[PropertyName.NodeTypeValue] ); } }
        public CswNbtNodePropNumber Order { get { return ( _CswNbtNode.Properties[PropertyName.Order] ); } }
        public CswNbtNodePropText TabName { get { return ( _CswNbtNode.Properties[PropertyName.TabName] ); } }

        #endregion


    }//CswNbtObjClassDesignNodeTypeTab

}//namespace ChemSW.Nbt.ObjClasses
