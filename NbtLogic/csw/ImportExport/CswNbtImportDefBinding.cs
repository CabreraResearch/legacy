using System;
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
        /// Add new Binding entries to a definition (for use by CswNbtImporter)
        /// </summary>
        public static void addBindingEntries( CswNbtResources CswNbtResources, DataTable BindingsDataTable, Dictionary<string, Int32> DefIdsBySheetName )
        {
            CswTableUpdate importBindingsUpdate = CswNbtResources.makeCswTableUpdate( "storeDefinition_Bindings_update", CswNbtImportTables.ImportDefBindings.TableName );
            foreach( DataRow BindingRow in BindingsDataTable.Rows )
            {

                //set blank instances to min value
                if( BindingRow["instance"] == DBNull.Value || String.IsNullOrEmpty( BindingRow["instance"].ToString() ) )
                {
                    BindingRow["instance"] = Int32.MinValue;
                }

                    CswNbtMetaDataNodeType DestNodeType = null;
                    CswNbtMetaDataNodeTypeProp DestProp = null;

                    string DestNTName = BindingRow["destnodetypename"].ToString();
                string DestNTPName = BindingRow["destpropname"].ToString();
                DestNodeType = CswNbtResources.MetaData.getNodeType( DestNTName );
                DestProp = DestNodeType.getNodeTypeProp( DestNTPName );

                if( null == DestNodeType )
                    {
                    throw new CswDniException( CswEnumErrorType.Error, "Error reading bindings", "Invalid destnodetype defined in 'Bindings' sheet: " + DestNTName );
                }
                else if( null == DestProp )
                        {
                    throw new CswDniException( CswEnumErrorType.Error, "Error reading bindings", "Invalid destproperty defined in 'Bindings' sheet: " + BindingRow["destpropname"].ToString() + " (nodetype: " + DestNTName + ")" );
                }
                else
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
                    BindingRow["destsubfield"] = DestSubFieldStr;

                }// else -- (when DestNodeType and DestProp are defined)
            } // foreach( DataRow BindingRow in BindingsDataTable.Rows )

            importBindingsUpdate.update( BindingsDataTable );

        } // addBindingEntries()

    } // class CswNbt2DBinding
} // namespace
