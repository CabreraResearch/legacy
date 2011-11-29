using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01K-02
    /// </summary>
    public class CswUpdateSchemaTo01K02 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 02 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
			// case 24166
            CswTableSelect ModulesTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "01K02_modules_select", "modules" );
            DataTable ModulesTable = ModulesTableSelect.getTable("where name = 'SI'");
			if( ModulesTable.Rows.Count > 0 )
			{
				Int32 SIModuleId = CswConvert.ToInt32( ModulesTable.Rows[0]["moduleid"] );
				Int32 OOCActionId = _CswNbtSchemaModTrnsctn.Actions[Actions.CswNbtActionName.OOC_Inspections].ActionId;

				_CswNbtSchemaModTrnsctn.createModuleActionJunction( SIModuleId, OOCActionId );
			}
        }//Update()

    }//class CswUpdateSchemaTo01K02

}//namespace ChemSW.Nbt.Schema


