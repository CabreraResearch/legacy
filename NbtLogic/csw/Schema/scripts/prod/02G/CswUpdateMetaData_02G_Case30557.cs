using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02G_Case30557 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Number Datatype"; } }

        public override string ScriptName
        {
            get { return "Case_30557DDL"; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30557; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.changeColumnDataType( "jct_nodes_props", "field1_numeric", CswEnumDataDictionaryPortableDataType.Number, 0 );
            _CswNbtSchemaModTrnsctn.changeColumnDataType( "jct_nodes_props", "field2_numeric", CswEnumDataDictionaryPortableDataType.Number, 0 );
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "jct_nodes_props", "field3_numeric" ) )
            {
                _CswNbtSchemaModTrnsctn.addNumberColumn( "jct_nodes_props", "field3_numeric", "for numeric values", false, true );
            }
        }
    }
}


