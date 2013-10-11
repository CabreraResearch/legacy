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
            CswNbtSchemaUpdateImportMgr MinInventoryLevelBindings = 
                new CswNbtSchemaUpdateImportMgr( 
                    _CswNbtSchemaModTrnsctn, 
                    "mininventory_basic", 
                    "Inventory Level",
                    "mininventory_view",
                    SourceColumn: "mininventorybasicid" );

            MinInventoryLevelBindings.importBinding( "mininventorybasicid", CswNbtObjClassInventoryLevel.PropertyName.LegacyId, "" );
            MinInventoryLevelBindings.importBinding( "mininventorylevel", CswNbtObjClassInventoryLevel.PropertyName.Level, CswEnumNbtSubFieldName.Value.ToString() );
            MinInventoryLevelBindings.importBinding( "mininventoryunitofmeasureid", CswNbtObjClassInventoryLevel.PropertyName.Level, CswEnumNbtSubFieldName.NodeID.ToString() );
            //We're using the top location associated with the given inventorygroupid
            MinInventoryLevelBindings.importBinding( "locationid", CswNbtObjClassInventoryLevel.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );
            MinInventoryLevelBindings.importBinding( "packageid", CswNbtObjClassInventoryLevel.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
            MinInventoryLevelBindings.importBinding( "inventorytype", CswNbtObjClassInventoryLevel.PropertyName.Type, CswEnumNbtSubFieldName.Value.ToString() );

            MinInventoryLevelBindings.finalize();

            CswNbtSchemaUpdateImportMgr MaxInventoryLevelBindings =
                new CswNbtSchemaUpdateImportMgr(
                    _CswNbtSchemaModTrnsctn,
                    "maxinventory_basic",
                    "Inventory Level",
                    "maxinventory_view",
                    SourceColumn: "maxinventorybasicid" );

            MaxInventoryLevelBindings.importBinding( "maxinventorybasicid", CswNbtObjClassInventoryLevel.PropertyName.LegacyId, "" );
            MaxInventoryLevelBindings.importBinding( "maxinventorylevel", CswNbtObjClassInventoryLevel.PropertyName.Level, CswEnumNbtSubFieldName.Value.ToString() );
            MaxInventoryLevelBindings.importBinding( "unitofmeasureid", CswNbtObjClassInventoryLevel.PropertyName.Level, CswEnumNbtSubFieldName.NodeID.ToString() );
            MaxInventoryLevelBindings.importBinding( "locationid", CswNbtObjClassInventoryLevel.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );
            //We're using the first packageid associated with the given materialid
            MaxInventoryLevelBindings.importBinding( "packageid", CswNbtObjClassInventoryLevel.PropertyName.Material, CswEnumNbtSubFieldName.NodeID.ToString() );
            MaxInventoryLevelBindings.importBinding( "inventorytype", CswNbtObjClassInventoryLevel.PropertyName.Type, CswEnumNbtSubFieldName.Value.ToString() );

            MaxInventoryLevelBindings.finalize();
        }

    }

}//namespace ChemSW.Nbt.Schema