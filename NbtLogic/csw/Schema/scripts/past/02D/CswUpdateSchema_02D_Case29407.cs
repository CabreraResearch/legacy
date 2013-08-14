using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29407
    /// </summary>
    public class CswUpdateSchema_02D_Case29407 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29407; }
        }

        public override void update()
        {
            CswNbtMetaDataFieldType BarcodeFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Barcode );
            string SelectText = "select nodetypepropid from nodetype_props where fieldtypeid = " + BarcodeFT.FieldTypeId;
            CswArbitrarySelect GetBarcodePropsSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "Get Barcode Props Select", SelectText );
            DataTable BarcodePropsTable = GetBarcodePropsSelect.getTable();
            foreach( DataRow BarcodePropsRow in BarcodePropsTable.Rows )
            {
                CswNbtMetaDataNodeTypeProp BarcodeProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( CswConvert.ToInt32( BarcodePropsRow["nodetypepropid"] ) );
                BarcodeProp.ReadOnly = true;
            }
        } // update()

    }//class CswUpdateSchema_02B_Case29407

}//namespace ChemSW.Nbt.Schema