using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;

using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-01
    /// </summary>
    public class CswUpdateSchemaTo01H01 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 01 ); } }
        public CswUpdateSchemaTo01H01( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // This script is reserved for schema changes, 
            // such as adding tables or columns, 
            // which need to take place before any other changes can be made.

            // BZ 9754
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "object_class_props", "isglobalunique", "Globally unique property value among all nodetypes", false, false );

            // BZ 10319
            _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "extended", "Generic Multi-purpose column", false, false, 10 );
            _CswNbtSchemaModTrnsctn.addStringColumn( "object_class_props", "extended", "Generic Multi-purpose column", false, false, 10 );
            _CswNbtSchemaModTrnsctn.addDateColumn( "jct_nodes_props", "field2_date", "Second Date Field", false, false);

            // BZ 5073
            _CswNbtSchemaModTrnsctn.dropColumn( "object_class_props", "defaultvalue" );
            _CswNbtSchemaModTrnsctn.addColumn( "defaultvalueid", DataDictionaryColumnType.Fk, Int32.MinValue, Int32.MinValue,
                                               string.Empty, "Default Value for Object Class Prop", "jctnodepropid", "jct_nodes_props",
                                               false, false, false, string.Empty, false, DataDictionaryPortableDataType.Long, false,
                                               false, "object_class_props", DataDictionaryUniqueType.None, false, string.Empty );
            _CswNbtSchemaModTrnsctn.addColumn( "objectclasspropid", DataDictionaryColumnType.Fk, Int32.MinValue, Int32.MinValue,
                                               string.Empty, "fk, if this row is an OCP default value", "objectclasspropid", "object_class_props",
                                               false, false, false, string.Empty, false, DataDictionaryPortableDataType.Long, false,
                                               false, "jct_nodes_props", DataDictionaryUniqueType.None, false, string.Empty );

            // Case 20081
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetype_props", "hideinmobile", "Exclude this property from Mobile", false, false );

            // Case 20083
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetype_props", "mobilesearch", "Include this property in Mobile Search", false, false );

            // Case 20509
            _CswNbtSchemaModTrnsctn.addBooleanColumn( "node_views", "formobile", "Include this view in Mobile", false, false );
            

            // New UserSelect Fieldtype
            CswTableUpdate FieldTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-05_FT_Update", "field_types" );
            DataTable NewFieldTypeTable = FieldTypesUpdate.getEmptyTable();
            DataRow NewFTRow = NewFieldTypeTable.NewRow();
            NewFTRow["fieldtype"] = "UserSelect";
            NewFTRow["datatype"] = "string";
            NewFTRow["deleted"] = "0";
            NewFieldTypeTable.Rows.Add( NewFTRow );
            FieldTypesUpdate.update( NewFieldTypeTable );

        }//Update()

    }//class CswUpdateSchemaTo01H01

}//namespace ChemSW.Nbt.Schema


