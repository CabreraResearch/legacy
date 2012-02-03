using System.Data;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-03
    /// </summary>
    public class CswUpdateSchemaTo01M03 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 03 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        private void AddRow( DataTable dt, int ft, string subfieldname, string subfieldalias, string reportable, string is_default )
        {
            DataRow dr = dt.NewRow();
            dr["fieldtypeid"] = ft;
            dr["subfieldname"] = subfieldname;
            dr["subfieldalias"] = subfieldalias;
            dr["reportable"] = reportable;
            dr["is_default"] = is_default;
            dt.Rows.Add( dr );
        }

        public override void update()
        {


            #region //case 22962

            //ADD subfield records
            CswTableUpdate ftsTbl = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "field_types_subfields_upd", "field_types_subfields" );
            DataTable ftsDataTbl = ftsTbl.getTable();

            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Barcode ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Composite ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.DateTime ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.DateTime ).FieldTypeId, "field1_date", "date", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.File ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Link ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.List ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Location ).FieldTypeId, "field1_fk", "FK", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Location ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.LocationContents ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Logical ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.LogicalSet ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Memo ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.MOL ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.MTBF ).FieldTypeId, "field1", "startdate", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.MTBF ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.MTBF ).FieldTypeId, "field2", "units", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.MTBF ).FieldTypeId, "field1_numeric", "value", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.MultiList ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.NFPA ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.NFPA ).FieldTypeId, "field1", "flammability", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.NFPA ).FieldTypeId, "field2", "reactivity", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.NFPA ).FieldTypeId, "field3", "health", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.NFPA ).FieldTypeId, "field4", "special", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Number ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Number ).FieldTypeId, "field1_numeric", "number", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.PropertyReference ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Quantity ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Quantity ).FieldTypeId, "field2", "units", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Quantity ).FieldTypeId, "field1_numeric", "quantity", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Question ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Question ).FieldTypeId, "field1", "answer", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Question ).FieldTypeId, "field2", "correctiveaction", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Question ).FieldTypeId, "field1_date", "date_answered", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Question ).FieldTypeId, "field2_date", "date_corrected", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Question ).FieldTypeId, "field3", "iscompliant", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Question ).FieldTypeId, "clobdata", "comments", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Relationship ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Relationship ).FieldTypeId, "field1_fk", "FK", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Scientific ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Scientific ).FieldTypeId, "field1_numeric", "base", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Scientific ).FieldTypeId, "field2_numeric", "exponent", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Sequence ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Static ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.Text ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.TimeInterval ).FieldTypeId, "gestalt", "", "1", "1" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.TimeInterval ).FieldTypeId, "field1", "interval", "1", "0" );
            AddRow( ftsDataTbl, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( MetaData.CswNbtMetaDataFieldType.NbtFieldType.TimeInterval ).FieldTypeId, "field1_date", "startdatetime", "1", "0" );


            #endregion


        }//Update()

    }//class CswUpdateSchemaTo01M03

}//namespace ChemSW.Nbt.Schema