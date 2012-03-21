using System;
using System.Data;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-09
    /// </summary>
    public class CswUpdateSchemaTo01M09 : CswUpdateSchemaTo
    {
//        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 09 ); } }
//        public override string Description { set { ; } get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 25210

            CswTableUpdate FTUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01M09_FT_Update", "field_types" );
            DataTable FTTable = FTUpdate.getTable();
            foreach( DataRow FTRow in FTTable.Rows )
            {
                CswNbtMetaDataFieldType.NbtFieldType FieldType = (CswNbtMetaDataFieldType.NbtFieldType) Enum.Parse( typeof( CswNbtMetaDataFieldType.NbtFieldType ), FTRow["fieldtype"].ToString() );

                if( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Image ||
                    FieldType == CswNbtMetaDataFieldType.NbtFieldType.Button ||
                    FieldType == CswNbtMetaDataFieldType.NbtFieldType.LogicalSet ||
                    FieldType == CswNbtMetaDataFieldType.NbtFieldType.ViewPickList ||
                    FieldType == CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect ||
                    FieldType == CswNbtMetaDataFieldType.NbtFieldType.MOL ||
                    FieldType == CswNbtMetaDataFieldType.NbtFieldType.MTBF ||
                    FieldType == CswNbtMetaDataFieldType.NbtFieldType.Grid ||
                    FieldType == CswNbtMetaDataFieldType.NbtFieldType.Password )
                {

                    FTRow["searchable"] = CswConvert.ToDbVal( false );
                }
            }
            FTUpdate.update( FTTable );

            #endregion case 25210

        }//Update()

    }//class CswUpdateSchemaTo01M09

}//namespace ChemSW.Nbt.Schema