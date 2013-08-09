using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DBinding
    {
        private CswNbtResources _CswNbtResources;
        private DataRow _row;

        public CswNbt2DBinding( CswNbtResources CswNbtResources, DataRow BindingRow )
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
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportBindings.importdefinitionid] ); }
        }
        public Int32 ImportBindingId
        {
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportBindings.importbindingid] ); }
        }
        public string SourceColumnName
        {
            get { return _row[CswNbt2DImportTables.ImportBindings.sourcecolumnname].ToString(); }
        }
        public string DestNodeTypeName
        {
            get { return _row[CswNbt2DImportTables.ImportBindings.destnodetypename].ToString(); }
        }
        public string DestPropName
        {
            get { return _row[CswNbt2DImportTables.ImportBindings.destpropname].ToString(); }
        }
        public string DestSubFieldName
        {
            get { return _row[CswNbt2DImportTables.ImportBindings.destsubfield].ToString(); }
        }
        public Int32 Instance
        {
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportBindings.instance] ); }
        }

        public string ImportDataColumnName
        {
            get { return SafeColName( SourceColumnName ); }
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
    } // class CswNbt2DBinding
} // namespace
