using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02K_Case31517B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31517; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            setNewFilterColumns( "nodetype_props", delegate( Int32 propid )
                {
                    return _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( propid );
                } );
            setNewFilterColumns( "object_class_props", delegate( Int32 propid )
                {
                    return _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( propid );
                } );
        }

        private const char FilterDelimiter = '|';

        // Set value of filtersubfield, filtermode, and filtervalue from filter
        private void setNewFilterColumns( string TableName, Func<Int32, ICswNbtMetaDataProp> getProp )
        {
            CswTableUpdate FilterUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "31517_filter_update", TableName );
            DataTable FilterTable = FilterUpdate.getTable( "where filter is not null" );
            foreach( DataRow FilterRow in FilterTable.Rows )
            {
                // adapted from CswNbtMetaDataNodeTypeProp.getFilter()
                if( FilterRow["filter"].ToString() != string.Empty )
                {
                    string[] filter = FilterRow["filter"].ToString().Split( FilterDelimiter );
                    ICswNbtMetaDataProp FilterProp = getProp( CswConvert.ToInt32( FilterRow["filterpropid"] ) );
                    if( FilterProp != null )
                    {
                        CswNbtSubField SubField = FilterProp.getFieldTypeRule().SubFields[(CswEnumNbtPropColumn) filter[0]];
                        string FilterValue = string.Empty;
                        if( filter.GetUpperBound( 0 ) > 1 )
                        {
                            FilterValue = filter[2];
                        }
                        if( null != SubField )
                        {
                            FilterRow["filtersubfield"] = SubField.Name.ToString();
                            FilterRow["filtermode"] = (CswEnumNbtFilterMode) filter[1].ToString();
                            FilterRow["filtervalue"] = FilterValue;
                        }
                    }
                }
            }
            FilterUpdate.update( FilterTable );
        } // update()

    }//class CswUpdateMetaData_02K_Case31517B

}//namespace ChemSW.Nbt.Schema