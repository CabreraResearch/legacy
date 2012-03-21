using System;
using System.Data;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-05
    /// </summary>
    public class CswUpdateSchemaTo01M05 : CswUpdateSchemaTo
    {
//        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 05 ); } }
//        public override string Description { set { ; } get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 24992
            
            // Fix firsttabversionid on nodetype_tabset
            CswTableUpdate TabUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01M05_tab_update", "nodetype_tabset" );
            DataTable TabTable = TabUpdate.getTable( "where firsttabversionid is null" );
            foreach( DataRow TabRow in TabTable.Rows )
            {
                TabRow["firsttabversionid"] = TabRow["nodetypetabsetid"];
            }
            TabUpdate.update( TabTable );
            
            #endregion case 24992

        }//Update()

    }//class CswUpdateSchemaTo01M05

}//namespace ChemSW.Nbt.Schema