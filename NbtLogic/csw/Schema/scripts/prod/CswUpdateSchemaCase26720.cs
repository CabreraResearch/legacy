using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26720
    /// </summary>
    public class CswUpdateSchemaCase26720 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Set 'Allow Inventory' on Room location and below

            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp LocationAllowInvOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.AllowInventoryPropertyName);

            // Make it required
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocationAllowInvOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            // Default values on nodetypes
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationAllowInvNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.AllowInventoryPropertyName );
                if( LocationNT.NodeTypeName == "Room" ||
                    LocationNT.NodeTypeName == "Cabinet" ||
                    LocationNT.NodeTypeName == "Shelf" ||
                    LocationNT.NodeTypeName == "Box" )
                {
                    LocationAllowInvNTP.DefaultValue.AsLogical.Checked = Tristate.True;
                }
                else
                {
                    LocationAllowInvNTP.DefaultValue.AsLogical.Checked = Tristate.False;
                }
            }

            // Values for existing nodes
            foreach(CswNbtObjClassLocation LocationNode in LocationOC.getNodes(false, true))
            {
                if( LocationNode.NodeType.NodeTypeName == "Room" ||
                    LocationNode.NodeType.NodeTypeName == "Cabinet" ||
                    LocationNode.NodeType.NodeTypeName == "Shelf" ||
                    LocationNode.NodeType.NodeTypeName == "Box" )
                {
                    LocationNode.AllowInventory.Checked = Tristate.True;
                }
                else
                {
                    LocationNode.AllowInventory.Checked = Tristate.False;
                }
                LocationNode.postChanges(false);
            } // foreach(CswNbtObjClassLocation in LocationOC.getNodes(false, true))
        }//Update()
    }//class CswUpdateSchemaCase26720

}//namespace ChemSW.Nbt.Schema