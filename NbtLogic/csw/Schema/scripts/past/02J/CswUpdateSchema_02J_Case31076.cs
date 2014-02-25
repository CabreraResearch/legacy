using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case31076 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31076; }
        }

        public override string Title
        {
            get { return "Fix Formula Searchable"; }
        }

        public override void update()
        {
            CswNbtMetaDataFieldType FormulaFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Formula );
            CswTableUpdate FieldTypeUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "31076_update_fieldtypes", "field_types" );
            DataTable FieldTypeTable = FieldTypeUpdate.getTable( "fieldtypeid", FormulaFT.FieldTypeId );
            if( FieldTypeTable.Rows.Count > 0 )
            {
                FieldTypeTable.Rows[0]["searchable"] = CswConvert.ToDbVal( true );
                FieldTypeUpdate.update( FieldTypeTable );
            }
        } // update()
    }

}//namespace ChemSW.Nbt.Schema