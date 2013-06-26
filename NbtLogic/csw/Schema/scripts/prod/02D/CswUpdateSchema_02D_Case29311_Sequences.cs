using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29311
    /// </summary>
    public class CswUpdateSchema_02D_Case29311_Sequences : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29311; }
        }

        public override void update()
        {
            CswTableUpdate jctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29311_jctddntp_update", "jct_dd_ntp" );
            DataTable jctTable = jctUpdate.getEmptyTable();

            // Set up Sequence Nodetype



        } // update()


        private void _addJctRow( DataTable JctTable, CswNbtMetaDataNodeTypeProp Prop, string TableName, string ColumnName, CswEnumNbtSubFieldName SubFieldName = null )
        {
            _CswNbtSchemaModTrnsctn.CswDataDictionary.setCurrentColumn( TableName, ColumnName );
            DataRow NodeTypeNameRow = JctTable.NewRow();
            NodeTypeNameRow["nodetypepropid"] = Prop.PropId;
            NodeTypeNameRow["datadictionaryid"] = _CswNbtSchemaModTrnsctn.CswDataDictionary.TableColId;
            if( null != SubFieldName )
            {
                NodeTypeNameRow["subfieldname"] = SubFieldName.ToString();
            }
            else if( null != Prop.getFieldTypeRule().SubFields.Default )
            {
                NodeTypeNameRow["subfieldname"] = Prop.getFieldTypeRule().SubFields.Default.Name;
            }
            JctTable.Rows.Add( NodeTypeNameRow );
        }


    }//class CswUpdateSchema_02D_Case29311_Design

}//namespace ChemSW.Nbt.Schema