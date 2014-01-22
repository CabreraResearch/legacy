using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
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
            public const string ServerManaged = "Server Managed";
            public const string TabName = "Tab Name";
        }


        //private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignNodeTypeTab( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            //_CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
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

        /// <summary>
        /// The NodeTypeTab that this node represents
        /// </summary>
        public CswNbtMetaDataNodeTypeTab RelationalNodeTypeTab
        {
            get
            {
                CswNbtMetaDataNodeTypeTab ret = null;
                if( CswTools.IsPrimaryKey( RelationalId ) )
                {
                    ret = _CswNbtResources.MetaData.getNodeTypeTab( RelationalId.PrimaryKey );
                }
                return ret;
            }
        }

        public Collection<CswNbtObjClassDesignNodeTypeProp> getPropNodesByDisplayOrder()
        {
            return this.RelationalNodeTypeTab.getPropNodesByDisplayOrder();
        } // getPropNodesByDisplayOrder()


        #region Inherited Events

        protected override void beforePromoteNodeLogic( bool OverrideUniqueValidation = false )
        {
            if( Int32.MinValue == Order.Value )
            {
                Order.Value = NodeType.getNextTabOrder();
            }
        }

        protected override void afterPromoteNodeLogic()
        {
            // ------------------------------------------------------------
            // This logic from makeNewTab in CswNbtMetaData.cs
            // ------------------------------------------------------------
            if( CswTools.IsPrimaryKey( RelationalId ) )
            {
                Int32 TabId = RelationalId.PrimaryKey;

                //CswTableUpdate TabsUpdate = _CswNbtResources.makeCswTableUpdate( "DesignNodeTypeTab_afterCreateNode_TabsUpdate", "nodetype_tabset" );
                //DataTable TabsTable = TabsUpdate.getTable( "nodetypetabsetid", TabId );
                //if( TabsTable.Rows.Count > 0 )
                //{

                // Version, if necessary
                //NodeType = CheckVersioning( NodeType );

                //DataRow Row = TabsTable.Rows[0];
                DataRow Row = RelationalNodeTypeTab._DataRow;
                if( null != Row )
                {
                    Row["firsttabversionid"] = CswConvert.ToDbVal( TabId );
                    //TabsUpdate.update( TabsTable );
                }

                CswNbtMetaDataNodeTypeProp SaveNtp = NodeType.getNodeTypeProp( CswNbtObjClass.PropertyName.Save );
                if( null != SaveNtp ) //Case 29181 - Save prop on new tabs
                {
                    //Note - when first creating a new NodeType and creating its first tab this will be null, which is expected
                    SaveNtp.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId: TabId, DisplayColumn: 1, DisplayRow: Int32.MaxValue );
                }
                //} // if( TabsTable.Rows.Count > 0 )
            } // if( CswTools.IsPrimaryKey( RelationalId ) )
        } // afterPromoteNode()

        /// <summary>
        /// True if the delete is a result of deleting the nodetype
        /// </summary>
        public bool InternalDelete = false;

        protected override void afterDeleteNodeLogic()
        {
            if( false == InternalDelete )
            {
                if( this.ServerManaged.Checked == CswEnumTristate.True )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Cannot delete Server Managed tabs.", "User attempted to delete " + TabName.Text + ", which is Server Managed." );
                }

                //if( CauseVersioning )
                //{
                //    string OriginalTabName = NodeTypeTab.TabName;
                //    CswNbtMetaDataNodeType NodeType = CheckVersioning( NodeTypeTab.getNodeType() );
                //    NodeTypeTab = NodeType.getNodeTypeTab( OriginalTabName );
                //}

                //// Move the properties to another tab
                //CswNbtMetaDataNodeTypeTab NewTab = RelationalNodeTypeTab.getNodeType().getFirstNodeTypeTab();
                //if( NewTab == RelationalNodeTypeTab ) // BZ 8353
                //{
                //    NewTab = RelationalNodeTypeTab.getNodeType().getSecondNodeTypeTab();
                //}

                //Collection<CswNbtMetaDataNodeTypeProp> PropsToReassign = new Collection<CswNbtMetaDataNodeTypeProp>();
                //foreach( CswNbtMetaDataNodeTypeProp Prop in NodeTypeTab.getNodeTypeProps() )
                //{
                //    PropsToReassign.Add( Prop );
                //}

                //foreach( CswNbtMetaDataNodeTypeProp Prop in PropsToReassign )
                //{
                //    Prop.updateLayout( CswEnumNbtLayoutType.Edit, true, NewTab.TabId, Int32.MinValue, Int32.MinValue );
                //    // BZ 8353 - To avoid constraint errors, post this change immediately
                //    _CswNbtMetaDataResources.NodeTypePropTableUpdate.update( Prop._DataRow.Table );
                //}


                //// Update MetaData
                //refreshAll();
                ////_CswNbtMetaDataResources.NodeTypeTabsCollection.clearCache();

                //// Delete NodeType Tab record
                //NodeTypeTab._DataRow.Delete();
                //_CswNbtMetaDataResources.NodeTypeTabTableUpdate.update( NodeTypeTab._DataRow.Table );

            } // if( false == InternalDelete )

            //_CswNbtObjClassDefault.afterDeleteNode();

        } //afterDeleteNode()        


        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion
        
        #region Custom Logic
 
        /// <summary>
        /// Sets the order for this tab and all other tabs on the same nodetype. Use this instead of changing Order directly to ensure your tab ends up where you intend
        /// </summary>
        /// <param name="NewPosition">the 1-based index of the tab</param>
        public void UpdateTabPosition( int NewPosition )
        {

            //this is not efficient, at all. However, its necessary because we have code that scrambles tab order, but the number we receive from ExtJS is an array position
            CswNbtMetaDataNodeType TabNT = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass ).FirstNodeType;

            //construct a view of all the tabs between the old position and the new position
            CswNbtView TabList = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship TabRelationship = TabList.AddViewRelationship( TabNT, false );
            //constrain by the nodetype to which this tab is attached
            TabList.AddViewPropertyAndFilter( TabRelationship, TabNT.getNodeTypePropByObjectClassProp( PropertyName.NodeTypeValue ),
                SubFieldName : CswEnumNbtSubFieldName.NodeID,
                FilterMode : CswEnumNbtFilterMode.Equals,
                Value: NodeTypeValue.RelatedNodeId.PrimaryKey.ToString()
                );
            //then order from first to last tab by Order property
            CswNbtViewProperty OrderProp = TabList.AddViewProperty( TabRelationship, TabNT.getNodeTypePropByObjectClassProp( PropertyName.Order ) );
            TabList.setSortProperty( OrderProp, CswEnumNbtViewPropertySortMethod.Ascending );

            //iterate through each tab on this nodetype, and set the position
            double OriginalPosition = Order.Value;
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( TabList, IncludeSystemNodes : true, RequireViewPermissions : false, IncludeHiddenNodes : true );
            for( int i = 0; i < Tree.getChildNodeCount(); i++ )
            {
                Tree.goToNthChild( i );
                CswNbtObjClassDesignNodeTypeTab Tab = Tree.getNodeForCurrentPosition();
                if( Tab.NodeId == this.NodeId )
                {
                    Tab.Order.Value = NewPosition;
                }
                else
                {
                    int Spacer = ( (i >= NewPosition && NewPosition < OriginalPosition) || ( i > NewPosition && NewPosition >= OriginalPosition ) ? 1 : 0 ) - ( ( i >= OriginalPosition ) ? 1 : 0 );
                    Tab.Order.Value = i + Spacer;  
                }
                Tab.postChanges( false );
                Tree.goToParentNode();
            }


        }//UpdateTabPosition

        #endregion

        #region Object class specific properties


        public CswNbtNodePropLogical IncludeInReport { get { return ( _CswNbtNode.Properties[PropertyName.IncludeInReport] ); } }
        public CswNbtNodePropRelationship NodeTypeValue { get { return ( _CswNbtNode.Properties[PropertyName.NodeTypeValue] ); } }
        public CswNbtNodePropNumber Order { get { return ( _CswNbtNode.Properties[PropertyName.Order] ); } }
        public CswNbtNodePropLogical ServerManaged { get { return ( _CswNbtNode.Properties[PropertyName.ServerManaged] ); } }
        public CswNbtNodePropText TabName { get { return ( _CswNbtNode.Properties[PropertyName.TabName] ); } }

        #endregion

    }//CswNbtObjClassDesignNodeTypeTab

}//namespace ChemSW.Nbt.ObjClasses
