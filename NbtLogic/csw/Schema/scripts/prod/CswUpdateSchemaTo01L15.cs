using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-15
    /// </summary>
    public class CswUpdateSchemaTo01L15 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 15 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region 24515

            CswTableUpdate ftTblUpd = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01L-15_fieldtype_update", "field_types" );
            DataTable dt = ftTblUpd.getEmptyTable();
            DataRow dtr = dt.NewRow();
            dtr["datatype"] = "JSON";
            dtr["deleted"] = "0";
            dtr["fieldtype"] = CswNbtMetaDataFieldType.NbtFieldType.Comments.ToString();
            dt.Rows.Add( dtr );
            ftTblUpd.update( dt );

            #endregion 24515
        }//Update()

    }//class CswUpdateSchemaTo01L15

}//namespace ChemSW.Nbt.Schema


