using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Container Module
    /// </summary>
    public class CswNbtModuleRuleContainers : CswNbtModuleRule
    {
        public CswNbtModuleRuleContainers( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.Containers; } }
        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswNbtModuleName.CISPro );
            }

            //Show the following Location properties...
            //   Containers
            //   Inventory Levels
            //   Allow Inventory
            //   Inventory Group
            //   Control Zone
            //   Storate Compatibility
            CswNbtMetaDataObjectClass locationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            foreach( int NodeTypeId in locationOC.getNodeTypeIds() )
            {
                _addPropToTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.Containers, "Containers" );
                _addPropToTab( NodeTypeId, "Inventory Levels", "Inventory Levels", 2 );
                _addPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _addPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _addPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.ControlZone );
                _addPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );
            }

            //Show the following Material properties...
            //   Inventory Levels
            //   Sizes
            //   Containers
            //   Approved for Receiving
            //   Receive (button)
            //   Request (button)
            //   Storage Compatibility
            int materialOC_ID = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType materialNT in _CswNbtResources.MetaData.getNodeTypes( materialOC_ID ) )
            {
                string sizesNTPName = materialNT.NodeTypeName + " Sizes";
                _addPropToTab( materialNT.NodeTypeId, sizesNTPName, "Containers", 99 );

                _addPropToTab( materialNT.NodeTypeId, "Inventory Levels", "Containers", 99 );

                string containersNTPName = materialNT.NodeTypeName + " Containers";
                _addPropToTab( materialNT.NodeTypeId, containersNTPName, "Containers", 99 );

                CswNbtMetaDataNodeTypeTab materialNTT = materialNT.getFirstNodeTypeTab();
                _addPropToTab( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.ApprovedForReceiving, materialNTT );

                CswNbtMetaDataNodeTypeTab materialIdentityNTT = materialNT.getIdentityTab();
                _addPropToTab( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Receive, materialIdentityNTT, 2, 2 );
                _addPropToTab( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Request, materialIdentityNTT, 1, 2 );

                _addPropToTab( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.StorageCompatibility, "Hazards" );
            }

            //Show the following User props...
            //   Work Unit
            int userOC_Id = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.UserClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( userOC_Id ) )
            {
                _addPropToFirstTab( NodeTypeId, CswNbtObjClassUser.PropertyName.WorkUnit );
            }

            //Show all views in the Containers category
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "Containers", NbtViewVisibility.Global );

            //Show all reports in the Containers category
            _toggleReportNodes( "Containers", false );

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            _CswNbtResources.Modules.ToggleAction( true, CswNbtActionName.KioskMode );

            //Show Print Labels with a dependent NodeType
            _togglePrintLabels( false );
        }

        public override void OnDisable()
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.MLM ) )
            {
                _CswNbtResources.Modules.DisableModule( CswNbtModuleName.MLM );
            }

            //Hide the following Location properties...
            //   Containers
            //   Inventory Levels
            //   Allow Inventory
            //   Inventory Group
            //   Control Zone
            //   Storate Compatibility
            CswNbtMetaDataObjectClass locationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            foreach( int NodeTypeId in locationOC.getNodeTypeIds() )
            {
                _hideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.Containers );
                _hideProp( NodeTypeId, "Inventory Levels" );
                _hideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _hideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _hideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.ControlZone );
                _hideProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );
            }

            //Hide the following Material properties...
            //   Inventory Levels
            //   Sizes
            //   Containers
            //   Approved for Receiving
            //   Receive (button)
            //   Request (button)
            //   Storage Compatibility
            int materialOC_ID = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType materialNT in _CswNbtResources.MetaData.getNodeTypes( materialOC_ID ) )
            {
                string sizesNTPName = materialNT.NodeTypeName + " Sizes";
                _hideProp( materialNT.NodeTypeId, sizesNTPName );

                _hideProp( materialNT.NodeTypeId, "Inventory Levels" );

                string containersNTPName = materialNT.NodeTypeName + " Containers";
                _hideProp( materialNT.NodeTypeId, containersNTPName );

                _hideProp( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.ApprovedForReceiving );
                _hideProp( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Receive );
                _hideProp( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Request );
                _hideProp( materialNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.StorageCompatibility );
            }

            //Hide the following User props...
            //   Work Unit
            int userOC_Id = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.UserClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( userOC_Id ) )
            {
                _hideProp( NodeTypeId, CswNbtObjClassUser.PropertyName.WorkUnit );
            }

            //Hide all views in the Containers category
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "Containers", NbtViewVisibility.Global );

            //Hide all reports in the Containers category
            _toggleReportNodes( "Containers", true );

            //We handle Kiosk Mode in module logic because it can be turned on by different modules
            _CswNbtResources.Modules.ToggleAction( false, CswNbtActionName.KioskMode );

            //Hide Print Labels with a dependent NodeType
            _togglePrintLabels( true );

        } // OnDisable()

        #region Private helpers

        private void _toggleReportNodes( string Category, bool Hidden )
        {
            CswNbtMetaDataObjectClass reportOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp categoryOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.Category );

            CswNbtView reportsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = reportsView.AddViewRelationship( reportOC, false );
            reportsView.AddViewPropertyAndFilter( parent, categoryOCP,
                Value: Category,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            ICswNbtTree reportsTree = _CswNbtResources.Trees.getTreeFromView( reportsView, false, true, true );
            int childCount = reportsTree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                reportsTree.goToNthChild( i );
                CswNbtNode reportNode = reportsTree.getNodeForCurrentPosition();
                reportNode.Hidden = Hidden;
                reportNode.postChanges( false );
                reportsTree.goToParentNode();
            }
        }

        private void _togglePrintLabels( bool Hidden )
        {
            CswNbtMetaDataObjectClass printLabelOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp nodetypesOCP = printLabelOC.getObjectClassProp( CswNbtObjClassPrintLabel.PropertyName.NodeTypes );

            CswNbtView printLabelsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = printLabelsView.AddViewRelationship( printLabelOC, false );

            CswTableSelect childObjectClasses_TS = _CswNbtResources.makeCswTableSelect( "getModuleChildren", "jct_modules_objectclass" );

            int containerModuleId = _CswNbtResources.Modules.GetModuleId( CswNbtModuleName.Containers );
            DataTable childObjClasses_DT = childObjectClasses_TS.getTable( "where moduleid = " + containerModuleId );
            bool first = true;
            foreach( DataRow Row in childObjClasses_DT.Rows )
            {
                int ObjClassId = CswConvert.ToInt32( Row["objectclassid"] );
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes( ObjClassId ) )
                {
                    if( first )
                    {
                        printLabelsView.AddViewPropertyAndFilter( parent, nodetypesOCP,
                            Value: NodeType.NodeTypeName,
                            FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains );

                        first = false;
                    }
                    else
                    {
                        printLabelsView.AddViewPropertyAndFilter( parent, nodetypesOCP,
                            Value: NodeType.NodeTypeName,
                            FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains,
                            Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.Or );
                    }
                }
            }


            ICswNbtTree printLabelsTree = _CswNbtResources.Trees.getTreeFromView( printLabelsView, false, true, true );
            int childCount = printLabelsTree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                printLabelsTree.goToNthChild( i );
                CswNbtNode printLabelNode = printLabelsTree.getNodeForCurrentPosition();
                printLabelNode.Hidden = Hidden;
                printLabelNode.postChanges( false );
                printLabelsTree.goToParentNode();
            }
        }

        private void _addPropToFirstTab( int NodeTypeId, string PropName, int Row = Int32.MinValue, int Col = Int32.MinValue )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtMetaDataNodeTypeTab firstNTT = NodeType.getFirstNodeTypeTab();
            _addPropToTab( NodeTypeId, PropName, firstNTT, Row, Col );
        }

        private void _addPropToTab( int NodeTypeId, string PropName, CswNbtMetaDataNodeTypeTab Tab, int Row = Int32.MinValue, int Col = Int32.MinValue )
        {
            CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeId, PropName );
            if( null != NodeTypeProp )
            {
                CswNbtMetaDataNodeType locationNT = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( Int32.MinValue != Row && Int32.MinValue != Col )
                {
                    NodeTypeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, Tab.TabId, DisplayRow: Row, DisplayColumn: Col );
                }
                else
                {
                    NodeTypeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, Tab.TabId );
                }
            }
        }

        private void _addPropToTab( int NodeTypeId, string PropName, string TabName, int TabOrder = 99 )
        {
            CswNbtMetaDataNodeTypeTab tab = _CswNbtResources.MetaData.getNodeTypeTab( NodeTypeId, TabName );
            if( null == tab )
            {
                CswNbtMetaDataNodeType locationNT = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                tab = _CswNbtResources.MetaData.makeNewTab( locationNT, TabName, TabOrder );
            }
            _addPropToTab( NodeTypeId, PropName, tab );
        }

        private void _hideProp( int NodeTypeId, string PropName )
        {
            CswNbtMetaDataNodeTypeProp NodeTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypeId, PropName );
            if( null != NodeTypeProp )
            {
                NodeTypeProp.removeFromAllLayouts();
            }
        }

        #endregion

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
