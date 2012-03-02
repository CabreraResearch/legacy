using System;
using System.Data;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-12
    /// </summary>
    public class CswUpdateSchemaTo01M12 : CswUpdateSchemaTo
    {
        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 12 ); } }
        //public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            CswTableUpdate ActionsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01M12_Actions_update", "actions" );
            DataTable ActionsTable = ActionsUpdate.getTable( "where actionname = 'Future Scheduling'" );
            foreach( DataRow ActionsRow in ActionsTable.Rows )
            {
                ActionsRow["url"] = DBNull.Value;
            }
            ActionsUpdate.update( ActionsTable );

        }//Update()

    }//class CswUpdateSchemaTo01M12

}//namespace ChemSW.Nbt.Schema