using System.Data;
using System.Collections.Generic;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Search;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02A_Case28927_01 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {
            //Create action
            _CswNbtSchemaModTrnsctn.createAction( CswEnumNbtActionName.Assign_Inventory_Groups, true, "", "System" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswEnumNbtModuleName.CISPro.ToString(), CswEnumNbtActionName.Assign_Inventory_Groups );

            //Make inventory group property required
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            if( null != LocationOC )
            {

                CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
                if( null != InventoryGroupOC )
                {
                    CswNbtMetaDataObjectClassProp InventoryGroupOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                    if( null != InventoryGroupOCP )
                    {

                        CswNbtSearch InventoryGroupNodeSearch = _CswNbtSchemaModTrnsctn.CswNbtSearch;
                        InventoryGroupNodeSearch.SearchTerm = "Default Inventory Group";
                        InventoryGroupNodeSearch.addFilter( InventoryGroupOC, false );

                        ICswNbtTree TreeOfArbitraryNodes = InventoryGroupNodeSearch.Results();
                        if( TreeOfArbitraryNodes.getRootTreeNode().ChildNodes.Count > 0 )
                        {

                            CswNbtTreeNode IGTreeNode = TreeOfArbitraryNodes.getRootTreeNode().ChildNodes[0];
                            CswNbtNode DefaultInventoryGroupNode = _CswNbtSchemaModTrnsctn.Nodes[IGTreeNode.NodeKey];
                            if( null != DefaultInventoryGroupNode )
                            {
                                //*** get tree of locations that don't have an inventory group
                                CswNbtView ViewOfLocationsWithNullIG = _CswNbtSchemaModTrnsctn.makeView();
                                //                            ViewOfLocationsWithNullIG.makeNew( "Null IVG Locations", NbtViewVisibility.Global, null, null );
                                ViewOfLocationsWithNullIG.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                                ViewOfLocationsWithNullIG.Category = "System";
                                ViewOfLocationsWithNullIG.Width = 100;
                                CswNbtViewRelationship ViewRelLocationsOC = ViewOfLocationsWithNullIG.AddViewRelationship( LocationOC, true );


                                CswNbtViewProperty ViewPropIGOCP = ViewOfLocationsWithNullIG.AddViewProperty( ViewRelLocationsOC, InventoryGroupOCP );

                                ViewOfLocationsWithNullIG.AddViewPropertyFilter( ViewPropIGOCP, CswEnumNbtSubFieldName.NodeID, CswEnumNbtFilterMode.Null );

                                ICswNbtTree TreeOfLocations = _CswNbtSchemaModTrnsctn.getTreeFromView( ViewOfLocationsWithNullIG, true );


                                TreeOfLocations.goToRoot();
                                int TotalLocationNodes = TreeOfLocations.getChildNodeCount();
                                for( int idx = 0; idx < TotalLocationNodes; idx++ )
                                {
                                    TreeOfLocations.goToNthChild( idx );
                                    CswNbtObjClassLocation CurrentLocationNode = TreeOfLocations.getNodeForCurrentPosition();
                                    CurrentLocationNode.InventoryGroup.RelatedNodeId = DefaultInventoryGroupNode.NodeId;
                                    CurrentLocationNode.postChanges( true );
                                    TreeOfLocations.goToParentNode(); 

                                } // for( int idx = 0; idx < TotalTargetNodes; idx++ )


                            }//if there are any matching inventory group nodes




                            //foreach( CswNbtNode CurrentLocationNode in LocationOC.getNodes( false, true ) )
                            //{
                            //    if( null == CurrentLocationNode.Properties[InventoryGroupOCP.PropName].AsRelationship.RelatedNodeId )
                            //    {
                            //        CurrentLocationNode.Properties[InventoryGroupOCP.PropName].AsRelationship.RelatedNodeId = DefaultInventoryGroupNode.NodeId;
                            //        CurrentLocationNode.postChanges( true );

                            //    }//if the location doesn't have an inventory group

                            //}//iterate location nodes

                        }//if we found a default inventory group


                        _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InventoryGroupOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

                    }//if we found the inventory group OCP

                }//if we found the inventory group OC

            }//if we found the location OC

        } // update()

    }//class  CswUpdateSchema_02A_Case28927_01

}//namespace ChemSW.Nbt.Schema