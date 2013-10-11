using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case30048_InventoryLevels: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30048; }
        }

        public override string ScriptName
        {
            get { return "02H_Case30048_InventoryLevels"; }
        }

        public override string Title
        {
            get { return "CAF Import - Inventory Levels"; }
        }

        public override void update()
        {
            CswNbtSchemaUpdateImportMgr InventoryLevelBindings = 
                new CswNbtSchemaUpdateImportMgr( 
                    _CswNbtSchemaModTrnsctn, 
                    "mininventory_basic",//we have two source tables (maxinventory_basic) - is that a problem?
                    "Inventory Level",
                    "inventory_view",
                    SourceColumn: "inventorybasicid" );

            InventoryLevelBindings.importBinding( "inventorybasicid", CswNbtObjClassInventoryLevel.PropertyName.LegacyId, "" );
            InventoryLevelBindings.importBinding( "inventorylevel", CswNbtObjClassInventoryLevel.PropertyName.Level, CswEnumNbtSubFieldName.Value.ToString() );
            InventoryLevelBindings.importBinding( "unitofmeasureid", CswNbtObjClassInventoryLevel.PropertyName.Level, CswEnumNbtSubFieldName.NodeID.ToString() );
            InventoryLevelBindings.importBinding( "locationid", CswNbtObjClassInventoryLevel.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );
            InventoryLevelBindings.importBinding( "packageid", CswNbtObjClassInventoryLevel.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
            InventoryLevelBindings.importBinding( "inventorytype", CswNbtObjClassInventoryLevel.PropertyName.Type, CswEnumNbtSubFieldName.Value.ToString() );

            InventoryLevelBindings.finalize();
        }

    }

}//namespace ChemSW.Nbt.Schema