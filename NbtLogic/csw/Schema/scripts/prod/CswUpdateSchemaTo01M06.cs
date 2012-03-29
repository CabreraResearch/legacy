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
    /// Updates the schema to version 01M-06
    /// </summary>
    public class CswUpdateSchemaTo01M06 : CswUpdateSchemaTo
    {
        public override void update()
        {
            #region case 24481

            CswTableUpdate FTUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01M06_fieldtypes_update", "field_types" );
            DataTable FTTAble = FTUpdate.getTable();
            foreach( DataRow FTRow in FTTAble.Rows )
            {
                if( FTRow["fieldtype"].ToString() == CswNbtMetaDataFieldType.NbtFieldType.Password.ToString() )
                {
                    FTRow["searchable"] = CswConvert.ToDbVal( false );
                }
                else
                {
                    FTRow["searchable"] = CswConvert.ToDbVal( true );
                }
            }
            FTUpdate.update( FTTAble );

            #endregion case 24481

        }//Update()

    }//class CswUpdateSchemaTo01M06

}//namespace ChemSW.Nbt.Schema