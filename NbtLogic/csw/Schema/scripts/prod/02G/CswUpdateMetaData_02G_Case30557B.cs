using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02G_Case30557B : CswUpdateSchemaTo
    {
        public override string Title { get { return "Quantity Val_kg and Val_Liters SubFields"; } }

        public override string ScriptName
        {
            get { return "Case_30557FT"; }
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
            CswNbtFieldTypeRuleQuantity QuantityFTRule = (CswNbtFieldTypeRuleQuantity) _CswNbtSchemaModTrnsctn.MetaData.getFieldTypeRule( CswEnumNbtFieldType.Quantity );
            CswNbtMetaDataFieldType QuantityFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Quantity );
            //Fix "value" so that we don't get a duplicate with "Value"
            CswTableUpdate FTSubUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "UserSelectUpdate", "field_types_subfields" );
            DataTable FTSubTable = FTSubUpdate.getTable( "where fieldtypeid = " + QuantityFT.FieldTypeId + " and propcolname = '" + CswEnumNbtPropColumn.Field1_Numeric + "'" );
            if( FTSubTable.Rows.Count > 0 )
            {
                FTSubTable.Rows[0].Delete();
                FTSubUpdate.update( FTSubTable );
            }


            foreach( CswNbtSubField SubField in QuantityFTRule.SubFields )
            {
                _CswNbtSchemaModTrnsctn.createFieldTypesSubFieldsJunction( QuantityFT, SubField.Column, SubField.Name, true );
            }
        }
    }
}


