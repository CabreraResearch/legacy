using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02D_Case30304 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 30304; }
        }

        public override void update()
        {
            // Set field2 = field1 on any existing List values
            CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "30304_jct_update", "jct_nodes_props" );
            DataTable JctTable = JctUpdate.getTable( new CswCommaDelimitedString() {"jctnodepropid", "nodetypepropid", "field1", "field2"},
                                                     "",
                                                     Int32.MinValue,
                                                     @"where field1 is not null 
                                                         and field2 is null 
                                                         and nodetypepropid in (select nodetypepropid 
                                                                                  from nodetype_props 
                                                                                 where fieldtypeid in (select fieldtypeid 
                                                                                                         from field_types 
                                                                                                        where fieldtype = 'List'))",
                                                     false );
            foreach( DataRow Row in JctTable.Rows )
            {
                Row["field2"] = Row["field1"];
            }
            JctUpdate.update( JctTable );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema