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
    /// Updates the schema to version 01M-03
    /// </summary>
    public class CswUpdateSchemaTo01M03 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 03 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        private void AddRow( DataTable dt, int ft, string propcolname, string subfieldname, bool reportable, bool is_default )
        {
            DataRow dr = dt.NewRow();
            dr["fieldtypeid"] = CswConvert.ToDbVal(ft);
            dr["propcolname"] = propcolname;
            dr["subfieldname"] = subfieldname;
            dr["reportable"] = CswConvert.ToDbVal(reportable);
            dr["is_default"] = CswConvert.ToDbVal(is_default);
            dt.Rows.Add( dr );
        }

        public override void update()
        {


            #region //case 22962

            //ADD subfield records
            CswTableUpdate ftsTbl = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "field_types_subfields_upd", "field_types_subfields" );
            DataTable ftsDataTbl = ftsTbl.getTable();

            Dictionary<CswNbtMetaDataFieldType.NbtFieldType, Int32> FieldTypeIds = _CswNbtSchemaModTrnsctn.MetaData.FieldTypeIds;
            foreach( CswNbtMetaDataFieldType.NbtFieldType FieldType in _CswNbtSchemaModTrnsctn.MetaData.FieldTypeIds.Keys )
            {
                ICswNbtFieldTypeRule Rule = _CswNbtSchemaModTrnsctn.MetaData.getFieldTypeRule( FieldType );
                bool reportable = false;
                if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Composite
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.DateTime
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.File
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Link
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.List
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Location
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.LocationContents
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Logical
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.LogicalSet
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Memo
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.MOL
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.MTBF
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.MultiList
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.NFPA
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Number
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.PropertyReference
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Quantity
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Scientific
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Sequence
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Static
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.Text
                    || FieldType == CswNbtMetaDataFieldType.NbtFieldType.TimeInterval
                    )
                {
                    reportable = true;
                }
                //add gestalt row, because it is not a subfield
                AddRow( ftsDataTbl, FieldTypeIds[FieldType], "gestalt", "", reportable, true );

                foreach( CswNbtSubField SubField in Rule.SubFields )
                {
                    bool is_default = false;
                    string subfname = SubField.Name.ToString().ToLower();
                    if( SubField.RelationalColumn.ToLower() == "gestalt" )
                    {
                        is_default = true;
                        subfname="";
                    }
                    AddRow(ftsDataTbl,FieldTypeIds[FieldType],SubField.Column.ToString(),subfname,SubField.isReportable,is_default);
                }

            }
            ftsTbl.update( ftsDataTbl );


            #endregion


        }//Update()

    }//class CswUpdateSchemaTo01M03

}//namespace ChemSW.Nbt.Schema