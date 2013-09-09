using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30043_InventoryGroups : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30043; }
        }

        public override void update()
        {
            // Case 30043 - CAF Migration: Sites/Locations/Work Units
            CswNbtSchemaUpdateImportMgr importMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "inventory_groups", "Inventory Group" );

            // Binding
            importMgr.importBinding( "inventorygroupname", CswNbtObjClassInventoryGroup.PropertyName.Name, "" );
            importMgr.importBinding( "iscentralgroup", CswNbtObjClassInventoryGroup.PropertyName.Central, "" );

            // Relationship
            //none

            importMgr.finalize();

            //Columns in inventory_groups table
            //deleted
            //inventorygroupid
            //inventorygroupname
            //iscentralgroup
            //workunitid
            //inventorygroupcode

        } // update()

    } // class CswUpdateSchema_02F_Case30043_InventoryGroups

}//namespace ChemSW.Nbt.Schema