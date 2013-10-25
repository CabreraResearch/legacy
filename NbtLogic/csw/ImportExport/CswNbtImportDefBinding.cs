﻿using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbtImportDefBinding
    {
        private CswNbtResources _CswNbtResources;
        private DataRow _row;

        public CswNbtImportDefBinding( CswNbtResources CswNbtResources, DataRow BindingRow )
        {
            _CswNbtResources = CswNbtResources;
            if( null != BindingRow )
            {
                _row = BindingRow;
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid import binding", "CswNbt2DBinding was passed a null BindingRow" );
            }
        }


        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefBindings.importdefid] ); }
        }

        public Int32 ImportBindingId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefBindings.importdefbindingid] ); }
        }

        public string SourceColumnName
        {
            get { return _row[CswNbtImportTables.ImportDefBindings.sourcecolumnname].ToString(); }
        }

        public string DestNodeTypeName
        {
            get { return _row[CswNbtImportTables.ImportDefBindings.destnodetypename].ToString(); }
        }

        public string DestPropName
        {
            get { return _row[CswNbtImportTables.ImportDefBindings.destpropname].ToString(); }
        }

        public string DestSubFieldName
        {
            get { return _row[CswNbtImportTables.ImportDefBindings.destsubfield].ToString(); }
        }

        public Int32 Instance
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefBindings.instance] ); }
        }

        public string ImportDataColumnName
        {
            get { return SafeColName( SourceColumnName ); }
        }

        public string SourceBlobTableName
        {
            get { return _row[CswNbtImportTables.ImportDefBindings.blobtablename].ToString(); }
        }

        public string SourceClobTableName
        {
            get { return _row[CswNbtImportTables.ImportDefBindings.clobtablename].ToString(); }
        }

        public string SourceLobDataPkColOverride
        {
            get { return _row[CswNbtImportTables.ImportDefBindings.lobdatapkcoloverride].ToString(); }
        }

        public static string SafeColName( string ColName )
        {
            string ret = ColName;
            ret = ret.Replace( "'", "" );
            ret = ret.Replace( " ", "" );
            return ret;
        }

        public CswNbtMetaDataNodeType DestNodeType
        {
            get { return _CswNbtResources.MetaData.getNodeType( DestNodeTypeName ); }
        }

        public CswNbtMetaDataNodeTypeProp DestProperty
        {
            get
            {
                CswNbtMetaDataNodeTypeProp ret = null;
                if( null != DestNodeType )
                {
                    ret = DestNodeType.getNodeTypeProp( DestPropName );
                }
                return ret;
            }
        }

        public CswNbtSubField DestSubfield
        {
            get
            {
                CswNbtSubField ret = null;
                if( null != DestProperty )
                {
                    ret = DestProperty.getFieldTypeRule().SubFields[(CswEnumNbtSubFieldName) DestSubFieldName];
                    if( ret == null )
                    {
                        ret = DestProperty.getFieldTypeRule().SubFields.Default;
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// Get a DataTable to fill out, for use with addBindingEntries()
        /// </summary>
        /// <returns></returns>
        public static DataTable getDataTableForNewBindingEntries()
        {
            DataTable Table = new DataTable();
            Table.Columns.Add( "sheet" );
            Table.Columns.Add( "destnodetype" );
            Table.Columns.Add( "destproperty" );
            Table.Columns.Add( "destsubfield" );
            Table.Columns.Add( "sourcecolumnname" );
            Table.Columns.Add( "instance" );
            Table.Columns.Add( "clobtablename" );
            Table.Columns.Add( "blobtablename" );
            Table.Columns.Add( "lobdatapkcoloverride" );
            return Table;
        }


        /// <summary>
        /// Add new Binding entries to a definition (for use by CswNbtImporter)
        /// </summary>
        public static void addBindingEntries( CswNbtResources CswNbtResources, DataTable BindingsDataTable, Dictionary<string, Int32> DefIdsBySheetName )
        {
            CswTableUpdate importBindingsUpdate = CswNbtResources.makeCswTableUpdate( "storeDefinition_Bindings_update", CswNbtImportTables.ImportDefBindings.TableName );
            DataTable importBindingsTable = importBindingsUpdate.getEmptyTable();
            foreach( DataRow BindingRow in BindingsDataTable.Rows )
            {
                string SheetName = BindingRow["sheetname"].ToString();
                if( false == string.IsNullOrEmpty( SheetName ) )
                {
                    CswNbtMetaDataNodeType DestNodeType = null;
                    CswNbtMetaDataNodeTypeProp DestProp = null;

                    string DestNTName = BindingRow["destnodetypename"].ToString();
                    if( false == string.IsNullOrEmpty( DestNTName ) )
                    {
                        DestNodeType = CswNbtResources.MetaData.getNodeType( DestNTName );
                        if( null != DestNodeType )
                        {
                            string DestNTPName = BindingRow["destpropname"].ToString();
                            DestProp = DestNodeType.getNodeTypeProp( DestNTPName );
                            if( null != DestProp )
                            {
                                string DestSubFieldStr = BindingRow["destsubfield"].ToString();
                                if( DestSubFieldStr != CswEnumNbtSubFieldName.Blob.ToString() )
                                {
                                    CswNbtSubField DestSubfield = DestProp.getFieldTypeRule().SubFields[(CswEnumNbtSubFieldName) BindingRow["destsubfield"].ToString()];
                                    if( DestSubfield == null )
                                    {
                                        DestSubfield = DestProp.getFieldTypeRule().SubFields.Default;
                                        DestSubFieldStr = DestSubfield.Name.ToString();
                                    }
                                }
                                string DestSubFieldName = DestSubFieldStr;

                                DataRow row = importBindingsTable.NewRow();
                                row[CswNbtImportTables.ImportDefBindings.importdefid] = DefIdsBySheetName[SheetName];
                                row[CswNbtImportTables.ImportDefBindings.sourcecolumnname] = BindingRow["sourcecolumnname"].ToString();
                                row[CswNbtImportTables.ImportDefBindings.destnodetypename] = DestNTName;
                                row[CswNbtImportTables.ImportDefBindings.destpropname] = DestNTPName;
                                row[CswNbtImportTables.ImportDefBindings.destsubfield] = DestSubFieldName;
                                row[CswNbtImportTables.ImportDefBindings.instance] = CswConvert.ToDbVal( BindingRow["instance"].ToString() );
                                if( BindingRow.Table.Columns.Contains( CswNbtImportTables.ImportDefBindings.blobtablename ) )
                                {
                                    row[CswNbtImportTables.ImportDefBindings.blobtablename] = BindingRow[CswNbtImportTables.ImportDefBindings.blobtablename].ToString();
                                }
                                if( BindingRow.Table.Columns.Contains( CswNbtImportTables.ImportDefBindings.clobtablename ) )
                                {
                                    row[CswNbtImportTables.ImportDefBindings.clobtablename] = BindingRow[CswNbtImportTables.ImportDefBindings.clobtablename].ToString();
                                }
                                if( BindingRow.Table.Columns.Contains( CswNbtImportTables.ImportDefBindings.lobdatapkcoloverride ) )
                                {
                                    row[CswNbtImportTables.ImportDefBindings.lobdatapkcoloverride] = BindingRow[CswNbtImportTables.ImportDefBindings.lobdatapkcoloverride].ToString();
                                }
                                importBindingsTable.Rows.Add( row );
                            }
                            else
                            {
                                throw new CswDniException( CswEnumErrorType.Error, "Error reading bindings", "Invalid destproperty defined in 'Bindings' sheet: " + BindingRow["destproperty"].ToString() + " (nodetype: " + DestNTName + ")" );
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "Error reading bindings", "Invalid destnodetype defined in 'Bindings' sheet: " + DestNTName );
                        }
                    } // if( false == string.IsNullOrEmpty( DestNTName ) )
                } // if( false == string.IsNullOrEmpty( SheetName ) )
            } // foreach( DataRow BindingRow in BindingsDataTable.Rows )
            importBindingsUpdate.update( importBindingsTable );
        } // addBindingEntries()

    } // class CswNbt2DBinding
} // namespace
