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
        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {
            //Create action
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Assign_Inventory_Groups, true, "", "System" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswNbtModuleName.CISPro.ToString(), CswNbtActionName.Assign_Inventory_Groups );

            //Make inventory group property required
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            if( null != LocationOC )
            {

                CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryGroupClass );
                if( null != InventoryGroupOC )
                {
                    CswNbtMetaDataObjectClassProp InventoryGroupOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );
                    if( null != InventoryGroupOCP )
                    {
                        CswNbtTreeNode IGTreeNode = null;
                        CswNbtNode DefaultInventoryGroupNode = null;
                        ICswNbtTree TreeOfArbitraryNodes = _CswNbtSchemaModTrnsctn.getArbitraryNodes( "Default Inventory Group", InventoryGroupOC );
                        if( null != ( IGTreeNode = TreeOfArbitraryNodes.getRootTreeNode().ChildNodes[0] ) && ( null != ( DefaultInventoryGroupNode = _CswNbtSchemaModTrnsctn.Nodes[IGTreeNode.NodeKey] ) ) )
                        {
                            foreach( CswNbtNode CurrentLocationNode in LocationOC.getNodes( false, true ) )
                            {
                                if( null == CurrentLocationNode.Properties[InventoryGroupOCP.PropName].AsRelationship.RelatedNodeId )
                                {
                                    CurrentLocationNode.Properties[InventoryGroupOCP.PropName].AsRelationship.RelatedNodeId = DefaultInventoryGroupNode.NodeId;
                                    CurrentLocationNode.postChanges( true );

                                }//if the location doesn't have an inventory group

                            }//iterate location nodes

                        }//if we found a default inventory group


                        _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InventoryGroupOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

                    }//if we found the inventory group OCP

                }//if we found the inventory group OC

            }//if we found the location OC

        } // update()

    }//class  CswUpdateSchema_02A_Case28927_01

}//namespace ChemSW.Nbt.Schema