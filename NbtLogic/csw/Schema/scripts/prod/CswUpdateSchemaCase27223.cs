using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27223
    /// </summary>
    public class CswUpdateSchemaCase27223 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // This script is to clean up bad data in some client schemata.  
            // For an ordinary script, this shouldn't really do anything.


            // Delete bad nodetypes
            string[] DoomedNodeTypes = {
                                          "CISPro Location",
                                          "CISPro Unit Of Measure",
                                          "CISPro User",
                                          "CISPro Vendor",
                                          "Chemical Material",
                                          "PackDetail",
                                          "Package",
                                          "Synonym"
                                       };
            foreach( string DoomedNTName in DoomedNodeTypes )
            {
                CswNbtMetaDataNodeType DoomedNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( DoomedNTName );
                if( null != DoomedNT )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( DoomedNT );
                }
            }


            // Convert Generic nodetypes
            CswNbtMetaDataNodeType ContainerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( null != ContainerNT )
            {
                if( ContainerNT.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass )
                {
                    CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
                    _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( ContainerNT, ContainerOC );
                    ContainerNT.TableName = "nodes";
                }
            }

            CswNbtMetaDataNodeType InventoryGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Group" );
            if( null != InventoryGroupNT )
            {
                if( InventoryGroupNT.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass )
                {
                    CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
                    _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( InventoryGroupNT, InventoryGroupOC );
                    InventoryGroupNT.TableName = "nodes";
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase27223

}//namespace ChemSW.Nbt.Schema