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
    public class CswUpdateSchema_02A_Case28927_02 : CswUpdateSchemaTo
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

            CswNbtMetaDataNodeType InventoryGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Group" );
            if( null != InventoryGroupNT ) 
            {
                CswNbtMetaDataNodeTypeProp LocationsNTP = InventoryGroupNT.getNodeTypeProp( "Locations" );
                if( null != LocationsNTP )
                {
                    CswNbtViewId LocationsViewId = LocationsNTP.ViewId;
                    CswNbtView IvgLocationsView = _CswNbtSchemaModTrnsctn.restoreView( LocationsViewId );
                    if( ( null != IvgLocationsView ) && ( null != IvgLocationsView.Root.ChildRelationships[0] ) && ( null != IvgLocationsView.Root.ChildRelationships[0].ChildRelationships[0] ) )
                    {
                        IvgLocationsView.Root.ChildRelationships[0].ChildRelationships[0].AddChildren = NbtViewAddChildrenSetting.None;
                        IvgLocationsView.save();
                    }

                }//if we have the locations nt

                CswNbtMetaDataNodeTypeProp AssignLocationsNTP = InventoryGroupNT.getNodeTypeProp( CswNbtObjClassInventoryGroup.PropertyName.AssignLocation );
                CswNbtMetaDataNodeTypeProp LocationsGridNTP = InventoryGroupNT.getNodeTypeProp( "Locations" );
                if( ( null != AssignLocationsNTP ) && ( null != LocationsGridNTP )  )
                {
                    CswNbtMetaDataNodeTypeTab LocationsTab = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeTab( InventoryGroupNT.NodeTypeId, "Locations" );
                    if( null != LocationsTab )
                    {
                        
                        //AssignLocationsNTP
                        //_CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.
                        _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                            CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                            InventoryGroupNT.NodeTypeId,
                            AssignLocationsNTP,
                            true,
                            LocationsTab.TabId,
                            1
                            );

                        _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                            CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                            InventoryGroupNT.NodeTypeId,
                            LocationsGridNTP,
                            true,
                            LocationsTab.TabId,
                            2
                            );
                        //AssignLocationsNTP.
                    }//if we have a locatioins tab

                }//if we have an assign locations button. 

            }//if we have the inventory group nt
            
        } // update()

    }//class  CswUpdateSchema_02A_Case28927_02

}//namespace ChemSW.Nbt.Schema