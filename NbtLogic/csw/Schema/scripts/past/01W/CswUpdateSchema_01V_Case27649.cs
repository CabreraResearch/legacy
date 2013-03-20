using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27649
    /// </summary>
    public class CswUpdateSchema_01V_Case27649 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 27649; }
        }

        public override void update()
        {
            // Fix 'searchable' on fieldtypes

            CswTableUpdate FTUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "27649_fieldtypes", "field_types" );
            DataTable FTTable = FTUpdate.getTable( "where searchable is null" );
            foreach(DataRow Row in FTTable.Rows)
            {
                if( CswConvert.ToString( Row["fieldtype"] ) == CswNbtMetaDataFieldType.NbtFieldType.External )
                {
                    Row["searchable"] = CswConvert.ToDbVal( false );
                }
                else
                {
                    Row["searchable"] = CswConvert.ToDbVal( true );
                }
            }
            FTUpdate.update( FTTable );

        } //Update()

    }//class CswUpdateSchema_01V_Case27649

}//namespace ChemSW.Nbt.Schema