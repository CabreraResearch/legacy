using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!

            /* TODO: Delete on moving to Quince */
            //case 26881


            CswSequenceName CswSequenceName = new Nbt.CswSequenceName( "tablecolid" );
            if( false == _CswNbtSchemaModTrnsctn.doesSequenceExist( CswSequenceName ) )
            {
                string Query = "select max(tablecolid) as \"max\" from data_dictionary";
                CswArbitrarySelect CswArbitrarySelectDataDictionary = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "maxdatadictionaryrows", Query );
                DataTable DataTableDataDictionary = CswArbitrarySelectDataDictionary.getTable();
                if( DataTableDataDictionary.Rows.Count <= 0 )
                {
                    throw ( new CswDniException( "The following query should have returned one row but did not: " + Query ) );
                }

                Int32 MaxSequenceVal = CswConvert.ToInt32( DataTableDataDictionary.Rows[0]["max"] );
                MaxSequenceVal++;

                _CswNbtSchemaModTrnsctn.makeSequence( CswSequenceName, "", "", 0, MaxSequenceVal );
            }

            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "valuepropid" ) )
            {
                _CswNbtSchemaModTrnsctn.changeColumnDataType( "object_class_props", "valuepropid", DataDictionaryPortableDataType.Int, 12 );
            }

            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "valueproptype" ) )
            {
                _CswNbtSchemaModTrnsctn.changeColumnDataType( "object_class_props", "valueproptype", DataDictionaryPortableDataType.String, 40 );
            }

            // case 24441
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "textarearows" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "object_class_props", "textarearows", "Height in rows(memo) or pixels(image)", false, false );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "textareacols" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "object_class_props", "textareacols", "Width in characters(memo) or pixels(image)", false, false );
            }

            /* TODO: Delete on moving to Quince */
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "jct_nodes_props", "hidden" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "jct_nodes_props", "hidden", "Determines whether property displays.", true, false );
            }

            // case 26957
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_layout", "tabgroup" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_layout", "tabgroup", "Assign properties into a group on a tab", false, false, 50 );
            }

            //case 27327 - change SizeClass "Capacity" to "Initial Capacity"
            CswNbtMetaDataObjectClass sizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp initialQuantityOCP = sizeOC.getObjectClassProp( "Capacity" );
            if( null != initialQuantityOCP )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( initialQuantityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, "Initial Quantity" );
            }

            //case 27391
            CswTableUpdate SequencesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "RbEeOu_01_sequences", "sequences" );
            DataTable SequenceTable = SequencesUpdate.getTable();
            foreach( DataRow Row in SequenceTable.Rows )
            {
                string Name = CswConvert.ToString( Row["sequencename"] );
                switch( Name )
                {
                    case "Fire Extinguisher Barcode":
                        Row["sequencename"] = "Inspection Barcode";
                        Row["prep"] = "I";
                        break;
                    case "Equipment Barcode":
                        if( CswConvert.ToString( Row["prep"] ) != "E" )
                        {
                            Row["prep"] = "E";
                        }
                        break;
                    case "Request Items":
                        if( CswConvert.ToString( Row["prep"] ) != "R" )
                        {
                            Row["prep"] = "R";
                        }
                        break;
                    case "Feedback CaseNo":
                        if( CswConvert.ToString( Row["prep"] ) != "F" )
                        {
                            Row["prep"] = "F";
                        }
                        break;
                }
                if( CswConvert.ToInt32( Row["pad"] ) != 6 )
                {
                    Row["pad"] = CswConvert.ToDbVal( 6 );
                }
            }
            SequencesUpdate.update( SequenceTable );

        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


